using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    private CharacterController controller;
    private Animator animator;

    public AudioSource footstepSource;
    public AudioClip footstepClip;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        if (footstepSource != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
        }
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * 20f;
        transform.Rotate(0f, mouseX, 0f);

        Vector3 move = Vector3.zero;
        float currentSpeed = moveSpeed;
        string trigger = "";
        bool isMoving = false;

        if (Input.GetKey(KeyCode.W))
        {
            move += transform.forward;
            trigger = "Forward";
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move -= transform.forward;
            currentSpeed *= 0.6f;
            trigger = "Backward";
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            move -= transform.right;
            currentSpeed *= 0.8f;
            trigger = "Left";
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move += transform.right;
            currentSpeed *= 0.8f;
            trigger = "Right";
            isMoving = true;
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

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);

        // Παίξε ή σταμάτα τα βήματα ανάλογα με το αν κινείται
        if (footstepSource != null)
        {
            if (isMoving && !footstepSource.isPlaying)
                footstepSource.Play();
            else if (!isMoving && footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }

    void ResetAllTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Forward");
        animator.ResetTrigger("Backward");
        animator.ResetTrigger("Left");
        animator.ResetTrigger("Right");
    }
}
