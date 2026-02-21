using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("ตั้งค่าการเปลี่ยนฉาก")]
    public string gameSceneName = "Level1";
    public TMP_InputField nameInputField;

    [Header("ตั้งค่าปุ่ม")]
    public GameObject continueButton; // ลากปุ่ม Continue มาใส่ช่องนี้

    void Start()
    {
        // เช็กว่าเคยมีเซฟหรือไม่ (1 = มีเซฟ, 0 = ไม่มี)
        if (PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            if (continueButton != null) continueButton.SetActive(true); // โชว์ปุ่มเล่นต่อ
        }
        else
        {
            if (continueButton != null) continueButton.SetActive(false); // ซ่อนปุ่มเล่นต่อ
        }
    }

    // --- ฟังก์ชัน 1: เริ่มเกมใหม่ (New Game) ---
    public void NewGame()
    {
        SavePlayerName();

        // ล้างข้อมูลเซฟทั้งหมด
        PlayerPrefs.SetInt("HasSave", 0);
        PlayerPrefs.Save();

        // บอก CheckpointManager ว่าไม่ต้องใช้จุดเซฟ
        CheckpointManager.hasCheckpoint = false;

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    // --- ฟังก์ชัน 2: เล่นต่อ (Continue) ---
    public void ContinueGame()
    {
        SavePlayerName();

        // บอก CheckpointManager ว่าให้โหลดจุดเซฟด้วย
        CheckpointManager.hasCheckpoint = true;

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void SavePlayerName()
    {
        string playerName = nameInputField != null ? nameInputField.text.Trim() : "Guest";
        if (string.IsNullOrEmpty(playerName)) playerName = "Guest";

        PlayerPrefs.SetString("CurrentPlayerName", playerName);

        string allNames = PlayerPrefs.GetString("AllPlayerNames", "");
        if (!allNames.Contains(playerName))
        {
            allNames += playerName + ",";
            PlayerPrefs.SetString("AllPlayerNames", allNames);
        }
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Debug.Log("ออกจากเกมแล้ว!");
        Application.Quit();
    }
}