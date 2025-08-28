using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSetup : MonoBehaviourPun
{
    public Material blueMaterial;
    public Material redMaterial;

    public enum Team { Blue, Red }
    public Team myTeam;

   private void Start()
{
    // Κάμερα μόνο για τον local player
    if (!photonView.IsMine)
    {
        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
            cam.enabled = false;
        return;
    }

    // ----------------------- TEAM SETUP -----------------------
    myTeam = (photonView.Owner.ActorNumber <= 1) ? Team.Blue : Team.Red;

    ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
    ht["Team"] = (int)myTeam;
    PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

    // Ορίζουμε χρώμα σε ΟΛΟΥΣ τους clients
    string color = (myTeam == Team.Blue) ? "blue" : "red";
    photonView.RPC("SetPlayerColor", RpcTarget.AllBuffered, color);

    // ----------------------- DEBUG -----------------------
    Debug.Log($"Player {photonView.Owner.NickName} (Actor {photonView.Owner.ActorNumber}) assigned to team {myTeam}");
}


    // --------------------- RPC αλλαγής χρώματος -----------------------
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

                Material mat = (color == "blue") ? blueMaterial : redMaterial;

                if (rend != null)
                    rend.material = mat;
                else if (skinnedRend != null)
                    skinnedRend.material = mat;
            }
        }
    }
}
