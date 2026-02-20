using UnityEngine;
using System.Collections;

public class SpikeLauncher : MonoBehaviour
{
    [Header("ตั้งค่ากระสุน")]
    public GameObject spikePrefab;   // ลาก Prefab หนามมาใส่
    public Transform firePoint;     // จุดที่หนามจะโผล่ออกมา

    [Header("โหมดการยิง")]
    public bool autoFire = true;    // ยิงรัวอัตโนมัติ
    public float fireRate = 2f;     // ยิงทุกๆ กี่วินาที

    [Header("โหมดเซ็นเซอร์ (ถ้าไม่ Auto)")]
    public bool fireOnTrigger = false;

    private bool canFire = true;

    [Header("เวลาหน่วงก่อนเริ่มยิงครั้งแรก")]
    public float startDelay = 0f;

    void Start()
    {
        if (autoFire)
        {
            StartCoroutine(AutoFireRoutine());
        }
    }

    IEnumerator AutoFireRoutine()
    {
        // รอตามเวลาที่ตั้งไว้ก่อน ค่อยเริ่มลูปการยิง
        yield return new WaitForSeconds(startDelay);

        while (autoFire)
        {
            FireSpike(); // ยิงก่อนค่อยรอ หรือรอค่อยยิง สลับที่ได้ตามชอบครับ
            yield return new WaitForSeconds(fireRate);
        }
    }

    public void FireSpike()
    {
        if (spikePrefab != null && firePoint != null)
        {
            Instantiate(spikePrefab, firePoint.position, firePoint.rotation);
        }
    }

    // ถ้าอยากให้เดินผ่านเซ็นเซอร์แล้วยิง
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fireOnTrigger && collision.CompareTag("Player") && canFire)
        {
            FireSpike();
            StartCoroutine(FireCooldown());
        }
    }

    IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}