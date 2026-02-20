using UnityEngine;
using System.Collections;

public class TrollSpring : MonoBehaviour
{
    [Header("ความแรงในการทารุณกรรม")]
    public float bounceForce = 25f; // ความแรงที่ใช้ส่งไปสวรรค์

    [Header("เอฟเฟกต์บล็อกกระแทก")]
    public float bumpDistance = 0.2f; // ระยะที่บล็อกจะยุบลงไปตอนโดนเหยียบ
    public float bumpSpeed = 0.05f;   // ความเร็วในการกระแทก (ยิ่งค่าน้อย ยิ่งกระแทกไว)

    [Header("กราฟิกและเสียง (ปล่อยว่างได้ถ้าใช้เป็นพื้น)")]
    public Sprite springUpSprite;
    public AudioClip boingSound;

    private SpriteRenderer sr;
    private Sprite originalSprite;
    private Vector3 originalPos;
    private bool isBumping = false; // ป้องกันการกระแทกซ้อนกันหลายรอบ

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null) originalSprite = sr.sprite;

        // จำตำแหน่งเดิมของบล็อกไว้
        originalPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // เช็กว่าคนที่มาชนคือ "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // เช็กว่ามา "เหยียบจากด้านบน"
            if (collision.GetContact(0).normal.y < -0.5f)
            {
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // 1. รีเซ็ตความเร็วเดิมก่อน
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

                    // 2. อัดแรงดีดขึ้นฟ้าเต็มแม็กซ์!
                    rb.linearVelocity += Vector2.up * bounceForce;

                    // 3. เล่นเอฟเฟกต์บล็อกกระแทก (ถ้ายังไม่เล่นอยู่)
                    if (!isBumping)
                    {
                        StartCoroutine(PlayBumpEffect());
                    }
                }
            }
        }
    }

    IEnumerator PlayBumpEffect()
    {
        isBumping = true;

        // เล่นเสียง
        if (boingSound != null) AudioSource.PlayClipAtPoint(boingSound, transform.position);

        // เปลี่ยนรูป (ถ้ามีใส่ไว้)
        if (sr != null && springUpSprite != null) sr.sprite = springUpSprite;

        // --- เริ่มเอฟเฟกต์ยุบตัว ---
        Vector3 targetPos = originalPos + new Vector3(0, -bumpDistance, 0); // คำนวณจุดที่ยุบลง

        // จังหวะที่ 1: กดบล็อกลง
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / bumpSpeed;
            transform.position = Vector3.Lerp(originalPos, targetPos, t);
            yield return null;
        }

        // จังหวะที่ 2: ดันบล็อกกลับขึ้นมาที่เดิม
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / bumpSpeed;
            transform.position = Vector3.Lerp(targetPos, originalPos, t);
            yield return null;
        }

        // --- จบเอฟเฟกต์ ---
        // ล็อกตำแหน่งให้กลับมาเป๊ะๆ กันเบี้ยว
        transform.position = originalPos;

        // คืนรูปเดิม (ถ้ามีใส่ไว้)
        if (sr != null && springUpSprite != null) sr.sprite = originalSprite;

        isBumping = false;
    }
}