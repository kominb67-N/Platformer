using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("ตั้งค่าการเปลี่ยนฉาก")]
    public string gameSceneName = "Level1";
    public TMP_InputField nameInputField; // ช่องใส่ชื่อที่สร้างไว้ก่อนหน้านี้

    public void PlayGame()
    {
        // ระบบจำชื่อเพื่อเก็บสถิติการตาย
        string playerName = nameInputField != null ? nameInputField.text.Trim() : "Guest";
        if (string.IsNullOrEmpty(playerName)) playerName = "Guest";

        PlayerPrefs.SetString("CurrentPlayerName", playerName);

        // จัดการรายชื่อรวม
        string allNames = PlayerPrefs.GetString("AllPlayerNames", "");
        if (!allNames.Contains(playerName))
        {
            allNames += playerName + ",";
            PlayerPrefs.SetString("AllPlayerNames", allNames);
        }
        PlayerPrefs.Save();

        // เริ่มเกม
        SceneManager.LoadScene(gameSceneName);
        Time.timeScale = 1f;
    }

    // --- ฟังก์ชันออกเกมที่หายไป ---
    public void QuitGame()
    {
        // ข้อความเช็กการทำงานใน Unity Editor
        Debug.Log("ออกจากเกมแล้ว! (คำสั่งนี้จะเห็นผลตอน Build เกมออกมาเล่นจริงเท่านั้น)");

        // สั่งปิดแอปพลิเคชัน
        Application.Quit();
    }
}