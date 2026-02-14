using UnityEngine;

public class TrollBlock : MonoBehaviour
{
    [SerializeField] private GameObject hiddenBlock; // ลากบล็อกที่จะโผล่มาขวางใส่ช่องนี้ใน Inspector
    private bool isTriggered = false;

    void Start()
    {
        // ซ่อนบล็อกไว้ตั้งแต่เริ่มเกม
        if (hiddenBlock != null)
        {
            hiddenBlock.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้าผู้เล่นกระโดดมาโดน Trigger นี้ (เช่น ดักไว้ตรงรอยต่อหน้าผา)
        if (other.CompareTag("Player") && !isTriggered)
        {
            hiddenBlock.SetActive(true); // บล็อกโผล่มาชนหัวผู้เล่นร่วงลงเหว
            isTriggered = true;
        }
    }
}