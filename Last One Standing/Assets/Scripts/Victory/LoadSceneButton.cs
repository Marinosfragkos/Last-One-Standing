using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    // Αυτή η μέθοδος θα συνδεθεί με το κουμπί από το Unity Editor
    public void LoadScene1()
    {
        SceneManager.LoadScene(0); // Φόρτωσε τη σκηνή με index 1
    }
}
