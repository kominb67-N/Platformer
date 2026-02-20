using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq; // สำหรับการเรียงลำดับ

public class ScoreboardManager : MonoBehaviour
{
    public TMP_Text scoreboardText; // ลาก TextMeshPro ที่จะโชว์ลำดับมาใส่
    public GameObject scoreboardPanel;

    // คลาสสำหรับเก็บข้อมูลชั่วคราวเพื่อเอามาเรียง
    private class PlayerData
    {
        public string name;
        public int deaths;
    }

    public void ShowScoreboard()
    {
        scoreboardPanel.SetActive(true);

        // 1. ดึงรายชื่อทั้งหมดออกมา
        string allNamesRaw = PlayerPrefs.GetString("AllPlayerNames", "");
        string[] names = allNamesRaw.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

        List<PlayerData> playerList = new List<PlayerData>();

        // 2. ไปดึงคะแนน (จำนวนตาย) ของแต่ละชื่อมาใส่ List
        foreach (string n in names)
        {
            int d = PlayerPrefs.GetInt("Deaths_" + n, 0);
            playerList.Add(new PlayerData { name = n, deaths = d });
        }

        // 3. เรียงลำดับจาก ตายเยอะที่สุด ไป ตายน้อยที่สุด
        playerList = playerList.OrderByDescending(p => p.deaths).ToList();

        // 4. สร้างข้อความเพื่อเอาไปโชว์
        string summary = "🏆 DEATH RANKING 🏆\n\n";
        int rank = 1;
        foreach (var p in playerList)
        {
            summary += rank + ". " + p.name + " : " + p.deaths + " Deaths\n";
            rank++;
        }

        if (playerList.Count == 0) summary = "No stats found yet!";

        scoreboardText.text = summary;
    }

    public void CloseScoreboard()
    {
        scoreboardPanel.SetActive(false);
    }

    // (แถม) ปุ่มล้างสถิติทั้งหมด
    public void ResetAllStats()
    {
        PlayerPrefs.DeleteAll();
        ShowScoreboard(); // อัปเดตหน้าจอทันที
    }
}