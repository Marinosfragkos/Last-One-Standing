using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaxPinSettingUI : MonoBehaviour
{
    public Slider maxPinSlider;
    public TextMeshProUGUI valueText;

   void Start()
{
    int savedPin = PlayerPrefs.GetInt("MaxPinAllowed", 150);
    maxPinSlider.value = savedPin;
    UpdateValueText(savedPin);
    maxPinSlider.onValueChanged.AddListener(UpdateValueText);
}


    void UpdateValueText(float value)
    {
        int pin = Mathf.RoundToInt(value);
        valueText.text = pin.ToString();
        // Αποθήκευση σε PlayerPrefs (προαιρετικά)
        PlayerPrefs.SetInt("MaxPinAllowed", pin);
    }
}
