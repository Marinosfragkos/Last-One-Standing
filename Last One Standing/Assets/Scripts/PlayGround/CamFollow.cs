using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target; // Ο παίκτης
    public Vector3 offset = new Vector3(0f, 10f, -7f); // Θέση κάμερας σε σχέση με τον παίκτη (π.χ., 10 μονάδες πάνω, 7 πίσω)
    public float smoothSpeed = 0.125f; // Ομαλή μετάβαση στην επιθυμητή θέση

    void LateUpdate()
    {
        // Υπολογισμός της επιθυμητής θέσης
        Vector3 desiredPosition = target.position + offset;

        // Κρατάμε σταθερή την κατακόρυφη θέση (Y) της κάμερας, ώστε να μην "πέφτει"
        desiredPosition.y = offset.y + target.position.y; // Εξασφαλίζει ότι η Y παραμένει σταθερή

        // Ομαλή μετάβαση της κάμερας στην επιθυμητή θέση
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Εξασφαλίζουμε ότι η κάμερα κοιτάζει τον χαρακτήρα (target)
        transform.LookAt(target);
    }
}
