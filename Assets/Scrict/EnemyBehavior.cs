using UnityEngine;
using System.Collections;

public class EnemyBehavior : MonoBehaviour
{
    [Header("ตั้งค่าศัตรู")]
    public float moveSpeed = 2f;     // ความเร็วในการเดิน
    public int damage = 1;           // พลังโจมตีเมื่อผู้เล่นเดินชน
    public float bounceForce = 7f;   // แรงเด้งของตัวละครตอนเหยียบหัวศัตรู

    private Rigidbody2D rb;
    private bool movingRight = true; // กำหนดให้เริ่มเดินไปทางขวาก่อน
    private bool isDead = false;     // สถานะว่าตายหรือยัง

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    [System.Obsolete]
    void Update()
    {
        if (isDead) return; // ถ้าตายแล้ว ให้หยุดทำงานทันที

        // สั่งให้ศัตรูเดินซ้าย-ขวาอย่างต่อเนื่อง
        rb.linearVelocity = new Vector2(movingRight ? moveSpeed : -moveSpeed, rb.linearVelocity.y);
    }

    [System.Obsolete]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // --- 1. ตรวจสอบการชนกับ "ผู้เล่น" ---
        if (collision.gameObject.CompareTag("Player"))
        {
            // ดึงข้อมูลจุดที่ชนกันมาตรวจสอบทิศทาง
            ContactPoint2D contact = collision.GetContact(0);

            // normal.y ที่ติดลบ (น้อยกว่า -0.5) แปลว่าผู้เล่นลอยอยู่เหนือศัตรูแล้วตกลงมากระแทก
            if (contact.normal.y < -0.5f)
            {
                // สั่งให้ผู้เล่นเด้งกระโดดขึ้นไป
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                }

                // สั่งให้ศัตรูตัวนี้โดนบี้ตาย
                StartCoroutine(SquashAndDie());
            }
            else
            {
                // ถ้าไม่ได้ตกลงมาจากด้านบน แปลว่าเป็นการชนด้านข้าง = ผู้เล่นโดนดาเมจ
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
        // --- 2. ตรวจสอบการชนกับ "สิ่งแวดล้อมอื่น" (กำแพง, บล็อก, ศัตรูด้วยกัน) ---
        else
        {
            ContactPoint2D contact = collision.GetContact(0);

            // ตรวจสอบว่าเป็นการชนเข้าที่ด้านข้างซ้ายหรือขวาอย่างจังหรือไม่
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                Flip(); // สั่งให้หันหลังกลับ
            }
        }
    }

    // ฟังก์ชันสำหรับกลับหลังหัน
    void Flip()
    {
        movingRight = !movingRight;

        // กลับด้านรูปภาพศัตรู
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ฟังก์ชันแอนิเมชันโดนบี้ตาย
    IEnumerator SquashAndDie()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero; // สั่งให้หยุดเดินทันที

        // ปิดกล่องชน จะได้ไม่พลาดไปทำดาเมจใส่ผู้เล่นซ้ำตอนกำลังจะตาย
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // เตรียมทำเอฟเฟกต์บี้ (ลดขนาดความสูง แกน Y ลง)
        Vector3 originalScale = transform.localScale;

        // บี้ลงเหลือความสูงแค่ 20%
        Vector3 squashedScale = new Vector3(originalScale.x, originalScale.y * 0.2f, originalScale.z);

        float timer = 0f;
        float squashDuration = 0.15f; // ใช้เวลาในการแบนลง 0.15 วินาที (เร็วมากๆ)

        while (timer < squashDuration)
        {
            // ค่อยๆ ลดขนาดลงจนแบนติดพื้น
            transform.localScale = Vector3.Lerp(originalScale, squashedScale, timer / squashDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // แบนเสร็จแล้ว ให้ทำลายตัวเองทิ้งออกจากฉาก
        Destroy(gameObject);
    }
}