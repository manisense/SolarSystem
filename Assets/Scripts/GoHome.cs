// GoHome.cs

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using System.Collections; // Required for using coroutines

public class GoHome : MonoBehaviour
{
    [SerializeField] string homeScene = "HomeScreen";

    // This is the function your button will continue to call from the Inspector
    public void OnClickGoHome()
    {
        // Instead of doing the work here, we start a coroutine
        StartCoroutine(GoHomeRoutine());
    }

    private IEnumerator GoHomeRoutine()
    {
        // Find and disable the AR Session. Disabling this is usually enough.
        var arSess = FindObjectOfType<ARSession>();
        if (arSess != null)
        {
            arSess.enabled = false;
        }

        // --- THIS IS THE IMPORTANT PART ---
        // Wait for the end of the current frame to ensure AR shuts down cleanly
        yield return null;

        // Now, load the home scene
        SceneManager.LoadScene(homeScene, LoadSceneMode.Single);
    }
}