using UnityEngine;
using System.Collections;
using UnityEngine.UI; // สำหรับใช้งาน UI

public class PlayerHealth : MonoBehaviour
{
    [Header("ตั้งค่าเลือด")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("ใส่รูปหัวใจ UI ตรงนี้ (กด + หรือใส่ตัวเลข)")]
    public GameObject[] hearts;

    [Header("เอฟเฟกต์ตอนโดนตี")]
    public Image redFlashImage;
    public SimpleCameraFollow cam;

    public bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        // ถ้าเป็นอมตะอยู่ จะไม่รับดาเมจ
        if (isInvincible) return;

        // ลดเลือดและอัปเดตหน้าจอ
        currentHealth -= damage;
        UpdateHealthUI();

        // 1. สั่งกล้องสั่น
        if (cam != null) cam.TriggerShake(0.2f, 0.15f);

        // 2. สั่งเปิดจอแดง
        if (redFlashImage != null) StartCoroutine(FlashRedScreen());

        // เช็คว่าตายหรือยัง
        if (currentHealth <= 0)
        {
            // Use the non-obsolete API to find the GameOverManager instance
            FindFirstObjectByType<GameOverManager>()?.PlayerDied();
        }
        else
        {
            // ถ้ายังไม่ตาย ให้เป็นอมตะชั่วคราว 1.5 วินาที
            StartCoroutine(InvincibilityRoutine());
        }
    }

    void UpdateHealthUI()
    {
        // ถ้าไม่ได้ใส่รูปหัวใจไว้ ให้ข้ามไปเลย จะได้ไม่เกิด Error
        if (hearts == null || hearts.Length == 0) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth) hearts[i].SetActive(true);
            else hearts[i].SetActive(false);
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(1.5f);
        isInvincible = false;
    }

    IEnumerator FlashRedScreen()
    {
        if (redFlashImage == null) yield break;

        redFlashImage.color = new Color(1f, 0f, 0f, 0.5f);

        while (redFlashImage.color.a > 0)
        {
            float newAlpha = redFlashImage.color.a - (Time.deltaTime * 2f);
            redFlashImage.color = new Color(1f, 0f, 0f, newAlpha);
            yield return null;
        }
    }
}