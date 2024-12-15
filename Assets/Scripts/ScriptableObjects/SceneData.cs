using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObjects/SceneData", order = 50)]
public class SceneData : ScriptableObject
{
    public AssetReference sceneReference;
}
