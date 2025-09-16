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
   //private bool canUseZ = true; // flag για cooldown
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
    Collider myCollider = GetComponentInParent<Collider>();
    PhotonView myPV = GetComponentInParent<PhotonView>();
    int myActor = myPV != null ? myPV.OwnerActorNr : -1;
    PlayerTeam.Team myTeamEnum = playerTeam != null ? playerTeam.team : PlayerTeam.Team.Red;

    RaycastHit[] hits = Physics.RaycastAll(ray, range);

    if (hits.Length > 0)
    {
        // Ταξινόμηση από κοντινότερο προς μακρινότερο
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == myCollider)
                continue; // Αγνοούμε τον εαυτό μας

            TargetHealth target = hit.transform.GetComponentInParent<TargetHealth>();
            PlayerTeam targetTeamComp = hit.transform.GetComponentInParent<PlayerTeam>();
            PhotonView targetPV = hit.transform.GetComponentInParent<PhotonView>();

            PlayerTeam.Team? targetTeam = targetTeamComp != null ? (PlayerTeam.Team?)targetTeamComp.team : null;

            string hitInfo = $"Hit object: {hit.transform.name}";
            if (targetPV != null)
                hitInfo += $" | Actor #{targetPV.OwnerActorNr}";
            if (targetTeam != null)
                hitInfo += $" | Team {targetTeam}";

            hitInfo += $" | My Actor #{myActor} | My Team {myTeamEnum}";
            Debug.Log(hitInfo);

            // Αν υπάρχει TargetHealth και δεν είναι στην ίδια ομάδα, εφαρμόζουμε damage
            if (target != null && targetTeam != null && targetTeam != myTeamEnum)
            {
                targetPV?.RPC("TakeDamageRPC", RpcTarget.All, damage);
                Debug.Log($"✅ Dealt {damage} damage to {hit.transform.name}");
            }
            else if (targetTeam != null && targetTeam == myTeamEnum)
            {
                Debug.Log("❌ Friendly fire ignored");
            }

            break; // Σταματάμε στο πρώτο hit
        }
    }
    else
    {
        Debug.Log("❌ Raycast missed everything!");
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
