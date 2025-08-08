/*using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private bool hasSpawned = false;

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(783.3f, 10.93596f, 1722.5f),
        new Vector3(623.8f, 10.93596f, 1722.5f)
    };

    private string[] spawnPrefabs = new string[]
    {
        "Cube",      // Παίκτης 1
        "Cylinder"   // Παίκτης 2
    };

    void Start()
    {
        if (!hasSpawned)
            SpawnPlayer();
    }

    void SpawnPlayer()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = (actorNumber - 1) % spawnPositions.Length;

        Vector3 spawnPos = spawnPositions[index];
        string prefabName = spawnPrefabs[index];

        PhotonNetwork.Instantiate(prefabName, spawnPos, Quaternion.identity);
        Debug.Log("Spawned player: " + PhotonNetwork.LocalPlayer.NickName + " with prefab: " + prefabName);
        hasSpawned = true;
    }
}
*/





using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private Vector3[] spawnPositions = new Vector3[]
    {
       // new Vector3(783.3f, 10.93596f, 1722.5f),
        new Vector3(623.8f, 10.93596f, 1722.5f)
        //new Vector3(700f, 10.93596f, 1600f),
        //new Vector3(650f, 10.93596f, 1600f)
    };

    void Start()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = (actorNumber - 1) % spawnPositions.Length;
        Vector3 spawnPos = spawnPositions[index];
        PhotonView view = PhotonNetwork.Instantiate("Cylinder", spawnPos, Quaternion.identity).GetComponent<PhotonView>();
        Debug.Log("Spawned player with owner: " + view.Owner.NickName + " | ViewID: " + view.ViewID);
        Debug.Log("ActorNumber: " + PhotonNetwork.LocalPlayer.ActorNumber);


    }
}
/*
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject Cube;      // Παίκτης 1
    public GameObject Cylinder;  // Παίκτης 2

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(783.3f, 10.93596f, 1722.5f),  // Θέση παίκτη 1
        new Vector3(623.8f, 10.93596f, 1722.5f)   // Θέση παίκτη 2
    };

    void Start()
    {
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int index = (actorNumber - 1) % spawnPositions.Length;
        Vector3 spawnPos = spawnPositions[index];

        GameObject prefabToSpawn = (index == 0) ? Cube : Cylinder;

        GameObject spawnedPlayer = PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPos, Quaternion.identity);
        Debug.Log($"Spawned {prefabToSpawn.name} for Actor {actorNumber} at position {spawnPos}");
    }
}

*/

