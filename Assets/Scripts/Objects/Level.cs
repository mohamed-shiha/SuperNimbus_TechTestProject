
public class Level
{

    int[] SpawnOrder;
    int currentEnemy;
    int StartingGold;

    public float SpawnSpeed;
    public int EnemyCount => SpawnOrder.Length;

    public Level(int[] spawnOrder, int startingGold)
    {
        SpawnOrder = spawnOrder;
        StartingGold = startingGold;
    }

    public Level(int[] spawnOrder)
    {
        SpawnOrder = spawnOrder;
    }

    public int GetAndMove()
    {
        if(currentEnemy >= SpawnOrder.Length)
        {
            currentEnemy = 0;
        }
        return SpawnOrder[currentEnemy++];
    }

    public bool HasNext()
    {
        return currentEnemy < SpawnOrder.Length;
    }
}