using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ZoneTrigger : MonoBehaviour
{
    [Header("Zone Settings")]
    public GameObject[] cubesToChange;
    public Material newMaterial;
    public Material originalMaterial;

    [Header("Progress Settings")]
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI baseNameText;
    public string baseName = "A";

    [Header("UI Image to Change Color")]
    public RawImage rawImageToChangeColor;
    public float fillSpeed = 1f;

    public bool IsComplete => progress >= 100f;

    private bool isActive = false;
    private float progress = 0f;
    private bool playerInside = false;
    private Color originalRawImageColor;

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
        if (!isActive) return;

        if (playerInside && progress < 100f)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null && movement.IsFinalDead())
                    return;
            }

            progress += fillSpeed * Time.deltaTime;
            progress = Mathf.Min(progress, 100f);
            UpdateProgressUI();
        }
    }

    private void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"{progress:0}%";
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

        playerInside = true;

        foreach (GameObject cube in cubesToChange)
        {
            Renderer rend = cube.GetComponent<Renderer>();
            if (rend != null)
                rend.material = newMaterial;
        }

        if (rawImageToChangeColor != null)
            rawImageToChangeColor.color = new Color(9f / 255f, 80f / 255f, 178f / 255f, 0.8431373f);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

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

    public void ResetZone()
    {
        progress = 0f;
        UpdateProgressUI();
        UpdateBaseNameUI();
    }

    private bool IsPlayerInside()
    {
        Collider zoneCollider = GetComponent<Collider>();
        if (zoneCollider == null) return false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        Collider playerCollider = player.GetComponent<Collider>();
        if (playerCollider == null) return false;

        // Έλεγχος αν τα collider επικαλύπτονται
        return zoneCollider.bounds.Intersects(playerCollider.bounds);
    }

    public void SetActive(bool state)
    {
        isActive = state;

        if (state)
        {
            UpdateBaseNameUI();
            EnableCubes();

            if (IsPlayerInside())
                playerInside = true;
            else
                playerInside = false;
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
