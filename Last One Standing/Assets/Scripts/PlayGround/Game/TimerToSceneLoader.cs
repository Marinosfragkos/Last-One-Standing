using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;  // Για TextMeshPro

public class TimerToSceneLoader : MonoBehaviour
{
    public float timeToWait =600f; // 10 λεπτά
    private float timer = 0f;

    public TextMeshProUGUI countdownText; // Σύνδεσέ το από το Inspector

    void Update()
    {
        timer += Time.deltaTime;
        float timeLeft = timeToWait - timer;

        if (timeLeft < 0) timeLeft = 0;

        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timer >= timeToWait)
        {
            SceneManager.LoadScene(4);
        }
    }
}

