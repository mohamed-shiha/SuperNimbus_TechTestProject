using System.Linq;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] WorldObject[] Prefabs;
    ObjectsQueueSpawner<NormalBullet> bulletSpawner;
    ObjectsListSpawner<EnemyController> enemySpawner;
    ObjectsListSpawner<TowerController> towerSpawner;

    #region Debug
    [SerializeField] bool _Debug;
    [SerializeField] TextMeshProUGUI Debugtext;
    #endregion

    public void Initialize()
    {
        bulletSpawner = new ObjectsQueueSpawner<NormalBullet>((i) => MakeObject<NormalBullet>(i));
        enemySpawner = new ObjectsListSpawner<EnemyController>((i) => MakeObject<EnemyController>(i));
        towerSpawner = new ObjectsListSpawner<TowerController>((i) => MakeObject<TowerController>(i));
    }

    public void SpawnBullet(Vector2 pos, SpawnData data)
    {
        var bullet = bulletSpawner.GiveMeOne();
        bullet.SetData(data);
        bullet.RestartAlive(pos, Vector2.right);
        DebugUpdateText();
    }

    public void SpawnEnemy(int id, Vector2 pos, SpawnData data)
    {
        var enemy = enemySpawner.GetByID(id);
        enemy.SetData(data);
        enemy.RestartAlive(pos, Vector2.left);
        DebugUpdateText();
    }

    public void SpawnTower(int id, Vector2 pos, SpawnData data)
    {
        var tower = towerSpawner.GetByID(id);
        tower.SetData(data);
        tower.RestartAlive(pos, Vector2.zero);
        DebugUpdateText();
    }


    internal T MakeObject<T>(int id) where T : WorldObject
    {
        T prefabe = Prefabs.OfType<T>().First(obj => obj.ID == id);
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

        if (returnedObject is TowerController tower)
        {
            towerSpawner.TakeBack(tower);
            return;
        }

    }

    private void DebugUpdateText()
    {
        if (!_Debug) return;

        Debugtext.text = string.Format(
            $"bullet: {bulletSpawner.counter}\n" +
            $"enemy: {enemySpawner.counter}\n"
            );
    }
}