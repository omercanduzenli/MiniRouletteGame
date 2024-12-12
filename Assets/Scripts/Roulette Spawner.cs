using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RouletteSpawner : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private List<Transform> panels;
    [SerializeField] private PanelConfig panelConfig;

    private async void Start()
    {
        RewardManager rewardManager = FindFirstObjectByType<RewardManager>();
        SlotManager slotManager = FindFirstObjectByType<SlotManager>();
        HighlightManager highlightManager = FindFirstObjectByType<HighlightManager>();

        if (rewardManager == null || slotManager == null || highlightManager == null)
        {
            Debug.LogError("Some managers are missing.");
            return;
        }

        rewardManager.InitializeRewardQueue();
        var rewardQueue = rewardManager.GetRewardQueue();

        Dictionary<string, int> panelSlotCounts = panelConfig.GetPanelSlotCounts();

        await slotManager.CreateSlots(panels, panelSlotCounts, rewardQueue);
        highlightManager.Initialize(slotManager.Slots);
        StartCoroutine(slotManager.ActivateSlotsWithAnimation());
    }
}
