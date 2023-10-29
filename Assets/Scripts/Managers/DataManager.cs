using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public Action OnDataReady;

    [SerializeField] string charactersSpriteSheet;
    Sprite[] charactersSprites;
    [SerializeField] GameData DebugData;
    [SerializeField] GameData localData;
    GameData OnlineData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            charactersSprites = Resources.LoadAll<Sprite>(charactersSpriteSheet);
            ConnectionManager.Instance.OnConnected += InitializeOnline;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.OnConnected -= InitializeOnline;
    }

    async void InitializeOnline()
    {
        if (OnlineData == null)
        {
            OnlineData = ScriptableObject.CreateInstance<GameData>();
        }


        var towersTask = await ConnectionManager.Instance.GetAllTowersData();
        OnlineData.Towers = towersTask;

        var enemiesTask = await ConnectionManager.Instance.GetAllEnemiesData();
        OnlineData.Enemies = enemiesTask;

        //TODO: Add levels data
        OnDataReady?.Invoke();
    }
    
    async void InitializeLocal()
    {
        if (localData == null)
        {
            localData = ScriptableObject.CreateInstance<GameData>();
        }


        var towersTask = await ConnectionManager.Instance.GetAllTowersData();
        localData.Towers = towersTask;

        var enemiesTask = await ConnectionManager.Instance.GetAllEnemiesData();
        localData.Enemies = enemiesTask;

        //TODO: Add levels data
        OnDataReady?.Invoke();
    }


    public GameData GetGameData(bool forcOffline = false)
    {
        if (forcOffline)
            return DebugData;
        else
        {
            return ConnectionManager.Instance.IsConnected ? OnlineData : DebugData;
        }
    }

    internal Sprite GetSpriteByName(string name)
    {
        return charactersSprites.FirstOrDefault(s => s.name == name);
    }

    internal void SaveGold(int total)
    {
        PlayerPrefs.SetFloat("gold", total);
        PlayerPrefs.Save();

        //save it online if the game is connected
        // if not connected set a bool in prefs to update the value when the game is online
    }

    internal void SaveUnlockedTowers(string ids)
    {
        PlayerPrefs.SetString("unlockedTowers", ids);
        PlayerPrefs.Save();

        //save it online if the game is connected
        // if not connected set a bool in prefs to update the value when the game is online
    }

    
}
