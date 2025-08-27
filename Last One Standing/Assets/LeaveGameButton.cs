using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LeaveGameButton : MonoBehaviourPunCallbacks
{
    public Camera fallbackCamera; // Η απενεργοποιημένη camera που θα χρησιμοποιηθεί προσωρινά

    public void OnLeaveGamePressed()
    {
        Debug.Log("[LeaveGameButton] OnLeaveGamePressed called.");

        // Ενεργοποίηση fallback camera
        if(fallbackCamera != null)
        {
            fallbackCamera.gameObject.SetActive(true);
            Debug.Log("[LeaveGameButton] Fallback camera activated.");
        }

        if (PhotonNetwork.InRoom)
        {
            Debug.Log("[LeaveGameButton] InRoom = TRUE → Calling PhotonNetwork.LeaveRoom()");
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene("ThirdLoadingScene");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("[LeaveGameButton] OnLeftRoom CALLED.");

        // Καταστροφή players και managers
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            Destroy(player);

        foreach (var gm in GameObject.FindGameObjectsWithTag("GameManager"))
            Destroy(gm);

        Debug.Log("[LeaveGameButton] Loading ThirdLoadingScene...");
        SceneManager.LoadScene("ThirdLoadingScene");
    }
}
