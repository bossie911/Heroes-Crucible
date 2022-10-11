using UnityEngine;

public class MainMenuCursor : MonoBehaviour
{
    //This script simply makes sure the user can move their cursor out of the window from the main menu, is also handy for in-editor work. 
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

}
