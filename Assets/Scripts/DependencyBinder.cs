using UnityEngine;

public class DependencyBinder : MonoBehaviour
{
    [Header("Controllers & Managers")]
    [SerializeField] private SpinController _spinController;
    [SerializeField] private HighlightController _highlightController;
    [SerializeField] private ResultHandler _resultHandler;
    [SerializeField] private SlotManager _slotManager;
    [SerializeField] private RewardManager _rewardManager;
    [SerializeField] private WinPopupManager _winPopupManager;
    [SerializeField] private RewardFlyAnimation _rewardFlyAnimation;
    [SerializeField] private WalletManager _walletManager;
    [SerializeField] private RouletteSpawner _rouletteSpawner;

    private void Awake()
    {
        BindDependencies();
    }

    // Binds dependencies between controllers and managers
    private void BindDependencies()
    {
        _resultHandler.Initialize(
            _walletManager,
            _rewardFlyAnimation,
            _winPopupManager,
            _slotManager
        );

        _spinController.Initialize(
            _slotManager,
            _highlightController,
            _resultHandler
        );

        _rouletteSpawner.Initialize(
            _rewardManager,
            _slotManager,
            _highlightController
        );

        _winPopupManager.Initialize(
            _rewardManager,
            _slotManager,
            _highlightController,
            _spinController
        );

        _slotManager.Initialize(_spinController);
    }
}
