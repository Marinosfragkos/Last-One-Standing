using UnityEngine;
using TMPro;

public class QualityDropdownHandler : MonoBehaviour
{
    public TMP_Dropdown qualityDropdown;

    void Start()
    {
        foreach (var name in QualitySettings.names)
{
    Debug.Log(name);
}

       int savedQuality = PlayerPrefs.GetInt("qualityLevel", QualitySettings.GetQualityLevel());
    QualitySettings.SetQualityLevel(savedQuality);
    qualityDropdown.value = savedQuality;
    qualityDropdown.RefreshShownValue();
    qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
    PlayerPrefs.SetInt("qualityLevel", index);
    }
}
