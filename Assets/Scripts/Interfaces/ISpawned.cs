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
    public bool RandomReward;
    [SerializeField]int _RewardPerKill;
    public int CurrentHits;

    public int GetRewardPerKill()
    {
        return RandomReward ? UnityEngine.Random.Range(1, _RewardPerKill) : _RewardPerKill;
    }

    //public Vector2 MovementDirection;

    public SpawnData(int iD, float speed, ObjectType type, string name, int hitsToDie, int rewardPerKill/*, Vector2 movementDirection*/)
    {
        ID = iD;
        Speed = speed;
        Type = type;
        Name = name;
        HitsToDie = hitsToDie;
        CurrentHits = 0;
        _RewardPerKill = rewardPerKill;
        RandomReward = true;
        //MovementDirection = movementDirection;
    }



}


public interface ISpawned
{
    public void SetData(SpawnData spawnData);
    public void RestartAlive(Vector2 SpawnPos, Vector2 dir);
    public void Die();
}
