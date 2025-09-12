using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(PhotonView))]
public class AmmoZone : MonoBehaviour
{
    public string playerTag = "Player"; 
    public GameObject objectToDisable;  
    public float disableTime = 10f;     

    private bool playerInside = false;
    private GameObject currentPlayer;

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
            if (pv != null)
            {
                // Στέλνουμε RPC σε όλους τους παίκτες
                pv.RPC("DisableObjectRPC", RpcTarget.All, disableTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
        currentPlayer = other.gameObject;

        // Ενημέρωση GunScript αν χρειάζεται
        GunScript gun = other.GetComponentInParent<GunScript>();
        PhotonView gunPV = other.GetComponentInParent<PhotonView>();
        int actor = gunPV != null && gunPV.Owner != null ? gunPV.OwnerActorNr : -1;

        if (gun != null)
            gun.SetInsidePanel(true, actor);

        Debug.Log($"AmmoZone: OnTriggerEnter by {other.name} (Actor {actor})");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
        currentPlayer = null;

        // Ενημέρωση GunScript αν χρειάζεται
        GunScript gun = other.GetComponentInParent<GunScript>();
        PhotonView gunPV = other.GetComponentInParent<PhotonView>();
        int actor = gunPV != null && gunPV.Owner != null ? gunPV.OwnerActorNr : -1;

        if (gun != null)
            gun.SetInsidePanel(false, actor);

        Debug.Log($"AmmoZone: OnTriggerExit by {other.name} (Actor {actor})");
    }

    [PunRPC]
    private void DisableObjectRPC(float time)
    {
        if (objectToDisable != null)
            StartCoroutine(DisableTemporarily(objectToDisable, time));
    }

    private IEnumerator DisableTemporarily(GameObject obj, float time)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(time);
        obj.SetActive(true);
    }
}
