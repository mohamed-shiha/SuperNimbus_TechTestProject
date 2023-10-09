using UnityEngine;

public abstract class WorldObject : MonoBehaviour, ISpawned
{

    public ObjectType GetHitFrom;
    [SerializeField] bool _UseDebugData;
    SpawnData _data;
    SpawnData _debugData = new SpawnData(0, 1, ObjectType.NA, "MockData", 3, 1);
    public SpawnData Data { get { return _UseDebugData ? _debugData : _data; } private set { _data = value; } }

    int CurrentHits;

    Rigidbody2D rigidbodyS;
    void Awake()
    {
        rigidbodyS = GetComponent<Rigidbody2D>();
    }

    public virtual void OnDeath()
    {
        Debug.Log(string.Format($"The object with the ID: {Data.ID} Triggered OnDeath"));
        GameManager.Instance.OnObjectDeath(this);
    }

    public void Die()
    {
        // on any object death it will get back to the objects queue and set it's position far from the screen.
        OnDeath();
        rigidbodyS.velocity = Vector2.zero;
        var random = Random.Range(1, 100);
        transform.position = Data.Type == ObjectType.Enemy ? Vector3.one * 555 * random  : Vector3.one * -555 * random;
        gameObject.SetActive(false);
    }

    public void RestartAlive(Vector2 newPos, Vector2 dir)
    {

        gameObject.SetActive(true);
        transform.position = newPos;
        // reset the health to be full health
        CurrentHits = 0;
        //_data.MovementDirection = dir;
        rigidbodyS.velocity = Data.Speed * dir;
    }

    public void SetData(SpawnData spawnData)
    {
        Data = spawnData;
    }

    public void GetHit()
    {
        CurrentHits += 1;
        Debug.Log(name + ": Got damage " + CurrentHits);
        if (CurrentHits >= Data.HitsToDie) Die();
    }
}
