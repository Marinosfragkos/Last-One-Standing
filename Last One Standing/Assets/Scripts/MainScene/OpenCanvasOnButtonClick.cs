using UnityEngine;
using UnityEngine.UI;

public class OpenCanvasOnButtonClick : MonoBehaviour
{
    public Canvas targetCanvas;           // Το Canvas που θέλουμε να ανοίξουμε
    public Button button;                 // Το κουμπί που θα πατηθεί για να ανοίξει το Canvas
    public Button buttonInsideCanvas;     // Το κουμπί μέσα στο Canvas (π.χ. Done)

    void Start()
    {
        // Κρύβουμε αρχικά το canvas
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);

        // Ορίζουμε listeners για τα κουμπιά
        if (button != null)
            button.onClick.AddListener(OpenCanvas);

        if (buttonInsideCanvas != null)
            buttonInsideCanvas.onClick.AddListener(CloseCanvas);
    }

    public void OpenCanvas()
    {
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(true);

        // Παγώνει την κάμερα/rotation του παίκτη
        //PlayerMovement.isSettingsOpen = true;
        SettingsUI.isSettingsOpen = true; // Ενημερώνει όλα τα scripts
    }

    public void CloseCanvas()
    {
        if (targetCanvas != null)
            targetCanvas.gameObject.SetActive(false);

        // Ξαναενεργοποιεί την κάμερα/rotation του παίκτη
        // PlayerMovement.isSettingsOpen = false;
       SettingsUI.isSettingsOpen = false;
    }
}
