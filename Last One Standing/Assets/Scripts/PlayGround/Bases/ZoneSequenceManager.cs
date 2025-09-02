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
    public GameObject extraCanvasToDeactivate; // Νέο canvas που θα απενεργοποιείται

    [Header("Player Settings")]
    public string playerTag = "Player";       

    private int blueScore = 0;
    private int redScore = 0;

    private void Start()
    {
        foreach (var zone in zones)
            zone.SetActive(false);

        StartCoroutine(ActivateZonesSequentially());
    }

private IEnumerator ActivateZonesSequentially()
{
    for (int i = 0; i < zones.Length; i++)
    {
        // Έλεγχος αν κάποια ομάδα έχει ήδη κερδίσει 2 βάσεις
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

        // Αντίστροφη μέτρηση 10 δευτερολέπτων πριν ενεργοποίηση
        for (int t = 10; t > 0; t--)
        {
            currentZone.UpdateBaseNameUI(t.ToString());

            if (audioSource != null)
            {
                if (t == 9 && countdownTickSound9 != null)
                    audioSource.PlayOneShot(countdownTickSound9);
                else if (t == 4 && countdownTickSound5 != null)
                    audioSource.PlayOneShot(countdownTickSound5);
            }

            yield return new WaitForSeconds(1f);
        }

        // Καθαρίζουμε το countdown πριν ενεργοποιηθεί η ζώνη
        if (countdownText != null)
            countdownText.text = "";

        // Ήχος τέλους αντίστροφης μέτρησης
        if (audioSource != null)
        {
            if (i == 0 && countdownEndSoundBase1 != null)
                audioSource.PlayOneShot(countdownEndSoundBase1);
            else if (i == 1 && countdownEndSoundBase2 != null)
                audioSource.PlayOneShot(countdownEndSoundBase2);
            else if (i == 2 && countdownEndSoundBase3 != null)
                audioSource.PlayOneShot(countdownEndSoundBase3);
        }

        currentZone.ResetZone();
        currentZone.SetActive(true);

        // Χρόνος για την ολοκλήρωση της βάσης
        float zoneTime = 30f;
        float timer = 0f;
        bool pointAwarded = false;

        while (!currentZone.IsComplete && timer < zoneTime)
        {
            timer += Time.deltaTime;
            float timeLeft = Mathf.Max(zoneTime - timer, 0f); // ποτέ αρνητικό

            int minutes = Mathf.FloorToInt(timeLeft / 60f);
            int seconds = Mathf.FloorToInt(timeLeft % 60f);
            if (countdownText != null)
                countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return null;
        }

        // Καθαρίζουμε το countdown μετά τη λήξη
        if (countdownText != null)
            countdownText.text = "";

        // Απονομή πόντου στη βάση
        if (!pointAwarded)
        {
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
        }

        blueScoreText.text = blueScore.ToString();
        redScoreText.text = redScore.ToString();

        // Έλεγχος μετά την απονομή
        if (blueScore >= 2 || redScore >= 2)
        {
            bool blueWon = blueScore > redScore;
            photonView.RPC("ShowEndGameUI", RpcTarget.All, blueWon);
            yield break;
        }

        currentZone.SetActive(false);

        if (i + 1 < zones.Length)
            yield return new WaitForSeconds(10f); // αναμονή πριν την επόμενη ζώνη
    }

    // Τελικά καθαρίζουμε το countdown
    if (countdownText != null)
        countdownText.text = "";
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


}
