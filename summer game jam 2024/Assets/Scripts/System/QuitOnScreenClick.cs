using UnityEngine;

public class QuitOnScreenClick : MonoBehaviour
{
    void Update()
    {
        // Check if the left mouse button (or screen touch) is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // If running in the editor, stop playing
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            // If running in a build, quit the application
            Application.Quit();
            #endif
        }
    }
}
