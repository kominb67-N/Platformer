using UnityEngine;
using UnityEngine.UI;

public class AlphaClickFix : MonoBehaviour
{
    public float alphaThreshold = 0.1f; // ค่าความใสที่ยอมให้กดผ่านได้ (0.1 คือใสมาก)

    void Start()
    {
        // ดึงภาพของปุ่มมา
        Image btnImage = GetComponent<Image>();

        // สั่งให้ปุ่ม "สนใจเฉพาะพิกเซลที่มีสี" พื้นที่ใสๆ จะกดไม่ติดอีกต่อไป!
        if (btnImage != null)
        {
            btnImage.alphaHitTestMinimumThreshold = alphaThreshold;
        }
    }
}