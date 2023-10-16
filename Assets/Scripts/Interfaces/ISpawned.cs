using System;
using System.ComponentModel;
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
    [SerializeField] int RewardPerKill;
    public int CurrentHits;
    public int Value;
    public Sprite Icon;

    int randomReward;

    public int GetRewardPerKill()
    {
        if(IsRandomReward && randomReward == -99)
        {
            ResetReward();
        }
        return IsRandomReward ? randomReward : RewardPerKill;
    }

    public void ResetReward()
    {
        randomReward = UnityEngine.Random.Range(1, RewardPerKill + 1);
    }
    //public Vector2 MovementDirection;

    public SpawnData(int iD, float speed, ObjectType type, string name, int hitsToDie, int rewardPerKill, int cost = 0, Sprite icon = null)
    {
        ID = iD;
        Speed = speed;
        Type = type;
        Name = name;
        HitsToDie = hitsToDie;
        CurrentHits = 0;
        RewardPerKill = rewardPerKill;
        IsRandomReward = true;
        Value = cost;
        Icon = icon;
        randomReward = -99;
    }



}


public interface ISpawned
{
    public void SetData(SpawnData spawnData);
    public void RestartAlive(Vector2 SpawnPos, Vector2 dir);
    public void Die(SpawnData killer);
}
