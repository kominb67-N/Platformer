using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    public Sprite activeSprite; // รูปตอนเปิดใช้งานแล้ว (เช่น ธงเปลี่ยนสี)
    private SpriteRenderer sr;
    private bool isActivated = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // บันทึกตำแหน่งลงใน Manager
            CheckpointManager.lastCheckpointPos = transform.position;
            CheckpointManager.hasCheckpoint = true;

            // เปลี่ยนรูปโชว์ว่าเซฟแล้ว
            if (sr != null && activeSprite != null) sr.sprite = activeSprite;

            Debug.Log("Checkpoint: บันทึกจุดเซฟแล้ว!");
        }
    }
}