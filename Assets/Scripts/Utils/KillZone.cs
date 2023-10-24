using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] string TargetTag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        WorldObject other = collision.GetComponent<WorldObject>();
        if (other != null && other.tag.Equals(TargetTag, System.StringComparison.OrdinalIgnoreCase))
        {
            other.Die(new SpawnData() { Name = "KillZone"});
        }
    }
}
