using System;
using System.Linq;
using UnityEngine;

public enum DataSource
{
    Local,
    Cloud,
    Debug
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public Action OnDataReady;

    [SerializeField] string charactersSpriteSheet;
    [SerializeField] int StartingGoldAmount;
    [SerializeField] GameData DebugData;
    [SerializeField] GameData localData;
    GameData OnlineData;
    Sprite[] charactersSprites;
    string TowersPrefKey = "Towers";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            charactersSprites = Resources.LoadAll<Sprite>(charactersSpriteSheet);
            ConnectionManager.Instance.OnConnected += InitializeOnline;
            ConnectionManager.Instance.OnConnectionFailed += InitializeLocal;
            ConnectionManager.Instance.OnStartOffLine += InitializeLocal;
            //Debug
            PlayerPrefs.DeleteAll();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.OnConnected -= InitializeOnline;
        ConnectionManager.Instance.OnConnectionFailed -= InitializeLocal;
        ConnectionManager.Instance.OnStartOffLine -= InitializeLocal;
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

        //TODO: get Gold and save it locally
        //TODO: get levels data from the cloud


        GameManager.Instance.SetDataSource(DataSource.Cloud);
        OnDataReady?.Invoke();
    }

    void InitializeLocal()
    {
        Debug.Log("InitializeLocal : Will use \'LocalData\'");

        if (localData == null)
        {
            localData = ScriptableObject.CreateInstance<GameData>();
        }

        CheckFirstRun();

        var towersJson = PlayerPrefs.GetString(TowersPrefKey, "");
        if (towersJson.Length != 0)
        {
            var towersLocalIDs = JsonUtility.FromJson<ArrayWrapper<int>>(towersJson);
            var unlockedTowers = DebugData.Towers.Where(t => towersLocalIDs.Data.Contains(t.ID));
            localData.Towers = unlockedTowers.ToArray();
        }

        //TODO: Add levels data
        OnDataReady?.Invoke();
    }


    public GameData GetGameData(DataSource dataSource)
    {
        switch (dataSource)
        {
            case DataSource.Local:
                return localData;
            case DataSource.Cloud:
                return OnlineData;
            case DataSource.Debug:
                return DebugData;
            default:
                throw new NullReferenceException("No Game Data will be used");
        }
    }

    bool CheckFirstRun()
    {
        if (!PlayerPrefs.HasKey("FirstRun"))
        {
            // this is the first time the game will run 
            // add PlayerPrefs Keys
            // - "FristRun" => means we have data saved locally
            PlayerPrefs.SetInt("FristRun", 0);
            // - "Towers" => will hold a Json object of the Towers IDs unlocked for this player 
            // we will unlock the first tower for new players 
            SaveUnlockedTowers(new int[] { 0 });
            // - "Gold" => will set it to the default starting amount 
            SaveGold(StartingGoldAmount);
            // Might add
            // - "Levels" => will hold a Json object of the levels IDs unlocked for this player 
            return true;
        }

        return false;
    }

    internal Sprite GetSpriteByName(string name)
    {
        return charactersSprites.FirstOrDefault(s => s.name == name);
    }

    internal void SaveGold(int total)
    {
        PlayerPrefs.SetFloat("Gold", total);
        PlayerPrefs.Save();

        if (ConnectionManager.Instance.IsConnected)
        {
            // update player wallet
            OnlineData.SetGold(total);
        }

        localData.SetGold(total);
    }

    internal void SaveUnlockedTowers(int[] ids)
    {
        var jsonString = JsonUtility.ToJson(new ArrayWrapper<int>(ids));
        PlayerPrefs.SetString(TowersPrefKey, jsonString);
        PlayerPrefs.Save();

        //save it online if the game is connected
        // if not connected set a bool in prefs to update the value when the game is online
    }


}
