using UnityEngine;
using Photon.Pun;

public class RevivePanelTrigger : MonoBehaviour
{
    public GameObject indicatorUI; // UI σταυρός
    private TargetHealth ownerHealth; // downed παίκτης
    public bool isPlayerInside = false;
    public GameObject playerInside;
    
    void Awake()
    {
        ownerHealth = GetComponentInParent<TargetHealth>();
    }

   void Update()
    {
        if (isPlayerInside && playerInside != null)
        {
            PlayerTeam reviverTeam = playerInside.GetComponent<PlayerTeam>();
            PlayerTeam downedTeam = ownerHealth.GetComponent<PlayerTeam>();

            // Εμφανίζουμε μόνο αν είναι στην ίδια ομάδα
            if (reviverTeam != null && downedTeam != null && reviverTeam.team == downedTeam.team)
            {
                indicatorUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.B) && ownerHealth.isDown)
                {
                    ownerHealth.StartReviveFromOther(playerInside);

                    PhotonView pv = ownerHealth.GetComponent<PhotonView>();
                    if (pv != null)
                        pv.RPC("StartReviveRPC", pv.Owner, playerInside.GetComponent<PhotonView>().ViewID);

                    Debug.Log($"[{playerInside.name}] started revive for [{ownerHealth.name}] via RPC");
                }
            }
            else
            {
                indicatorUI.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            isPlayerInside = true;
            playerInside = other.gameObject;

            if (indicatorUI != null)
            {
                indicatorUI.SetActive(true);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            isPlayerInside = false;
            playerInside = null;

            if (indicatorUI != null)
            {
                indicatorUI.SetActive(false);
            }
        }
    }
    public bool IsPlayerInside(GameObject player)
    {
        return playerInside == player;
    }


}
