using UnityEngine;
using System.Collections;

public class SpikeTrapController : MonoBehaviour
{
    [Header("ใส่หนามทั้งหมดที่อยากให้พุ่ง")]
    public Transform[] spikes;

    [Header("ตั้งค่าการพุ่งของหนาม")]
    public float popUpDistance = 3f;
    public float popUpSpeed = 30f;
    public float stayTime = 1.0f;
    public float dropSpeed = 5f;

    private Vector3[] startPositions;
    private Vector3[] targetPositions;

    // ตัวแปรนี้แหละครับที่เป็นกุญแจสำคัญ
    private bool isTriggered = false;

    void Start()
    {
        startPositions = new Vector3[spikes.Length];
        targetPositions = new Vector3[spikes.Length];

        for (int i = 0; i < spikes.Length; i++)
        {
            if (spikes[i] != null)
            {
                startPositions[i] = spikes[i].position;
                targetPositions[i] = startPositions[i] + new Vector3(0, popUpDistance, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้าคนเล่นเดินมาเหยียบ และยังไม่เคยแทงมาก่อน
        if (collision.CompareTag("Player") && !isTriggered)
        {
            StartCoroutine(SpikeRoutine());
        }
    }

    IEnumerator SpikeRoutine()
    {
        // ล็อกแม่กุญแจทันที! ไม่ให้ทำงานซ้ำได้อีก
        isTriggered = true;

        // --- 1. พุ่งขึ้น! ---
        bool isMovingUp = true;
        while (isMovingUp)
        {
            isMovingUp = false;
            for (int i = 0; i < spikes.Length; i++)
            {
                if (spikes[i] != null)
                {
                    spikes[i].position = Vector3.MoveTowards(spikes[i].position, targetPositions[i], popUpSpeed * Time.deltaTime);
                    if (Vector3.Distance(spikes[i].position, targetPositions[i]) > 0.01f)
                    {
                        isMovingUp = true;
                    }
                }
            }
            yield return null;
        }

        for (int i = 0; i < spikes.Length; i++)
        {
            if (spikes[i] != null) spikes[i].position = targetPositions[i];
        }

        // --- 2. ค้างโชว์ความสะใจ ---
        yield return new WaitForSeconds(stayTime);

        // --- 3. หดกลับลงหลุม! ---
        bool isMovingDown = true;
        while (isMovingDown)
        {
            isMovingDown = false;
            for (int i = 0; i < spikes.Length; i++)
            {
                if (spikes[i] != null)
                {
                    spikes[i].position = Vector3.MoveTowards(spikes[i].position, startPositions[i], dropSpeed * Time.deltaTime);
                    if (Vector3.Distance(spikes[i].position, startPositions[i]) > 0.01f)
                    {
                        isMovingDown = true;
                    }
                }
            }
            yield return null;
        }

        for (int i = 0; i < spikes.Length; i++)
        {
            if (spikes[i] != null) spikes[i].position = startPositions[i];
        }

    }
}