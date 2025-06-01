using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GunScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip shootSound;

    public int maxAmmo = 30;
    public int reserveAmmo = 90;
    private int currentAmmo;

    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;

    public float range = 1000000f;
    public float damage = 10f;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab;

    public TMP_Text ammoText; // UI για σφαίρες

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

   void Shoot()
{
    muzzleFlash.Play();
    currentAmmo--;
    UpdateAmmoUI();

    // Παίξε τον ήχο πυροβολισμού
    if (audioSource != null && shootSound != null)
    {
        audioSource.PlayOneShot(shootSound);
    }

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



    void Reload()
    {
        int bulletsNeeded = maxAmmo - currentAmmo;
        if (reserveAmmo <= 0 || bulletsNeeded == 0) return;

        int bulletsToReload = Mathf.Min(bulletsNeeded, reserveAmmo);
        currentAmmo += bulletsToReload;
        reserveAmmo -= bulletsToReload;

        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
    }
}
