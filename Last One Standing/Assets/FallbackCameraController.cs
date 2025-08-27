using UnityEngine;
using UnityEngine.SceneManagement;

public class FallbackCameraController : MonoBehaviour
{
    void OnEnable()
    {
        // Συνδρομή στο event όταν αλλάζει σκηνή
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Απενεργοποίηση της camera μόλις φορτωθεί η νέα σκηνή
        gameObject.SetActive(false);
        Debug.Log("[FallbackCameraController] Fallback camera deactivated after scene load.");
    }
}
