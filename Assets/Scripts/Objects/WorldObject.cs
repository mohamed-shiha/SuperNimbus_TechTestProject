using UnityEngine;

public abstract class WorldObject : MonoBehaviour, ISpawned
{
    public int ID;
    public ObjectType GetHitFrom;

    [SerializeField] bool _UseDebugData;

    protected int CurrentHits;
    Rigidbody2D rigidbodyS;
    SpawnData _data;
    SpawnData _debugData = new SpawnData(0, 1, ObjectType.NA, "MockData", 3, 1);
    public SpawnData Data { get { return _UseDebugData ? _debugData : _data; } private set { _data = value; } }


    void Awake()
    {
        rigidbodyS = GetComponent<Rigidbody2D>();
    }

    public virtual void OnDeath(SpawnData killer)
    {
        //Debug.Log($"The object with the ID: {Data.ID} Triggered OnDeath");
        GameManager.Instance.OnObjectDeath(this, killer);
    }

    public void Die(SpawnData killerData)
    {
        // on any object death it will get back to the objects queue and set it's position far from the screen.
        OnDeath(killerData);
        //rigidbodyS.velocity = Vector2.zero;
        var random = Random.Range(1, 100);
        transform.position = Data.Type == ObjectType.Enemy ? Vector3.one * 555 * random  : Vector3.one * -555 * random;
        gameObject.SetActive(false);
    }

    public virtual void RestartAlive(Vector2 newPos, Vector2 dir)
    {

        gameObject.SetActive(true);
        transform.position = newPos;
        // reset the health to be full health
        CurrentHits = 0;
        //_data.MovementDirection = dir;
        rigidbodyS.velocity = Data.Speed * dir;
    }

    public virtual void SetData(SpawnData spawnData)
    {
        Data = spawnData;
        ID = Data.ID;
    }

    public void GetHit(SpawnData hitFrom)
    {
        CurrentHits += 1;
        Debug.Log(name + ": Got damage " + CurrentHits);
        if (CurrentHits >= Data.HitsToDie) Die(hitFrom);
    }
}
