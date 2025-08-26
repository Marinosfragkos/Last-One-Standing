using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public static AudioToggle instance; // Singleton instance

    public AudioMixer audioMixer;

    public GameObject onButton;  // Δείχνει "On" όταν είναι mute
    public GameObject offButton; // Δείχνει "Off" όταν ο ήχος είναι ενεργός

    private const string VolumeParam = "MasterVolume";
    private const string MuteKey = "AudioMuted";

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Αν υπάρχει ήδη ένα instance, καταστρέφουμε αυτό
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Δεν καταστρέφεται σε νέα σκηνή
    }

    void Start()
    {
        int isMuted = PlayerPrefs.GetInt(MuteKey, 0);
        if (isMuted == 1)
            SetAudioOff();
        else
            SetAudioOn();
    }

    public void SetAudioOn()
    {
        audioMixer.SetFloat(VolumeParam, 0f); // ή όποια είναι η default τιμή σου
        PlayerPrefs.SetInt(MuteKey, 0);
        UpdateButtons(isMuted: false);
    }

    public void SetAudioOff()
    {
        audioMixer.SetFloat(VolumeParam, -80f); // Mute
        PlayerPrefs.SetInt(MuteKey, 1);
        UpdateButtons(isMuted: true);
    }

    private void UpdateButtons(bool isMuted)
    {
        if(onButton != null) onButton.SetActive(isMuted);
        if(offButton != null) offButton.SetActive(!isMuted);
    }
}
