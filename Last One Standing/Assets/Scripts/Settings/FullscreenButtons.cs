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
    Screen.fullScreen = true;
    UpdateButtons();
}

public void SetFullscreenOff()
{
    Screen.fullScreenMode = FullScreenMode.Windowed;
    Screen.fullScreen = false;
    UpdateButtons();
}


    void UpdateButtons()
    {
        bool isFullscreen = Screen.fullScreen;

        fullscreenOnButton.SetActive(!isFullscreen);
        fullscreenOffButton.SetActive(isFullscreen);
    }
}
