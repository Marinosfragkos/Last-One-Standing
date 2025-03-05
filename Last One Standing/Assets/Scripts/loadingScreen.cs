using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class loadingScreen : MonoBehaviour
{
    public Slider LoadingBar;
    public TextMeshProUGUI ProgressText;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // Δεν επιτρέπει στην σκηνή να ενεργοποιηθεί αυτόματα μέχρι να είναι έτοιμο το UI
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Η πρόοδος της φόρτωσης κυμαίνεται από 0.0 έως 0.9, οπότε τη μετατρέπουμε για να φτάσει το 100%
            float progress = Mathf.Clamp01(operation.progress / 0.9f); 

            // Ενημέρωση του slider με την πρόοδο φόρτωσης
            LoadingBar.value = progress;

            // Υπολογισμός του ποσοστού
            float progressPercentage = progress * 100;

            // Ενημέρωση του κειμένου με το ποσοστό
            ProgressText.text = Mathf.RoundToInt(progressPercentage) + "%";

            // Όταν η φόρτωση είναι σχεδόν ολοκληρωμένη (0.9), ενεργοποιούμε τη σκηνή
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
