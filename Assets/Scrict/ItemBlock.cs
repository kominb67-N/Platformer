using UnityEngine;

public class ItemBlock : MonoBehaviour
{
    [Header("ลากไอเทม (Prefab) มาใส่ตรงนี้")]
    public GameObject itemPrefab;

    [Header("ระยะห่างที่ไอเทมจะโผล่ออกมาใต้บล็อก")]
    public float spawnOffsetY = -0.6f; // ค่ายิ่งติดลบ ไอเทมยิ่งโผล่ต่ำลงมา

    private bool isHit = false; // ตัวแปรเช็คว่าบล็อกนี้ถูกโหม่งไปแล้วหรือยัง

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. เช็คว่าคนที่มาชนคือผู้เล่น และบล็อกนี้ยังไม่เคยถูกโหม่ง
        if (collision.gameObject.CompareTag("Player") && !isHit)
        {
            // 2. เช็คว่าเป็นการกระโดดโหม่งจากด้านล่างจริงๆ 
            // โดยดูว่าตำแหน่งของผู้เล่น (แกน Y) อยู่ต่ำกว่าจุดกึ่งกลางของบล็อก
            if (collision.transform.position.y < transform.position.y)
            {
                SpawnItem();
            }
        }
    }

    private void SpawnItem()
    {
        isHit = true; // ล็อกบล็อกไว้ ไม่ให้โหม่งซ้ำได้อีก

        // 3. กำหนดตำแหน่งที่จะสร้างไอเทม (ตำแหน่งบล็อก + ระยะห่างด้านล่าง)
        Vector3 spawnPosition = transform.position + new Vector3(0, spawnOffsetY, 0);

        // 4. สร้างไอเทมออกมาในฉาก
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }

        // 5. (ลูกเล่นเสริม) เปลี่ยนสีบล็อกให้มืดลง เพื่อให้ผู้เล่นรู้ว่าของหมดแล้ว
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.gray;
        }
    }
}