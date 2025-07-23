using UnityEngine;

public class SettingsCanvasManager : MonoBehaviour
{
    public Canvas settingsCanvas; // Το βασικό Canvas ρυθμίσεων
    public Canvas[] categoryCanvases; // Τα Canvas για τις κατηγορίες ρυθμίσεων
    public GameObject[] categoryButtons; // Τα κουμπιά που αντιστοιχούν σε κάθε κατηγορία

    private Canvas currentCategoryCanvas; // Το τρέχον ανοιχτό Canvas κατηγορίας

    void Start()
    {
        // Ενεργοποιούμε μόνο το πρώτο Canvas (π.χ. Graphics), και απενεργοποιούμε τα υπόλοιπα
        for (int i = 0; i < categoryCanvases.Length; i++)
        {
            if (i == 0)
            {
                categoryCanvases[i].gameObject.SetActive(true);
                currentCategoryCanvas = categoryCanvases[i];
            }
            else
            {
                categoryCanvases[i].gameObject.SetActive(false);
            }
        }
    }

    // Εναλλαγή κατηγορίας (π.χ. Graphics, Audio κλπ.)
    public void SwitchCategoryCanvas(int categoryIndex)
    {
        if (currentCategoryCanvas != null)
        {
            currentCategoryCanvas.gameObject.SetActive(false);
        }

        if (categoryCanvases[categoryIndex] != null)
        {
            categoryCanvases[categoryIndex].gameObject.SetActive(true);
            currentCategoryCanvas = categoryCanvases[categoryIndex];
        }
    }

    // Κλείσιμο όλων των κατηγοριών και επαναφορά στο Settings Canvas
    public void CloseAllCategoryCanvases()
    {
        foreach (Canvas categoryCanvas in categoryCanvases)
        {
            categoryCanvas.gameObject.SetActive(false);
        }

        settingsCanvas.gameObject.SetActive(true);
        currentCategoryCanvas = null;
    }
}
