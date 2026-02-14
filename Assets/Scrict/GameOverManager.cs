using UnityEngine;
using UnityEngine.SceneManagement; // จำเป็นต้องมีบรรทัดนี้เพื่อใช้คำสั่งโหลดฉาก

public class GameOverManager : MonoBehaviour
{
    [Header("ลาก Panel Game Over มาใส่ตรงนี้")]
    public GameObject gameOverUI;

    void Start()
    {
        // ซ่อนหน้า Game Over ไว้เสมอตอนเริ่มด่าน
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อผู้เล่นตาย
    public void PlayerDied()
    {
        gameOverUI.SetActive(true); // โชว์หน้า Game Over
        Time.timeScale = 0f; // หยุดเวลาในเกม (ทำให้ตัวละครและศัตรูหยุดนิ่ง)
    }

    // ฟังก์ชันนี้จะผูกไว้กับปุ่ม Restart
    public void RestartGame()
    {
        Time.timeScale = 1f; // คืนค่าเวลาให้กลับมาเดินตามปกติก่อนโหลดฉาก

        // โหลดด่านปัจจุบันที่กำลังเล่นอยู่ขึ้นมาใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}