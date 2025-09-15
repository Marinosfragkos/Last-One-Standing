using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class ZoneTrigger : MonoBehaviourPun
{
    [Header("Zone Settings")]
    public GameObject[] cubesToChange;
    public Material blueMaterial;
    public Material redMaterial;
    public Material originalMaterial;

    [Header("Progress Settings")]
    public TextMeshProUGUI blueProgressText;
    public TextMeshProUGUI redProgressText;
    public TextMeshProUGUI baseNameText;
    public string baseName = "A";

    [Header("UI Image to Change Color")]
    public RawImage rawImageToChangeColor;
    public float fillSpeed = 10f;

    public bool IsComplete => blueProgress >= 100f || redProgress >= 100f;

    private bool isActive = false;
    public float blueProgress = 0f;
    public float redProgress = 0f;
    private string lastTeam = "neutral";   // τελευταία ενεργή ομάδα
    private Color originalRawImageColor;

    // Παίκτες ανά ομάδα μέσα
    private readonly HashSet<int> bluePlayersInside = new HashSet<int>();
    private readonly HashSet<int> redPlayersInside = new HashSet<int>();

    private void Start()
    {
        if (rawImageToChangeColor != null)
            originalRawImageColor = rawImageToChangeColor.color;

        UpdateBaseNameUI();
        UpdateProgressUI();
        DisableCubes();
    }

    private void Update()
    {
        if (!isActive) return;

        if (!PhotonNetwork.IsMasterClient) return; // μόνο ο MasterClient χειρίζεται την πρόοδο

        string controllingTeam = GetControllingTeam();

        // Debug για να βλέπουμε ποιοι είναι μέσα
        Debug.Log($"[ZoneTrigger] Blue inside: {bluePlayersInside.Count}, Red inside: {redPlayersInside.Count}, Controlling: {controllingTeam}");

        // Προσθέτουμε progress μόνο για την ομάδα που ελέγχει
        if (controllingTeam == "blue")
        {
            blueProgress += fillSpeed * Time.deltaTime;
            blueProgress = Mathf.Min(blueProgress, 100f);
        }
        else if (controllingTeam == "red")
        {
            redProgress += fillSpeed * Time.deltaTime;
            redProgress = Mathf.Min(redProgress, 100f);
        }

        // Στέλνουμε update σε όλους
        photonView.RPC("SyncProgressAndColor", RpcTarget.AllBuffered, blueProgress, redProgress, controllingTeam);
        lastTeam = controllingTeam;
    }

    private string GetControllingTeam()
    {
        if (bluePlayersInside.Count > 0 && redPlayersInside.Count == 0) return "blue";
        if (redPlayersInside.Count > 0 && bluePlayersInside.Count == 0) return "red";
        return "neutral";
    }

    private void UpdateProgressUI()
    {
        if (blueProgressText != null)
            blueProgressText.text = $"{blueProgress:0}%";
        if (redProgressText != null)
            redProgressText.text = $"{redProgress:0}%";
    }

    public void UpdateBaseNameUI(string customText)
    {
        if (baseNameText != null)
            baseNameText.text = customText;
    }

    private void UpdateBaseNameUI()
    {
        if (baseNameText != null)
            baseNameText.text = baseName;
    }

    [PunRPC]
    private void SyncProgressAndColor(float blue, float red, string team)
    {
        blueProgress = blue;
        redProgress = red;
        UpdateProgressUI();
        UpdateZoneMaterial(team);
    }

    [PunRPC]
    private void UpdateZoneMaterial(string team)
    {
        Material mat = originalMaterial;
        if (team == "blue") mat = blueMaterial;
        else if (team == "red") mat = redMaterial;

        foreach (GameObject cube in cubesToChange)
        {
            if (cube == null) continue;
            var rend = cube.GetComponent<Renderer>();
            if (rend != null) rend.material = mat;
        }

        if (rawImageToChangeColor != null)
        {
            if (team == "blue")
                rawImageToChangeColor.color = new Color(0f, 0.4f, 1f, 0.8f);
            else if (team == "red")
                rawImageToChangeColor.color = new Color(0.9f, 0f, 0f, 0.9f);
            else
                rawImageToChangeColor.color = originalRawImageColor;
        }
    }

   private void OnTriggerEnter(Collider other)
{
    if (!isActive || !other.CompareTag("Player")) return;

    var movement = other.GetComponent<PlayerMovement>();
    if (movement == null) movement = other.GetComponentInParent<PlayerMovement>();
    if (movement == null) return;

    // ✅ Αποφυγή για τραυματισμένους παίκτες
    if (movement.health != null && movement.health.currentHealth <= 0)
    {
        Debug.Log($"[ZoneTrigger] Player {movement.photonView.OwnerActorNr} is down, ignoring zone entry.");
        return;
    }

    var setup = other.GetComponent<PlayerSetup>();
    if (setup == null) setup = other.GetComponentInParent<PlayerSetup>();
    if (setup == null) return;

    int actorId = setup.photonView.Owner.ActorNumber;

    // Παίρνουμε σωστά την ομάδα του παίκτη από το Photon
    PlayerSetup.Team team = setup.myTeam;
    object teamObj;
    if (setup.photonView.Owner.CustomProperties.TryGetValue("Team", out teamObj))
        team = (PlayerSetup.Team)teamObj;

    Debug.Log($"[ZoneTrigger] Player {actorId} entered zone. Team: {team}");

    // Προσθήκη στον σωστό HashSet
    if (team == PlayerSetup.Team.Blue)
        bluePlayersInside.Add(actorId);
    else if (team == PlayerSetup.Team.Red)
        redPlayersInside.Add(actorId);

    // Ανανεώνουμε άμεσα το controlling team
    UpdateControllingTeam();
}


private void OnTriggerExit(Collider other)
{
    if (!isActive || !other.CompareTag("Player")) return;

    var setup = other.GetComponent<PlayerSetup>();
    if (setup == null) setup = other.GetComponentInParent<PlayerSetup>();
    if (setup == null) return;

    int actorId = setup.photonView.Owner.ActorNumber;

    // Παίρνουμε σωστά την ομάδα του παίκτη από το Photon
    PlayerSetup.Team team = setup.myTeam;
    object teamObj;
    if (setup.photonView.Owner.CustomProperties.TryGetValue("Team", out teamObj))
        team = (PlayerSetup.Team)teamObj;

    Debug.Log($"[ZoneTrigger] Player {actorId} exited zone. Team: {team}");

    if (team == PlayerSetup.Team.Blue)
        bluePlayersInside.Remove(actorId);
    else if (team == PlayerSetup.Team.Red)
        redPlayersInside.Remove(actorId);

    // Αν δεν υπάρχει κανείς στη ζώνη, γίνεται neutral
    if (bluePlayersInside.Count == 0 && redPlayersInside.Count == 0)
    {
        lastTeam = "neutral";
        if (photonView != null)
            photonView.RPC("UpdateZoneMaterial", RpcTarget.AllBuffered, "neutral");
    }
    else
    {
        UpdateControllingTeam();
    }
}

// Νέο helper method για να καθορίσει ποιος ελέγχει τη ζώνη
private void UpdateControllingTeam()
{
    string controllingTeam = "neutral";

    if (bluePlayersInside.Count > 0 && redPlayersInside.Count == 0)
        controllingTeam = "blue";
    else if (redPlayersInside.Count > 0 && bluePlayersInside.Count == 0)
        controllingTeam = "red";

    if (controllingTeam != lastTeam)
    {
        lastTeam = controllingTeam;
        if (photonView != null)
            photonView.RPC("UpdateZoneMaterial", RpcTarget.AllBuffered, controllingTeam);
    }

    Debug.Log($"[ZoneTrigger] Blue inside: {bluePlayersInside.Count}, Red inside: {redPlayersInside.Count}, Controlling: {controllingTeam}");
}

    public void ResetZone()
    {
        blueProgress = 0f;
        redProgress = 0f;
        lastTeam = "neutral";
        UpdateProgressUI();
        UpdateZoneMaterial("neutral");
    }

    public void SetActive(bool state)
{
    isActive = state;

    if (state)
    {
        UpdateBaseNameUI();
        EnableCubes();

        // Καθαρίζουμε τυχόν παλιά δεδομένα
        bluePlayersInside.Clear();
        redPlayersInside.Clear();

        // Παίρνουμε τον collider της ζώνης
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            // Βρίσκουμε όλα τα colliders που είναι ήδη μέσα
            Collider[] hits = Physics.OverlapBox(col.bounds.center, col.bounds.extents, transform.rotation);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    var setup = hit.GetComponent<PlayerSetup>();
                    if (setup == null) setup = hit.GetComponentInParent<PlayerSetup>();
                    if (setup == null) continue;

                    int actorId = setup.photonView.Owner.ActorNumber;

                    // Βρίσκουμε ομάδα από custom properties
                    PlayerSetup.Team team = setup.myTeam;
                    object teamObj;
                    if (setup.photonView.Owner.CustomProperties.TryGetValue("Team", out teamObj))
                        team = (PlayerSetup.Team)teamObj;

                    if (team == PlayerSetup.Team.Blue)
                        bluePlayersInside.Add(actorId);
                    else if (team == PlayerSetup.Team.Red)
                        redPlayersInside.Add(actorId);

                    Debug.Log($"[ZoneTrigger] Player {actorId} already inside zone on activation. Team: {team}");
                }
            }

            // Ενημερώνουμε άμεσα το controlling team
            UpdateControllingTeam();
        }
    }
    else
    {
        DisableCubes();
        bluePlayersInside.Clear();
        redPlayersInside.Clear();
        lastTeam = "neutral";
    }
}


    public void EnableCubes()
    {
        foreach (GameObject cube in cubesToChange)
            cube.SetActive(true);
    }

    public void DisableCubes()
    {
        foreach (GameObject cube in cubesToChange)
            cube.SetActive(false);
    }
}
