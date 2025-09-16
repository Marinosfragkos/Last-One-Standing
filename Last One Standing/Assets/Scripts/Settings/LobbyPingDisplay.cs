using UnityEngine;
using TMPro;
using Photon.Pun;

public class LobbyPingDisplay : MonoBehaviour
{
    public TextMeshProUGUI pingText;

    private float timer = 0f;
    private float updateInterval = 0.5f;

    void Update()
{
    if (pingText == null)
    {
        Debug.LogError("⚠️ pingText is NULL! Δεν έχει γίνει assign στο Inspector.");
        return;
    }

    timer += Time.deltaTime;
    if (timer >= updateInterval)
    {
        int currentPing = PhotonNetwork.GetPing();
        pingText.text = $"{currentPing} ms";

        if (currentPing <= 150)
            pingText.color = Color.green;
        else if (currentPing <= 300)
            pingText.color = new Color(1f, 0.65f, 0f);
        else
            pingText.color = Color.red;

        timer = 0f;
    }
}

}
