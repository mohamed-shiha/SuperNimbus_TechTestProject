using System;
using UnityEngine;

[Serializable]
public class Level
{

    [SerializeField] EnemyNames[] SpawnOrderType;
    [SerializeField] string Name;
    int currentEnemy = 0;

    public int StartingGold { get; private set; }

    public float SpawnSpeed;
    public int EnemyCount => SpawnOrderType.Length;

    public Level(EnemyNames[] spawnOrderType, int startingGold, float spawnSpeed)
    {
        SpawnOrderType = spawnOrderType;
        StartingGold = startingGold;
        SpawnSpeed = spawnSpeed;
        currentEnemy = 0;
    }

    public int GetAndMove()
    {
        if (currentEnemy >= SpawnOrderType.Length)
        {
            currentEnemy = 0;
        }
        return (int)SpawnOrderType[currentEnemy++];
    }

    public bool HasNext()
    {
        return currentEnemy < SpawnOrderType.Length;
    }

    internal Level GetCopy()
    {
        return new Level(SpawnOrderType, StartingGold, SpawnSpeed);
    }

    internal void OnRestart()
    {
        currentEnemy = 0;
        //TODO: reset gold value
    }
}

