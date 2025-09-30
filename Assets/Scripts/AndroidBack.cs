// AndroidBack.cs - FIXED VERSION

using UnityEngine;

public class AndroidNativeBack : MonoBehaviour
{
    // Link your BackButton GameObject here in the Inspector
    public GoHome goHomeHandler;

    void Update()
    {
        // Detects the Android back button press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (goHomeHandler != null)
            {
                // Calls the same function as your UI button
                goHomeHandler.OnClickGoHome();
            }
        }
    }
}