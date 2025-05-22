using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 2, -4f); // Πίσω από τον παίκτη

    void LateUpdate()
    {
        transform.position = player.position + player.TransformDirection(offset);
        transform.rotation = Quaternion.Euler(0, player.eulerAngles.y, 0);
    }
}
