using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class ZoneSequenceManager : MonoBehaviourPun
{
    public ZoneTrigger[] zones;

    [Header("UI Elements")]
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI redScoreText;

    [Header("Audio Settings")]
    public AudioClip countdownTickSound9;
    public AudioClip countdownTickSound5;
    public AudioClip countdownEndSoundBase1;
    public AudioClip countdownEndSoundBase2;
    public AudioClip countdownEndSoundBase3;
    public AudioSource audioSource;

    [Header("End Game UI")]
    public GameObject commonEndCanvas;
    public GameObject winningTeamCanvas;
    public GameObject losingTeamCanvas;
    public GameObject drawCanvas;
    public GameObject extraCanvasToDeactivate; // Νέο canvas που θα απενεργοποιείται

    [Header("Player Settings")]
    public string playerTag = "Player";

    private int blueScore = 0;
    private int redScore = 0;

    private void Start()
    {
        // foreach (var zone in zones)
        //zone.SetActive(false);

        //StartCoroutine(ActivateZonesSequentially());

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(DelayedStart());
        }
    }

    private IEnumerator ActivateZonesSequentially()
    {
        for (int i = 0; i < zones.Length; i++)
        {
            // Έλεγχος νίκης
            if (blueScore >= 2 || redScore >= 2)
            {
                bool blueWon = blueScore > redScore;
                photonView.RPC("ShowEndGameUI", RpcTarget.All, blueWon);
                yield break;
            }

            ZoneTrigger currentZone = zones[i];
            if (currentZone == null)
            {
                Debug.LogWarning($"Zone at index {i} is null, skipping.");
                continue;
            }

            // Countdown 10 δευτ. πριν ενεργοποίηση
            double startTime = PhotonNetwork.Time;
            double endTime = startTime + 10f;

            while (PhotonNetwork.Time < endTime)
            {
                float remaining = (float)(endTime - PhotonNetwork.Time);
                int t = Mathf.CeilToInt(remaining);

                // Στείλε σε όλους την τρέχουσα αντίστροφη μέτρηση
                photonView.RPC("SyncCountdownTextRPC", RpcTarget.All, t.ToString());

                // Παίξε ήχο μόνο τοπικά αν θέλεις
                if (t == 9 && countdownTickSound9 != null)
                    audioSource.PlayOneShot(countdownTickSound9);
                else if (t == 5 && countdownTickSound5 != null)
                    audioSource.PlayOneShot(countdownTickSound5);

                yield return null;
            }

            // Καθαρισμός countdown σε όλους
            photonView.RPC("SyncCountdownTextRPC", RpcTarget.All, "");

            // Ενεργοποίηση ζώνης
            currentZone.ResetZone();
            currentZone.SetActive(true);
            photonView.RPC("SyncZoneActiveRPC", RpcTarget.Others, i, true);

            // Timer για completion της βάσης
            float zoneTime = 30f;
            float timer = 0f;

            while (!currentZone.IsComplete && timer < zoneTime)
            {
                timer += Time.deltaTime;
                float timeLeft = Mathf.Max(zoneTime - timer, 0f);

                int minutes = Mathf.FloorToInt(timeLeft / 60f);
                int seconds = Mathf.FloorToInt(timeLeft % 60f);
                photonView.RPC("SyncCountdownTextRPC", RpcTarget.All, string.Format("{0:00}:{1:00}", minutes, seconds));

                yield return null;
            }

            photonView.RPC("SyncCountdownTextRPC", RpcTarget.All, "");

            // Απονομή πόντου
            if (currentZone.blueProgress >= 100f)
                blueScore++;
            else if (currentZone.redProgress >= 100f)
                redScore++;
            else
            {
                if (currentZone.blueProgress > currentZone.redProgress)
                    blueScore++;
                else if (currentZone.redProgress > currentZone.blueProgress)
                    redScore++;
            }

            photonView.RPC("SyncScoreRPC", RpcTarget.All, blueScore, redScore);

            // Έλεγχος νίκης μετά την απονομή
            if (blueScore >= 2 || redScore >= 2)
            {
                bool blueWon = blueScore > redScore;
                photonView.RPC("ShowEndGameUI", RpcTarget.All, blueWon);
                yield break;
            }

            // Απενεργοποίηση ζώνης
            currentZone.SetActive(false);
            photonView.RPC("SyncZoneActiveRPC", RpcTarget.Others, i, false);

            if (i + 1 < zones.Length)
                yield return new WaitForSeconds(10f); // αναμονή πριν την επόμενη ζώνη
        }

        photonView.RPC("SyncCountdownTextRPC", RpcTarget.All, "");
    }
    [PunRPC]
    private void SyncCountdownTextRPC(string text)
    {
        if (countdownText != null)
            countdownText.text = text;
    }
    // RPC για συγχρονισμό scores
    [PunRPC]
    private void SyncScoreRPC(int blue, int red)
    {
        blueScoreText.text = new string('I', Mathf.Clamp(blue, 0, 3));
        redScoreText.text = new string('I', Mathf.Clamp(red, 0, 3));
    }

    // RPC για συγχρονισμό ενεργοποίησης ζώνης
    [PunRPC]
    private void SyncZoneActiveRPC(int index, bool active)
    {
        if (index >= 0 && index < zones.Length && zones[index] != null)
            zones[index].SetActive(active);
    }



    [PunRPC]
    private void ShowEndGameUI(bool blueWon)
    {
        StartCoroutine(EndGameSequence(blueWon));
    }

    private IEnumerator EndGameSequence(bool blueWon)
    {
        // Παράλληλα “παγώνουμε” όλους τους παίκτες (disable μόνο τα scripts ελέγχου)
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        foreach (GameObject p in players)
        {
            if (p.TryGetComponent(out PhotonView pv) && pv.IsMine)
            {
                // Απενεργοποιούμε μόνο τα scripts του παίκτη
                var gun = p.GetComponent<GunScript>();
                if (gun != null) gun.enabled = false;

                var movement = p.GetComponent<PlayerMovement>();
                if (movement != null) movement.enabled = false;
            }
        }

        // Απενεργοποίηση extra canvas
        if (extraCanvasToDeactivate != null)
            extraCanvasToDeactivate.SetActive(false);

        // Κοινό canvas για 3 δευτερόλεπτα
        if (commonEndCanvas != null)
            commonEndCanvas.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (commonEndCanvas != null)
            commonEndCanvas.SetActive(false);
         if ((blueScore == 1 && redScore == 1 && drawCanvas != null) || (blueScore == 1 && redScore == 0 && drawCanvas != null) || (blueScore == 0 && redScore == 1 && drawCanvas != null) || (blueScore == 0 && redScore == 0 && drawCanvas != null))
         {
             drawCanvas.SetActive(true);
             yield break; // σταματάμε εδώ, δεν δείχνουμε winning/losing canvas
         }
        // Λογική νίκης/ήττας ανα παίκτη
        int myTeamInt = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        PlayerSetup.Team myTeam = (PlayerSetup.Team)myTeamInt;

        if (blueWon)
        {
            if (myTeam == PlayerSetup.Team.Blue) // Μπλε ομάδα κέρδισε
            {
                if (winningTeamCanvas != null) winningTeamCanvas.SetActive(true);
            }
            else // Κόκκινη ομάδα έχασε
            {
                if (losingTeamCanvas != null) losingTeamCanvas.SetActive(true);
            }
        }
        else
        {
            if (myTeam == PlayerSetup.Team.Red) // Κόκκινη ομάδα κέρδισε
            {
                if (winningTeamCanvas != null) winningTeamCanvas.SetActive(true);
            }
            else // Μπλε ομάδα έχασε
            {
                if (losingTeamCanvas != null) losingTeamCanvas.SetActive(true);
            }
        }
    }
    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(15f); // Περιμένει 15 δευτερόλεπτα

        // Τώρα ξεκινάει η λογική σου
        StartCoroutine(ActivateZonesSequentially());
    }
}

