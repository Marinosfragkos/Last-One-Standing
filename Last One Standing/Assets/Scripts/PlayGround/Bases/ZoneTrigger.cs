using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ZoneTrigger : MonoBehaviour
{
    [Header("Zone Settings")]
    public GameObject[] cubesToChange;
    public Material blueMaterial;
    public Material redMaterial;
    public Material originalMaterial;

    [Header("Progress Settings")]
    public TextMeshProUGUI blueProgressText;
    public TextMeshProUGUI redProgressText;
    public TextMeshProUGUI baseNameText;
    public string baseName = "A";

    public bool IsComplete => blueProgress >= 100f || redProgress >= 100f;

    [Header("UI Image to Change Color")]
    public RawImage rawImageToChangeColor;
    public Color blueUIColor = new Color(9f / 255f, 80f / 255f, 178f / 255f, 0.84f);
    public Color redUIColor = new Color(178f / 255f, 9f / 255f, 9f / 255f, 0.84f);
    private Color originalRawImageColor;

    public float fillSpeed = 1f;

    private float blueProgress = 0f;
    private float redProgress = 0f;

    private bool isActive = false;
    private bool playerInside = false;
    private string currentTeamInside = null;

    private void Start()
    {
        if (rawImageToChangeColor != null)
            originalRawImageColor = rawImageToChangeColor.color;

        UpdateBaseNameUI();
        UpdateProgressUI();
        DisableCubes();
    }

    private void Update()
    {
        if (!isActive || !playerInside) return;

        if (currentTeamInside == "Blue" && blueProgress < 100f)
        {
            blueProgress += fillSpeed * Time.deltaTime;
            blueProgress = Mathf.Min(blueProgress, 100f);
            UpdateProgressUI();
        }
        else if (currentTeamInside == "Red" && redProgress < 100f)
        {
            redProgress += fillSpeed * Time.deltaTime;
            redProgress = Mathf.Min(redProgress, 100f);
            UpdateProgressUI();
        }
    }

    private void UpdateProgressUI()
    {
        if (blueProgressText != null)
            blueProgressText.text = $"{blueProgress:0}%";

        if (redProgressText != null)
            redProgressText.text = $"{redProgress:0}%";
    }

    public void UpdateBaseNameUI(string customText)
    {
        if (baseNameText != null)
            baseNameText.text = customText;
    }

    private void UpdateBaseNameUI()
    {
        if (baseNameText != null)
            baseNameText.text = baseName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        Player player = pv.Owner;
        if (player == null || !player.CustomProperties.ContainsKey("team")) return;

        currentTeamInside = (string)player.CustomProperties["team"];
        playerInside = true;

        foreach (GameObject cube in cubesToChange)
        {
            Renderer rend = cube.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = currentTeamInside == "Blue" ? blueMaterial : redMaterial;
            }
        }

        if (rawImageToChangeColor != null)
            rawImageToChangeColor.color = currentTeamInside == "Blue" ? blueUIColor : redUIColor;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

        PhotonView pv = other.GetComponent<PhotonView>();
        if (pv == null) return;

        Player player = pv.Owner;
        if (player == null || !player.CustomProperties.ContainsKey("team")) return;

        string team = (string)player.CustomProperties["team"];
        if (team == currentTeamInside)
        {
            playerInside = false;

            foreach (GameObject cube in cubesToChange)
            {
                Renderer rend = cube.GetComponent<Renderer>();
                if (rend != null)
                    rend.material = originalMaterial;
            }

            if (rawImageToChangeColor != null)
                rawImageToChangeColor.color = originalRawImageColor;
        }
    }

    public void ResetZone()
    {
        blueProgress = 0f;
        redProgress = 0f;
        UpdateProgressUI();
        UpdateBaseNameUI();
    }

    public void SetActive(bool state)
    {
        isActive = state;

        if (state)
        {
            UpdateBaseNameUI();
            EnableCubes();
        }
        else
        {
            DisableCubes();
            playerInside = false;
        }
    }

    public void EnableCubes()
    {
        foreach (GameObject cube in cubesToChange)
            cube.SetActive(true);
    }

    public void DisableCubes()
    {
        foreach (GameObject cube in cubesToChange)
            cube.SetActive(false);
    }
}
