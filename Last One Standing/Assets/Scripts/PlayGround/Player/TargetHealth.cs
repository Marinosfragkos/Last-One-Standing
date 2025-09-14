using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Photon.Pun;

public class TargetHealth : MonoBehaviourPun
{
    public float maxHealth = 100f;
    public float currentHealth;
    public TMP_Text healthText; // νέο πεδίο για την υγεία

    public Slider healthSlider;
    public Image fillImage;
    public TMP_Text reviveText;

    [HideInInspector]
    public bool isDown = false;

    private Coroutine reviveCoroutine;
    private Vector3 lastPosition;
    private bool isTakingDamage = false;
    public GameObject HealthUI;
    private bool canUseZ = true; // flag για cooldown
    public float zCooldown = 10f; // δευτερόλεπτα
    public static double globalZCooldownEndTime = 0f; // PhotonNetwork.Time-based
    private bool isInsidePanel = false; // ✅ αν ο παίκτης είναι μέσα στο panel
    


    void Start()
    {
        currentHealth = maxHealth;
        isDown = false;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (fillImage != null)
            fillImage.color = Color.green;

        if (reviveText != null)
            reviveText.gameObject.SetActive(false);

        lastPosition = transform.position;
    }

   void Update()
{
    if (!photonView.IsMine) return; // ✅ μόνο ένας έλεγχος

    // Ενημέρωση UI κειμένου
    if (healthText != null)
    {
        healthText.text = $"HP: {Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
    }

    // 🔹 Test damage μόνο στον local παίκτη με το V
    if (Input.GetKeyDown(KeyCode.V))
    {
        ApplyLocalDamage(10f);
    }

    if (isDown)
    {
        if (Input.GetKeyDown(KeyCode.B) && reviveCoroutine == null)
        {
            reviveCoroutine = StartCoroutine(ReviveCountdownCoroutine());
        }

        if (reviveCoroutine != null && Vector3.Distance(transform.position, lastPosition) > 0.05f)
        {
            StopCoroutine(reviveCoroutine);
            reviveCoroutine = null;
            if (reviveText != null)
                reviveText.gameObject.SetActive(false);
            Debug.Log("Revive canceled due to movement.");
        }

        lastPosition = transform.position;
    }

    if (isInsidePanel && Input.GetKeyDown(KeyCode.X) && !isDown)
    {
        double now = PhotonNetwork.Time;
        if (now >= globalZCooldownEndTime)
        {
            currentHealth = 100;
            UpdateHealthUI();
            Debug.Log($"💥 Υγεία 100 στον {PhotonNetwork.LocalPlayer.ActorNumber}");

            photonView.RPC("StartGlobalZCooldownRPC2", RpcTarget.All);
        }
    }
}


    // Καλείται τοπικά και στέλνει RPC
    public void TakeDamage(float amount)
    {
        photonView.RPC("TakeDamageRPC", RpcTarget.All, amount);
    }

    [PunRPC]
    public void TakeDamageRPC(float amount)
    {
        if (isDown) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            EnterDownState();
            photonView.RPC("SyncDownStateRPC", RpcTarget.Others, true);
        }

        // Ξεκινάει αναγέννηση αν δεν είναι down
        if (!isDown)
        {
            StopCoroutine("RegenerateHealth");
            StartCoroutine(RegenerateHealth());
        }
    }

    public void UpdateHealthUI()
    {
        if (photonView == null || !photonView.IsMine) return;
        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (fillImage != null)
            fillImage.color = currentHealth <= 10f ? Color.red : Color.green;
        if (healthText != null)
        healthText.text = $"{Mathf.RoundToInt(currentHealth)} / {Mathf.RoundToInt(maxHealth)}";
    }

    void EnterDownState()
    {
        isDown = true;
        currentHealth = 0f;
        UpdateHealthUI();
    }

    [PunRPC]
    void SyncDownStateRPC(bool down)
    {
        isDown = down;
        UpdateHealthUI();
    }

    IEnumerator ReviveCountdownCoroutine()
    {
        if (reviveText != null)
            reviveText.gameObject.SetActive(true);

        float countdown = 4f;
        while (countdown > 0f)
        {
            if (reviveText != null)
                reviveText.text = $"{countdown:F1}s";

            countdown -= Time.deltaTime;
            yield return null;
        }

        if (reviveText != null)
            reviveText.gameObject.SetActive(false);

        reviveCoroutine = null;
        Revive();
    }

    IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(3f);
        isTakingDamage = false;

        while (currentHealth < maxHealth && !isTakingDamage && !isDown)
        {
            currentHealth += 2f;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            UpdateHealthUI();

            yield return new WaitForSeconds(3f);
        }
    }

    // Wrapper για να καλέσεις την RPC
    public void Revive(bool fullRevive = false)
    {
        photonView.RPC("ReviveRPC", RpcTarget.All, fullRevive);
    }

    [PunRPC]
    public void ReviveRPC(bool fullRevive)
    {
        isDown = false;
        currentHealth = fullRevive ? maxHealth : 30f;
        UpdateHealthUI();

        // Reset όπλου αν υπάρχει
        GunScript gun = GetComponent<GunScript>();
        if (gun != null && fullRevive)
        {
            gun.ResetAmmo();
        }

        // Ξεκινάει αναγέννηση υγείας
        StopCoroutine("RegenerateHealth");
        StartCoroutine(RegenerateHealth());
    }
     private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health")) // βάζεις το tag στο panel
        {
            SetInsidePanel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Health"))
        {
             SetInsidePanel(false);
        }
    }
    public void SetInsidePanel(bool state, int actor = -1)
    {
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine)
            return;

        isInsidePanel = state;

        // Αν το Z είναι σε cooldown, δεν ανοίγουμε το HealthUI
        double now = PhotonNetwork.Time;
        bool isGlobalCooldown = now < globalZCooldownEndTime;

        if (!isGlobalCooldown)
        {
            // Ενημερώνουμε το UI μόνο αν δεν είναι σε cooldown
            if (HealthUI != null)
                HealthUI.SetActive(state);
        }
        else
        {
            // Αν είναι σε cooldown, αφήνουμε το UI κλειστό
            if (HealthUI != null)
                HealthUI.SetActive(false);
        }

        Debug.Log($"{(state ? "Entered" : "Exited")} Health Panel for actor {actor}");
    }


[PunRPC]
private void StartGlobalZCooldownRPC2()
{
    double now = PhotonNetwork.Time;
    globalZCooldownEndTime = now + zCooldown;

    // Κλείνουμε το HealthUI για όλους
    if (HealthUI != null)
        HealthUI.SetActive(false);

    // Ξεκινάμε coroutine για επανενεργοποίηση μετά το τέλος του cooldown
    StartCoroutine(GlobalZUICheck());
}

private IEnumerator GlobalZUICheck()
{
    while (PhotonNetwork.Time < globalZCooldownEndTime)
        yield return null;

    // Επανενεργοποίηση UI μόνο αν ο παίκτης είναι μέσα στο panel
    if (isInsidePanel && HealthUI != null)
        HealthUI.SetActive(true);
}
private void ApplyLocalDamage(float amount)
{
    if (isDown) return;

    currentHealth -= amount;
    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    UpdateHealthUI();

    if (currentHealth <= 0)
        EnterDownState();
}

}
