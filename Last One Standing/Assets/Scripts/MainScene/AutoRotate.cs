using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f); // βαθμοί/δευτερόλεπτο

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
