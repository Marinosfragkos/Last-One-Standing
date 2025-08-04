using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private Vector3[] spawnPositions = new Vector3[]
    {
        //new Vector3(783.3f, 10.93596f, 1722.5f),
        new Vector3(623.8f, 10.93596f, 1722.5f)
    };

    void Start()
    {
        int playerIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        Vector3 spawnPos = spawnPositions[Mathf.Clamp(playerIndex, 0, spawnPositions.Length - 1)];

        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }
}
