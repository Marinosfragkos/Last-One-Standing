using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Περιστροφή παίκτη με ποντίκι
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(0f, mouseX * rotationSpeed, 0f);

        Vector3 move = Vector3.zero;
        float currentSpeed = moveSpeed;
        string trigger = "";

        if (Input.GetKey(KeyCode.W))
        {
            move += transform.forward;
            trigger = "Forward";
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move -= transform.forward;
            currentSpeed *= 0.6f;
            trigger = "Backward";
        }
        else if (Input.GetKey(KeyCode.A))
        {
            move -= transform.right;
            currentSpeed *= 0.8f;
            trigger = "Left";
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move += transform.right;
            currentSpeed *= 0.8f;
            trigger = "Right";
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
