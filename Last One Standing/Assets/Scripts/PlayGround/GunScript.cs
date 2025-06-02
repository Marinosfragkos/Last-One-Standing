using UnityEngine;
using TMPro;
using System.Collections;

public class GunScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    public int maxAmmo = 30;
    public int reserveAmmo = 90;
    private int currentAmmo;

    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;

    public float range = 10000f;
    public float damage = 10f;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;

    public TMP_Text ammoText;
    public TMP_Text reloadingText; // UI text για Reloading

    private Coroutine fadeCoroutine;
    private bool isShootingSoundPlaying = false;
    private bool isReloading = false; // για να μπλοκάρουμε το shooting κατά το reload
    private float defaultVolume;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        if (audioSource != null)
        {
            defaultVolume = audioSource.volume;
        }

        if (reloadingText != null)
            reloadingText.gameObject.SetActive(false); // κρύβουμε το reloading UI στην αρχή
    }

    void Update()
    {
        if (isReloading)
            return; // Απαγορεύουμε πυροβολισμό κατά το reload



//damage to me///////////
            if (Input.GetKeyDown(KeyCode.V))
    {
        TargetHealth targetHealth = GetComponent<TargetHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(10f);
            Debug.Log("Self damage 10 applied.");
        }
    }
////////////////////////////





        bool canShoot = Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0;

        if (canShoot)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();

            if (!isShootingSoundPlaying && audioSource != null && shootSound != null)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);

                audioSource.clip = shootSound;
                audioSource.loop = true;
                audioSource.volume = defaultVolume;
                audioSource.Play();
                isShootingSoundPlaying = true;
            }
        }

        if ((!Input.GetButton("Fire1") || currentAmmo <= 0) && isShootingSoundPlaying)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutShootingSound());
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        currentAmmo--;
        UpdateAmmoUI();

        Ray ray = fpsCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            GameObject hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(hitEffect, 0.5f);

            TargetHealth target = hit.transform.GetComponent<TargetHealth>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("Nothing hit!");
        }

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        // Σταμάτα τον ήχο πυροβολισμού με fade
        if (isShootingSoundPlaying)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutShootingSound());
            if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        }
        // Παίξε τον ήχο reload (χωρίς loop)
         if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        // Εμφάνισε το UI Reloading με αντίστροφη μέτρηση 2 δευτερολέπτων
        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(true);
            float reloadTime = 1f;
            while (reloadTime > 0f)
            {
                reloadingText.text = $"Reloading... {reloadTime:F1}s";
                reloadTime -= Time.deltaTime;
                yield return null;
            }
            reloadingText.gameObject.SetActive(false);
        }
        else
        {
            // Αν δεν έχεις UI, απλά περίμενε 2 δευτερόλεπτα
            yield return new WaitForSeconds(2f);
        }

        // Γέμισε το όπλο
        int bulletsNeeded = maxAmmo - currentAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, reserveAmmo);
        currentAmmo += bulletsToReload;
        reserveAmmo -= bulletsToReload;

        UpdateAmmoUI();
       
        isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
    }

    IEnumerator FadeOutShootingSound(float duration = 0.3f)
    {
        isShootingSoundPlaying = false;

        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = defaultVolume;
    }
    public void ResetAmmo()
{
    currentAmmo = maxAmmo;
    reserveAmmo = 90; // Ή ό,τι αρχική τιμή έχεις
    UpdateAmmoUI();
}

}
