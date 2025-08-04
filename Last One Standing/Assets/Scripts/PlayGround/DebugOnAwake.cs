using UnityEngine;

public class DebugOnAwake : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Player prefab instantiated! Stack trace:\n" + System.Environment.StackTrace);
    }
}
