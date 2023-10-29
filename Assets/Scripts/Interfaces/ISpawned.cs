using System;
using UnityEngine;

public enum ObjectType
{
    NA,
    Bullet,
    Enemy,
    Tower
}

[Serializable]
public struct SpawnData
{
    public int ID;
    public float Speed;
    public ObjectType Type;
    public string Name;
    public int HitsToDie;
    public bool IsRandomReward;
    public int RewardPerKill;
    //public int CurrentHits;
    public int Value;
    public string IconName;

    int randomReward;

    public Sprite GetIcon()
    {
        return DataManager.Instance.GetSpriteByName(IconName);
    }

    public int GetRewardPerKill()
    {
        if (IsRandomReward && randomReward == -99)
        {
            ResetReward();
        }
        return IsRandomReward ? randomReward : RewardPerKill;
    }

    public void ResetReward()
    {
        randomReward = UnityEngine.Random.Range(1, RewardPerKill + 1);
    }

    public SpawnData(int iD, float speed, ObjectType type, string name, int hitsToDie, int rewardPerKill, int cost = 0, string iconName = "")
    {
        ID = iD;
        Speed = speed;
        Type = type;
        Name = name;
        HitsToDie = hitsToDie;
        //CurrentHits = 0;
        RewardPerKill = rewardPerKill;
        IsRandomReward = true;
        Value = cost;
        randomReward = -99;
        IconName = iconName;
    }
}


public interface ISpawned
{
    public void SetData(SpawnData spawnData);
    public void RestartAlive(Vector2 SpawnPos, Vector2 dir);
    public void Die(SpawnData killer);
}
