using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TPSPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 720f; // Μοίρες ανά δευτερόλεπτο για ομαλό στρίψιμο

    private CharacterController controller;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Κλείδωμα ποντικιού
    }



void Update()
{
    MoveAndGravity();
}

void MoveAndGravity()
{
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    Vector3 move = new Vector3(h, 0, v).normalized;

    // Μετατροπή από τοπικό σε παγκόσμιο χώρο (αν έχεις κάμερα rotation)
    move = transform.TransformDirection(move);

    // Gravity
    if (controller.isGrounded && velocity.y < 0)
        velocity.y = -2f;

    velocity.y += gravity * Time.deltaTime;

    // Τελική κίνηση
    Vector3 finalVelocity = move * moveSpeed + new Vector3(0, velocity.y, 0);
    controller.Move(finalVelocity * Time.deltaTime);
}

}
