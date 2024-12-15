using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpinController : MonoBehaviour, ISpinController
{
    [SerializeField] private Button _spinButton;
    [SerializeField] private TextMeshProUGUI _spinButtonText;

    private float _initialWaitTime = 0.1f;
    private float _finalWaitTime = 0.25f;

    private Slot _currentSlot;
    private ISlotManager _slotManager;
    private IHighlightController _highlightController;
    private IResultHandler _resultHandler;

    private bool _isSpinning;
    private bool _areSlotsVisible;

    // Initializes the spin controller with the required managers.
    public void Initialize(ISlotManager slotManager, IHighlightController highlightController, IResultHandler resultHandler)
    {
        _slotManager = slotManager;
        _highlightController = highlightController;
        _resultHandler = resultHandler;

        _spinButton.gameObject.SetActive(false);
        _resultHandler.OnAnimationsComplete = OnAllAnimationsComplete;       
    }

    // Handles the spin button click event.
    public void OnSpinButtonClicked()
    {
        if (!_areSlotsVisible || _isSpinning || _slotManager?.Slots == null || _slotManager.Slots.Count == 0)
        {
            Debug.LogError("Cannot spin at the moment.");
            return;
        }

        StartSpin();
    }

    // Starts the spinning process.
    public void StartSpin()
    {
        if (_isSpinning) return;

        _isSpinning = true;
        _spinButton.interactable = false;
        _spinButtonText.text = "SPINNING";

        int stepsToTarget = _slotManager.SelectFinalSlotForSpin();
        StartCoroutine(SpinRoulette(stepsToTarget));
    }

    // Enables or disables the spin button.
    public void SetSpinButtonState(bool isEnabled)
    {
        _areSlotsVisible = isEnabled;
        _spinButton.gameObject.SetActive(isEnabled);
    }


    // Handles the spinning animation logic.
    private IEnumerator SpinRoulette(int stepsToTarget)
    {
        int totalSlots = _slotManager.Slots.Count;
        int currentIndex = 0;

        for (int i = 0; i <= stepsToTarget; i++)
        {
            _currentSlot = _slotManager.Slots[currentIndex];
            UpdateHighlightsForCurrentSlot(_currentSlot);
            currentIndex = (currentIndex + 1) % totalSlots;

            float waitTime = CalculateWaitTime(i, stepsToTarget);
            yield return new WaitForSeconds(waitTime);
        }

        _highlightController.FadeOutHighlightTrail(_currentSlot, OnFadeOutComplete);
    }

    // Updates the visual highlight for the current slot.
    private void UpdateHighlightsForCurrentSlot(Slot slot)
    {
        _highlightController.ShowCurrentHighlight(slot);
        _highlightController.UpdateHighlightTrail();
    }

    // Calculates the wait time for each step of the spin.
    private float CalculateWaitTime(int currentStep, int totalSteps)
    {
        float t = (float)currentStep / totalSteps;
        return Mathf.Lerp(_initialWaitTime, _finalWaitTime, t);
    }

    // Called when the fade-out of highlight trail completes.
    private void OnFadeOutComplete()
    {
        _highlightController.BlinkAndFadeOut(_currentSlot, OnBlinkComplete);
    }

    // Called when the blinking animation for the slot completes.
    private void OnBlinkComplete()
    {
        _resultHandler.HandleSpinResult(_currentSlot);
    }

    // Resets the spin button and sets the spinning state to false.
    private void OnAllAnimationsComplete()
    {
        _isSpinning = false;
        _spinButton.interactable = true;
        _spinButtonText.text = "SPIN";
    }
}
