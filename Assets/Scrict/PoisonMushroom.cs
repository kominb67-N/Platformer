using UnityEngine;
using System.Collections;

public class PoisonMushroom : MonoBehaviour
{
    [Header("1. การเดินและฟิสิกส์")]
    public float moveSpeed = 2f;
    private int direction = 1;
    private Rigidbody2D rb;
    private bool isCollected = false;

    [Header("2. ตั้งค่าพิษ (Shrink & Slow)")]
    public float duration = 4f;           // ติดพิษนานกี่วินาที
    public float shrinkMultiplier = 0.4f;   // ตัวหดเหลือ 40%
    public float slowMultiplier = 0.5f;     // วิ่งช้าลงครึ่งนึง

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.freezeRotation = true; // ล็อกไม่ให้เห็ดหมุนกลิ้ง
    }

    void FixedUpdate()
    {
        // เดินไปข้างหน้าเรื่อยๆ ถ้ายังไม่มีใครกิน
        if (!isCollected && rb != null)
        {
            rb.linearVelocity = new Vector2(moveSpeed * direction, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isCollected) return;

        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(ApplyPoison(col.gameObject));
        }
        // ชนกำแพงแล้วกลับด้านเหมือนไอเทมปกติ
        else if (col.contacts.Length > 0 && Mathf.Abs(col.contacts[0].normal.x) > 0.5f)
        {
            direction *= -1;
        }
    }

    IEnumerator ApplyPoison(GameObject player)
    {
        isCollected = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        PlayerController move = player.GetComponent<PlayerController>();
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        // --- เริ่มเอฟเฟกต์ตัวหด (Shrink Animation) ---
        // หยุดเวลาชั่วคราวเพื่อให้เห็นเอฟเฟกต์ชัดๆ
        Time.timeScale = 0f;
        Vector3 originalScale = player.transform.localScale;
        Vector3 targetScale = originalScale * shrinkMultiplier;

        // ทำการกระตุกตัว หด-ขยาย 3 รอบก่อนจะตัวเล็กถาวร
        for (int i = 0; i < 3; i++)
        {
            player.transform.localScale = targetScale;
            yield return new WaitForSecondsRealtime(0.1f);
            player.transform.localScale = originalScale;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        player.transform.localScale = targetScale; // ตั้งค่าตัวเล็กถาวร
        Time.timeScale = 1f; // กลับมาเดินเครื่องเกมต่อ

        // --- สถานะติดพิษ ---
        if (move) move.moveSpeed *= slowMultiplier; // วิ่งช้าลง
        if (sr) sr.color = Color.magenta; // เปลี่ยนเป็นสีม่วง

        // รอจนกว่าจะหมดเวลา
        yield return new WaitForSeconds(duration);

        // --- คืนค่าร่างปกติ ---
        player.transform.localScale = originalScale; // กลับมาตัวเท่าเดิม
        if (move) move.moveSpeed /= slowMultiplier; // กลับมาวิ่งไวเท่าเดิม
        if (sr) sr.color = Color.white; // คืนสีปกติ

        Destroy(gameObject);
    }
}