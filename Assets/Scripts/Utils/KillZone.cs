using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Kill zone hit " + collision.transform.name);
        WorldObject other = collision.GetComponent<WorldObject>();
        if (other != null)
        {
            other.Die();
        }
    }
}
