using UnityEngine;

public class DefaultResolutionSetter : MonoBehaviour
{
    void Awake()
    {
        // Θέτει την ανάλυση 1920x1080 με fullscreen
        Screen.SetResolution(1920, 1080, true);
        Debug.Log("Default resolution set to 1920x1080 fullscreen");
    }
}
