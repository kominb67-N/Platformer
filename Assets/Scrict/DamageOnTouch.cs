using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ดึงสคริปต์ PlayerHealth จากตัวละคร แล้วสั่งลดเลือด 1 ดวง
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}