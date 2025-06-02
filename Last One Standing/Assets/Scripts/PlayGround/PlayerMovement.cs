using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI; // ✅ Για Image

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;

    private CharacterController controller;
    private Animator animator;
    private TargetHealth health;

    public AudioSource footstepSource;
    public AudioClip footstepClip;

    public AudioSource crawlAudioSource;
    public AudioClip crawlClip;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private float originalHeight;
    private Vector3 originalCenter;

    private bool isReviving = false;

    public TMP_Text deathCountdownText;

    // ✅ ΝΕΟ: Εικόνα που εμφανίζεται κατά τη διάρκεια του FinalDeath
    public Image downedImage;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<TargetHealth>();

        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        originalHeight = controller.height;
        originalCenter = controller.center;

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

        // ✅ Απόκρυψη εικόνας στην αρχή
        if (downedImage != null)
            downedImage.gameObject.SetActive(false);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * 20f;
        transform.Rotate(0f, mouseX, 0f);

        bool isDown = health != null && health.currentHealth <= 0;

        if (isDown)
        {
            Vector3 move = Vector3.zero;
            bool isMoving = false;

            if (Input.GetKey(KeyCode.W))
            {
                move += transform.forward;
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                move -= transform.forward;
                isMoving = true;
            }

            ResetAllTriggers();
            animator.SetTrigger("Crawl");

            if (Input.GetKeyDown(KeyCode.V) && !isReviving)
            {
                Debug.Log("Trigger FinalDeath");

                controller.height = originalHeight;
                controller.center = originalCenter;

                ResetAllTriggers();
                animator.SetTrigger("FinalDeath");

                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 10f))
                {
                    transform.position = hit.point;
                }

                StartCoroutine(ReviveAfterDeath());
                return;
            }

            if (crawlAudioSource != null)
            {
                if (isMoving && !crawlAudioSource.isPlaying)
                    crawlAudioSource.Play();
                else if (!isMoving && crawlAudioSource.isPlaying)
                    crawlAudioSource.Stop();
            }

            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();

            float crawlSpeed = moveSpeed * 0.6f;
            controller.Move(move.normalized * crawlSpeed * Time.deltaTime);
            return;
        }

        // Κανονική κίνηση...
        Vector3 moveNormal = Vector3.zero;
        float currentSpeed = moveSpeed;
        string trigger = "";
        bool isMovingNormal = false;

        if (Input.GetKey(KeyCode.W))
        {
            moveNormal += transform.forward;
            trigger = "Forward";
            isMovingNormal = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveNormal -= transform.forward;
            currentSpeed *= 0.6f;
            trigger = "Backward";
            isMovingNormal = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveNormal -= transform.right;
            currentSpeed *= 0.8f;
            trigger = "Left";
            isMovingNormal = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveNormal += transform.right;
            currentSpeed *= 0.8f;
            trigger = "Right";
            isMovingNormal = true;
        }
        else if (!Input.anyKey)
        {
            trigger = "Idle";
        }

        if (!string.IsNullOrEmpty(trigger))
        {
            ResetAllTriggers();
            animator.SetTrigger(trigger);
        }

        controller.Move(moveNormal.normalized * currentSpeed * Time.deltaTime);

        if (footstepSource != null)
        {
            if (isMovingNormal && !footstepSource.isPlaying)
                footstepSource.Play();
            else if (!isMovingNormal && footstepSource.isPlaying)
                footstepSource.Stop();
        }

        if (crawlAudioSource != null && crawlAudioSource.isPlaying)
            crawlAudioSource.Stop();
    }

    void ResetAllTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Forward");
        animator.ResetTrigger("Backward");
        animator.ResetTrigger("Left");
        animator.ResetTrigger("Right");
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

        if (deathCountdownText != null)
            deathCountdownText.gameObject.SetActive(true);

        // ✅ Εμφάνιση εικόνας "Downed"
        if (downedImage != null)
            downedImage.gameObject.SetActive(true);

        float countdown = 5f;
        while (countdown > 0f)
        {
            if (deathCountdownText != null)
                deathCountdownText.text = $"Respawning in {Mathf.CeilToInt(countdown)}...";

            countdown -= Time.deltaTime;
            yield return null;
        }

        if (deathCountdownText != null)
            deathCountdownText.gameObject.SetActive(false);

        // ✅ Απόκρυψη εικόνας "Downed"
        if (downedImage != null)
            downedImage.gameObject.SetActive(false);

        animator.speed = 1f;

        health.Revive(true);

        controller.enabled = false;
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;
        controller.enabled = true;

        controller.height = originalHeight;
        controller.center = originalCenter;

        ResetAllTriggers();
        animator.SetTrigger("Idle");

        if (crawlAudioSource != null && crawlAudioSource.isPlaying)
            crawlAudioSource.Stop();

        isReviving = false;
    }
}
