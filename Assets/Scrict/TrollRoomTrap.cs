using UnityEngine;
using System.Collections;

public class TrollRoomTrap : MonoBehaviour
{
    [Header("กำแพงปิดตาย")]
    public GameObject blockingWall; // กำแพงที่จะเสกมาปิดทางหนี

    [Header("ตั้งค่าการเสกศัตรู")]
    public GameObject enemyPrefab;  // Prefab ของศัตรู (หรือของอันตรายที่จะให้ร่วงลงมา)
    public Transform[] spawnPoints; // จุดเกิดศัตรู (ใส่ไว้หลายๆ จุดให้มันสุ่มเกิด)
    public float spawnDelay = 0.5f; // เสกเร็วแค่ไหน? (วินาที/ตัว) ยิ่งน้อยยิ่งนรก

    private bool isTriggered = false;

    void Start()
    {
        // สั่งซ่อนกำแพงไว้ก่อนตั้งแต่เริ่มเกม
        if (blockingWall != null)
        {
            blockingWall.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้าคนเล่นเดินมาเข้าโซนเชือด และยังไม่เคยทำงานมาก่อน
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;

            // 1. เสกกำแพงปิดทางหนี!
            if (blockingWall != null)
            {
                blockingWall.SetActive(true);
            }

            // 2. เริ่มมหกรรมเสกศัตรูรัวๆ
            StartCoroutine(SpawnEnemiesRoutine());
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        // ใช้ while(true) เพื่อให้มันเสกวนไปเรื่อยๆ "ไม่มีวันหยุด" จนกว่าผู้เล่นจะตาย (ตายแล้วฉากรีเซ็ต)
        while (true)
        {
            // ถ้าลืมใส่จุดเกิด ให้หยุดทำงาน จะได้ไม่ Error
            if (spawnPoints.Length == 0) yield break;

            // สุ่มเลือกจุดเกิด 1 จุด จากที่เราตั้งไว้
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnSpot = spawnPoints[randomIndex];

            // เสกศัตรูออกมา!
            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, spawnSpot.position, Quaternion.identity);
            }

            // รอเวลาก่อนเสกตัวต่อไป (สุ่มดีเลย์นิดหน่อยให้จังหวะมันดูวุ่นวายขึ้น)
            float randomDelay = spawnDelay + Random.Range(-0.1f, 0.1f);
            yield return new WaitForSeconds(randomDelay);
        }
    }
}