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
}

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
}*/

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
/*
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
        // Κάθε client κάνει spawn μόνο τον ΕΑΥΤΟ του
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
            playerTeam.team = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? Team.Blue : Team.Red;
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
*/

/*
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public Material blueMaterial;
    public Material redMaterial;
    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;  // Το player prefab (με PhotonView)
    [SerializeField] private Transform[] spawnPoints;  // 2 spawn points για 2 players

    private void Awake()
    {
        // Κρατάμε το NetworkManager alive ανά σκηνή
        DontDestroyOnLoad(this.gameObject);

        // Απενεργοποιούμε το αυτόματο sync της σκηνής
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    private void Start()
    {
        // Αν ήδη είμαστε στο room, κάνουμε spawn
        if (PhotonNetwork.InRoom)
            SpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("PlayerPrefab ή SpawnPoints δεν έχουν οριστεί!");
            return;
        }

        // ActorNumber ξεκινάει από 1 => index 0 ή 1
        int spawnIndex = Mathf.Clamp(PhotonNetwork.LocalPlayer.ActorNumber - 1, 0, spawnPoints.Length - 1);

        // PhotonNetwork.Instantiate δημιουργεί μοναδικό PhotonView ID για κάθε παίκτη
        PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation
        );
    }
}

*/

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public Material blueMaterial;
    public Material redMaterial;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;  // Το player prefab (με PhotonView)
    [SerializeField] private Transform[] spawnPoints;  // 2 spawn points για 2 players

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    private void Start()
    {
        if (PhotonNetwork.InRoom)
            SpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("PlayerPrefab ή SpawnPoints δεν έχουν οριστεί!");
            return;
        }

        int spawnIndex = Mathf.Clamp(PhotonNetwork.LocalPlayer.ActorNumber - 1, 0, spawnPoints.Length - 1);

        // Instantiate player
        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation
        );

        // Αλλαγή υλικού ανάλογα με τον ActorNumber
       // ApplyPlayerMaterial(player);
    }

    private void ApplyPlayerMaterial(GameObject player)
    {
        Transform player1 = player.transform.Find("Player1");
        if (player1 != null)
        {
            Transform alphaSurface = player1.Find("Alpha_Surface");
            if (alphaSurface != null)
            {
                Renderer rend = alphaSurface.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? blueMaterial : redMaterial;
                }
                else
                {
                    SkinnedMeshRenderer skinnedRend = alphaSurface.GetComponent<SkinnedMeshRenderer>();
                    if (skinnedRend != null)
                    {
                        skinnedRend.material = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? blueMaterial : redMaterial;
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
