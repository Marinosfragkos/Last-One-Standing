using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    // Αυτή η μέθοδος θα συνδεθεί με το κουμπί από το Unity Editor
    public void LoadScene1()
    {
        SceneManager.LoadScene(0); // Φόρτωσε τη σκηνή με index 5
    }
}
/*
using UnityEngine;

public class LoadSceneButton : MonoBehaviour
{
    public void OnClickGoToLobby()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToLobby();
        }
        else
        {
            Debug.LogError("GameManager not found in scene!");
        }
    }
}
*/