using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public AudioMixer audioMixer;

    public GameObject onButton;  // Κουμπί για ενεργό ήχο
    public GameObject offButton; // Κουμπί για mute

    void Start()
    {
        int isMuted = PlayerPrefs.GetInt("AudioMuted", 0);
        if (isMuted == 1)
            SetAudioOff();
        else
            SetAudioOn();
    }

    public void SetAudioOn()
    {
        audioMixer.SetFloat("MasterVolume", 0f);
        PlayerPrefs.SetInt("AudioMuted", 0);

        onButton.SetActive(false);
        offButton.SetActive(true);
    }

    public void SetAudioOff()
    {
        audioMixer.SetFloat("MasterVolume", -80f);
        PlayerPrefs.SetInt("AudioMuted", 1);

        onButton.SetActive(true);
        offButton.SetActive(false);
    }
}
