using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Realtime;

public class PlayerVoiceSetup : MonoBehaviourPun
{
    private Recorder recorder;
    private bool micOn = false; // ξεκινάει κλειστό

   void Start()
{
    if (!photonView.IsMine) return;

    recorder = GetComponent<Recorder>();
    if (recorder == null)
    {
        Debug.LogError("Recorder component missing on Player!");
        return;
    }

    // Ορίζουμε σε ποιο group θα στέλνει ο παίκτης
    byte myGroup = (byte)((PhotonNetwork.LocalPlayer.ActorNumber <= 1) ? 1 : 2);
    recorder.InterestGroup = myGroup;

    recorder.TransmitEnabled = micOn;

    // Ξεκινάμε Coroutine που περιμένει μέχρι να συνδεθεί ο client
    StartCoroutine(WaitForVoiceConnection(myGroup));
}

private System.Collections.IEnumerator WaitForVoiceConnection(byte myGroup)
{
    // Περιμένουμε μέχρι να είναι connected ο PunVoiceClient
    yield return new WaitUntil(() => 
        PunVoiceClient.Instance.Client != null && 
        PunVoiceClient.Instance.Client.State == ClientState.Joined
    );

    // Τώρα μπορούμε να αλλάξουμε τα audio groups
    PunVoiceClient.Instance.Client.ChangeAudioGroups(new byte[0], new byte[] { myGroup });
}


    void Update()
    {
        if (!photonView.IsMine || recorder == null) return;

        // Toggle μικροφώνου με το M
        if (Input.GetKeyDown(KeyCode.M))
        {
            micOn = !micOn;
            recorder.TransmitEnabled = micOn;
            Debug.Log("Microphone: " + (micOn ? "ON" : "OFF"));
        }
    }
}
