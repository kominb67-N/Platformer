using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("ลากหน้าจอ PauseUI มาใส่ตรงนี้")]
    public GameObject pauseMenuUI;

    // ตัวแปรเช็กสถานะว่าหยุดอยู่ไหม
    public static bool isPaused = false;

    void Start()
    {
        // เริ่มเกมมาต้องปิดหน้าจอนี้ไว้ก่อน
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        // ปุ่มลัด: กด Esc หรือ P เพื่อหยุด/ไปต่อ (สำหรับเล่นบนคอม)
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    // ฟังก์ชันหยุดเกม
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // หยุดเวลาเดิน
        isPaused = true;
    }

    // ฟังก์ชันเล่นต่อ
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // ให้เวลาเดินต่อ
        isPaused = false;
    }

    // ฟังก์ชันเริ่มด่านใหม่
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ฟังก์ชันกลับเมนูหลัก
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}