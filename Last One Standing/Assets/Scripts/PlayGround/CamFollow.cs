using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -4f); // Πίσω και λίγο πάνω από τον παίκτη
    public float followSpeed = 10f;
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        // Υπολογίζει τη νέα θέση πίσω από τον παίκτη, με βάση την περιστροφή του
        Vector3 desiredPosition = player.position + player.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Η κάμερα κοιτάζει πάντα τον παίκτη
        Vector3 lookTarget = player.position + Vector3.up * 1.5f;
        Quaternion lookRotation = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }
}