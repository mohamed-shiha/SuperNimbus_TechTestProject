using System.Linq;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] WorldObject[] Prefabs;
    [SerializeField] int RewardID;
    ObjectsQueueSpawner<NormalBullet> bulletSpawner;
    ObjectsQueueSpawner<RewardDisplayObject> rewardsSpawner;
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
        rewardsSpawner = new ObjectsQueueSpawner<RewardDisplayObject>((i) => MakeObject<RewardDisplayObject>(RewardID));
    }

    public void SpawnBullet(Vector2 pos, SpawnData data)
    {
        var bullet = bulletSpawner.GiveMeOne(data.ID);
        bullet.SetData(data);
        bullet.RestartAlive(pos, Vector2.right);
        DebugUpdateText();
    }

    public void SpawnRewardUI(Vector2 pos, SpawnData data)
    {
        var reward = rewardsSpawner.GiveMeOne();
        reward.SetData(data);
        reward.RestartAlive(pos, Vector2.right);
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
        try
        {
            T prefabe = Prefabs.OfType<T>().First(obj => obj.ID == id);
            T result;
            result = Instantiate(prefabe);
            return result;
        }
        catch (System.Exception)
        {

            Debug.LogError($"Cannot find prefabe of type {typeof(T)}");
            throw;
        }   
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

        if (returnedObject is RewardDisplayObject reward)
        {
            rewardsSpawner.TakeBack(reward);
            return;
        }

    }

    private void DebugUpdateText()
    {
        if (!_Debug) return;

        Debugtext.text = string.Format(
            $"bullet: {bulletSpawner.counter}\n" +
            $"towers: {towerSpawner.counter}\n" +
            $"rewards: {rewardsSpawner.counter}\n" +
            $"enemy: {enemySpawner.counter}\n"
            );
    }
}