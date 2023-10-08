using UnityEngine;

public abstract class WorldObject : MonoBehaviour, ISpawned
{

    public ObjectType GetHitFrom;
    SpawnData _data;
    public SpawnData Data { get { return _data; } private set { _data = value; } }

    Rigidbody2D rigidbodyS;
    void Awake()
    {
        rigidbodyS = GetComponent<Rigidbody2D>();
    }

    public virtual void OnDeath()
    {
        Debug.Log(string.Format($"The object with the ID: {Data.ID} Triggered OnDeath"));
    }

    public void Die()
    {
        // on any object death it will get back to the objects queue and set it's position far from the screen.
        OnDeath();
        rigidbodyS.velocity = Vector2.zero;
        var random = Random.Range(1, 100);
        transform.position = Data.Type == ObjectType.Enemy ? Vector3.one * 555 * random  : Vector3.one * -555 * random;
    }

    public void RestartAlive(Transform newTransform, Vector2 dir)
    {
        transform.position = newTransform.position;
        // reset the health to be full health
        _data.CurrentHits = 0;
        //_data.MovementDirection = dir;
        rigidbodyS.velocity = Data.Speed * dir;
    }

    public void SetData(SpawnData spawnData)
    {
        Data = spawnData;
    }

    public void GetHit()
    {
        _data.CurrentHits += 1;
        if (Data.CurrentHits == Data.HitsToDie) Die();
    }
}
