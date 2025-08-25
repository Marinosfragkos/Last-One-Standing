
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;  // για TextMeshPro

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public Material blueMaterial;
    public Material redMaterial;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;  
    [SerializeField] private Transform[] spawnPoints;  

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

        GameObject player = PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation
        );

        ApplyPlayerMaterial(player);
        ApplyActorNumberText(player);
    }

    private void ApplyPlayerMaterial(GameObject player)
    {
        Transform player1 = player.transform.Find("Player1");
        if (player1 == null) return;

        Transform alphaSurface = player1.Find("Alpha_Surface");
        if (alphaSurface == null) return;

        Photon.Realtime.Player[] allPlayers = PhotonNetwork.PlayerList; 
        int myIndex = System.Array.IndexOf(allPlayers, PhotonNetwork.LocalPlayer);

        Material mat = (myIndex < 2) ? blueMaterial : redMaterial;

        Renderer rend = alphaSurface.GetComponent<Renderer>();
        SkinnedMeshRenderer skinnedRend = alphaSurface.GetComponent<SkinnedMeshRenderer>();

        if (rend != null) rend.material = mat;
        if (skinnedRend != null) skinnedRend.material = mat;
    }

    private void ApplyActorNumberText(GameObject player)
    {
        // Υποθέτω ότι το prefab έχει TextMeshPro κάπου, π.χ. παιδί που λέγεται "PlayerText"
        TextMeshPro text = player.GetComponentInChildren<TextMeshPro>();
        if (text != null)
        {
            text.text = "Player " + PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }
}






/*
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    public Material blueMaterial;
    public Material redMaterial;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

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

        // ------------------------------
        // Ανάθεση ομάδας στον παίκτη
        PlayerTeam playerTeam = player.GetComponent<PlayerTeam>();
        if (playerTeam != null)
        {
            // Παράδειγμα: Άρτιοι -> Blue, Περιττοί -> Red
            if (PhotonNetwork.LocalPlayer.ActorNumber % 2 == 0)
                playerTeam.team = Team.Blue;
            else
                playerTeam.team = Team.Red;
        }
        // ------------------------------

        // Μπορείς να ενεργοποιήσεις και τα υλικά
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
                }
            }
        }
    }
}
*/
