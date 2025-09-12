using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider), typeof(PhotonView))]
public class AmmoZoneDisableImmediate : MonoBehaviour
{
    public string playerTag = "Player"; 
    public GameObject objectToDisable;  

    private bool playerInside = false;
    private GameObject currentPlayer;
    private bool hasBeenDisabled = false; // Flag για να μην ξανακλείνει

    private PhotonView pv;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        Collider c = GetComponent<Collider>();
        if (c == null || !c.isTrigger)
            Debug.LogWarning($"{name}: Collider missing or not set to isTrigger!");
    }

    private void Update()
    {
        if (playerInside && currentPlayer != null && Input.GetKeyDown(KeyCode.Z))
        {
            if (!hasBeenDisabled && pv != null)
            {
                pv.RPC("DisableObjectRPC", RpcTarget.All);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
        currentPlayer = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
        currentPlayer = null;
    }

    [PunRPC]
    private void DisableObjectRPC()
    {
        if (!hasBeenDisabled && objectToDisable != null)
        {
            objectToDisable.SetActive(false);
            hasBeenDisabled = true;
        }
    }
}
