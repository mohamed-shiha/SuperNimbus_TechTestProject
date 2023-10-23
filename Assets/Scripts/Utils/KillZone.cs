using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        WorldObject other = collision.GetComponent<WorldObject>();
        if (other != null)
        {
            other.Die(new SpawnData() { Name = "KillZone"});
        }
    }
}
