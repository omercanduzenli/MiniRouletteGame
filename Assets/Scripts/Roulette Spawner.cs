using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RouletteSpawner : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private List<Transform> panels;

    private Dictionary<string, int> panelSlotCounts = new Dictionary<string, int>
    {
        { "TopPanel", 4 },
        { "BottomPanel", 4 },
        { "RightPanel", 3 },
        { "LeftPanel", 3 }
    };

    private async void Start()
    {
        RewardManager rewardManager = FindFirstObjectByType<RewardManager>();
        SlotManager slotManager = FindFirstObjectByType<SlotManager>();
        HighlightManager highlightManager = FindFirstObjectByType<HighlightManager>();

        rewardManager.InitializeRewardQueue();

        Queue<RewardData> rewardQueue = rewardManager.GetRewardQueue();
        await slotManager.CreateSlots(panels, panelSlotCounts, rewardQueue);

        highlightManager.Initialize(slotManager.Slots);
        StartCoroutine(slotManager.ActivateSlotsWithAnimation());
    }
}
