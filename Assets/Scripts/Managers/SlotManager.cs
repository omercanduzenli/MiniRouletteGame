using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SlotManager : MonoBehaviour, ISlotManager
{
    [Header("Prefab References")]
    [SerializeField] private AssetReference _slotPrefabReference;

    private ISpinController _spinController;
    private readonly List<Slot> _slots = new();

    public IReadOnlyList<Slot> Slots => _slots;

    // Initializes the slot manager with the required spin controller.
    public void Initialize(ISpinController spinController)
    {
        _spinController = spinController;
    }

    // Asynchronously creates slots and assigns rewards.
    public async Task CreateSlots(List<Transform> panels, Dictionary<string, int> panelSlotCounts, Queue<(RewardData reward, int amount)> rewardQueue)
    {
        _slots.Clear();

        int slotCounter = 0;

        foreach (var panel in panels)
        {
            if (!panelSlotCounts.TryGetValue(panel.tag, out int slotCount)) continue;

            for (int i = 0; i < slotCount; i++)
            {
                var slotPrefab = await InstantiateSlot(panel, slotCounter++);
                if (!slotPrefab) continue;

                slotPrefab.SetActive(false);

                var slot = slotPrefab.GetComponent<Slot>();
                slot.Initialize();
                AssignRewardToSlot(slot, rewardQueue);
                _slots.Add(slot);
            }
        }
    }

    // Starts the animation to activate slots.
    public void StartSlotActivationAnimation()
    {
        StartCoroutine(ActivateSlotsWithAnimation());
    }

    // Selects the final slot and calculates steps to target.
    public int SelectFinalSlotForSpin()
    {
        var (availableIndices, totalWeight) = GetAvailableSlotIndicesAndTotalWeight();

        if (availableIndices.Count == 0)
        {
            Debug.LogError("No available slots to choose from!");
            return -1;
        }

        int chosenSlotIndex = ChooseSlotByWeight(availableIndices, totalWeight);
        return CalculateStepsToTarget(chosenSlotIndex);
    }

    // Resets all slots and reassigns rewards.
    public void ResetSlots(Queue<(RewardData reward, int amount)> rewardQueue)
    {
        foreach (var slot in _slots) slot.CompletelyResetToInitialState();

        foreach (var slot in _slots)
        {
            AssignRewardToSlot(slot, rewardQueue);
            slot.gameObject.SetActive(false);
        }

        Debug.Log("Slots have been completely reset.");
    }

    // Activates slots with animation in a coroutine.
    private IEnumerator ActivateSlotsWithAnimation()
    {
        foreach (var slot in _slots)
        {
            slot.gameObject.SetActive(true);
            slot.transform.localScale = Vector3.zero;
            slot.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
        }
        _spinController.SetSpinButtonState(true);
    }

    // Instantiates a slot prefab asynchronously.
    private async Task<GameObject> InstantiateSlot(Transform parent, int slotIndex)
    {
        var slotPrefab = await LoadPrefabAsync(_slotPrefabReference, parent);
        if (slotPrefab)
        {
            slotPrefab.name = $"Slot{slotIndex}";
        }
        return slotPrefab;
    }

    // Assigns a reward to a specific slot.
    private void AssignRewardToSlot(Slot slot, Queue<(RewardData reward, int amount)> rewardQueue)
    {
        if (rewardQueue.Count > 0)
        {
            var rewardDataWithAmount = rewardQueue.Dequeue();
            slot.SetReward(
                rewardDataWithAmount.reward.rewardSprite,
                rewardDataWithAmount.reward.rewardID,
                rewardDataWithAmount.amount,
                rewardDataWithAmount.reward
            );
        }
    }

    // Retrieves available slot indices and their total weight.
    private (List<int> availableIndices, float totalWeight) GetAvailableSlotIndicesAndTotalWeight()
    {
        List<int> availableIndices = new();
        float totalWeight = 0f;

        for (int i = 0; i < _slots.Count; i++)
        {
            if (!_slots[i].IsEliminated())
            {
                availableIndices.Add(i);

                var rewardData = _slots[i].GetRewardData();
                float weight = rewardData != null ? rewardData.weight : 1f;
                totalWeight += weight;
            }
        }

        return (availableIndices, totalWeight);
    }

    // Chooses a slot index based on weight.
    private int ChooseSlotByWeight(List<int> availableIndices, float totalWeight)
    {
        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (int slotIndex in availableIndices)
        {
            var rewardData = _slots[slotIndex].GetRewardData();
            float weight = rewardData != null ? rewardData.weight : 1f;
            cumulative += weight;

            if (randomValue < cumulative)
            {
                return slotIndex;
            }
        }

        return availableIndices[0];
    }

    // Calculates the number of steps to reach the chosen slot.
    private int CalculateStepsToTarget(int chosenSlotIndex)
    {
        int totalSlots = _slots.Count;
        return Random.Range(1, 3) * totalSlots + chosenSlotIndex;
    }

    // Loads a prefab asynchronously using Addressables.
    private async Task<GameObject> LoadPrefabAsync(AssetReference reference, Transform parent)
    {
        if (reference.OperationHandle.IsValid() &&
            reference.OperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return Instantiate(reference.OperationHandle.Result as GameObject, parent);
        }

        var handle = reference.LoadAssetAsync<GameObject>();
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return Instantiate(handle.Result, parent);
        }

        Debug.LogError($"Failed to load AssetReference: {reference.RuntimeKey}");
        return null;
    }
}
