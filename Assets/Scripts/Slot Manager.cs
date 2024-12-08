using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using System.Collections;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private AssetReference slotPrefabReference;
    [SerializeField] private AssetReference highlightPrefabReference;
    [SerializeField] private AssetReference rewardPrefabReference;

    private List<Slot> slots = new List<Slot>();
    public IReadOnlyList<Slot> Slots => slots;

    public async Task CreateSlots(List<Transform> panels, Dictionary<string, int> panelSlotCounts, Queue<RewardData> rewardQueue)
    {
        int slotCounter = 0;

        foreach (Transform panel in panels)
        {
            if (panelSlotCounts.TryGetValue(panel.tag, out int slotCount))
            {
                for (int i = 0; i < slotCount; i++)
                {
                    GameObject slotObj = await LoadPrefabAsync(slotPrefabReference, panel);

                    if (slotObj)
                    {
                        slotObj.name = $"Slot{slotCounter++}";

                        Slot slot = slotObj.GetComponent<Slot>();
                        GameObject highlight = await LoadPrefabAsync(highlightPrefabReference, slotObj.transform);
                        GameObject reward = await LoadPrefabAsync(rewardPrefabReference, slotObj.transform);

                        slot.Initialize(reward, highlight);

                        if (rewardQueue.Count > 0)
                        {
                            RewardData rewardData = rewardQueue.Dequeue();
                            slot.SetReward(rewardData.rewardSprite);
                        }

                        slotObj.SetActive(false);
                        slots.Add(slot);
                    }
                }
            }
        }
    }

    private async Task<GameObject> LoadPrefabAsync(AssetReference reference, Transform parent = null)
    {
        if (reference.OperationHandle.IsValid() && reference.OperationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return Instantiate(reference.OperationHandle.Result as GameObject, parent);
        }

        var handle = reference.LoadAssetAsync<GameObject>();
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return Instantiate(handle.Result, parent);
        }
        else
        {
            Debug.LogError($"Asset yüklenemedi: {reference}");
            return null;
        }
    }
    public IEnumerator ActivateSlotsWithAnimation()
    {
        foreach (Slot slot in slots)
        {
            slot.gameObject.SetActive(true);
            slot.transform.localScale = Vector3.zero;
            slot.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
