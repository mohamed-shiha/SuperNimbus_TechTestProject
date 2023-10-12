using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameData", fileName = "NewGameData")]
public class GameData : ScriptableObject
{
    public SpawnData[] Enemies;
    public SpawnData[] Towers;

    public SpawnData this[ObjectType type, int id]
    {
        get
        {
            return type == ObjectType.Enemy ? Enemies.First(item => item.ID == id) : Towers.First(item => item.ID == id);
        }
    }
}
