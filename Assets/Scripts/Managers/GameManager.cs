using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] SpawnManager SpawnManager;
    [SerializeField] GameData Data;
    int LevelReward;
    int Kills;
    Level CurrentLevel;
    Transform[] EnemySpawnPoints;
    public Action<WorldObject> OnObjectDeath;
    public Action<Level> OnGameStarted;
    public Action<int> OnTowerSelected;
    public Player player;

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

    void OnGameStartedCall(Level level)
    {
        CurrentLevel = level;
        SpawnEnemy();
    }

    public void RewardPlayer(int reward)
    {
        LevelReward += reward;
    }

    private void OnObjectDeathCall(WorldObject obj)
    {
        SpawnManager.TakeBackObject(obj);
        if (obj is EnemyController enemy)
        {
            Kills++;
            SpawnEnemy();
            RewardPlayer(enemy.Data.GetRewardPerKill());
        }
    }

    public void Fire(Vector3 spawnPos)
    {
        SpawnManager.SpawnBullet(spawnPos, new SpawnData(0, 1, ObjectType.Bullet, "bullet", 1, 1));
    }

    public void SpawnEnemy()
    {
        if (!CurrentLevel.HasNext())
        {
            Debug.Log("Spawn Queue Finished");
            return;
        }
        int id = CurrentLevel.GetAndMove();
        var point = EnemySpawnPoints[UnityEngine.Random.Range(1, EnemySpawnPoints.Length)];
        Debug.Log($"spawning {Data[ObjectType.Enemy, id].Name} on {point.name}");
        var pos=  point.position;
        var data = Data[ObjectType.Enemy, id];
        SpawnManager.SpawnEnemy(id, pos, data);
        //Invoke("SpawnEnemy", CurrentLevel.SpawnSpeed);
    }

    public void SpawnTower(int id, Vector3 pos)
    {
        var data = Data[ObjectType.Tower, id];
        SpawnManager.SpawnTower(id, pos, data);
    }
}
