using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class CustomResolutionSelector : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Button FullScreenOnButton;
    public Button FullScreenOffButton;
    public List<CanvasScaler> canvasScalers;

    bool IsFullScreen;
    int SelectedResolution;

    List<Resolution> AllowedResolutions = new List<Resolution>()
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1366, height = 768 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 }
    };

    void Start()
    {
        IsFullScreen = true;

        List<string> resolutionStringList = new List<string>();
        foreach (Resolution res in AllowedResolutions)
        {
            resolutionStringList.Add(res.width + " x " + res.height);
        }
        ResDropDown.ClearOptions();
        ResDropDown.AddOptions(resolutionStringList);

        // Default επιλογή 1920x1080
        int defaultIndex = AllowedResolutions.FindIndex(res => res.width == 1920 && res.height == 1080);
        if (defaultIndex == -1)
            defaultIndex = 0;

        ResDropDown.value = defaultIndex;
        SelectedResolution = defaultIndex;

        ApplyResolution();

        // Αυτόματη εύρεση CanvasScaler στη σκηνή
        canvasScalers = new List<CanvasScaler>(Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None));

        // Button listeners για FullScreen on/off
        if (FullScreenOnButton != null)
            FullScreenOnButton.onClick.AddListener(SetFullScreenOn);

        if (FullScreenOffButton != null)
            FullScreenOffButton.onClick.AddListener(SetFullScreenOff);
    }

    public void ChangeResolution()
    {
        SelectedResolution = ResDropDown.value;
        ApplyResolution();
    }

    void ApplyResolution()
    {
        Resolution res = AllowedResolutions[SelectedResolution];
        Screen.SetResolution(res.width, res.height, IsFullScreen);

        foreach (CanvasScaler scaler in canvasScalers)
        {
            scaler.referenceResolution = new Vector2(res.width, res.height);
        }
    }

    public void SetFullScreenOn()
    {
        IsFullScreen = true;
        ApplyResolution();
    }

    public void SetFullScreenOff()
    {
        IsFullScreen = false;
        ApplyResolution();
    }
}
