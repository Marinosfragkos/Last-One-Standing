using UnityEngine;
using UnityEngine.UI;

public class FullscreenButtons : MonoBehaviour
{
    public GameObject fullscreenOnButton;
    public GameObject fullscreenOffButton;

    void Start()
    {
        UpdateButtons();
    }

    public void SetFullscreenOn()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        UpdateButtons();
    }

    public void SetFullscreenOff()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        UpdateButtons();
    }

    void UpdateButtons()
    {
        bool isFullscreen = Screen.fullScreen;

        fullscreenOnButton.SetActive(!isFullscreen);
        fullscreenOffButton.SetActive(isFullscreen);
    }
}
