using System;
using UnityEngine;

[Serializable]
public class Level
{

    [SerializeField] EnemyNames[] SpawnOrderType;
    int currentEnemy = 0;
    int StartingGold;

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
        return new Level(SpawnOrderType,StartingGold,SpawnSpeed);
    }
}

