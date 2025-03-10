using UnityEngine;

public class SettingsCanvasManager : MonoBehaviour
{
    public Canvas settingsCanvas; // Το βασικό Canvas ρυθμίσεων (θα παραμείνει ανοιχτό)
    public Canvas[] categoryCanvases; // Τα Canvas για τις κατηγορίες ρυθμίσεων
    public GameObject[] categoryButtons; // Τα κουμπιά που αντιστοιχούν σε κάθε κατηγορία

    private Canvas currentCategoryCanvas; // Το τρέχον ανοιχτό Canvas κατηγορίας

    void Start()
    {
        // Αρχικά απενεργοποιούμε όλα τα Canvas κατηγοριών, εκτός του αρχικού Settings Canvas
        foreach (Canvas categoryCanvas in categoryCanvases)
        {
            categoryCanvas.gameObject.SetActive(false);
        }
    }

    // Μέθοδος για αλλαγή κατηγορίας και ενεργοποίηση του αντίστοιχου Canvas
    public void SwitchCategoryCanvas(int categoryIndex)
    {
        // Κλείνουμε το προηγούμενο ανοιχτό Canvas κατηγορίας αν υπάρχει
        if (currentCategoryCanvas != null)
        {
            currentCategoryCanvas.gameObject.SetActive(false);
        }

        // Ενεργοποιούμε το νέο Canvas κατηγορίας
        if (categoryCanvases[categoryIndex] != null)
        {
            categoryCanvases[categoryIndex].gameObject.SetActive(true);
            currentCategoryCanvas = categoryCanvases[categoryIndex]; // Αποθηκεύουμε το τρέχον ανοιχτό Canvas
        }
    }

    // Μέθοδος για το κουμπί κλεισίματος που απενεργοποιεί όλα τα Canvas κατηγοριών
    public void CloseAllCategoryCanvases()
    {
        // Απενεργοποιούμε όλα τα Canvas κατηγοριών
        foreach (Canvas categoryCanvas in categoryCanvases)
        {
            categoryCanvas.gameObject.SetActive(false);
        }

        // Επαναφέρουμε το αρχικό Canvas (Settings)
        settingsCanvas.gameObject.SetActive(true);
        currentCategoryCanvas = null; // Δεν υπάρχει ανοιχτό Canvas κατηγορίας πλέον
    }
}
