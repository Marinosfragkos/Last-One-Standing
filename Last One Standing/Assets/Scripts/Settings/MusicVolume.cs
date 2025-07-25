using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MusicVolume : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Text volumeText;
    public AudioMixer audioMixer;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        volumeSlider.value = savedVolume;
        UpdateVolume(savedVolume);
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float value)
    {
        float volumeInDb = (value > 0.0001f) ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("MusicVolume", volumeInDb);

        PlayerPrefs.SetFloat("MusicVolume", value);
        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
