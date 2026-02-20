using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("1. ค่าพลังชีวิต")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("2. ระบบอมตะ (I-Frames)")]
    public float invincibilityDuration = 1.5f;
    private float invincibilityTimer;
    public bool isInvincible = false;

    [Header("3. UI & Effects")]
    public GameObject[] hearts;      // ลากรูปหัวใจใน UI มาใส่
    public Image redFlashImage;     // ลาก Image สีแดงเต็มจอมาใส่
    public SimpleCameraFollow cam;  // ลากกล้องมาใส่เพื่อให้จอสั่น

    [Header("4. เสียงประกอบ")]
    public AudioClip deathSound;
    public AudioClip hurtSound;
    private AudioSource audioSource;

    private SpriteRenderer playerSprite;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        playerSprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        UpdateHealthUI();
    }

    void Update()
    {
        // เอฟเฟกต์กะพริบตัวตอนอมตะ
        if (invincibilityTimer > 0 && currentHealth > 0 && Time.timeScale > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            float blinkSpeed = 0.1f;
            if (playerSprite != null)
                playerSprite.enabled = (Mathf.Repeat(Time.time, blinkSpeed * 2) > blinkSpeed);
        }
        else
        {
            if (playerSprite != null && !playerSprite.enabled) playerSprite.enabled = true;
        }

        // เอฟเฟกต์จอแดงจางหาย
        if (redFlashImage != null && redFlashImage.color.a > 0)
        {
            Color c = redFlashImage.color;
            c.a -= Time.deltaTime * 2f;
            redFlashImage.color = c;
        }
    }

    public void TakeDamage(int damage)
    {
        // ถ้าอมตะอยู่ หรือตายไปแล้ว ไม่ต้องรับดาเมจ
        if (isInvincible || invincibilityTimer > 0 || currentHealth <= 0 || isDead) return;

        currentHealth -= damage;
        invincibilityTimer = invincibilityDuration;
        UpdateHealthUI();

        // เล่นเสียงเจ็บ
        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        // จอสั่นและจอแดง
        if (cam != null) cam.TriggerShake(0.2f, 0.15f);
        if (redFlashImage != null) redFlashImage.color = new Color(1, 0, 0, 0.5f);

        if (currentHealth <= 0)
        {
            StartCoroutine(EpicDeathSequence());
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (hearts == null || hearts.Length == 0) return;
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i] != null) hearts[i].SetActive(i < currentHealth);
    }

    IEnumerator EpicDeathSequence()
    {
        isDead = true;

        // --- 📊 ส่วนการบันทึกสถิติการตาย (Ranking System) ---
        // ดึงชื่อจาก MainMenuManager ที่บันทึกไว้
        string playerName = PlayerPrefs.GetString("CurrentPlayerName", "Guest");

        // ดึงจำนวนตายเดิมของชื่อนี้มา แล้วบวก 1
        int currentDeaths = PlayerPrefs.GetInt("Deaths_" + playerName, 0);
        PlayerPrefs.SetInt("Deaths_" + playerName, currentDeaths + 1);
        PlayerPrefs.Save();
        // ------------------------------------------------

        // เล่นเสียงตาย
        if (audioSource != null && deathSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(deathSound);
        }

        // ปิดการควบคุมและคอลไลเดอร์
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (playerSprite != null) { playerSprite.enabled = true; playerSprite.sortingOrder = 100; }

        // เอฟเฟกต์ดีดตัวขึ้นฟ้าและหมุนตัว
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f;
            rb.linearVelocity = new Vector2(0, 15f);
        }

        float timer = 0f;
        Vector3 startScale = transform.localScale;
        while (timer < 2.0f)
        {
            // ตัวขยายใหญ่ขึ้นเรื่อยๆ ตอนตาย
            transform.localScale = Vector3.Lerp(startScale, startScale * 5f, timer / 0.5f);
            transform.Rotate(0, 0, 1000f * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        // เรียกหน้าจอ Game Over
        FindFirstObjectByType<GameOverManager>()?.PlayerDied();
    }
}