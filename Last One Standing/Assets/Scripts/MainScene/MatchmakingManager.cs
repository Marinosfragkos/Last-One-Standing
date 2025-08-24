using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MatchmakingManager : MonoBehaviourPunCallbacks
{
    public Button startButton;
    public TextMeshProUGUI startButtonText;
    public Image startButtonImage;
    public GameObject timeoutPanel;
    public Button waitButton;
    public Button leaveButton;

    private Color originalColor;
    private Color disabledColor = new Color(0f, 0f, 0f, 0.5f);

    private float elapsedTime = 0f;
    private bool isCounting = false;
    private bool wantsToJoinRoom = false;
    private bool hasShownTimeoutPanel = false;

    private const string roomName = "MainRoom"; // Σταθερό όνομα δωματίου

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        startButtonText.text = "Start";
        timeoutPanel.SetActive(false);
        startButton.interactable = true;

        if (startButtonImage == null)
            startButtonImage = startButton.GetComponent<Image>();

        originalColor = startButtonImage.color;

        waitButton.onClick.AddListener(OnWaitButtonPressed);
        leaveButton.onClick.AddListener(OnLeaveButtonPressed);
    }

    public void OnStartButtonPressed()
    {
        if (!isCounting)
        {
            wantsToJoinRoom = true;
            elapsedTime = 0f;
            isCounting = true;
            hasShownTimeoutPanel = false;
            timeoutPanel.SetActive(false);
            startButtonImage.color = disabledColor;

            if (!PhotonNetwork.IsConnected)
                PhotonNetwork.ConnectUsingSettings();
            else
                JoinOrCreateMainRoom();
        }
        else
        {
            StopMatchmaking();
        }
    }

    void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;

            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            startButtonText.text = string.Format("<size=65%>Matchmaking: {0:00}:{1:00}</size>", minutes, seconds);

            if (!hasShownTimeoutPanel && elapsedTime >= 30f)
            {
                hasShownTimeoutPanel = true;
                timeoutPanel.SetActive(true);
            }
        }
    }

    void JoinOrCreateMainRoom()
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 1};
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");
        if (wantsToJoinRoom)
        {
            JoinOrCreateMainRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name} | Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
        CheckStartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player entered: {newPlayer.NickName} | Total: {PhotonNetwork.CurrentRoom.PlayerCount}");
        CheckStartGame();
    }

    void CheckStartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("All players ready, loading SecondLoadingScene...");
            PhotonNetwork.LoadLevel("SecondLoadingScene");
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
    }

    void StopMatchmaking()
    {
        isCounting = false;
        elapsedTime = 0f;
        startButtonText.text = "Start";
        wantsToJoinRoom = false;
        timeoutPanel.SetActive(false);
        startButtonImage.color = originalColor;

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        else if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }

    void OnWaitButtonPressed()
    {
        timeoutPanel.SetActive(false);
    }

    void OnLeaveButtonPressed()
    {
        StopMatchmaking();
    }
}
