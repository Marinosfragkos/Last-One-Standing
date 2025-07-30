using UnityEngine;
using TMPro;

public class QualityDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public TMP_Text qualityLabel;  // <-- Το νέο Text που δείχνει το όνομα της ποιότητας

    void Start()
    {
        // Προσθέτει τα ονόματα στο dropdown (αν δεν τα έχεις ήδη από το Inspector)
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        int savedQuality = PlayerPrefs.GetInt("qualityLevel", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(savedQuality);
        qualityDropdown.value = savedQuality;
        qualityDropdown.RefreshShownValue();

        UpdateQualityLabel(savedQuality); // ενημέρωσε το label στην αρχή

        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt("qualityLevel", index);
        UpdateQualityLabel(index);  // ενημέρωσε το label όταν αλλάζει
    }

    void UpdateQualityLabel(int index)
    {
        if (qualityLabel != null)
        {
            qualityLabel.text = $"{QualitySettings.names[index]}";
        }
    }
}
