using System.Collections.Generic;
using UnityEngine;

public class RouletteSpawner : MonoBehaviour
{
    private IRewardManager _rewardManager;
    private ISlotManager _slotManager;
    private IHighlightController _highlightController;

    [Header("Panels")]
    [SerializeField] private List<Transform> _panels;
    [SerializeField] private PanelConfig _panelConfig;

    // Initializes the roulette spawner with required managers.
    public void Initialize(IRewardManager rewardManager, ISlotManager slotManager, IHighlightController highlightController)
    {
        _rewardManager = rewardManager;
        _slotManager = slotManager;
        _highlightController = highlightController;
    }

    // Sets up slots and initializes highlights on start.
    private async void Start()
    {
        if (CheckMissingReferences()) return;

        _rewardManager.InitializeRewardQueue();
        var rewardQueue = _rewardManager.GetRewardQueue();

        var panelSlotCounts = _panelConfig.GetPanelSlotCounts();
        await _slotManager.CreateSlots(_panels, panelSlotCounts, rewardQueue);

        _highlightController.Initialize(_slotManager.Slots);
        _slotManager.StartSlotActivationAnimation();
    }

    // Checks for missing manager references.
    private bool CheckMissingReferences()
    {
        if (_rewardManager == null || _slotManager == null || _highlightController == null)
        {
            Debug.LogError("Some managers or controllers are missing in RouletteSpawner!");
            return true;
        }
        return false;
    }
}
