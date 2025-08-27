using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameResetter : MonoBehaviourPunCallbacks
{
    public override void OnLeftRoom()
    {
        // Καταστροφή όλων των player objects (Photon aware)
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>() != null)
                PhotonNetwork.Destroy(player);
            else
                Destroy(player);
        }

        // Καταστροφή managers που ήταν μόνο για το game scene
        var managers = GameObject.FindGameObjectsWithTag("GameManager");
        foreach (var m in managers)
        {
            Destroy(m);
        }

        // Μετά από όλα → φόρτωση Lobby
        SceneManager.LoadScene("ThirdLoadingScene");
    }
}
