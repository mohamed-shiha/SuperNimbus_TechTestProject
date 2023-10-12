using UnityEngine;

public class TowerController : WorldObject
{
    [SerializeField] Transform FirePoint;
    Animator animator;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SetData(new SpawnData(0, Random.Range(0.1f, 1), ObjectType.Bullet, "Defender", 0, 1));
    }

    public override void SetData(SpawnData spawnData)
    {
        base.SetData(spawnData);
        if (animator != null)
        {
            animator.SetBool("Attack", true);
            animator.SetFloat("AttackSpeed", Data.Speed);
        }
        else
        {
            Debug.LogWarning("Animator was not found");
        }
    }

    public void Fire()
    {
        GameManager.Instance.Fire(FirePoint.position);
    }
}
