/*using UnityEngine;
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
}*/

/*
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;
    // public Transform spawnPoint3;
   // public Transform spawnPoint4;

    private bool playerSpawned = false;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !playerSpawned)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        playerSpawned = true;

        Transform spawn = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? spawnPoint1 : spawnPoint2;

        PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);
    }
}
*/
/*
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public Material blueMaterial;
    public Material redMaterial;

    private bool playerSpawned = false;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !playerSpawned)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        playerSpawned = true;

        Transform spawn = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? spawnPoint1 : spawnPoint2;

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);

        // Βρίσκουμε το child "Alpha_surface" στο spawned prefab
        Transform alphaSurface = player.transform.Find("Alpha_surface");
        if (alphaSurface != null)
        {
            Renderer rend = alphaSurface.GetComponent<Renderer>();
            if (rend != null)
            {
                // Ανάλογα με το ActorNumber, αλλάζουμε το υλικό
                if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                    rend.material = blueMaterial;
                else
                    rend.material = redMaterial;
            }
            else
            {
                Debug.LogWarning("Renderer not found on Alpha_surface.");
            }
        }
        else
        {
            Debug.LogWarning("Alpha_surface child not found in player prefab.");
        }
    }
}*/

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public Transform spawnPoint1;
    public Transform spawnPoint2;

    public Material blueMaterial;
    public Material redMaterial;

    private bool playerSpawned = false;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && !playerSpawned)
        {
            SpawnPlayer();
        }
    }

    void SpawnPlayer()
    {
        playerSpawned = true;

        Transform spawn = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? spawnPoint1 : spawnPoint2;

        // Ορίζουμε την ομάδα στο CustomProperties πριν spawn
        Hashtable props = new Hashtable();
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            props["team"] = "Blue";
        else
            props["team"] = "Red";

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawn.position, spawn.rotation);

        // Ορισμός ομάδας
        PlayerTeam playerTeam = player.GetComponent<PlayerTeam>();
        if (playerTeam != null)
        {
            playerTeam.team = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? PlayerTeam.Team.Blue : PlayerTeam.Team.Red;
        }
        else
        {
            Debug.LogWarning("PlayerTeam component missing on player prefab.");
        }

        // Βρίσκουμε το "Player1" child
        Transform player1 = player.transform.Find("Player1");
        if (player1 != null)
        {
            // Μέσα στο "Player1" βρίσκουμε το "Alpha_Surface"
            Transform alphaSurface = player1.Find("Alpha_Surface");
            if (alphaSurface != null)
            {
                Renderer rend = alphaSurface.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                        rend.material = blueMaterial;
                    else
                        rend.material = redMaterial;
                }
                else
                {
                    SkinnedMeshRenderer skinnedRend = alphaSurface.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedRend != null)
                    {
                        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
                            skinnedRend.material = blueMaterial;
                        else
                            skinnedRend.material = redMaterial;
                    }
                    else
                    {
                        Debug.LogWarning("Neither Renderer nor SkinnedMeshRenderer found on Alpha_Surface.");
                    }
                }
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
