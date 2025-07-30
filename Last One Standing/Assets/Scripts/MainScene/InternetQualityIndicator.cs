using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InternetQualityChecker : MonoBehaviour
{
    public GameObject veryLowImg;
    public GameObject lowImg;
    public GameObject mediumImg;
    public GameObject goodImg;

    private void Start()
    {
        StartCoroutine(CheckConnectionLoop());
    }

    IEnumerator CheckConnectionLoop()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternetSpeed());
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator CheckInternetSpeed()
    {
        float startTime = Time.time;

        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com/generate_204");
        request.timeout = 3;
        yield return request.SendWebRequest();

        float elapsedTime = (Time.time - startTime) * 1000f; // σε milliseconds

        UpdateUI(request.result == UnityWebRequest.Result.Success, elapsedTime);
    }

    void UpdateUI(bool success, float ms)
    {
        veryLowImg.SetActive(false);
        lowImg.SetActive(false);
        mediumImg.SetActive(false);
        goodImg.SetActive(false);

        if (!success || ms > 1000)
        {
            veryLowImg.SetActive(true);
        }
        else if (ms > 600)
        {
            lowImg.SetActive(true);
        }
        else if (ms > 300)
        {
            mediumImg.SetActive(true);
        }
        else
        {
            goodImg.SetActive(true);
        }

        Debug.Log($"Ping: {ms} ms");
    }
}
