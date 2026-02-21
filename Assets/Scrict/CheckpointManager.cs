using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 lastCheckpointPos;
    public static bool hasCheckpoint = false;
    public Transform defaultSpawnPoint;

    void Awake()
    {
        // ถ้าถูกสั่งให้โหลดเซฟ (กดจากปุ่ม Continue)
        if (hasCheckpoint && PlayerPrefs.GetInt("HasSave", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("SaveX", 0);
            float y = PlayerPrefs.GetFloat("SaveY", 0);
            float z = PlayerPrefs.GetFloat("SaveZ", 0);
            lastCheckpointPos = new Vector3(x, y, z);
        }
        // ถ้าเริ่มใหม่หมด
        else if (defaultSpawnPoint != null)
        {
            lastCheckpointPos = defaultSpawnPoint.position;
            hasCheckpoint = true;
        }
    }
}