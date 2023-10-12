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


    private void Awake()
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
            // test 
            OnGameStarted.Invoke(new Level(new int[] { 7, 8, 9, 6, 7, 8, 9, 6 , 7, 8, 9, 6 , 7, 8, 9, 6 , 7, 8, 9, 6 , 7, 8, 9, 6 }) { SpawnSpeed = 3.5f });
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

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            SpawnManager.SpawnBullet(pos, new SpawnData(0, 1, ObjectType.Bullet, "bullet", 1, 1));
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
        Vector2 pos = EnemySpawnPoints[UnityEngine.Random.Range(0, EnemySpawnPoints.Length - 1)].position;
        var data = Data[ObjectType.Enemy, id];
        SpawnManager.SpawnEnemy(id, pos, data);
        //Invoke("SpawnEnemy", CurrentLevel.SpawnSpeed);
    }
}
