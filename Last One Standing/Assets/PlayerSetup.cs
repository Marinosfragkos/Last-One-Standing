using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPun
{
    void Start()
    {
        if (!photonView.IsMine)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cam.enabled = false;

            // Αν έχεις άλλα components που πρέπει να είναι ενεργά μόνο για τοπικό παίκτη, απενεργοποίησέ τα εδώ
        }
    }
}
