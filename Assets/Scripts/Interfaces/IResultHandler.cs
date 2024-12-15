using System;

public interface IResultHandler
{
    Action OnAnimationsComplete { get; set; }
    void HandleSpinResult(Slot selectedSlot);
}
