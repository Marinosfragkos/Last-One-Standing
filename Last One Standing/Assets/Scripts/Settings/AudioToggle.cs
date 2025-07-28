using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public AudioMixer audioMixer;

    public GameObject onButton;  // Δείχνει "On" όταν είναι mute (για να ενεργοποιήσεις τον ήχο)
    public GameObject offButton; // Δείχνει "Off" όταν ο ήχος είναι ενεργός (για να τον απενεργοποιήσεις)

    private const string VolumeParam = "MasterVolume";
    private const string MuteKey = "AudioMuted";

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
        // Αν είναι mute, δείχνουμε το ON button για να επανενεργοποιήσει ο χρήστης τον ήχο
        onButton.SetActive(isMuted);
        offButton.SetActive(!isMuted);
    }
}
