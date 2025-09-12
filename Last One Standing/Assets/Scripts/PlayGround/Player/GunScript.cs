/*using UnityEngine;
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
    public TMP_Text reloadingText;

    private Coroutine fadeCoroutine;
    private bool isShootingSoundPlaying = false;
    private bool isReloading = false;
    private float defaultVolume;

    private TargetHealth health; // NEW

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        if (audioSource != null)
            defaultVolume = audioSource.volume;

        if (reloadingText != null)
            reloadingText.gameObject.SetActive(false);

        // Προσπαθούμε να βρούμε το TargetHealth του παίκτη
        health = GetComponentInParent<TargetHealth>(); // NEW
    }

   void Update()
{
    // Αν είναι ανοιχτές οι ρυθμίσεις, σταματάμε τον ήχο και επιστρέφουμε
    if (SettingsUI.isSettingsOpen)
    {
        if (isShootingSoundPlaying && audioSource.isPlaying)
        {
            audioSource.Stop();
            isShootingSoundPlaying = false;
        }
        return;
    }

    if (isReloading)
        return;

    // NEW: Μπλοκάρουμε τα πάντα αν ο παίκτης είναι σε crawl
    if ((health != null && health.currentHealth <= 0f) || (health != null && IsPlayerDown()))
        return;

    // Self-damage για debug
    if (Input.GetKeyDown(KeyCode.V))
    {
        if (health != null)
        {
            health.TakeDamage(10f);
            Debug.Log("Self damage 10 applied.");
        }
    }

    // Shooting
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

    Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
    RaycastHit hit;

   int layerMask = ~0; // Χτυπάει όλα

    if (Physics.Raycast(ray, out hit, range, layerMask))
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

        if (isShootingSoundPlaying)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutShootingSound());
        }

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

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
            yield return new WaitForSeconds(2f);
        }

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
        reserveAmmo = 90;
        UpdateAmmoUI();
    }

    // Helper για την κατάσταση crawl
    private bool IsPlayerDown()
    {
        return health != null && health.GetType().GetField("isDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(health) is bool b && b;
    }
}*/



using UnityEngine;
using TMPro;
using Photon.Pun;
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

    public float range = 100f;
    public float damage = 10f;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;

    public TMP_Text ammoText;
    public TMP_Text reloadingText;

    private TargetHealth health;
    private PlayerTeam playerTeam;
    public GameObject ammoUI;
    private bool canUseZ = true; // flag για cooldown
    public float zCooldown = 10f; // δευτερόλεπτα
    public static double globalZCooldownEndTime = 0f; // PhotonNetwork.Time-based

    private Coroutine fadeCoroutine;
    private bool isShootingSoundPlaying = false;
    private bool isReloading = false;
    private float defaultVolume;
    private bool isInsidePanel = false; // ✅ αν ο παίκτης είναι μέσα στο panel
   

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();

        health = GetComponentInParent<TargetHealth>();
        playerTeam = GetComponentInParent<PlayerTeam>();

        if (audioSource != null)
            defaultVolume = audioSource.volume;

        if (reloadingText != null)
            reloadingText.gameObject.SetActive(false);
    }

    void Update()
    {
        // ===========================
        // 1) Έλεγχος Settings
        // ===========================
        if (SettingsUI.isSettingsOpen)
        {
            // Σταματάει ο ήχος πυροβολισμού αν παίζει
            if (isShootingSoundPlaying && audioSource != null && audioSource.isPlaying)
            {
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);

                fadeCoroutine = StartCoroutine(FadeOutShootingSound());
            }
            return; // μπλοκάρουμε όλο το Update
        }

        // ===========================
        // 2) Έλεγχος reload / player down
        // ===========================
        if (isReloading) return;
        if (health != null && (health.currentHealth <= 0f || IsPlayerDown())) return;

        // ===========================
        // 3) Shooting
        // ===========================
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

        // ===========================
        // 4) Reload
        // ===========================
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadCoroutine());
        }
        // ===========================
        // 5) Ammo Panel
        // ===========================
        PhotonView pv = GetComponent<PhotonView>();
    if (pv != null && !pv.IsMine)
        return;

         if (isInsidePanel && Input.GetKeyDown(KeyCode.Z))
    {
       double now = PhotonNetwork.Time;
    if (now >= globalZCooldownEndTime)
    {
        reserveAmmo += 90;
        UpdateAmmoUI();
        Debug.Log($"💥 Δόθηκαν 90 σφαίρες στον {PhotonNetwork.LocalPlayer.ActorNumber}! Νέο reserveAmmo: {reserveAmmo}");

        // Στέλνουμε RPC σε όλους για να κλείσει το ammoUI και να ξεκινήσει το global cooldown
        pv.RPC("StartGlobalZCooldownRPC", RpcTarget.All);
        }
    }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        currentAmmo--;
        UpdateAmmoUI();

        Ray ray = new Ray(fpsCam.transform.position, fpsCam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(hitEffect, 0.5f);

            TargetHealth target = hit.transform.GetComponent<TargetHealth>();
            PlayerTeam targetTeam = hit.transform.GetComponent<PlayerTeam>();

            if (target != null && targetTeam != null && playerTeam != null)
            {
                if (targetTeam.team != playerTeam.team)
                {
                    target.photonView.RPC("TakeDamageRPC", RpcTarget.All, damage);
                }
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        if (isShootingSoundPlaying)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOutShootingSound());
        }

        if (audioSource != null && reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        if (reloadingText != null)
        {
            reloadingText.gameObject.SetActive(true);
            float reloadTime = 1f;
            while (reloadTime > 0f)
            {
                reloadingText.text = $"Reloading...\n{reloadTime:F1}s";
                reloadTime -= Time.deltaTime;
                yield return null;
            }
            reloadingText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        int bulletsNeeded = maxAmmo - currentAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, reserveAmmo);
        currentAmmo += bulletsToReload;
        reserveAmmo -= bulletsToReload;

        UpdateAmmoUI();
        isReloading = false;
    }

    public void UpdateAmmoUI()
    {
        PhotonView pv = GetComponent<PhotonView>();
    if (pv != null && !pv.IsMine)
        return;
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
        reserveAmmo = 90;
        UpdateAmmoUI();
    }

    private bool IsPlayerDown()
    {
        return health != null && health.GetType().GetField("isDown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(health) is bool b && b;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo")) // βάζεις το tag στο panel
        {
            SetInsidePanel(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ammo"))
        {
             SetInsidePanel(false);
        }
    }

    // GunScript.cs
 public void SetInsidePanel(bool state, int actor = -1)
{
    PhotonView pv = GetComponent<PhotonView>();
    if (pv != null && !pv.IsMine)
        return;

    isInsidePanel = state;

    // Αν το Z είναι σε cooldown, δεν ανοίγουμε το ammoUI
    double now = PhotonNetwork.Time;
    bool isGlobalCooldown = now < globalZCooldownEndTime;

    if (!isGlobalCooldown)
    {
        // Ενημερώνουμε το UI μόνο αν δεν είναι σε cooldown
        if (ammoUI != null)
            ammoUI.SetActive(state);
    }
    else
    {
        // Αν είναι σε cooldown, αφήνουμε το UI κλειστό
        if (ammoUI != null)
            ammoUI.SetActive(false);
    }

    Debug.Log($"{(state ? "Entered" : "Exited")} Ammo Panel for actor {actor}");
}


[PunRPC]
private void StartGlobalZCooldownRPC()
{
    double now = PhotonNetwork.Time;
    globalZCooldownEndTime = now + zCooldown;

    // Κλείνουμε το ammoUI για όλους
    if (ammoUI != null)
        ammoUI.SetActive(false);

    // Ξεκινάμε coroutine για επανενεργοποίηση μετά το τέλος του cooldown
    StartCoroutine(GlobalZUICheck());
}

private IEnumerator GlobalZUICheck()
{
    while (PhotonNetwork.Time < globalZCooldownEndTime)
        yield return null;

    // Επανενεργοποίηση UI μόνο αν ο παίκτης είναι μέσα στο panel
    if (isInsidePanel && ammoUI != null)
        ammoUI.SetActive(true);
}

}
