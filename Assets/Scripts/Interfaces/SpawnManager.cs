using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] WorldObject[] Prefabs;
    ObjectsSpawner<NormalBullet> bulletSpawner;
    ObjectsSpawner<EnemyController> enemySpawner;
    ObjectsSpawner<Tower> towersSpawner;

    public void SpawnBullet(Transform transform, SpawnData data)
    {
        var bullet = bulletSpawner.GiveMeOne();
        bullet.SetData(data);
        bullet.RestartAlive(transform, Vector2.right);
    }

    public void SpawnEnemy(Transform transform, SpawnData data)
    {
        var enemy = enemySpawner.GiveMeOne();
        enemy.SetData(data);
        enemy.RestartAlive(transform, Vector2.left);
    }

    internal T MakeObject<T>(int id) where T : WorldObject
    {
        T prefabe = (T)Prefabs.First(item => item.Data.ID == id);
        T result;
        if (prefabe == null)
        {
            Debug.LogError($"Cannot find prefabe of type {typeof(T)}");
            return default;
        }

        result = Instantiate(prefabe);
        return result;
    }

    public void TakeBackObject<T>(T returnedObject)
    {
        if (returnedObject is NormalBullet bullet)
        {
            bulletSpawner.TakeBack(bullet);
            return;
        }

        if (returnedObject is EnemyController enemy)
        {
            enemySpawner.TakeBack(enemy);
            return;
        }

        if (returnedObject is Tower tower)
        {
            towersSpawner.TakeBack(tower);
            return;
        }
    }


}