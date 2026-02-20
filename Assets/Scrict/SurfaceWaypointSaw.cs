using UnityEngine;

public class SurfaceWaypointSaw : MonoBehaviour
{
    [Header("1. กำหนดจุดวิ่ง (จำกัดเขต)")]
    public Transform[] waypoints;
    public float speed = 4f;
    public bool loop = true;

    [Header("2. การหมุนและแรงยึดเกาะ")]
    public float rotationSpeed = 600f;
    public LayerMask groundLayer;   // Layer ของพื้น/กำแพง
    public float stickForce = 10f;  // แรงดึงให้ติดพื้น
    public float wallDistance = 0.5f;

    private int currentPointIndex = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.bodyType = RigidbodyType2D.Kinematic; // ใช้ Kinematic เพื่อคุมเอง
    }

    void Update()
    {
        // 1. หมุนใบเลื่อย
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        if (waypoints.Length == 0) return;

        // 2. เคลื่อนที่ไปหาจุดหมาย (เคลื่อนที่ตามแนวพื้น)
        Transform target = waypoints[currentPointIndex];

        // เคลื่อนที่ทีละนิดไปหาจุดหมาย
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // 3. ระบบ Sticky (ดึงให้ติดกำแพงตลอดเวลา)
        ApplySurfaceSnapping();

        // 4. เมื่อถึงจุด ให้เปลี่ยนจุด
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
    }

    void ApplySurfaceSnapping()
    {
        // ยิง Raycast รอบๆ ตัว 4 ทิศทางเพื่อหาพื้นผิวที่ใกล้ที่สุด
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (hit.collider != null)
        {
            // ปรับตำแหน่งให้ห่างจากพื้นในระยะที่พอดี ไม่ให้จมพื้น
            Vector2 targetPos = hit.point + (hit.normal * wallDistance);
            transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * stickForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            if (health != null) health.TakeDamage(1); // ลดเลือด
        }
    }
}