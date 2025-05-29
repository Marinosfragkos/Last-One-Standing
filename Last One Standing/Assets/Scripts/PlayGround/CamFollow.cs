using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -4f);
    public float followSpeed = 10f;
    public float lookSpeed = 10f;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), lookSpeed * Time.deltaTime);
    }
}
