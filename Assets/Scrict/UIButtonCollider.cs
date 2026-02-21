using UnityEngine;
using UnityEngine.UI;

// บังคับว่าปุ่มนี้ต้องมี Collider 2D แปะอยู่ด้วยเสมอ
[RequireComponent(typeof(Collider2D))]
public class UIButtonCollider : MonoBehaviour, ICanvasRaycastFilter
{
    private Collider2D myCollider;
    private RectTransform rectTransform;

    void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        rectTransform = GetComponent<RectTransform>();
    }

    // ฟังก์ชันนี้เป็นตัวกรองว่า "กดโดนปุ่มจริงๆ หรือเปล่า?"
    public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector3 worldPoint;

        // แปลงพิกัดหน้าจอ (เมาส์/นิ้ว) ให้กลายเป็นพิกัดในโลก 2D
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out worldPoint);

        // เช็กว่าพิกัดนั้น จิ้มโดนเส้น Collider ของเราหรือไม่!
        return myCollider.OverlapPoint(worldPoint);
    }
}