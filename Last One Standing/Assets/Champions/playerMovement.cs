using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;

    public float speed = 6f;
    public LayerMask groundMask;

    void Update()
    {
        MoveRelativeToCamera();
        RotateTowardsMouse();
    }

    void MoveRelativeToCamera()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Λήψη κατευθύνσεων από την κάμερα
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Μηδενίζουμε το Y για να μένει στο έδαφος
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Υπολογισμός τελικής κατεύθυνσης
        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;
        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundMask))
        {
            Vector3 lookDirection = hitInfo.point - transform.position;
            lookDirection.y = 0f; // μην στρέφεται προς τα πάνω/κάτω
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
