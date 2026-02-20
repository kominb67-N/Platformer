using UnityEngine;

public class WaypointSaw : MonoBehaviour
{
    [Header("1. การเคลื่อนที่ตามจุด")]
    public float speed = 4f;
    public Transform[] waypoints; // ลากจุด A, B, C มาใส่ที่นี่
    public bool loop = true;      // true = วนวงกลม, false = วิ่งย้อนกลับทางเดิม

    [Header("2. การหมุน")]
    public float rotationSpeed = 600f;

    private int currentPointIndex = 0;
    private bool movingForward = true;

    void Update()
    {
        // 1. หมุนรูปใบเลื่อยให้ดูน่ากลัว
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // 2. ถ้าไม่ได้กำหนดจุดไว้ ก็ไม่ต้องเดิน
        if (waypoints.Length == 0) return;

        // 3. เคลื่อนที่ไปหาจุดหมายปัจจุบัน
        Transform target = waypoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // 4. เมื่อถึงจุดหมาย ให้เปลี่ยนไปจุดถัดไป
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            UpdateNextWaypoint();
        }
    }

    void UpdateNextWaypoint()
    {
        if (loop)
        {
            // แบบวนลูป: 1 -> 2 -> 3 -> 1
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
        else
        {
            // แบบไป-กลับ (Ping Pong): 1 -> 2 -> 3 -> 2 -> 1
            if (movingForward)
            {
                currentPointIndex++;
                if (currentPointIndex >= waypoints.Length)
                {
                    currentPointIndex = waypoints.Length - 2;
                    movingForward = false;
                }
            }
            else
            {
                currentPointIndex--;
                if (currentPointIndex < 0)
                {
                    currentPointIndex = 1;
                    movingForward = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ลดเลือดผู้เล่น 1 แต้ม
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(1);
        }
    }
}