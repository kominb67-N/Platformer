using UnityEngine;
using UnityEngine.SceneManagement; // ต้องมีบรรทัดนี้เพื่อเปลี่ยนฉาก

public class MainMenuManager : MonoBehaviour
{
    [Header("ใส่ชื่อ Scene ของด่านที่คุณต้องการให้โหลดเมื่อกดเริ่มเกม")]
    public string gameSceneName = "Level1"; // เปลี่ยนข้อความตรงนี้ให้ตรงกับชื่อ Scene ด่านของคุณเป๊ะๆ นะครับ

    // ฟังก์ชันนี้ผูกกับปุ่ม Start Game
    public void PlayGame()
    {
        // สั่งให้โหลด Scene ตามชื่อที่ตั้งไว้
        SceneManager.LoadScene(gameSceneName);

        // ให้มั่นใจว่าเวลาในเกมเดินตามปกติ (เผื่อติดมาจากหน้า Game Over)
        Time.timeScale = 1f;
    }

    // ฟังก์ชันนี้ผูกกับปุ่ม Quit Game
    public void QuitGame()
    {
        Debug.Log("ออกจาเกมแล้ว! (คำสั่งนี้จะเห็นผลตอน Build เกมออกมาเล่นจริงเท่านั้น ใน Editor จะไม่ปิดให้)");
        Application.Quit();
    }
}