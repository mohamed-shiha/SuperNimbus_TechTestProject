using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] SpawnManager SpawnManager;
    [SerializeField] GameData Data;
    [SerializeField] bool UseOfflineData;
    [SerializeField] bool KeepSpawning;

    int LevelReward;
    int Kills;
    [SerializeField] Level CurrentLevel;
    Transform[] EnemySpawnPoints;
    public Action<WorldObject, SpawnData> OnObjectDeath;
    public Action<Level> OnGameStarted;
    public Action<int> OnTowerSelected;
    public Action<int> OnTowerUnlocked;
    public Action<int, int> OnPlayerRewarded;
    public Action<int> OnPlayerLivesChanged;
    public Player player;

    public GameData AllData { get { return Data; } }


    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(SpawnManager);
            SpawnManager.Initialize();
            OnObjectDeath += OnObjectDeathCall;
            OnGameStarted += OnGameStartedCall;
            EnemySpawnPoints = Camera.main.transform.GetChild(0).GetComponentsInChildren<Transform>();
            player.gameObject.SetActive(true);
            // test 
            //OnGameStarted.Invoke(new Level(new int[] { 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6, 7, 8, 9, 6 }) { SpawnSpeed = 3.5f });
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        OnObjectDeath -= OnObjectDeathCall;
        OnGameStarted -= OnGameStartedCall;
    }

    void OnGameStartedCall(Level level)
    {
        CurrentLevel = level;
        SpawnNextEnemy();
    }

    public void RewardPlayer(int reward)
    {
        LevelReward += reward;
    }

    private void OnObjectDeathCall(WorldObject obj, SpawnData killData)
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
        var result = UseOfflineData ? Data.Towers : player.GetUnlockedTowers();
        return result;
    }

    public void StartLevel(Level newLevel = null)
    {
        if (newLevel == null)
        {
            newLevel = Data.Levels[0].GetCopy();
        }
        OnGameStarted(newLevel);
    }

}
