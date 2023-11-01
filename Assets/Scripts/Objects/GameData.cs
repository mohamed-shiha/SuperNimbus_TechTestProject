using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameData", fileName = "NewGameData")]
public class GameData : ScriptableObject
{
    public SpawnData[] Enemies;
    public SpawnData[] Towers;
    [SerializeField] Level[] Levels;
    [SerializeField] int Gold;

    public int GetGold() => Gold;

    public Level GetLevel(int index) => Levels[index].GetCopy();

    public SpawnData this[ObjectType type, int id]
    {
        get
        {
            return type == ObjectType.Enemy ? Enemies.First(item => item.ID == id) : Towers.First(item => item.ID == id);
        }
    }

    internal void SetGold(int newTotal)
    {
        Gold = newTotal;
    }
}
