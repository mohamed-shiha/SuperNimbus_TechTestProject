using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] WorldObject[] Prefabs;
    ObjectsSpawner<NormalBullet> bulletSpawner;
    ObjectsSpawner<TowerController> enemySpawner;
    ObjectsSpawner<EnemyController> towersSpawner;

    #region Debug
    [SerializeField] bool _Debug;
    [SerializeField] TextMeshProUGUI Debugtext;
    #endregion

    public void Initialize()
    {
        bulletSpawner = new ObjectsSpawner<NormalBullet>((i) => MakeObject<NormalBullet>(i));
        enemySpawner = new ObjectsSpawner<TowerController>((i) => MakeObject<TowerController>(i));
        towersSpawner = new ObjectsSpawner<EnemyController>((i) => MakeObject<EnemyController>(i));

    }

    public void SpawnBullet(Vector2 pos, SpawnData data)
    {
        var bullet = bulletSpawner.GiveMeOne();
        bullet.SetData(data);
        bullet.RestartAlive(pos, Vector2.right);
        DebugUpdateText();
    }

    public void SpawnEnemy(Vector2 pos, SpawnData data)
    {
        var enemy = enemySpawner.GiveMeOne();
        enemy.SetData(data);
        enemy.RestartAlive(pos, Vector2.left);
        DebugUpdateText();
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
        if (returnedObject is NormalBullet bullet)
        {
            bulletSpawner.TakeBack(bullet);
            return;
        }

        if (returnedObject is TowerController enemy)
        {
            enemySpawner.TakeBack(enemy);
            return;
        }

        if (returnedObject is EnemyController tower)
        {
            towersSpawner.TakeBack(tower);
            return;
        }
    }

    private void DebugUpdateText()
    {
        if (!_Debug) return;

        Debugtext.text = string.Format(
            $"bullet: {bulletSpawner.counter}\n" +
            $"towers: {towersSpawner.counter}\n" +
            $"enemy: {enemySpawner.counter}\n"
            );
    }
}