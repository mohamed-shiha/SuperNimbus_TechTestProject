using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Action<int> OnTowerSelected;
    public Action<int> OnTowerUnlocked;
    public Action<int, int> OnPlayerRewarded;
    public Action<int> OnPlayerLivesChanged;
    public Action<bool> OnLevelEnded;

    [SerializeField] DataSource dataSource;
    [SerializeField] Player player;
    [SerializeField] SpawnManager SpawnManager;
    [SerializeField] bool KeepSpawning;
    [SerializeField] bool ForceDebug;

    public GameData Data => DataManager.Instance.GetGameData(dataSource);
    int CurrentGold;
    int Kills;
    int Lives = 3;
    Level CurrentLevel;
    Transform[] EnemySpawnPoints;

    public void SetDataSource(DataSource newSource)
    {
        if (!ForceDebug)
        {
            dataSource = newSource;
        }
    }

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
                CurrentGold += killData.GetRewardPerKill();
                OnPlayerRewarded?.Invoke(CurrentGold, Kills);
                return;
            }

            if (killData.Name.ToLower().Contains("killzone"))
            {
                //TODO: reduce player lives 
                Lives--;
                OnPlayerLivesChanged.Invoke(1);
                if(Lives <= 0)
                {
                    GameOver(false);
                }
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
            // the level finished spawning
            // that means the player won 
            // invoke level ended event
            // Connection -> save/update player gold with the level reward
            // UI -> show level ended with kills and gold maybe some animation
            
            GameOver(true);
            Debug.Log("Spawn Queue Finished");
            return;
        }

        int id = CurrentLevel.GetAndMove();
        var point = EnemySpawnPoints[UnityEngine.Random.Range(1, EnemySpawnPoints.Length)];
        var pos = point.position;
        var data = Data[ObjectType.Enemy, id];
        SpawnManager.SpawnEnemy(id, pos, data);
        Invoke(nameof(SpawnNextEnemy), CurrentLevel.SpawnSpeed);
    }

    private void GameOver(bool isWin)
    {
        Time.timeScale = 0;
        OnLevelEnded.Invoke(isWin);
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

        Time.timeScale = 1;
        //TODO: get player's gold
        if(CurrentGold == 0)
        {
            CurrentGold = CurrentLevel.StartingGold;
        }
        Kills = 0;
        Lives = 3;
        OnPlayerRewarded?.Invoke(CurrentGold, Kills);
        SpawnNextEnemy();
    }

    public void OnRestart()
    {
        CleanDataAndObjects();
        StartLevel();
    }

    public void OnBackToMain()
    {
        CleanDataAndObjects();
        CurrentLevel.OnRestart();
    }

    private void CleanDataAndObjects()
    {
        CancelInvoke(nameof(SpawnNextEnemy));
        SpawnManager.CleanScene();
        player.OnRestart();
        CurrentLevel.OnRestart();
    }
}
