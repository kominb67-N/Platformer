using UnityEngine;

public class DisappearingBlock : MonoBehaviour
{
    // 1. ทำงานเมื่อชนแบบของแข็ง (ไม่ได้ติ๊ก Is Trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }

    // 2. ทำงานเมื่อเดินทะลุเข้าไป (ติ๊ก Is Trigger ไว้)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}