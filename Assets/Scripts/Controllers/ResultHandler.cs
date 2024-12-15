using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResultHandler : MonoBehaviour, IResultHandler
{
    [SerializeField] private Sprite _paleGraySprite;

    private IWalletManager _walletManager;
    private ISlotManager _slotManager;
    private RewardFlyAnimation _rewardFlyAnimation;
    private WinPopupManager _winPopupManager;

    public Action OnAnimationsComplete { get; set; }

    // Initializes the result handler with required managers.
    public void Initialize(IWalletManager walletManager, RewardFlyAnimation rewardFlyAnimation, WinPopupManager winPopupManager, ISlotManager slotManager)
    {
        _walletManager = walletManager;
        _rewardFlyAnimation = rewardFlyAnimation;
        _winPopupManager = winPopupManager;
        _slotManager = slotManager;
    }

    // Handles the spin result, processes rewards, and starts animations.
    public void HandleSpinResult(Slot selectedSlot)
    {
        AddRewardToWallet(selectedSlot);
        StartCoroutine(HandleSlotAnimationsAfterBlink(selectedSlot));
    }

    // Adds the reward from the selected slot to the wallet.
    private void AddRewardToWallet(Slot slot)
    {
        _walletManager.AddReward(slot.GetRewardID(), slot.GetRewardAmount());
    }

    // Manages animations for the slot after blinking.
    private IEnumerator HandleSlotAnimationsAfterBlink(Slot currentSlot)
    {
        currentSlot.ActivateBlueSlot();
        var rewardObj = currentSlot.GetRewardObject();

        if (rewardObj)
        {
            yield return AnimateMaskAndRewardFly(currentSlot, rewardObj);
        }
        else
        {
            Debug.LogWarning("Reward object not found in current slot!");
        }
    }

    // Handles mask expansion and reward fly animations.
    private IEnumerator AnimateMaskAndRewardFly(Slot currentSlot, GameObject rewardObj)
    {
        var maskContainer = currentSlot.GetMaskContainerObject();
        if (maskContainer)
        {
            yield return AnimateMaskExpansion(maskContainer);
            AnimateRewardFly(currentSlot, rewardObj);
        }
        else
        {
            Debug.LogWarning("MaskContainer not found under RewardPrefab!");
        }
    }

    // Expands the mask as part of the animation.
    private IEnumerator AnimateMaskExpansion(RectTransform maskContainer)
    {
        const float finalWidth = 100f;
        maskContainer.sizeDelta = new Vector2(0, maskContainer.sizeDelta.y);
        bool animationCompleted = false;

        maskContainer.DOSizeDelta(new Vector2(finalWidth, maskContainer.sizeDelta.y), 1.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => animationCompleted = true);

        while (!animationCompleted)
        {
            yield return null;
        }
    }

    // Animates the reward flying to its target.
    private void AnimateRewardFly(Slot currentSlot, GameObject rewardObj)
    {
        if (!_rewardFlyAnimation) return;

        Sprite rewardSprite = currentSlot.GetRewardSprite();
        _rewardFlyAnimation.PlayRewardFlyAnimation(rewardSprite, () =>
        {
            HideRewardImage(rewardObj);
            UpdateBlueHighlightSprite(currentSlot);
            currentSlot.Eliminate();
            OnAnimationsComplete?.Invoke();
            CheckAndShowWinPopup();
        });
    }

    // Hides the reward image after the animation.
    private void HideRewardImage(GameObject rewardObj)
    {
        var rewardImageObj = rewardObj.transform.GetChild(0);
        if (rewardImageObj)
        {
            rewardImageObj.gameObject.SetActive(false);
        }
    }

    // Updates the blue highlight to a pale gray sprite.
    private void UpdateBlueHighlightSprite(Slot currentSlot)
    {
        var blueHighlightObj = currentSlot.GetBlueHighlightObject();
        if (blueHighlightObj && _paleGraySprite)
        {
            var blueHighlightImg = blueHighlightObj.transform.GetChild(0).GetComponent<Image>();
            if (blueHighlightImg)
            {
                blueHighlightImg.sprite = _paleGraySprite;
            }
        }
    }

    // Checks if all rewards have been collected and shows the win popup if true.
    private void CheckAndShowWinPopup()
    {
        if (_slotManager == null) return;

        if (AllRewardsCollected())
        {
            ShowWinPopup();
        }
    }

    // Determines if all rewards have been collected.
    private bool AllRewardsCollected()
    {
        foreach (var slot in _slotManager.Slots)
        {
            if (!slot.IsEliminated()) return false;
        }
        return true;
    }

    // Displays the win popup when all rewards are collected.
    private void ShowWinPopup()
    {
        _winPopupManager?.ShowPopup();
    }
}
