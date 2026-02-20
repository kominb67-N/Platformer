using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverUI;

    public void PlayerDied()
    {
        gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartFromCheckpoint()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartFromBeginning()
    {
        CheckpointManager.hasCheckpoint = false; // ล้างค่าจุดเซฟทิ้ง
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}