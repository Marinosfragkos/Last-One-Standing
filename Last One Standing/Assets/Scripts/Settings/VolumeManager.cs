using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public AudioMixer audioMixer;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        UpdateVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float value)
    {
        // Ανάλογα με το Audio Mixer, μετατρέπουμε το 0-1 σε dB
        float volumeInDb = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MasterVolume", volumeInDb);

        PlayerPrefs.SetFloat("MasterVolume", value);
        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
