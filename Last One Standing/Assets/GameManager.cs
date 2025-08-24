using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPun
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToLobby()
    {
        StartCoroutine(CleanAndLoadLobby());
    }

    private IEnumerator CleanAndLoadLobby()
    {
        // 🔹 Καθαρίζουμε όλους τους παίκτες και αντικείμενα Photon
        PhotonView[] allPVs = Object.FindObjectsByType<PhotonView>(FindObjectsSortMode.None);
        foreach (var pv in allPVs)
        {
            if (pv.IsMine || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.RemoveRPCs(pv); // Αφαιρεί buffered RPCs
                PhotonNetwork.Destroy(pv.gameObject);
            }
        }

        // 🔹 Καθαρίζουμε UI elements που δεν είναι PhotonViews
        Canvas[] allCanvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var c in allCanvases)
        {
            Destroy(c.gameObject);
        }

       TextMeshProUGUI[] allTexts = Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);

        foreach (var t in allTexts)
        {
            Destroy(t.gameObject);
        }

        // 🔹 Καθαρίζουμε zones (αν είναι απλά GameObjects)
       ZoneTrigger[] allZones = Object.FindObjectsByType<ZoneTrigger>(FindObjectsSortMode.None);
        foreach (var z in allZones)
        {
            Destroy(z.gameObject);
        }

        yield return new WaitForSeconds(0.1f); // Μικρό delay για να καθαρίσει το δίκτυο

        // 🔹 Φεύγουμε από το room και πάμε στο Lobby
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        SceneManager.LoadScene("Lobby"); // Ονομασία σκηνής Lobby
    }
}
