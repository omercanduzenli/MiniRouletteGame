using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightManager : MonoBehaviour
{
    private List<Slot> slots = new List<Slot>();
    private Slot currentHighlight;
    private bool isSpinning = false;

    public void Initialize(IReadOnlyList<Slot> slotList)
    {
        slots = new List<Slot>(slotList);
    }

    public void StartSpin(float spinTime)
    {
        if (isSpinning || slots.Count == 0) return;

        StartCoroutine(SpinRoulette(spinTime));
    }

    private IEnumerator SpinRoulette(float spinTime)
    {
        isSpinning = true;

        int totalSlots = slots.Count;
        int currentIndex = 0;

        float initialWaitTime = 0.02f;
        float finalWaitTime = 0.2f;

        int finalIndex = Random.Range(0, totalSlots);

        int fullRotations = Random.Range(2, 4);
        int stepsToTarget = (fullRotations * totalSlots) + finalIndex - currentIndex;

        for (int i = 0; i <= stepsToTarget; i++)
        {
            if (currentHighlight)
            {
                currentHighlight.SetHighlight(false);
            }

            currentHighlight = slots[currentIndex];
            currentHighlight.SetHighlight(true);

            currentIndex = (currentIndex + 1) % totalSlots;

            float t = Mathf.Pow((float)i / stepsToTarget, 2);
            float waitTime = Mathf.Lerp(initialWaitTime, finalWaitTime, t);

            yield return new WaitForSeconds(waitTime);
        }

        isSpinning = false;

        Debug.Log("Seçilen Ödül: " + currentHighlight.GetReward()?.GetComponentInChildren<Image>()?.sprite?.name);
    }
}
