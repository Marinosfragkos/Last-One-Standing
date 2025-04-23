using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{
    public Transform target;        // Η κάψουλα που ακολουθεί η κάμερα
    public float distance = 5f;     // Απόσταση κάμερας από τον παίκτη
    public float mouseSensitivity = 100f;  // Ευαισθησία περιστροφής με το ποντίκι

    private float xRotation = 0f;   // Γωνία περιστροφής κάμερας πάνω-κάτω (pitch)
    private float yRotation = 0f;   // Γωνία περιστροφής κάμερας αριστερά-δεξιά (yaw)

    void Start()
    {
        // Κλείδωσε τον κέρσορα στο κέντρο και κρύψε τον
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Αρχικές γωνίες από την τρέχουσα θέση της κάμερας
        Vector3 angles = transform.eulerAngles;
        xRotation = angles.x;
        yRotation = angles.y;
    }

    void LateUpdate()
    {
        // Πάρε τις κινήσεις του mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Ανανέωσε τις γωνίες περιστροφής
        yRotation += mouseX;
        xRotation -= mouseY;

        // Περιορισμός της κάθετης γωνίας για να μην κάνει πλήρη περιστροφή (πχ να μην κοιτάει ανάποδα)
        xRotation = Mathf.Clamp(xRotation, -40f, 85f);

        // Δημιούργησε την περιστροφή από τις γωνίες
        Quaternion rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Υπολόγισε τη θέση της κάμερας με βάση την περιστροφή και την απόσταση
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        // Ενημέρωσε θέση και περιστροφή κάμερας
        transform.position = position;
        transform.rotation = rotation;
    }
}
