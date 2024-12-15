using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WinPopupManager : MonoBehaviour
{
    [SerializeField] private GameObject _winPopupPanel;
    [SerializeField] private Button _playAgainButton;

    private IRewardManager _rewardManager;
    private ISlotManager _slotManager;
    private IHighlightController _highlightController;
    private ISpinController _spinController;

    // Initializes the WinPopupManager with the required managers and controllers.
    public void Initialize(IRewardManager rewardManager, ISlotManager slotManager, IHighlightController highlightController, ISpinController spinController)
    {
        _rewardManager = rewardManager;
        _slotManager = slotManager;
        _highlightController = highlightController;
        _spinController = spinController;
    }

    // Unity lifecycle method: Disables the popup panel on start.
    private void Start()
    {
        _winPopupPanel.SetActive(false);
    }

    // Displays the win popup with an animation.
    public void ShowPopup()
    {
        if (!_winPopupPanel)  return;

        _winPopupPanel.SetActive(true);
        AnimatePopup(_winPopupPanel.transform);
    }

    // Animates the popup to appear with scaling.
    private void AnimatePopup(Transform popupTransform)
    {
        popupTransform.localScale = Vector3.zero;
        popupTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() => Time.timeScale = 0);
    }

    // Resets the game state and restarts the gameplay.
    public void PlayAgain()
    {
        if (CheckMissingReferences()) return;

        DOTween.KillAll();
        _rewardManager.ResetRewards();
        var rewardQueue = _rewardManager.GetRewardQueue();
        _slotManager.ResetSlots(rewardQueue);
        _highlightController.ResetHighlights();
        _highlightController.Initialize(_slotManager.Slots);
        _slotManager.StartSlotActivationAnimation();

        _spinController.SetSpinButtonState(false);
        _winPopupPanel.SetActive(false);
        Time.timeScale = 1;
    }

    // Validates whether all necessary references are set.
    private bool CheckMissingReferences()
    {
        if (_rewardManager == null || _slotManager == null || _highlightController == null)
        {
            Debug.LogError("Missing references in WinPopupManager!");
            return true;
        }
        return false;
    }
}
