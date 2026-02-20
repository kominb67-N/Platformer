using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [Header("1. ตั้งค่าการเคลื่อนที่")]
    public float speed = 3f;
    public Transform[] waypoints;

    [Header("2. ระบบรอคนเหยียบ")]
    public bool waitToStep = true;

    [Header("3. ระบบดักควาย (ทรยศผู้เล่น)")]
    public bool willBreak = false;
    public Transform breakPoint;
    public float breakDistance = 0.5f;

    [Header("4. ระบบเกิดใหม่ (กันเกมไปต่อไม่ได้)")]
    public float respawnTime = 5f; // จะให้พื้นกลับมาที่เดิมในกี่วินาที

    private int currentPointIndex = 0;
    private bool isMoving = false;
    private Rigidbody2D rb;
    private Collider2D coll;
    private bool isBroken = false;

    // ระบบบันทึกค่าเริ่มต้นเพื่อใช้ตอนเกิดใหม่
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Vector2 lastPosition;
    private Vector2 actualVelocity;
    private PlayerController currentPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();

        // --- ส่วนที่เพิ่ม: จำตำแหน่งและหมุนเริ่มต้นไว้ ---
        initialPos = transform.position;
        initialRot = transform.rotation;

        lastPosition = rb.position;
        if (!waitToStep) isMoving = true;
    }

    void FixedUpdate()
    {
        // ถ้าพื้นพังอยู่ ไม่ต้องคำนวณการเดิน
        if (!isMoving || waypoints.Length == 0 || rb == null || isBroken)
        {
            actualVelocity = Vector2.zero;
            return;
        }

        Transform target = waypoints[currentPointIndex];
        Vector2 newPos = Vector2.MoveTowards(rb.position, target.position, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        actualVelocity = (newPos - lastPosition) / Time.fixedDeltaTime;
        lastPosition = newPos;

        if (currentPlayer != null)
        {
            currentPlayer.SetPlatformVelocity(actualVelocity);
        }

        if (Vector2.Distance(rb.position, target.position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }

        if (willBreak && breakPoint != null && currentPlayer != null && !isBroken)
        {
            if (Vector2.Distance(transform.position, breakPoint.position) <= breakDistance)
            {
                StartCoroutine(BetrayPlayer());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isBroken)
        {
            if (collision.transform.position.y > transform.position.y)
            {
                currentPlayer = collision.gameObject.GetComponent<PlayerController>();
                isMoving = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentPlayer != null) currentPlayer.SetPlatformVelocity(Vector2.zero);
            currentPlayer = null;
        }
    }

    IEnumerator BetrayPlayer()
    {
        isBroken = true;
        if (currentPlayer != null) currentPlayer.SetPlatformVelocity(Vector2.zero);
        currentPlayer = null;

        // ปิดการชนเพื่อให้ผู้เล่นร่วง
        if (coll != null) coll.enabled = false;

        // ทำให้พื้นร่วง
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f;
        }

        // --- แก้ไขจาก Destroy เป็นการรอเพื่อ Respawn ---
        yield return new WaitForSeconds(respawnTime);

        ResetPlatform();
    }

    void ResetPlatform()
    {
        isBroken = false;

        // คืนค่าฟิสิกส์ให้กลับมาเป็น Kinematic เหมือนเดิม
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // วาร์ปกลับไปที่จุดเริ่มต้น
        transform.position = initialPos;
        transform.rotation = initialRot;
        lastPosition = initialPos;

        // เปิดการชนใหม่เพื่อให้คนกลับมาเหยียบได้
        if (coll != null) coll.enabled = true;

        // ถ้าตั้งค่าให้รอคนเหยียบ ก็สั่งให้หยุดนิ่งก่อน
        if (waitToStep)
        {
            isMoving = false;
            currentPointIndex = 0;
        }
    }
}