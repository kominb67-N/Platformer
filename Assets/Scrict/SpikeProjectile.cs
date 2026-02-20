using UnityEngine;

public class SpikeProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 5f;
    public int damage = 1;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // ป้องกันหนามหมุนคว้าง
        rb.freezeRotation = true;
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // เปลี่ยนจาก transform.right (ขวาของฉัน) เป็น Vector2.right (ขวาของโลก)
            // ทิศนี้จะไปทางบวกของแกน X ในโลก 2D เสมอ ไม่ว่าวัตถุจะหมุนไปทางไหน
            rb.linearVelocity = Vector2.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}