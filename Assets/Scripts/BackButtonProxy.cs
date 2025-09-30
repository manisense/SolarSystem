// BackButtonProxy.cs - FIXED VERSION

using UnityEngine;

public class BackButtonProxy : MonoBehaviour
{
    // Hook this to the Button's OnClick in the planet scene
    public void OnClickBack()
    {
        // Add null check with debug logging
        if (SceneController.Instance != null)
        {
            Debug.Log("BackButtonProxy: Calling SceneController.GoBack()");
            SceneController.Instance.GoBack();
        }
        else
        {
            Debug.LogError("BackButtonProxy: SceneController.Instance is NULL! Make sure SceneController exists in the Home scene.");
        }
    }
}