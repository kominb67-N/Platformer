using UnityEngine;
using System.Collections;

public class CheckpointTrigger : MonoBehaviour
{
    public GameObject popUpText;
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // 1. บันทึกให้ระบบปัจจุบันรับรู้
            CheckpointManager.lastCheckpointPos = transform.position;
            CheckpointManager.hasCheckpoint = true;

            // 2. บันทึกลง PlayerPrefs (เครื่องคอม/มือถือ) ให้จำถาวร
            PlayerPrefs.SetInt("HasSave", 1);
            PlayerPrefs.SetFloat("SaveX", transform.position.x);
            PlayerPrefs.SetFloat("SaveY", transform.position.y);
            PlayerPrefs.SetFloat("SaveZ", transform.position.z);
            PlayerPrefs.Save();

            if (popUpText != null)
            {
                popUpText.SetActive(true);
                StartCoroutine(HideText());
            }
        }
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(2f);
        if (popUpText != null) popUpText.SetActive(false);
    }
}