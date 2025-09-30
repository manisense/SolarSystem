// SceneController.cs - FIXED VERSION

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Home")]
    [SerializeField] private GameObject homeScreenUI; // assign in Home scene Inspector

    private string currentPlanetScene;
    private GameObject planetUI; // found inside a planet scene (e.g., Back button parent)

    void Awake()
    {
        // Make this a singleton and keep it across scene loads
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("SceneController: Instance created and set to DontDestroyOnLoad");
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Called by planet buttons on the Home screen
    public void LoadPlanetScene(string sceneName)
    {
        Debug.Log($"SceneController: Loading planet scene: {sceneName}");
        StartCoroutine(LoadPlanetRoutine(sceneName));
    }

    private IEnumerator LoadPlanetRoutine(string sceneName)
    {
        if (homeScreenUI) 
        {
            homeScreenUI.SetActive(false);
            Debug.Log("SceneController: Home UI hidden");
        }

        if (!string.IsNullOrEmpty(currentPlanetScene))
        {
            Debug.Log($"SceneController: Unloading previous planet: {currentPlanetScene}");
            yield return SceneManager.UnloadSceneAsync(currentPlanetScene);
        }

        // Load the planet additively on top of Home
        Debug.Log($"SceneController: Loading {sceneName} additively");
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        currentPlanetScene = sceneName;

        // Try to find the planet UI root (tag a parent object as "PlanetUI")
        planetUI = GameObject.FindWithTag("PlanetUI");
        if (!planetUI) 
        {
            planetUI = GameObject.Find("BackButton"); // fallback
            Debug.Log("SceneController: Using BackButton as fallback planet UI");
        }
        
        if (planetUI) 
        {
            planetUI.SetActive(true);
            Debug.Log("SceneController: Planet UI found and activated");
        }
        else
        {
            Debug.LogWarning("SceneController: Planet UI not found! Make sure BackButton exists or tag an object as 'PlanetUI'");
        }
    }

    // Called by the Back button inside each planet scene
    public void GoBack()
    {
        Debug.Log("SceneController: GoBack() called");
        StartCoroutine(BackRoutine());
    }

    private IEnumerator BackRoutine()
    {
        if (planetUI) 
        {
            planetUI.SetActive(false);
            Debug.Log("SceneController: Planet UI hidden");
        }

        if (!string.IsNullOrEmpty(currentPlanetScene))
        {
            Debug.Log($"SceneController: Unloading planet scene: {currentPlanetScene}");
            yield return SceneManager.UnloadSceneAsync(currentPlanetScene);
            currentPlanetScene = null;
        }

        // Re-show Home UI
        if (homeScreenUI == null)
        {
            // Safety: if not assigned, try to find it by tag
            var candidate = GameObject.FindWithTag("HomeUI");
            if (candidate) 
            {
                homeScreenUI = candidate;
                Debug.Log("SceneController: Home UI found by tag");
            }
        }
        
        if (homeScreenUI) 
        {
            homeScreenUI.SetActive(true);
            Debug.Log("SceneController: Home UI shown");
        }
        else
        {
            Debug.LogError("SceneController: Home UI not found! Make sure to assign it in Inspector or tag it as 'HomeUI'");
        }
    }
}