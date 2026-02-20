using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 lastCheckpointPos;
    public static bool hasCheckpoint = false;
    public Transform defaultSpawnPoint;

    void Awake()
    {
        // ถ้าเริ่มด่านใหม่เอี่ยม และยังไม่มีจุดเซฟ ให้ใช้จุดเกิดเริ่มต้น
        if (!hasCheckpoint && defaultSpawnPoint != null)
        {
            lastCheckpointPos = defaultSpawnPoint.position;
            hasCheckpoint = true;
        }
    }
}