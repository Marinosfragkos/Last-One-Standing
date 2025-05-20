using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target; // CameraTarget (π.χ. πίσω από τον παίκτη)
    public Vector3 offset = new Vector3(0, 2, -1f); // Πίσω και πάνω
    public float followSpeed = 10f;
    public float lookHeightOffset = 1.5f; // Πού να κοιτάει (πάνω από το CameraTarget)

    void LateUpdate()
    {
        // Κίνηση κάμερας
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Να κοιτάει λίγο πάνω από το CameraTarget (π.χ. στο κεφάλι του παίκτη)
        Vector3 lookPoint = target.position + Vector3.up * lookHeightOffset;
        transform.LookAt(lookPoint);
    }
}
