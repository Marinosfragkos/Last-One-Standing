
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 3f;
    private Rigidbody rb;
    private Animator animator;
    private TargetHealth health;

    public AudioSource footstepSource;
    public AudioClip footstepClip;

    public AudioSource crawlAudioSource;
    public AudioClip crawlClip;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private bool isReviving = false;
    private bool isFinalDead = false;

    public TMP_Text deathCountdownText;
    public Image downedImage;

    public static bool isSettingsOpen = false;
    public Transform fpsCam;
    private float pitch = 0f;

    void Start()
    {
        if (!photonView.IsMine)
        {
            // Disable local-only components on remote players
            if (footstepSource != null) footstepSource.enabled = false;
            if (crawlAudioSource != null) crawlAudioSource.enabled = false;
            if (deathCountdownText != null) deathCountdownText.enabled = false;
            if (downedImage != null) downedImage.enabled = false;
            return;
        }

        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponentInChildren<Animator>();
        health = GetComponent<TargetHealth>();

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        if (footstepSource != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
        }

        if (crawlAudioSource != null)
        {
            crawlAudioSource.clip = crawlClip;
            crawlAudioSource.loop = true;
        }

        if (deathCountdownText != null)
            deathCountdownText.gameObject.SetActive(false);

        if (downedImage != null)
            downedImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (isFinalDead) StopAllMovementAudio();

        if (isFinalDead || isSettingsOpen) return;

        RotatePlayer();

        if (health != null && health.currentHealth <= 0)
        {
            HandleCrawlMovement();
            return;
        }

        HandleNormalMovement();
    }

    private void RotatePlayer()
    {
          float mouseY = Input.GetAxis("Mouse Y") * 2f;
      // Πάνω - Κάτω με clamp
        pitch -= mouseY;
    pitch = Mathf.Clamp(pitch, -45f, 45f); // όριο κλίσης
    transform.localRotation = Quaternion.Euler(pitch, transform.localEulerAngles.y, 0f);

    // Για αριστερά-δεξιά, περιστρέφουμε μόνο τον παίκτη
    float mouseX = Input.GetAxis("Mouse X") * 20f;
    transform.Rotate(0f, mouseX, 0f);
    }

    private void HandleCrawlMovement()
    {
        Vector3 move = Vector3.zero;
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            move += transform.forward;
            animator.SetTrigger("Crawl");
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move -= transform.forward;
            animator.SetTrigger("Crawl");
            isMoving = true;
        }
        else
        {
            animator.SetTrigger("CrawlIdle");
        }

        if (Input.GetKeyDown(KeyCode.V) && !isReviving)
        {
            isFinalDead = true;
            StartCoroutine(ReviveAfterDeath());
            return;
        }

        PlayCrawlAudio(isMoving);

        rb.MovePosition(rb.position + move.normalized * moveSpeed * 0.6f * Time.deltaTime);
        if (footstepSource != null && footstepSource.isPlaying) footstepSource.Stop();
    }

    private void HandleNormalMovement()
    {
        Vector3 move = Vector3.zero;
        float speedMultiplier = 1f;
        bool isMoving = false;
        string trigger = "Idle";

        if (Input.GetKey(KeyCode.W)) { move += transform.forward; trigger = "Forward"; isMoving = true; }
        else if (Input.GetKey(KeyCode.S)) { move -= transform.forward; trigger = "Backward"; speedMultiplier = 0.6f; isMoving = true; }
        else if (Input.GetKey(KeyCode.A)) { move -= transform.right; speedMultiplier = 0.8f; trigger = "Left"; isMoving = true; }
        else if (Input.GetKey(KeyCode.D)) { move += transform.right; speedMultiplier = 0.8f; trigger = "Right"; isMoving = true; }

        ResetAllTriggers();
        animator.SetTrigger(trigger);

        rb.MovePosition(rb.position + move.normalized * moveSpeed * speedMultiplier * Time.deltaTime);

        PlayFootstepAudio(isMoving);
        if (crawlAudioSource != null && crawlAudioSource.isPlaying) crawlAudioSource.Stop();
    }

    private void PlayFootstepAudio(bool moving)
    {
        if (footstepSource == null) return;
        if (moving && !footstepSource.isPlaying) footstepSource.Play();
        else if (!moving && footstepSource.isPlaying) footstepSource.Stop();
    }

    private void PlayCrawlAudio(bool moving)
    {
        if (crawlAudioSource == null) return;
        if (moving && !crawlAudioSource.isPlaying) crawlAudioSource.Play();
        else if (!moving && crawlAudioSource.isPlaying) crawlAudioSource.Stop();
    }

    private void StopAllMovementAudio()
    {
        if (footstepSource != null && footstepSource.isPlaying) footstepSource.Stop();
        if (crawlAudioSource != null && crawlAudioSource.isPlaying) crawlAudioSource.Stop();
    }

    void ResetAllTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Forward");
        animator.ResetTrigger("Backward");
        animator.ResetTrigger("Left");
        animator.ResetTrigger("Right");
        animator.ResetTrigger("CrawlIdle");
        animator.ResetTrigger("Crawl");
        animator.ResetTrigger("FinalDeath");
    }

    IEnumerator ReviveAfterDeath()
    {
        isReviving = true;

        ResetAllTriggers();
        animator.SetTrigger("FinalDeath");

        yield return new WaitForSeconds(2f);

        animator.speed = 0f;
        if (deathCountdownText != null) deathCountdownText.gameObject.SetActive(true);
        if (downedImage != null) downedImage.gameObject.SetActive(true);

        float countdown = 5f;
        while (countdown > 0f)
        {
            if (deathCountdownText != null) deathCountdownText.text = $"Respawning in {Mathf.CeilToInt(countdown)}...";
            countdown -= Time.deltaTime;
            yield return null;
        }

        if (deathCountdownText != null) deathCountdownText.gameObject.SetActive(false);
        if (downedImage != null) downedImage.gameObject.SetActive(false);

        animator.speed = 1f;
        isFinalDead = false;
        health.Revive(true);

        rb.position = spawnPosition;
        rb.rotation = spawnRotation;

        ResetAllTriggers();
        animator.SetTrigger("Idle");

        isReviving = false;
    }

    public bool IsFinalDead() => isFinalDead;
}
