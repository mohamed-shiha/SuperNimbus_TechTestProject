using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] WorldObject[] Prefabs;
    ObjectsSpawner<NormalBullet> bulletSpawner;
    ObjectsSpawner<EnemyController> enemySpawner;
    ObjectsSpawner<Tower> towersSpawner;

    public void Initialize()
    {
        bulletSpawner = new ObjectsSpawner<NormalBullet>((i) => MakeObject<NormalBullet>(i));
        enemySpawner = new ObjectsSpawner<EnemyController>((i) => MakeObject<EnemyController>(i));
        towersSpawner = new ObjectsSpawner<Tower>((i) => MakeObject<Tower>(i));

    }

    public void SpawnBullet(Vector2 pos, SpawnData data)
    {
        var bullet = bulletSpawner.GiveMeOne();
        bullet.SetData(data);
        bullet.RestartAlive(pos, Vector2.right);
    }

    public void SpawnEnemy(Vector2 pos, SpawnData data)
    {
        var enemy = enemySpawner.GiveMeOne();
        enemy.SetData(data);
        enemy.RestartAlive(pos, Vector2.left);
    }

    internal T MakeObject<T>(int id = 0) where T : WorldObject
    {
        T prefabe = Prefabs.OfType<T>().First();
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
        Debug.Log("Called on " + returnedObject);
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