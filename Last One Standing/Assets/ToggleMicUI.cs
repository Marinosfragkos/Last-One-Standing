using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ToggleMicUI : MonoBehaviourPun
{
    [Header("Mic UI")]
    public RawImage firstImage;    // RawImage που θα είναι ενεργό αρχικά
    public RawImage secondImage;   // RawImage που θα είναι απενεργό αρχικά

    private bool isFirstActive = true;

    void Start()
    {
        // Αν δεν είναι ο local player, κλείνουμε και τα δύο UI
        if (!photonView.IsMine)
        {
            if (firstImage != null) firstImage.gameObject.SetActive(false);
            if (secondImage != null) secondImage.gameObject.SetActive(false);
            return;
        }

        // Αρχική κατάσταση
        if (firstImage != null) firstImage.gameObject.SetActive(true);
        if (secondImage != null) secondImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // toggle με το M
        if (Input.GetKeyDown(KeyCode.M))
        {
            isFirstActive = !isFirstActive;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (firstImage != null) firstImage.gameObject.SetActive(isFirstActive);
        if (secondImage != null) secondImage.gameObject.SetActive(!isFirstActive);
    }
}
