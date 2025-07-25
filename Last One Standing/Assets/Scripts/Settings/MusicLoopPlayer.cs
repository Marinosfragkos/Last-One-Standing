using UnityEngine;

public class MusicLoopPlayer : MonoBehaviour
{
    public AudioClip musicClip; // Το τραγούδι που θέλεις να παίξει
    private AudioSource audioSource;

    void Awake()
    {
        // Αν δεν υπάρχει ήδη AudioSource, πρόσθεσέ το
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = 0.5f; // Όγκος (0.0 - 1.0)

        audioSource.Play();
    }
}
