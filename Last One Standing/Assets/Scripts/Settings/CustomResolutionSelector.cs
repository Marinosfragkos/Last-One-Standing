using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;


public class CustomResolutionSelector : MonoBehaviour
{
    public TMP_Dropdown ResDropDown;
    public Toggle FullScreenToggle;
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

    Screen.SetResolution(AllowedResolutions[SelectedResolution].width, AllowedResolutions[SelectedResolution].height, IsFullScreen);

    // Αυτόματη εύρεση CanvasScaler στη σκηνή
    canvasScalers = new List<CanvasScaler>(Object.FindObjectsByType<CanvasScaler>(FindObjectsSortMode.None));


    // Ρύθμιση του CanvasScaler για την αρχική ανάλυση
    foreach (CanvasScaler scaler in canvasScalers)
    {
        scaler.referenceResolution = new Vector2(AllowedResolutions[SelectedResolution].width, AllowedResolutions[SelectedResolution].height);
    }
}


    public void ChangeResolution()
{
    SelectedResolution = ResDropDown.value;

    Resolution res = AllowedResolutions[SelectedResolution];
    Screen.SetResolution(res.width, res.height, IsFullScreen);

    foreach (CanvasScaler scaler in canvasScalers)
    {
        scaler.referenceResolution = new Vector2(res.width, res.height);
    }
}


    public void ChangeFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;
        Screen.SetResolution(AllowedResolutions[SelectedResolution].width, AllowedResolutions[SelectedResolution].height, IsFullScreen);
    }
}
