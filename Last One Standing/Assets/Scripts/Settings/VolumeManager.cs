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
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        volumeSlider.value = savedVolume;
        UpdateVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    public void UpdateVolume(float value)
    {
        float volumeInDb = (value > 0.0001f) ? Mathf.Log10(value) * 20f : -80f;
        audioMixer.SetFloat("SFXVolume", volumeInDb); // Συνδέεται με το SFX group

        PlayerPrefs.SetFloat("SFXVolume", value);
        volumeText.text = Mathf.RoundToInt(value * 100f) + "%";
    }
}
