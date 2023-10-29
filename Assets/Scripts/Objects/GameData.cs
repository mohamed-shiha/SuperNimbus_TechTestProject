using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameData", fileName = "NewGameData")]
public class GameData : ScriptableObject
{
    public SpawnData[] Enemies;
    public SpawnData[] Towers;
    [SerializeField] Level[] Levels;

    public Level GetLevel(int index) => Levels[index].GetCopy();

    public SpawnData this[ObjectType type, int id]
    {
        get
        {
            return type == ObjectType.Enemy ? Enemies.First(item => item.ID == id) : Towers.First(item => item.ID == id);
        }
    }
}
