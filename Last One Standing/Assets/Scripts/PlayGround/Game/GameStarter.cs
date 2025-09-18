using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class GameStarter : MonoBehaviourPun
{
    [Header("Gameplay Objects to Start")]
    public GameObject[] gameplayObjects; // αντικείμενα που ενεργοποιούνται συγχρονισμένα
    public AudioSource startSound;       // ήχος που παίζει συγχρονισμένα

    [Header("Settings")]
    public int requiredPlayers = 3;    // αριθμός παικτών που πρέπει να μπουν
    public float bufferTime = 0.5f;      // buffer για να προλάβουν όλοι οι clients

    private bool countdownStarted = false;


    void Start()
    {
        StartCoroutine(WaitForAllPlayers());
    }

    private IEnumerator WaitForAllPlayers()
    {
        // Περιμένουμε να μπουν όλοι οι απαιτούμενοι παίκτες
        while (PhotonNetwork.CurrentRoom == null || 
               PhotonNetwork.CurrentRoom.PlayerCount < requiredPlayers)
        {
            yield return null;
        }

        if (!countdownStarted && PhotonNetwork.IsMasterClient)
        {
            countdownStarted = true;

            // Master υπολογίζει κοινό startTime στο μέλλον
            double startTime = PhotonNetwork.Time + bufferTime;
            photonView.RPC(nameof(StartGameRPC), RpcTarget.AllBuffered, startTime);
        }
    }

    [PunRPC]
    void StartGameRPC(double startTime)
    {
        StartCoroutine(StartGameCoroutine(startTime));
    }

    private IEnumerator StartGameCoroutine(double startTime)
    {
        // Περιμένουμε μέχρι όλοι να φτάσουν στον κοινό χρόνο
        while (PhotonNetwork.Time < startTime)
            yield return null;

        // Ενεργοποιούμε gameplay objects συγχρονισμένα
        if (gameplayObjects != null)
        {
            foreach (GameObject obj in gameplayObjects)
                obj.SetActive(true);
        }

        // Παίζουμε ήχο συγχρονισμένα
        if (startSound != null)
            startSound.Play();

        Debug.Log(">>> GAME STARTS NOW <<<");
    }
}


