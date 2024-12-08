using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneData sceneData; 

    public void LoadScene()
    {
        if (sceneData != null)
        {
            AsyncOperationHandle<SceneInstance> loadHandle = Addressables.LoadSceneAsync(sceneData.sceneAddress, LoadSceneMode.Single, true);
            loadHandle.Completed += OnSceneLoaded;
        }
    }
    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Scene Loaded: " + obj.Result.Scene.name);
        }
        else
        {
            Debug.LogError("Failed to load scene.");
        }
    }
}
