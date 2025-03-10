using UnityEngine;
using UnityEngine.UI; // Για τα UI στοιχεία

public class OpenCanvasOnButtonClick : MonoBehaviour
{
    public Canvas targetCanvas; // Το Canvas που θέλουμε να ανοίξουμε
    public Button button; // Το κουμπί που θα πατηθεί για να ανοίξει το Canvas
    public Button buttonInsideCanvas; // Το κουμπί που θέλουμε να πατηθεί μέσα στο Canvas

    void Start()
    {
        // Διασφαλίζουμε ότι το Canvas είναι κρυφό στην αρχή
        targetCanvas.gameObject.SetActive(false);

        // Συνδέουμε την μέθοδο που θα καλέσουμε όταν πατηθεί το κουμπί
        button.onClick.AddListener(OpenCanvas);
    }

    // Μέθοδος που καλείται όταν το κουμπί πατιέται
    public void OpenCanvas()
    {
        // Ενεργοποιούμε το Canvas
        targetCanvas.gameObject.SetActive(true);

        // Πατάμε αυτόματα το κουμπί μέσα στο Canvas
        buttonInsideCanvas.onClick.Invoke();
    }
}

