using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Bootstrapper : MonoBehaviour
{
    void Start()
    {
        Addressables.LoadSceneAsync("Main Menu", UnityEngine.SceneManagement.LoadSceneMode.Single)
            .Completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Main Menu sahnesi ba�ar�yla y�klendi.");
        }
        else
        {
            Debug.LogError("Main Menu sahnesi y�klenirken bir hata olu�tu.");
        }
    }
}
