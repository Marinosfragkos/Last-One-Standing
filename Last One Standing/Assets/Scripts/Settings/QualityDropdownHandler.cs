/*using UnityEngine;
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
*/
using UnityEngine;
using TMPro;

public class QualityDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;
    public TMP_Text qualityLabel;

    void Start()
    {
        // Προσθέτει τα ονόματα στο dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        int savedQuality = PlayerPrefs.HasKey("qualityLevel") 
            ? PlayerPrefs.GetInt("qualityLevel") 
            : QualitySettings.GetQualityLevel();

        ApplyQualitySettings(savedQuality);

        qualityDropdown.value = savedQuality;
        qualityDropdown.RefreshShownValue();

        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        ApplyQualitySettings(index);
    }

    void ApplyQualitySettings(int index)
    {
        // Αλλάζει το Quality Level
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt("qualityLevel", index);

        // Ρυθμίσεις που θα επηρεάζουν ορατά τα γραφικά
        switch(index)
        {
            case 0: // Low
                QualitySettings.shadowDistance = 25;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
                QualitySettings.antiAliasing = 0;
                QualitySettings.globalTextureMipmapLimit = 2; // 1/4 textures
                break;
            case 1: // Medium
                QualitySettings.shadowDistance = 50;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Medium;
                QualitySettings.antiAliasing = 2;
                QualitySettings.globalTextureMipmapLimit = 1; // 1/2 textures
                break;
            case 2: // High
                QualitySettings.shadowDistance = 100;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
                QualitySettings.antiAliasing = 4;
                QualitySettings.globalTextureMipmapLimit = 0; // full textures
                break;
            case 3: // Ultra
                QualitySettings.shadowDistance = 200;
                QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                QualitySettings.antiAliasing = 8;
                QualitySettings.globalTextureMipmapLimit = 0; // full textures
                break;
        }

        UpdateQualityLabel(index);
    }

    void UpdateQualityLabel(int index)
    {
        if (qualityLabel != null)
        {
            qualityLabel.text = (index >= 0 && index < QualitySettings.names.Length)
                ? QualitySettings.names[index]
                : "Unknown";
        }
    }
}
