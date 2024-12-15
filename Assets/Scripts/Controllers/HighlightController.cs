using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightController : MonoBehaviour, IHighlightController
{
    [Header("Highlight Settings")]
    [SerializeField] private float _highlightFadeDuration = 0.3f;
    [SerializeField] private int _maxHighlightTrailCount = 5;

    private IReadOnlyList<Slot> _slots;
    private readonly List<Image> _highlightTrail = new();

    // Initializes the highlight controller with a list of slots.
    public void Initialize(IReadOnlyList<Slot> slotList)
    {
        _slots = slotList;
        SetInvisibleHighlights();
    }

    // Resets all highlights.
    public void ResetHighlights()
    {
        if (_slots == null) return;
        SetInvisibleHighlights();
        _highlightTrail.Clear();
    }

    // Shows the highlight for the current slot.
    public void ShowCurrentHighlight(Slot currentSlot)
    {
        var currentHighlightRenderer = currentSlot.GetHighlightImage();
        if (currentHighlightRenderer)
        {
            _highlightTrail.Insert(0, currentHighlightRenderer);
            currentHighlightRenderer.DOFade(1, _highlightFadeDuration);
        }
    }

    // Updates the highlight trail.
    public void UpdateHighlightTrail()
    {
        UpdateTrailAlphaValues();
        LimitTrailCount();
    }

    // Fades out the highlight trail, excluding the current slot.
    public void FadeOutHighlightTrail(Slot currentSlot, Action onComplete)
    {
        FadeOutTrailExceptCurrent(currentSlot);
        _highlightTrail.Clear();
        onComplete?.Invoke();
    }

    // Blinks and fades out the highlight of the current slot.
    public void BlinkAndFadeOut(Slot currentSlot, Action onComplete)
    {
        var finalHighlightRenderer = currentSlot?.GetHighlightImage();
        if (finalHighlightRenderer)
        {
            BlinkHighlight(finalHighlightRenderer, onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    // Sets the highlight parameters.
    public void SetHighlightParameters(float fadeDuration, int maxTrailCount)
    {
        _highlightFadeDuration = fadeDuration;
        _maxHighlightTrailCount = maxTrailCount;
        Debug.Log($"Highlight parameters updated: FadeDuration = {fadeDuration}, MaxTrailCount = {maxTrailCount}");
    }

    // Resets all slot highlights to be invisible.
    private void SetInvisibleHighlights()
    {
        foreach (var slot in _slots)
        {
            var highlightRenderer = slot.GetHighlightImage();
            if (highlightRenderer)
            {
                highlightRenderer.color = new Color(1, 1, 1, 0);
            }
        }
    }

    // Updates the alpha values for the highlight trail.
    private void UpdateTrailAlphaValues()
    {
        for (int i = 0; i < _highlightTrail.Count; i++)
        {
            var renderer = _highlightTrail[i];
            float targetAlpha = i switch
            {
                0 => 1f,
                1 => 0.8f,
                2 => 0.5f,
                3 => 0.2f,
                _ => 0f
            };

            renderer.DOKill();
            renderer.DOFade(targetAlpha, _highlightFadeDuration);
        }
    }

    // Limits the trail count to the maximum allowed value.
    private void LimitTrailCount()
    {
        if (_highlightTrail.Count > _maxHighlightTrailCount)
        {
            _highlightTrail.RemoveAt(_highlightTrail.Count - 1);
        }
    }

    // Fades out all highlights except for the current slot.
    private void FadeOutTrailExceptCurrent(Slot currentSlot)
    {
        for (int i = 0; i < _highlightTrail.Count; i++)
        {
            var highlight = _highlightTrail[i];
            highlight.DOKill();

            if (highlight == currentSlot.GetHighlightImage())
            {
                highlight.DOFade(1, _highlightFadeDuration);
            }
            else
            {
                float fadeOutDelay = 0.1f * i;
                highlight.DOFade(0, _highlightFadeDuration).SetDelay(fadeOutDelay);
            }
        }
    }

    // Handles the blinking animation for a highlight.
    private void BlinkHighlight(Image highlightRenderer, Action onComplete)
    {
        highlightRenderer.color = new Color(1, 1, 1, 0);
        highlightRenderer.DOKill();
        highlightRenderer.DOFade(1, 0.1f)
            .SetLoops(10, LoopType.Yoyo)
            .OnComplete(() =>
            {
                highlightRenderer.color = new Color(1, 1, 1, 0);
                onComplete?.Invoke();
            });
    }
}
