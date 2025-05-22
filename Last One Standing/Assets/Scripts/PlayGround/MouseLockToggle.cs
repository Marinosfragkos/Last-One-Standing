using UnityEngine;

public class MouseLockToggle : MonoBehaviour
{
    private bool isLocked = true;

    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLocked)
                UnlockCursor();
            else
                LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isLocked = false;
    }
}
