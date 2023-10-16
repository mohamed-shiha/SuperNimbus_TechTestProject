using UnityEngine;

public class NormalBullet : WorldObject
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("bullet hit " + collision.transform.name);
        // if we hit an object 
        if (collision.GetComponent<WorldObject>() is WorldObject other && other != null)
        {
            // if the object can take damage from this bullet then deal damage and recycle the bullet
            if (other.GetHitFrom == ObjectType.Bullet)
            {
                Debug.Log("Dealing Damage " + other.Data.Name);
                other.GetHit(Data);
                Die(other.Data);
            }
        }
    }
}
