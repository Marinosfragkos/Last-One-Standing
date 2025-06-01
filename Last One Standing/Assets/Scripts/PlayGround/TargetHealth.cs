using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public Slider healthSlider;
    public Image fillImage;

    private bool isTakingDamage = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (fillImage != null)
        {
            fillImage.color = Color.green;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (fillImage != null && currentHealth <= 10f)
            fillImage.color = Color.red;

        isTakingDamage = true;

        StopAllCoroutines();
        StartCoroutine(RegenerateHealth());

        if (currentHealth <= 0)
        {
            // Die();
        }
    }

    IEnumerator RegenerateHealth()
    {
        yield return new WaitForSeconds(3f);
        isTakingDamage = false;

        while (currentHealth < maxHealth && !isTakingDamage)
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

    void Die()
    {
        Destroy(gameObject);
    }
}
