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
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            int currentPing = PhotonNetwork.GetPing();
            pingText.text = $"{currentPing} ms";

            if (currentPing <= 150)
                pingText.color = Color.green;          // Πράσινο κάτω ή ίσο με 150
            else if (currentPing <= 300)
                pingText.color = new Color(1f, 0.65f, 0f); // Πορτοκαλί (RGB: 255,165,0)
            else
                pingText.color = Color.red;            // Κόκκινο πάνω από 300

            timer = 0f;
        }
    }
}
