using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;

public class PlayerVoiceSetup : MonoBehaviourPun
{
    private Recorder recorder;
    private bool micOn = false; // ξεκινάει κλειστό

    void Start()
    {
        if (!photonView.IsMine) return;

        recorder = GetComponent<Recorder>();
        if (recorder == null) return;

        recorder.TransmitEnabled = micOn;
    }

    void Update()
    {
        if (!photonView.IsMine || recorder == null) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            micOn = !micOn;
            recorder.TransmitEnabled = micOn;
            Debug.Log("Microphone: " + (micOn ? "ON" : "OFF"));
        }
    }
}


