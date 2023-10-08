using UnityEngine;

public enum ObjectType
{
    Bullet,
    Enemy,
    Tower
}

public struct SpawnData
{
    public int ID;
    public float Speed;
    public ObjectType Type;
    public string Name;
    public int HitsToDie;

    public int CurrentHits;
    //public Vector2 MovementDirection;

    public SpawnData(int iD, float speed, ObjectType type, string name, int hitsToDie/*, Vector2 movementDirection*/)
    {
        ID = iD;
        Speed = speed;
        Type = type;
        Name = name;
        HitsToDie = hitsToDie;
        CurrentHits = 0;
        //MovementDirection = movementDirection;
    }
}


public interface ISpawned
{
    public void SetData(SpawnData spawnData);
    public void RestartAlive(Transform transform, Vector2 dir);
    public void Die();
}

public class NormalBullet : WorldObject
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if we hit an object 
        if (collision.GetComponent<WorldObject>() is WorldObject other && other != null)
        {
            // if the object can take damage from this bullet then deal damage and recycle the bullet
            if (other.GetHitFrom == ObjectType.Bullet)
            {
                other.GetHit();
                Die();
            }
        }
    }
}


public class EnemyController : WorldObject
{
    public override void OnDeath()
    {
        base.OnDeath();
        //spawn decals
        //give the player a reward
    }


}


public class Tower : WorldObject
{
    public override void OnDeath()
    {
        base.OnDeath();
        //spawn decals
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // when the tower get's hit by an world object that is should hit us then take famage
        if (collision.GetComponent<WorldObject>() is WorldObject other && GetHitFrom == other?.Data.Type)
        {
            if (GetHitFrom == other.Data.Type)
            {
                GetHit();
            }
        }
    }
}
