using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneLoader : MonoBehaviour
{
    private SceneData sceneToLoad;

    public void SetSceneToLoad(SceneData newSceneData)
    {
        sceneToLoad = newSceneData;
    }

    public void LoadScene()
    {
        if (sceneToLoad != null && sceneToLoad.sceneReference != null)
        {
            Addressables.LoadSceneAsync(sceneToLoad.sceneReference, UnityEngine.SceneManagement.LoadSceneMode.Single)
                .Completed += OnSceneLoaded;
        }
        else
        {
            Debug.LogError("SceneData or AssetReference is not assigned.");
        }
    }

    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene Loaded Successfully: " + obj.Result.Scene.name);
        }
        else
        {
            Debug.LogError("Failed to load scene.");
        }
    }
}
