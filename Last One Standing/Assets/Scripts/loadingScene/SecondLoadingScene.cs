using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;  // Χρειάζεται για το TextMeshPro

public class SecondLoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;  // Χρήση TextMeshPro

    void Start()
    {
        StartCoroutine(LoadSceneAsync(3)); // Το Scene Index 1 είναι η επόμενη σκηνή
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false; // Μην αλλάξει σκηνή μέχρι να ολοκληρωθεί το loading

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Κανονικοποίηση στο 100%
            progressBar.value = progress;
            progressText.text = $"{progress * 100:F0}%"; // Ποσοστό χωρίς δεκαδικά

            if (progress >= 1f)
            {
                yield return new WaitForSeconds(2f); // Μικρή καθυστέρηση για smooth effect
                operation.allowSceneActivation = true; // Μετάβαση στην επόμενη σκηνή
            }

            yield return null;
        }
    }
}
