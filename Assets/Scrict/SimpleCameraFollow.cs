using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("การติดตาม")]
    public Transform player;
    public float yOffset = 1.5f;
    public float smoothTime = 0.25f;

    [Header("ระบบตามตัวตอนกระเด็น (ใหม่!)")]
    public float catchUpDistance = 3.0f; // ถ้าระยะห่างเกินกี่เมตร ให้กล้องรีบตาม
    public float fastSmoothTime = 0.05f; // ความเร็วตอนสับเกียร์ตาม (ยิ่งน้อยยิ่งไว)

    [Header("ระบบซูมอัตโนมัติ (Idle Zoom)")]
    public float defaultSize = 5f;
    public float zoomInSize = 3.5f;
    public float idleThreshold = 2.0f;
    public float zoomSpeed = 2f;

    // --- โหมดคัตซีน ---
    private bool isCinematic = false;
    private Transform cinematicTarget;
    private float cinematicSize;

    private float currentIdleTime = 0f;
    private Camera cam;
    private Vector3 currentVelocity = Vector3.zero;
    private Rigidbody2D playerRb;

    // --- ตัวแปรสำหรับจอสั่น ---
    private float shakeTimeRemaining = 0f;
    private float shakePower = 0.1f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (player != null) playerRb = player.GetComponent<Rigidbody2D>();
        if (cam != null) cam.orthographicSize = defaultSize;
    }

    void LateUpdate()
    {
        // 1. โหมดคัตซีน
        if (isCinematic)
        {
            if (cinematicTarget != null)
            {
                Vector3 targetPos = new Vector3(cinematicTarget.position.x, cinematicTarget.position.y, -10f);
                transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);
            }
            if (cam != null)
            {
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, cinematicSize, Time.deltaTime * zoomSpeed);
            }
            return;
        }

        // 2. ระบบปกติ
        if (player != null && cam != null)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y + yOffset, -10f);

            // *** อัปเกรด: เช็กระยะห่างเพื่อสับเกียร์กล้อง ***
            float distance = Vector2.Distance(transform.position, targetPos);
            float currentSmoothTime = smoothTime; // ค่าเริ่มต้นคือตามปกติ

            // ถ้าระยะห่างมากเกินไป (โดนดีด โดนสปริง)
            if (distance > catchUpDistance)
            {
                currentSmoothTime = fastSmoothTime; // สับเกียร์ตามไวแสง!
            }

            // คำนวณตำแหน่งหลัก โดยใช้ currentSmoothTime ที่เช็กมาแล้ว
            Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, currentSmoothTime);

            // บวกแรงสั่นเข้าไป
            if (shakeTimeRemaining > 0)
            {
                smoothPosition += (Vector3)Random.insideUnitCircle * shakePower;
                shakeTimeRemaining -= Time.deltaTime;
            }

            transform.position = smoothPosition;

            // ระบบ Idle Zoom
            if (playerRb != null && playerRb.linearVelocity.magnitude < 0.1f) currentIdleTime += Time.deltaTime;
            else currentIdleTime = 0f;

            float targetSize = (currentIdleTime >= idleThreshold) ? zoomInSize : defaultSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
        }
    }

    public void StartCinematic(Transform target, float targetSize)
    {
        isCinematic = true;
        cinematicTarget = target;
        cinematicSize = targetSize;
        currentIdleTime = 0;
    }

    public void StopCinematic()
    {
        isCinematic = false;
        cinematicTarget = null;
    }

    public void TriggerShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;
    }

    // *** ฟังก์ชันสำหรับเห็ดอ้วน ***
    public void SetZoom(float newSize)
    {
        defaultSize = newSize;
    }
}