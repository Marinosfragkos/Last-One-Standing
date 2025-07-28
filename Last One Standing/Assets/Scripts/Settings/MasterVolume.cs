using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MasterVolume : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public AudioMixer audioMixer;

    public AudioToggle audioToggle; // ← αναφορά στο AudioToggle script

    private const string VolumeParam = "MasterVolume";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        UpdateVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float value)
    {
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(VolumeParam, volumeInDb);

        PlayerPrefs.SetFloat("MasterVolume", value);
        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";

        // Αν ο ήχος είναι mute και ο χρήστης ανεβάσει την ένταση, κάνε unmute
        if (PlayerPrefs.GetInt("AudioMuted", 0) == 1 && value > 0.001f)
        {
            audioToggle.SetAudioOn(); // αυτόματα ενεργοποιεί και το σωστό κουμπί
        }
    }
}
