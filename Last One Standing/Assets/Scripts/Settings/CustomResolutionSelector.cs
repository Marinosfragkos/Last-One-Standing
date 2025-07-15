using UnityEngine;
using TMPro;

public class CustomResolutionSelector : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        OnResolutionChanged(resolutionDropdown.value); // Set initial resolution
    }

    void OnResolutionChanged(int index)
    {
        switch (index)
        {
            case 0:
                SetResolution(1366, 768);
                break;
            case 1:
                SetResolution(1600, 900);
                break;
            case 2:
                SetResolution(1920, 1080);
                break;
            case 3:
                SetResolution(2560, 1440);
                break;
            case 4:
                SetResolution(3840, 2160);
                break;
        }
    }

    void SetResolution(int width, int height)
    {
        // Αν θέλεις Fullscreen:
        Screen.SetResolution(width, height, FullScreenMode.ExclusiveFullScreen);
        Debug.Log("Resolution set to: " + width + " x " + height);
    }
}
