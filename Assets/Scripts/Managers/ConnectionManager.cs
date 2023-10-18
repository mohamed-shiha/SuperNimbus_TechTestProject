using JetBrains.Annotations;
using Nakama;
using Nakama.TinyJson;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] string Scheme;
    [SerializeField] string HostName;
    [SerializeField] int Port;
    [SerializeField] string ServerKey;

    IClient client;
    ISession session;
    ISocket socket;

    private async void Start()
    {
        client = new Client(Scheme, HostName, Port, ServerKey, UnityWebRequestAdapter.Instance);
        await AuthenticateWithDevice();
        //socket = client.NewSocket();
        socket = Socket.From(client);
        await socket.ConnectAsync(session, true);

        //Debug.Log(session);
        //Debug.Log(socket);

        // i am now connected 
        await TestingStoringData();
        await TestingReadingData();
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
            Debug.Log("Authenticated with Device ID\n " + deviceId);
        }
        catch (ApiResponseException ex)
        {
            Debug.LogFormat("Error authenticating with Device ID: {0}", ex.Message);
        }

    }


    public class TowersStorageObject
    {
        public int[] UnlockedIDs;
    }

    async Task TestingStoringData()
    {


        var storageObject = new TowersStorageObject
        {
            UnlockedIDs = GameManager.Instance.AllData.Towers.Select(i => i.ID).ToArray()
        };
        var unlockedIDs = JsonWriter.ToJson(storageObject);
        Debug.Log("unlockedIDs: " + unlockedIDs);
        await client.WriteStorageObjectsAsync(session, new[]
        {
            new WriteStorageObject
            {
                Collection = "UnlockedTowers",
                Key = "towers",
                //Value = " 7,8,9,6,7,8,9,6,7,8,9,6,7,8,9,6,7,8,9,6,7,8,9,6 "
                Value = unlockedIDs,
                PermissionRead = 1,
                PermissionWrite = 1
            },
        });

        Debug.Log("Stored");
    }

    //async Task TestingReadingData()
    //{

    //    var result = await client.ReadStorageObjectsAsync(session, new[] {
    //        new StorageObjectId
    //        {
    //            Collection = "UnlockedTowers",
    //            Key = "towers",
    //            UserId = session.UserId
    //        }
    //    });

    //    Debug.Log("Read objects: [{0}]\n " + result.Objects);
    //}


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
