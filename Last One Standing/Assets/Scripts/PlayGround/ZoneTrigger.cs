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
    public TextMeshProUGUI progressText;  // Για το ποσοστό (π.χ. "35%")
    public TextMeshProUGUI baseNameText;  // Για το όνομα βάσης (π.χ. "C")
    public string baseName = "C";          // Το όνομα της βάσης

    [Header("UI Image to Change Color")]
    public RawImage rawImageToChangeColor;

    public float fillSpeed = 1f;           // 1% ανά δευτερόλεπτο

    private float progress = 0f;
    private bool playerInside = false;

    private Color originalRawImageColor;

    private void Start()
    {
        if (baseNameText != null)
            baseNameText.text = baseName;

        if (rawImageToChangeColor != null)
            originalRawImageColor = rawImageToChangeColor.color;
    }

    private void Update()
    {
        if (playerInside && progress < 100f)
        {
            progress += fillSpeed * Time.deltaTime;
            progress = Mathf.Min(progress, 100f);
        }

        if (progressText != null)
        {
            progressText.text = $"{progress:0}%";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;

            foreach (GameObject cube in cubesToChange)
            {
                Renderer rend = cube.GetComponent<Renderer>();
                if (rend != null)
                    rend.material = newMaterial;
            }

            if (rawImageToChangeColor != null)
                rawImageToChangeColor.color = new Color(9f / 255f, 80f / 255f, 178f / 255f, 0.8431373f); // #0950B2 με alpha fade
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            foreach (GameObject cube in cubesToChange)
            {
                Renderer rend = cube.GetComponent<Renderer>();
                if (rend != null)
                    rend.material = originalMaterial;
            }

            if (rawImageToChangeColor != null)
                rawImageToChangeColor.color = originalRawImageColor; // Επαναφορά αρχικού χρώματος
        }
    }
}
