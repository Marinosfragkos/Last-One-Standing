using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioSource audioSource; // Πρέπει να έχει συσχετισμένο AudioClip

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 = Left click
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("AudioSource or AudioClip not set!", this);
            }
        }
    }
}
