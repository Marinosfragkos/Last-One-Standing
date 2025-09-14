using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(Collider), typeof(PhotonView))]
public class AidZone : MonoBehaviour
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
        if (playerInside && currentPlayer != null && Input.GetKeyDown(KeyCode.X))
        {
            if (pv != null)
            {
                // Στέλνουμε RPC σε όλους τους παίκτες
                pv.RPC("DisableObjectRPC2", RpcTarget.All, disableTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = true;
        currentPlayer = other.gameObject;

        // Ενημέρωση TargetHealth αν χρειάζεται
        TargetHealth targetHealth = other.GetComponentInParent<TargetHealth>();
        PhotonView targetHealthPV = other.GetComponentInParent<PhotonView>();
        int actor = targetHealthPV != null && targetHealthPV.Owner != null ? targetHealthPV.OwnerActorNr : -1;

        if (targetHealth != null)
            targetHealth.SetInsidePanel(true, actor);

        Debug.Log($"AmmoZone: OnTriggerEnter by {other.name} (Actor {actor})");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInside = false;
        currentPlayer = null;

        // Ενημέρωση TargetHealth αν χρειάζεται
        TargetHealth targetHealth = other.GetComponentInParent<TargetHealth>();
        PhotonView targetHealthPV = other.GetComponentInParent<PhotonView>();
        int actor = targetHealthPV != null && targetHealthPV.Owner != null ? targetHealthPV.OwnerActorNr : -1;

        if (targetHealth != null)
            targetHealth.SetInsidePanel(false, actor);

        Debug.Log($"AmmoZone: OnTriggerExit by {other.name} (Actor {actor})");
    }

    [PunRPC]
    private void DisableObjectRPC2(float time)
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
