using UnityEngine;
using UnityEngine.SceneManagement; // ใช้สำหรับเปลี่ยนฉาก

public class LevelCompleteManager : MonoBehaviour
{
    [Header("ลากหน้าจอ LevelCompleteUI มาใส่ตรงนี้")]
    public GameObject levelCompleteUI;

    void Start()
    {
        if (levelCompleteUI != null) levelCompleteUI.SetActive(false);
    }

    public void ShowLevelComplete()
    {
        if (levelCompleteUI != null) levelCompleteUI.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลา

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }

    // --- ส่วนที่เพิ่มใหม่ ---
    public void RestartLevel()
    {
        Time.timeScale = 1f; // ต้องคืนค่าเวลาก่อนโหลดฉากใหม่!
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // เปลี่ยนชื่อให้ตรงกับฉากเมนูของคุณ
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}