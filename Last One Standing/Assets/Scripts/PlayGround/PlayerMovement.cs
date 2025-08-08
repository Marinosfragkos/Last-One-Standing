using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;  // Photon namespace


public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 3f;
  //  private PhotonView photonView;
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
    private bool isFinalDead = false;

    public TMP_Text deathCountdownText;
    public Image downedImage;

    /*void Awake()
{
    photonView = GetComponent<PhotonView>();
}*/

    void Start()
    {
         if (!photonView.IsMine)
        {
            // Disable audio, UI, input on remote players
            if (footstepSource != null) footstepSource.enabled = false;
            if (crawlAudioSource != null) crawlAudioSource.enabled = false;
            if (deathCountdownText != null) deathCountdownText.enabled = false;
            if (downedImage != null) downedImage.enabled = false;
            return; // Do not initialize further if not local player
        }

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

        if (downedImage != null)
            downedImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Controlling LOCAL player: " + PhotonNetwork.LocalPlayer.NickName);
        }
        else
        {
            Debug.Log("REMOTE player: " + photonView.Owner.NickName);
        }

        if (!photonView.IsMine) return;
        Debug.Log("Ελέγχω τον παίκτη μου: " + PhotonNetwork.LocalPlayer.NickName);

        if (isFinalDead)
        {
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
            if (crawlAudioSource != null && crawlAudioSource.isPlaying)
                crawlAudioSource.Stop();

            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * 20f;
        transform.Rotate(0f, mouseX, 0f);

        bool isDown = health != null && health.currentHealth <= 0;

        if (isDown)
        {
            Vector3 move = Vector3.zero;
            bool isMoving = false;

            if (Input.GetKey(KeyCode.W)) {
                ResetAllTriggers();
                move += transform.forward;
                animator.SetTrigger("Crawl");
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.S)) {
                ResetAllTriggers();
                move -= transform.forward;
                animator.SetTrigger("Crawl");
                isMoving = true;
            }
            else {
                ResetAllTriggers();
                animator.SetTrigger("CrawlIdle");
            }

            if (Input.GetKeyDown(KeyCode.V) && !isReviving)
            {
                isFinalDead = true;

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
                {
                    crawlAudioSource.Play();
                    move = Vector3.zero;
                    ResetAllTriggers();
                    animator.SetTrigger("CrawlIdle");
                    isMoving = false;
                }
                else if (!isMoving && crawlAudioSource.isPlaying)
                {
                    crawlAudioSource.Stop();
                }
            }

            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();

            float crawlSpeed = moveSpeed * 0.6f;
            controller.Move(move.normalized * crawlSpeed * Time.deltaTime);
            return;
        }

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

        if (deathCountdownText != null)
            deathCountdownText.gameObject.SetActive(true);

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
        if (downedImage != null)
            downedImage.gameObject.SetActive(false);

        animator.speed = 1f;
        isFinalDead = false;
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

    //ΧΡΗΣΙΜΟ ΓΙΑ ZoneTrigger
    public bool IsFinalDead()
    {
        return isFinalDead;
    }
}
