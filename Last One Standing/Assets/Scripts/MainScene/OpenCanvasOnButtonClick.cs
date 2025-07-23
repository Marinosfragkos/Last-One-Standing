using UnityEngine;
using UnityEngine.UI;

public class OpenCanvasOnButtonClick : MonoBehaviour
{
    public Canvas targetCanvas; // Το Canvas που θέλουμε να ανοίξουμε
    public Button button; // Το κουμπί που θα πατηθεί για να ανοίξει το Canvas
    public Button buttonInsideCanvas; // Το κουμπί μέσα στο Canvas (π.χ. Done)

    void Start()
    {
        targetCanvas.gameObject.SetActive(false); // Κρύβουμε αρχικά το canvas
        button.onClick.AddListener(OpenCanvas);   // Όταν πατιέται το κουμπί, ανοίγουμε το canvas
        buttonInsideCanvas.onClick.AddListener(CloseCanvas); // Όταν πατηθεί Done, κλείνουμε
    }

    public void OpenCanvas()
{
    targetCanvas.gameObject.SetActive(true);
    SettingsUI.isSettingsOpen = true;
    CamFollow.isCameraLocked = true; // ✅ Κλειδώνει την κάμερα
}

public void CloseCanvas()
{
    targetCanvas.gameObject.SetActive(false);
    SettingsUI.isSettingsOpen = false;
    CamFollow.isCameraLocked = false; // ✅ Ξεκλειδώνει την κάμερα
}

}
