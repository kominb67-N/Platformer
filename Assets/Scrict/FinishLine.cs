using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        // เช็คว่าคนที่มาชนคือผู้เล่นใช่หรือไม่
        if (other.CompareTag("Player"))
        {
            // ค้นหาสคริปต์ LevelCompleteManager แล้วสั่งให้โชว์หน้าจอผ่านด่าน
            LevelCompleteManager manager = FindObjectOfType<LevelCompleteManager>();

            if (manager != null)
            {
                manager.ShowLevelComplete();
            }
        }
    }
}