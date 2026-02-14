using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("ลากตัวละครมาใส่ตรงนี้")]
    public Transform player;

    [Header("ขอบเขตที่กล้องจะไปได้")]
    public float minX = 0f;
    public float maxX = 10f;

    // --- ส่วนที่เพิ่มเข้ามาสำหรับจอสั่น ---
    private float shakeTimeRemaining = 0f;
    private float shakePower = 0.1f;

    private Vector3 offset = new Vector3(0, 0, -10f);

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newCameraPosition = player.position + offset;
            newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, minX, maxX);

            // ถ้าระบบจอสั่นทำงานอยู่
            if (shakeTimeRemaining > 0)
            {
                // สุ่มขยับกล้องไปรอบๆ เป็นวงกลมตามความแรง
                Vector2 shakeOffset = Random.insideUnitCircle * shakePower;
                newCameraPosition.x += shakeOffset.x;
                newCameraPosition.y += shakeOffset.y;

                // ลดเวลาสั่นลงเรื่อยๆ ตามเวลาจริง
                shakeTimeRemaining -= Time.deltaTime;
            }

            transform.position = newCameraPosition;
        }
    }

    // ฟังก์ชันนี้เตรียมไว้ให้โค้ดอื่นสั่งให้กล้องสั่น
    public void TriggerShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;
    }
}