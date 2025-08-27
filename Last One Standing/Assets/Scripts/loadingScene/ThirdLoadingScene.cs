using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class ThirdLoadingScreen : MonoBehaviourPunCallbacks
{
    public Slider progressBar;
    public TMP_Text progressText;
    public string gameplayScene = "Lobby";
    void Start()
    {
        
        
            StartCoroutine(FakeLoadingAndLoadScene());
        
    }

    IEnumerator FakeLoadingAndLoadScene()
    {
        float fakeProgress = 0f;

        while (fakeProgress < 1f)
        {
            fakeProgress += Time.deltaTime / 2f; // 2 δευτερόλεπτα fake loading
            progressBar.value = fakeProgress;
            progressText.text = $"{(fakeProgress * 100f):F0}%";
            yield return null;
        }

        // Όταν τελειώσει, φορτώνουμε με Photon ώστε όλοι να syncάρουν
        PhotonNetwork.LoadLevel(gameplayScene);
    }
}
