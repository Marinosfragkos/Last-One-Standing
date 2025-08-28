
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
        ApplyActorNumberText(player);
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



