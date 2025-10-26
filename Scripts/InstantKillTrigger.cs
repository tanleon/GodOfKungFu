using UnityEngine;

public class InstantKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Damageable dmg = other.GetComponent<Damageable>();
            if (dmg != null)
            {
                dmg.IsAlive = false;
            }
        }
    }
}
