using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
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
        trigger = "Idle"; // ✅ Σωστή ανάθεση
    }

    if (!string.IsNullOrEmpty(trigger))
    {
        ResetAllTriggers();
       // Debug.Log("Setting Trigger: " + trigger); // 👈 Προσθήκη
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
