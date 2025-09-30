// LoadPlanetButton.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlanetButton : MonoBehaviour
{
    [SerializeField] private string sceneName; // e.g., "Neptune"
    public void OnClickLoad() => SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
}
