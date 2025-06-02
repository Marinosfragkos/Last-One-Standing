using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TargetHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Slider healthSlider;
    public Image fillImage;

    public TMP_Text reviveText; // UI για αντίστροφη μέτρηση revive

    private bool isTakingDamage = false;
    private bool isDown = false;
    private Coroutine reviveCoroutine;

    private Vector3 lastPosition;

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
        {
            fillImage.color = Color.green;
        }

        if (reviveText != null)
        {
            reviveText.gameObject.SetActive(false);
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        if (isDown)
        {
            if (Input.GetKeyDown(KeyCode.B) && reviveCoroutine == null)
            {
                reviveCoroutine = StartCoroutine(ReviveCountdownCoroutine());
            }

            // Έλεγχος για ακύρωση revive αν μετακινηθεί
            if (reviveCoroutine != null && Vector3.Distance(transform.position, lastPosition) > 0.05f)
            {
                StopCoroutine(reviveCoroutine);
                reviveCoroutine = null;

                if (reviveText != null)
                {
                    reviveText.gameObject.SetActive(false);
                }

                Debug.Log("Revive canceled due to movement.");
            }

            lastPosition = transform.position;
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDown) return; // Αν είναι ήδη down δεν παίρνει damage

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (fillImage != null && currentHealth <= 10f)
            fillImage.color = Color.red;

        isTakingDamage = true;

        StopAllCoroutines();
        if (!isDown)
            StartCoroutine(RegenerateHealth());

        if (currentHealth <= 0)
        {
            EnterDownState();
        }
    }

    IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(3f);
        isTakingDamage = false;

        while (currentHealth < maxHealth && !isTakingDamage && !isDown)
        {
            currentHealth += maxHealth * 0.05f;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (healthSlider != null)
                healthSlider.value = currentHealth;

            if (fillImage != null)
                fillImage.color = Color.green;

            yield return new WaitForSeconds(3f);
        }
    }

    void EnterDownState()
    {
        isDown = true;
        currentHealth = 0f;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (fillImage != null)
            fillImage.color = Color.red;

        StopAllCoroutines(); // Σταματάμε την αναγέννηση
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

            // Ενδιάμεσος έλεγχος μετακίνησης γίνεται στο Update()
        }

        if (reviveText != null)
            reviveText.gameObject.SetActive(false);

        reviveCoroutine = null;
        Revive();
    }

    public void Revive(bool fullRevive = false)
    {
        isDown = false;

        if (fullRevive)
            currentHealth = maxHealth;
        else
            currentHealth = 30f;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (fillImage != null)
            fillImage.color = Color.green;

        StopAllCoroutines();
        StartCoroutine(RegenerateHealth());

        // Ενημέρωση όπλου αν υπάρχει
        GunScript gun = GetComponent<GunScript>();
        if (gun != null && fullRevive)
        {
            gun.ResetAmmo();
        }
    }
}
