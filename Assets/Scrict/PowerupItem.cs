using UnityEngine;
using System.Collections;

public class PowerupItem : MonoBehaviour
{
    [Header("ตั้งค่าไอเทม")]
    public float duration = 4f;        // ระยะเวลาบัฟ (4 วินาที)
    public float scaleMultiplier = 2f; // ขยายขนาดตัว (2 เท่า)
    public float speedMultiplier = 2f; // เพิ่มความเร็ว (2 เท่า)

    [Header("ตั้งค่าการกระโดด")]
    public int extraJumps = 1;         // จำนวนการกระโดดที่เพิ่มขึ้น (เช่น +1 คือกระโดดกลางอากาศเพิ่มได้อีก 1 ครั้ง)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ตรวจสอบว่าคนที่มาชนคือผู้เล่นหรือไม่
        if (other.CompareTag("Player"))
        {
            // เริ่มการทำงานของไอเทม
            StartCoroutine(ApplyPowerup(other.gameObject));
        }
    }

    private IEnumerator ApplyPowerup(GameObject player)
    {
        // ==========================================
        // ส่วนที่ 1: เริ่มผลของไอเทม
        // ==========================================

        // ซ่อนรูปภาพและกล่องชนของไอเทม (ให้เหมือนเก็บไปแล้ว แต่สคริปต์ยังรันอยู่)
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // 1. เปิดโหมดอมตะ
        PlayerHealth healthScript = player.GetComponent<PlayerHealth>();
        if (healthScript != null)
        {
            healthScript.isInvincible = true;
        }

        // 2. ทำให้ตัวใหญ่ 2 เท่า (เก็บค่าขนาดเดิมไว้ก่อน)
        Vector3 originalScale = player.transform.localScale;
        player.transform.localScale = originalScale * scaleMultiplier;

        // 3. เพิ่มความเร็ว 2 เท่า และ เพิ่มจำนวนการกระโดด
        PlayerController movementScript = player.GetComponent<PlayerController>();
        if (movementScript != null)
        {
            movementScript.moveSpeed *= speedMultiplier;

            // ⚠️ สำคัญ: เปลี่ยนคำว่า maxJumps ให้ตรงกับ "ชื่อตัวแปรจำนวนการกระโดด" ในสคริปต์ PlayerController ของคุณ
            movementScript.maxJumps += extraJumps;
        }

        // ==========================================
        // ส่วนที่ 2: รอเวลา
        // ==========================================
        yield return new WaitForSeconds(duration); // รอ 4 วินาที

        // ==========================================
        // ส่วนที่ 3: หมดเวลา คืนค่าทุกอย่าง
        // ==========================================

        // 1. ปิดโหมดอมตะ
        if (healthScript != null)
        {
            healthScript.isInvincible = false;
        }

        // 2. หดตัวกลับเท่าเดิม
        player.transform.localScale = originalScale;

        // 3. ลดความเร็ว และ ลดจำนวนการกระโดด กลับเท่าเดิม
        if (movementScript != null)
        {
            movementScript.moveSpeed /= speedMultiplier;

            // ⚠️ สำคัญ: เปลี่ยนคำว่า maxJumps ให้ตรงกับด้านบน
            movementScript.maxJumps -= extraJumps;
        }

        // ทำลายไอเทมชิ้นนี้ทิ้งออกจากฉากอย่างสมบูรณ์
        Destroy(gameObject);
    }
}