using UnityEngine;
using System.Collections;

public class FallingPole : MonoBehaviour
{
    [Header("ความเร็วการฟาดและลุก")]
    public float fallSpeed = 350f;
    public float riseSpeed = 300f;
    public float waitTime = 1f;

    [Header("ดาเมจและกระเด็น (ตอนลุก)")]
    public int riseDamage = 1;
    public Vector2 knockbackForce = new Vector2(-15f, 15f);
    public float stunTime = 0.5f;

    [Header("แอนิเมชัน")]
    public string stunAnimTrigger = "Confused"; // ชื่อ Trigger แอนิเมชันงง

    private bool isFalling = false;
    private bool isRising = false;
    private bool isStomping = false;
    private bool isPoleActive = false;
    private bool hasKnockedBack = false;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    public void TriggerFall()
    {
        if (!isPoleActive)
        {
            StartCoroutine(TrollComboRoutine());
        }
    }

    IEnumerator TrollComboRoutine()
    {
        isPoleActive = true;
        hasKnockedBack = false;

        // --- 1. ล้ม ---
        isFalling = true;
        float angle = 0;
        while (angle > -90f)
        {
            angle -= fallSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, -90f);
        isFalling = false;

        yield return new WaitForSeconds(waitTime);

        // --- 2. ลุกพรวด! ---
        isRising = true;
        while (angle < 0f)
        {
            angle += riseSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 0f);
        isRising = false;

        yield return new WaitForSeconds(waitTime);

        // --- 3. ท่าไม้ตาย: หอกดาวตก! ---
        isStomping = true;

        Vector3 peakPos = startPos + new Vector3(0, 8f, 0);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, peakPos, t);
            yield return null;
        }

        float currentSpin = 0f;
        while (currentSpin < 180f)
        {
            float step = 600f * Time.deltaTime;
            transform.Rotate(0, 0, step);
            currentSpin += step;
            yield return null;
        }
        transform.rotation = Quaternion.Euler(0, 0, 180f);

        yield return new WaitForSeconds(0.6f);

        Vector3 smashPos = startPos + new Vector3(0, 4f, 0);

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            transform.position = Vector3.Lerp(peakPos, smashPos, t);
            yield return null;
        }

        isPoleActive = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isFalling || isStomping)
            {
                collision.GetComponent<PlayerHealth>()?.TakeDamage(99);
            }
            else if (isRising && !hasKnockedBack)
            {
                hasKnockedBack = true;
                StartCoroutine(ApplyKnockback(collision.gameObject));
            }
        }
    }

    IEnumerator ApplyKnockback(GameObject player)
    {
        // 1. ลดเลือด
        player.GetComponent<PlayerHealth>()?.TakeDamage(riseDamage);

        // 2. ปิดการควบคุม (มึน)
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        // *** 3. เล่นแอนิเมชันงง! ***
        Animator playerAnim = player.GetComponent<Animator>();
        if (playerAnim != null)
        {
            playerAnim.SetTrigger(stunAnimTrigger);
        }

        // 4. อัดแรงกระเด็น
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = knockbackForce;
        }

        // 5. รอให้ปลิวไปสักพัก
        yield return new WaitForSeconds(stunTime);

        // 6. คืนการควบคุม
        if (pc != null) pc.enabled = true;
    }
}