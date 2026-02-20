using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [Header("ตั้งค่าความเร็ว (0=นิ่ง, 1=ตามกล้อง)")]
    public float parallaxEffect; // แนะนำ: ท้องฟ้า=0.9, ภูเขา=0.5, ต้นไม้ไกลๆ=0.2

    private Transform cam;
    private Vector3 lastCamPos;
    private float textureUnitSizeX;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;

        // คำนวณความกว้างของรูป เพื่อเอาไว้ทำลูป (Infinite)
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Texture2D texture = sprite.sprite.texture;
        textureUnitSizeX = texture.width / sprite.sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {
        // คำนวณระยะที่กล้องขยับไป
        Vector3 deltaMovement = cam.position - lastCamPos;

        // ขยับพื้นหลังตามกล้อง แต่คูณด้วยอัตราส่วน (parallaxEffect)
        transform.position += new Vector3(deltaMovement.x * parallaxEffect, deltaMovement.y * parallaxEffect, 0);

        lastCamPos = cam.position;

        // --- ระบบวนลูป (Infinite Scrolling) ---
        // เช็กว่ากล้องเลยขอบรูปไปหรือยัง ถ้าเลยแล้ว ให้ย้ายรูปไปดักหน้า
        if (Mathf.Abs(cam.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cam.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cam.position.x + offsetPositionX, transform.position.y);
        }
    }
}