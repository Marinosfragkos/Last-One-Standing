using UnityEngine;

public class MouseLockToggle : MonoBehaviour
{
    private bool isLocked = false;

    void Start()
    {
        UnlockCursor();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && !isLocked)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Κρύβει τελείως τον κέρσορα και τον κλειδώνει
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
