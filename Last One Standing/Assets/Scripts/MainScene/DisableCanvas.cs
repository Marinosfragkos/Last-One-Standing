using UnityEngine;
using UnityEngine.UI;

public class DisableCanvas : MonoBehaviour
{
    public Canvas targetCanvas; // Το Canvas που θα απενεργοποιηθεί
    public Button disableButton; // Το κουμπί που θα το απενεργοποιεί

    void Start()
    {
        if (disableButton != null)
        {
            disableButton.onClick.AddListener(HideCanvas); // Συνδέουμε το event
        }
    }

    void HideCanvas()
    {
        if (targetCanvas != null)
        {
            targetCanvas.gameObject.SetActive(false); // Απενεργοποιούμε το Canvas
        }
    }
}
