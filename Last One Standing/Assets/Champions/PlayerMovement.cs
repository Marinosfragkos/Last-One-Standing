using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 100f;

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

  void Update()
{
    Vector3 move = Vector3.zero;
    float currentSpeed = moveSpeed;
    string trigger = "";

        // Κίνηση με WASD
        if (Input.GetKey(KeyCode.W))
        {
            move += transform.forward;
            trigger = "Forward";

            // Περιστροφή με ποντίκι ΜΟΝΟ όταν πατιέται W
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0f, mouseX, 0f); // Περιστρέφει τον χαρακτήρα στον Y άξονα
        }
        else if (Input.GetKey(KeyCode.S))
        {
            move -= transform.forward;
            currentSpeed *= 0.6f;
            trigger = "Backward";
            // Περιστροφή με ποντίκι ΜΟΝΟ όταν πατιέται W
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0f, mouseX, 0f); // Περιστρέφει τον χαρακτήρα στον Y άξονα
        }
        else if (Input.GetKey(KeyCode.A))
        {
            move -= transform.right;
            currentSpeed *= 0.8f;
            trigger = "Left";
            // Περιστροφή με ποντίκι ΜΟΝΟ όταν πατιέται W
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0f, mouseX, 0f); // Περιστρέφει τον χαρακτήρα στον Y άξονα
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move += transform.right;
            currentSpeed *= 0.8f;
            trigger = "Right";
                // Περιστροφή με ποντίκι ΜΟΝΟ όταν πατιέται W
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(0f, mouseX, 0f); // Περιστρέφει τον χαρακτήρα στον Y άξονα
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
