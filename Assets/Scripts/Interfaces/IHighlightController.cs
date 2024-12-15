using System;
using System.Collections.Generic;

    public interface IHighlightController
    {
        void Initialize(IReadOnlyList<Slot> slots);       
        void ResetHighlights();                           
        void ShowCurrentHighlight(Slot currentSlot);      
        void UpdateHighlightTrail();                      
        void FadeOutHighlightTrail(Slot currentSlot, Action onComplete); 
        void BlinkAndFadeOut(Slot currentSlot, Action onComplete);      
        void SetHighlightParameters(float fadeDuration, int maxTrailCount); 
    }
