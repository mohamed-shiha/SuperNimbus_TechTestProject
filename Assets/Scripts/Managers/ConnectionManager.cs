using Nakama;
using Nakama.TinyJson;
using System;
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
    [SerializeField] bool PlayOffLine;

    IClient client;
    ISession session;
    ISocket socket;

    public bool IsConnected => socket.IsConnected;
    public Action OnConnected;
    public Action OnConnectionFailed;
    public Action OnStartOffLine;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            if (!PlayOffLine)
            {
                StartCoroutine("Connect");
            }
            else
            {
                OnStartOffLine?.Invoke();
            }
        }
        else
        {
            Destroy(this);
        }
    }

    private async void Connect()
    {
        // create the client
        client = new Client(Scheme, HostName, Port, ServerKey, UnityWebRequestAdapter.Instance);
        // authenticate the device
        await AuthenticateWithDevice();
        // create the socket and connect it
        socket = Socket.From(client);
        await socket.ConnectAsync(session, true);
        // notify the subscribers about the connection status
        if (IsConnected)
        {
            OnConnected?.Invoke();
        }
        else
        {
            OnConnectionFailed?.Invoke();
        }
    }



    public async Task AuthenticateWithDevice()
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
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error authenticating with Device ID: {0}", ex.Message);
        }

    }


    async Task StoreObjectsToNakama<T>(T[] objectsToSend, string collection, string key)
    {
        // created storage object from the data
        var storageObject = new StorageObject<T>
        {
            StoreObjects = objectsToSend
        };

        // concert the data to Json
        var unlockedIDs = JsonWriter.ToJson(storageObject);

        // send the data to Nakama
        await client.WriteStorageObjectsAsync(session, new[]
        {
                new WriteStorageObject
                {
                    Collection = collection,
                    Key = key,
                    Value = unlockedIDs,
                    PermissionRead = 1,
                    PermissionWrite = 1
                },
        });
    }


    async Task TestingReadingData()
    {
        try
        {
            var result = await client.ReadStorageObjectsAsync(session, new[]
            {
                new StorageObjectId
                {
                    Collection = "UnlockedTowers",
                    Key = "towers",
                    UserId = session.UserId
                }
            });

            Debug.Log("Read objects: " + result.Objects);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogError($"Error reading storage objects. Code: {ex.StatusCode}, Message: {ex.Message}");
        }
    }

}

public class StorageObject<T>
{
    public T[] StoreObjects;
}
