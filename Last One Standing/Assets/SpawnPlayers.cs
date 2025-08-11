using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;
   // public Transform spawnPoint3;
   // public Transform spawnPoint4;


    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        Transform spawn;

        // Ανάλογα με το ActorNumber (σταθερό για κάθε παίκτη στο δωμάτιο)
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            spawn = spawnPoint1;
        else
            spawn = spawnPoint2;

        PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);
    }
}



/*using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    // Δύο σταθερές θέσεις spawn
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(783.3f, 10.93596f, 1722.5f), // θέση 1
        new Vector3(615.4f, 10.93596f, 1722.5f)  // θέση 2
    };

    private void Start()
    {
        // Βρίσκουμε ποιος index spawn να χρησιμοποιήσουμε
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int spawnIndex = (actorNumber - 1) % spawnPositions.Length;

        // Κάνουμε spawn
        PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPositions[spawnIndex],
            Quaternion.identity
        );
    }
}

*/