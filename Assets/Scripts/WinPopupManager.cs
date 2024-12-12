using UnityEngine;
using UnityEngine.UI;

public class WinPopupManager : MonoBehaviour
{
    [SerializeField] private GameObject winPopup;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        winPopup.SetActive(false); //baþlangýçta false
    }

    public void ShowPopup()
    {
        winPopup.SetActive(true);
        Time.timeScale = 0;
    }

    public void PlayAgain()
    {
        var rewardManager = FindFirstObjectByType<RewardManager>();
        var slotManager = FindFirstObjectByType<SlotManager>();
        var highlightManager = FindFirstObjectByType<HighlightManager>();

        if (rewardManager == null || slotManager == null || highlightManager == null) return;

        rewardManager.ResetRewards(); //rewardqueue temizlenir ve tekrar oluþturulur.
        var rewardQueue = rewardManager.GetRewardQueue(); //karýþtýrýlmýþ reward queuesu alýnýr.
        slotManager.ResetSlots(rewardQueue); //
        highlightManager.ResetHighlights();
        highlightManager.Initialize(slotManager.Slots);
        slotManager.StartCoroutine(slotManager.ActivateSlotsWithAnimation());
        winPopup.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Game has been reset for a new round.");
    }
}
