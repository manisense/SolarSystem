// SceneController.cs - COMPLETE FIXED VERSION
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    private static SceneController _instance;
    public static SceneController Instance 
    { 
        get 
        {
            if (_instance == null)
            {
                Debug.LogError("SceneController Instance is null! Creating emergency instance...");
                GameObject go = new GameObject("EmergencySceneController");
                _instance = go.AddComponent<SceneController>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
        private set => _instance = value;
    }

    [Header("Home")]
    [SerializeField] private GameObject homeScreenUI;

    private string currentPlanetScene;
    private GameObject planetUI;

    void Awake()
    {
        Debug.Log($"SceneController Awake called. Instance null? {_instance == null}");
        
        if (_instance != null && _instance != this) 
        {
            Debug.Log("Duplicate SceneController found, destroying this one");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log($"SceneController: Instance created and set to DontDestroyOnLoad. Instance = {_instance}");
        
        // Subscribe to scene loaded event to ensure we maintain the instance
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        Debug.Log($"SceneController OnDestroy called. Is this the instance? {_instance == this}");
        
        if (_instance == this)
        {
            _instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}, Instance still valid? {_instance != null}");
        
        // Re-find home UI if we're in the home scene
        if (scene.name == "HomeScreen" && homeScreenUI == null)
        {
            homeScreenUI = GameObject.FindWithTag("HomeUI");
            if (homeScreenUI)
            {
                Debug.Log("Re-acquired HomeUI reference after scene load");
            }
        }
    }

    public void LoadPlanetScene(string sceneName)
    {
        if (Instance == null)
        {
            Debug.LogError("SceneController Instance is null in LoadPlanetScene!");
            return;
        }
        
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
            
            // Check if the scene is actually loaded before trying to unload
            Scene oldScene = SceneManager.GetSceneByName(currentPlanetScene);
            if (oldScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(currentPlanetScene);
            }
        }

        Debug.Log($"SceneController: Loading {sceneName} additively");
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        currentPlanetScene = sceneName;

        // Wait a frame for objects to initialize
        yield return null;

        // Try multiple methods to find the planet UI
        planetUI = GameObject.FindWithTag("PlanetUI");
        
        if (!planetUI)
        {
            // Try to find the BackButtonProxy and get its parent
            BackButtonProxy proxy = FindObjectOfType<BackButtonProxy>();
            if (proxy != null)
            {
                planetUI = proxy.transform.root.gameObject;
                Debug.Log("Found planet UI via BackButtonProxy");
            }
        }
        
        if (!planetUI)
        {
            // Fallback to finding by name
            planetUI = GameObject.Find("BackButton");
            if (planetUI && planetUI.transform.parent != null)
            {
                planetUI = planetUI.transform.parent.gameObject;
            }
            Debug.Log("Using BackButton parent as planet UI");
        }
        
        if (planetUI) 
        {
            planetUI.SetActive(true);
            Debug.Log($"SceneController: Planet UI found and activated: {planetUI.name}");
        }
        else
        {
            Debug.LogError("SceneController: Planet UI not found! Make sure to tag your UI root as 'PlanetUI'");
        }
    }

    public void GoBack()
    {
        if (Instance == null)
        {
            Debug.LogError("SceneController Instance is null in GoBack!");
            return;
        }
        
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
            
            // Check if the scene is actually loaded
            Scene planetScene = SceneManager.GetSceneByName(currentPlanetScene);
            if (planetScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(currentPlanetScene);
            }
            
            currentPlanetScene = null;
        }

        // Re-find Home UI if needed
        if (homeScreenUI == null)
        {
            homeScreenUI = GameObject.FindWithTag("HomeUI");
            if (homeScreenUI)
            {
                Debug.Log("SceneController: Home UI found by tag");
            }
            else
            {
                // Try to find it by name as a last resort
                GameObject[] rootObjects = SceneManager.GetSceneByName("HomeScreen").GetRootGameObjects();
                foreach (GameObject obj in rootObjects)
                {
                    if (obj.name.Contains("UI") || obj.name.Contains("Canvas"))
                    {
                        homeScreenUI = obj;
                        Debug.Log($"Found potential Home UI: {obj.name}");
                        break;
                    }
                }
            }
        }
        
        if (homeScreenUI) 
        {
            homeScreenUI.SetActive(true);
            Debug.Log("SceneController: Home UI shown");
        }
        else
        {
            Debug.LogError("SceneController: Home UI not found! Make sure to tag it as 'HomeUI'");
        }
    }
}