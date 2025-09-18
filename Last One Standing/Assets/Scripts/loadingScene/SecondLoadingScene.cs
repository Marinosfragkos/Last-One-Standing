using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class SecondLoadingScreen : MonoBehaviourPunCallbacks
{
    public Slider progressBar;
    public TMP_Text progressText;
    public string gameplayScene = "GameScene";
    void Start()
    {
        
        
            StartCoroutine(FakeLoadingAndLoadScene());
        
    }

    IEnumerator FakeLoadingAndLoadScene()
{
    float fakeProgress = 0f;

    while (fakeProgress < 1f)
    {
        fakeProgress += Time.deltaTime / 2f; // αυξάνεται με σταθερό ρυθμό
        fakeProgress = Mathf.Clamp(fakeProgress, 0f, 1f); // ποτέ πάνω από 1

        progressBar.value = fakeProgress;
        progressText.text = $"{(fakeProgress * 100f):F0}%";
        yield return null;
    }

    // Όταν τελειώσει, φορτώνουμε με Photon ώστε όλοι να syncάρουν
    PhotonNetwork.LoadLevel(gameplayScene);
}

}
