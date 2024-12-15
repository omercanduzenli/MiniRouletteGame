using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    private SceneData _sceneToLoad;

    // Sets the scene data that will be loaded.
    public void SetSceneToLoad(SceneData newSceneData)
    {
        _sceneToLoad = newSceneData;
    }

    // Triggered when the load scene button is clicked.
    public void OnLoadSceneButtonClicked()
    {
        DOTween.KillAll();
        LoadSceneAsync();
    }

    // Asynchronously loads the scene specified in _sceneToLoad.
    private async void LoadSceneAsync()
    {
        if (_sceneToLoad == null || _sceneToLoad.sceneReference == null)
        {
            Debug.LogError("SceneData or AssetReference is not assigned.");
            return;
        }

        var handle = Addressables.LoadSceneAsync(_sceneToLoad.sceneReference, UnityEngine.SceneManagement.LoadSceneMode.Single);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Scene Loaded Successfully: {handle.Result.Scene.name}");
        }
        else
        {
            Debug.LogError("Failed to load scene.");
        }
    }
}
