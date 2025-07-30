using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RegionSelection : MonoBehaviour
{
    public TMP_Dropdown regionDropdown;  // Dropdown UI
    public TMP_Text selectedRegionText;  // Το Text που θα δείχνει την περιοχή

    private string[] photonRegions = new string[]
    {
        "eu",       // Europe
        "us",       // North America
        "asia",     // Asia
        "sa",       // South America
        "au",       // Australia
        "jp",       // Japan
        "in"        // India
    };

    private string[] regionNames = new string[]
    {
        "Europe",
        "North America",
        "Asia",
        "South America",
        "Australia",
        "Japan",
        "India"
    };

    void Start()
    {
        int savedIndex = PlayerPrefs.GetInt("SelectedRegion", 0);
        regionDropdown.value = savedIndex;

        // Ενημέρωσε το Text στην αρχή
        UpdateSelectedRegionText(savedIndex);

        regionDropdown.onValueChanged.AddListener(OnRegionChanged);
    }

    public void OnRegionChanged(int index)
    {
        PlayerPrefs.SetInt("SelectedRegion", index);
        string selectedRegion = photonRegions[index];
        Debug.Log("Selected Photon region: " + selectedRegion);

        // Ενημέρωσε το Text με το όνομα
        UpdateSelectedRegionText(index);

        // Ενημέρωσε το Photon
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = selectedRegion;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        PhotonNetwork.ConnectUsingSettings();
        
    }

    void UpdateSelectedRegionText(int index)
    {
        if (selectedRegionText != null && index >= 0 && index < regionNames.Length)
        {
            selectedRegionText.text = $"{regionNames[index]}";

        }
    }
}
