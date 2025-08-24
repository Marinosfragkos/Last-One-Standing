using UnityEngine;
using UnityEngine.UI;

public class MicToggleButton : MonoBehaviour
{
    public Button buttonOn;
    public Button buttonOff;

    void Start()
    {
        // αρχικά εμφανίζεται μόνο το σωστό κουμπί
        UpdateButtons();

        // Σύνδεση events
        buttonOn.onClick.AddListener(SetMicOff);
        buttonOff.onClick.AddListener(SetMicOn);
    }

    void UpdateButtons()
    {
        buttonOn.gameObject.SetActive(!VoiceSettings.MicOn);
        buttonOff.gameObject.SetActive(VoiceSettings.MicOn);
    }

    public void SetMicOn()
    {
        VoiceSettings.MicOn = true;
        UpdateButtons();
        Debug.Log("Mic turned ON from UI");
    }

    public void SetMicOff()
    {
        VoiceSettings.MicOn = false;
        UpdateButtons();
        Debug.Log("Mic turned OFF from UI");
    }

    void Update()
    {
        // Συγχρονισμός με keyboard toggle (P)
        UpdateButtons();
    }
}
