using UnityEngine;
using Photon.Pun;
using System.Collections;

public class InteractableObject : MonoBehaviourPun
{
    [Header("Cooldown Settings")]
    public float cooldown = 10f;

    /// <summary>
    /// Εξαφανίζει ή εμφανίζει το αντικείμενο για όλους τους παίκτες
    /// </summary>
    [PunRPC]
    public void RPC_SetInteractableActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Ξεκινάει το cooldown για να επανεμφανιστεί το αντικείμενο
    /// </summary>
    public void StartCooldown()
    {
        if (photonView.IsMine) // μόνο ένας κεντρικός παίχτης χειρίζεται το cooldown
            StartCoroutine(HandleCooldown());
    }

    private IEnumerator HandleCooldown()
    {
        photonView.RPC("RPC_SetInteractableActive", RpcTarget.All, false);
        yield return new WaitForSeconds(cooldown);
        photonView.RPC("RPC_SetInteractableActive", RpcTarget.All, true);
    }
}
