using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action<int> OnTowerSelected;
    public Action<int> OnTowerUnlocked;
    public Action<int, int> OnPlayerRewarded;
    public Action<int> OnPlayerLivesChanged;
    public Player player;

    [SerializeField] SpawnManager SpawnManager;
    [SerializeField] bool UseOfflineData;
    [SerializeField] bool UseDebugeData;
    [SerializeField] bool KeepSpawning;

    GameData Data => DataManager.Instance.GetGameData(UseOfflineData);
    int LevelReward;
    int Kills;
    Level CurrentLevel;
    Transform[] EnemySpawnPoints;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            EnemySpawnPoints = Camera.main.transform.GetChild(0).GetComponentsInChildren<Transform>();
            SpawnManager.Initialize();
            player.gameObject.SetActive(true);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnObjectDeathCall(WorldObject obj, SpawnData killData)
    {
        // return the object to the queue
        SpawnManager.TakeBackObject(obj);

        if (obj is EnemyController enemy)
        {
            // if the enemy is killed by a bullet reward the player
            if (killData.Name.ToLower().Contains("bullet"))
            {
                SpawnManager.SpawnRewardUI(obj.transform.position, killData);
                Kills++;
                LevelReward += killData.GetRewardPerKill();
                OnPlayerRewarded?.Invoke(LevelReward, Kills);
                return;
            }

            if (killData.Name.ToLower().Contains("killzone"))
            {
                // reduce player lives 
                OnPlayerLivesChanged.Invoke(1);
            }
        }

    }

    public void Fire(Vector3 spawnPos, SpawnData data)
    {
        SpawnManager.SpawnBullet(spawnPos, new SpawnData(-1, 1, ObjectType.Bullet, "bullet - " + data.Name, 1, data.GetRewardPerKill()));
    }

    public void SpawnNextEnemy()
    {
        if (!CurrentLevel.HasNext() && !KeepSpawning)
        {
            // the level ended
            // that means the player won 
            // invoke level ended event
            // Connection -> save/update player gold with the level reward
            // UI -> show level ended with kills and gold maybe some animation

            Debug.Log("Spawn Queue Finished");
            return;
        }

        int id = CurrentLevel.GetAndMove();
        var point = EnemySpawnPoints[UnityEngine.Random.Range(1, EnemySpawnPoints.Length)];
        var pos = point.position;
        var data = Data[ObjectType.Enemy, id];
        SpawnManager.SpawnEnemy(id, pos, data);
        Invoke("SpawnNextEnemy", CurrentLevel.SpawnSpeed);
    }

    public void SpawnTower(int id, Vector3 pos)
    {
        var data = Data[ObjectType.Tower, id];
        SpawnManager.SpawnTower(id, pos, data);
    }

    public SpawnData[] GetPlayerUnlockedTowers()
    {
        var result = Data.Towers;
        //var result = UseOfflineData ? Data.Towers : player.GetUnlockedTowers();
        return result;
    }

    // TODO: need to make sure a level is selected for now use a debug level
    public void StartLevel()
    {
        if (CurrentLevel == null)
        {
            CurrentLevel = Data.GetLevel(0);
        }

        SpawnNextEnemy();
    }

    public void OnRestart()
    {
        CancelInvoke("SpawnNextEnemy");
        SpawnManager.CleanScene();
        player.OnRestart();
        CurrentLevel.OnRestart();
        StartLevel();
    }

    public void OnBackToMain()
    {
        CancelInvoke("SpawnNextEnemy");
        SpawnManager.CleanScene();
        player.OnRestart();
        CurrentLevel.OnRestart();
    }
}
