using UnityEngine;

public class TowerController : WorldObject
{
    [SerializeField] Transform FirePoint;
    Animator animator;

    private void Start()
    {
        SetData(new SpawnData(0, Random.Range(0.1f, 1), ObjectType.Bullet, "Defender", 0, 1));
        animator = GetComponent<Animator>();
        animator.SetBool("Attack", true);
        animator.SetFloat("AttackSpeed", Data.Speed);
    }

    public void Fire()
    {
        GameManager.Instance.Fire(FirePoint.position);
    }
}
