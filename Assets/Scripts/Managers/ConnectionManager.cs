using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] string Scheme;
    [SerializeField] string HostName;
    [SerializeField] int Port;
    [SerializeField] string ServerKey;
    [SerializeField] bool ForceOffline;

    IClient client;
    ISession session;
    ISocket socket;

    public bool IsConnected => socket != null && socket.IsConnected;
    public Action OnConnected;
    public Action OnConnectionFailed;
    public Action OnStartOffLine;
    public Action<SpawnData[]> OnTowersDataReady;
    public Action<SpawnData[]> OnEnemiesDataReady;

    readonly string TowersKeyPrefix = "Tower";
    readonly string EnemiesKeyPrefix = "Enemy";
    //readonly string LevelsKeyPrefix = "Levels";
    readonly string UnlocksCollection = "Unlocks";
    readonly string DataCollection = "MainData";

    string TowersCollection => string.Format($"{TowersKeyPrefix}_{DataCollection}");
    string EnemiesCollection => string.Format($"{EnemiesKeyPrefix}_{DataCollection}");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (!ForceOffline)
            {
                StartCoroutine(nameof(TryConnect));
            }
            else
            {
                // delay invoke to give time to other classes to subscribe to offline event
                Invoke(nameof(StartOffline), 0.5f);  
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private async void TryConnect()
    {
        // create the client
        client = new Client(Scheme, HostName, Port, ServerKey, UnityWebRequestAdapter.Instance);
        // authenticate the device
        if (await AuthenticateWithDeviceAndConnect())
        {
            Debug.Log("Connected");
            OnConnected?.Invoke();
        }
        else
        {
            Debug.Log("Connection Failed");
            OnConnectionFailed?.Invoke();
        }
    }

    private void StartOffline()
    {
        GameManager.Instance.SetDataSource(DataSource.Local);
        OnStartOffLine?.Invoke();
    }

    public async Task UnlockTowerForPlayer(int[] towerIDs)
    {

        // get old unlocks 
        // if we have add to it 
        // else just send the new one 

        var unlcokedTowersIds = new ArrayWrapper<int>(towerIDs);

        string towersJson = JsonWriter.ToJson(unlcokedTowersIds);

        var writeObject = new WriteStorageObject
        {
            Collection = UnlocksCollection,
            Key = "Towers",
            Value = towersJson,
            PermissionRead = 1, // Only the server and owner can read
            PermissionWrite = 1, // The server and owner can write
        };

        await client.WriteStorageObjectsAsync(session, new[] { writeObject });

    }

    public async Task<SpawnData[]> GetAllTowersData()
    {
        var items = GetNakamaCollection<SpawnData>(TowersCollection);
        var allTowers = new List<SpawnData>();

        await foreach (var item in items)
        {
            allTowers.Add(item);
        }

        return allTowers.ToArray();
    }
    public async Task<SpawnData[]> GetAllEnemiesData()
    {
        var items = GetNakamaCollection<SpawnData>(EnemiesCollection);
        var enemies = new List<SpawnData>();

        await foreach (var item in items)
        {
            enemies.Add(item);
        }

        return enemies.ToArray();
    }
    public async Task<Dictionary<string, string>> GetWallet()
    {
        var account = await client.GetAccountAsync(session);
        var wallet = JsonParser.FromJson<Dictionary<string, string>>(account.Wallet);
        return wallet;
    }

    async Task<bool> AuthenticateWithDeviceAndConnect()
    {
        // If the user's device ID is already stored, grab that - alternatively get the System's unique device identifier.
        var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

        // If the device identifier is invalid then let's generate a unique one.
        if (deviceId == SystemInfo.unsupportedIdentifier)
        {
            deviceId = System.Guid.NewGuid().ToString();
        }

        // Save the user's device ID to PlayerPrefs so it can be retrieved during a later play session for re-authenticating.
        PlayerPrefs.SetString("deviceId", deviceId);

        // Authenticate with the Nakama server using Device Authentication.
        try
        {
            session = await client.AuthenticateDeviceAsync(deviceId);
            Debug.Log($"Authenticated with Device ID: {deviceId}");
            // create the socket and connect it
            socket = Socket.From(client);
            await socket?.ConnectAsync(session, true);
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogFormat("Error authenticating with Device ID: {0}", ex.Message);
            return false;
        }
    }
    async Task StoreObjectsToNakama<T>(T objectsToSend, string collection, string key, int readPermition = 1, int writePermition = 1)
    {
        var jsonObject = JsonWriter.ToJson(objectsToSend);
        //var unlockedIDs = JsonWriter.ToJson(storageObject);

        // send the data to Nakama
        await client.WriteStorageObjectsAsync(session, new[]
        {
                new WriteStorageObject
                {
                    Collection = collection,
                    Key = key,
                    Value = jsonObject,
                    PermissionRead = readPermition,
                    PermissionWrite = writePermition
                },
        });
    }
    async IAsyncEnumerable<T> GetNakamaCollection<T>(string collection, int limit = 20)
    {
        var result = await client.ListStorageObjectsAsync(session, collection, limit);

        if (result.Objects.Any())
        {
            foreach (var item in result.Objects)
            {
                var resultObject = JsonParser.FromJson<T>(item.Value);
                yield return resultObject;
            }
        }
    }

# if UNITY_EDITOR
    async Task UploadTowersAsync(SpawnData[] towersData)
    {
        foreach (var towerData in towersData)
        {
            var key = string.Format($"{TowersKeyPrefix}_{towerData.ID}_{towerData.Name}");
            await StoreObjectsToNakama(towerData, TowersCollection, key, 2);
        }
    }
    async Task UploadEnemiesAsync(SpawnData[] enemiesData)
    {
        foreach (var enemyData in enemiesData)
        {
            var key = string.Format($"{EnemiesKeyPrefix}_{enemyData.ID}_{enemyData.Name}");
            await StoreObjectsToNakama(enemyData, EnemiesCollection, key, 2);
        }
    }

#endif

}
