using UnityEngine;
using Photon.Pun;

public class SimpleLauncher : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;

    public Transform spawnPoint1;  // Ορίζεις στο Inspector το πρώτο spawn point
    public Transform spawnPoint2;  // Ορίζεις στο Inspector το δεύτερο spawn point

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        // Ορίζουμε spawn ανάλογα με το ActorNumber (1ος παίκτης στο spawnPoint1, 2ος στο spawnPoint2)
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        {
            if (spawnPoint1 != null)
            {
                spawnPos = spawnPoint1.position;
                spawnRot = spawnPoint1.rotation;
            }
        }
        else if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            if (spawnPoint2 != null)
            {
                spawnPos = spawnPoint2.position;
                spawnRot = spawnPoint2.rotation;
            }
        }
        else
        {
            // Αν έχουμε παραπάνω παίκτες, απλά βάζουμε στο spawnPoint1 ή όπου θες
            spawnPos = spawnPoint1 != null ? spawnPoint1.position : Vector3.zero;
            spawnRot = spawnPoint1 != null ? spawnPoint1.rotation : Quaternion.identity;
        }

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, spawnRot);
    }
}
