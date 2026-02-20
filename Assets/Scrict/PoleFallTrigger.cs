using UnityEngine;

public class PoleFallTrigger : MonoBehaviour
{
    [Header("ลากเสาธงมาใส่ช่องนี้")]
    public FallingPole targetPole;

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้าผู้เล่นเดินมาชน และยังไม่เคยทำงานมาก่อน
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            // สั่งให้เสาธงล้ม
            if (targetPole != null)
            {
                targetPole.TriggerFall();
            }
        }
    }
}