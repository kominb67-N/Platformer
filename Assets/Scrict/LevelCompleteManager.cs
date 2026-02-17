using UnityEngine;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("ลากหน้าจอ LevelCompleteUI มาใส่ตรงนี้")]
    public GameObject levelCompleteUI;

    void Start()
    {
        // ซ่อนหน้าจอไว้ก่อนตอนเริ่มเกม
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อผู้เล่นชนเส้นชัย
    public void ShowLevelComplete()
    {
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true); // แสดงหน้า UI
        }

        Time.timeScale = 0f; // หยุดเวลาในเกม
    }

    // ฟังก์ชันใหม่สำหรับปุ่ม "ออกเกม"
    public void QuitGame()
    {
        // ข้อความนี้จะเด้งในหน้าต่าง Console เพื่อให้รู้ว่าปุ่มทำงานแล้ว
        Debug.Log("ออกจาเกมแล้ว! (คำสั่งนี้จะเห็นผลตอน Build เกมออกมาเล่นจริงเท่านั้น ในหน้าต่าง Editor เกมจะไม่ปิดลงไปนะครับ)");

        // สั่งปิดแอปพลิเคชัน
        Application.Quit();
    }
}