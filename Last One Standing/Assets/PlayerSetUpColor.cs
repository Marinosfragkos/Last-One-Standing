using Photon.Pun;
using UnityEngine;

public class PlayerSetupColor : MonoBehaviourPun
{
    public Material blueMaterial;
    public Material redMaterial;

    void Start()
    {
        // Ορίζουμε χρώμα τοπικά
        if (photonView.IsMine)
        {
            string color = photonView.Owner.ActorNumber == 1 ? "blue" : "red";
            photonView.RPC("SetPlayerColor", RpcTarget.AllBuffered, color);
        }
    }

    [PunRPC]
    public void SetPlayerColor(string color)
    {
        Transform player1 = transform.Find("Player1");
        if (player1 != null)
        {
            Transform alphaSurface = player1.Find("Alpha_Surface");
            if (alphaSurface != null)
            {
                Renderer rend = alphaSurface.GetComponent<Renderer>();
                SkinnedMeshRenderer skinnedRend = alphaSurface.GetComponent<SkinnedMeshRenderer>();

                Material mat = color == "blue" ? blueMaterial : redMaterial;

                if (rend != null)
                    rend.material = mat;
                else if (skinnedRend != null)
                    skinnedRend.material = mat;
                else
                    Debug.LogWarning("Neither Renderer nor SkinnedMeshRenderer found on Alpha_Surface.");
            }
            else
            {
                Debug.LogWarning("Alpha_Surface child not found inside Player1.");
            }
        }
        else
        {
            Debug.LogWarning("Player1 child not found inside player prefab.");
        }
    }
}
