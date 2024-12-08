using UnityEngine;
using System.Collections.Generic;

public class WeightTest : MonoBehaviour
{
    [SerializeField] private RewardManager rewardManager;

    void Start()
    {
        rewardManager.InitializeRewardQueue();

        Dictionary<string, int> selectionCounts = new Dictionary<string, int>();
        for (int i = 0; i < 1000; i++) // 1000 kez se�im yap
        {
            RewardData selectedReward = rewardManager.GetRewardQueue().Dequeue();
            if (!selectionCounts.ContainsKey(selectedReward.rewardName))
            {
                selectionCounts[selectedReward.rewardName] = 0;
            }
            selectionCounts[selectedReward.rewardName]++;
            rewardManager.InitializeRewardQueue(); // Kuyru�u tekrar doldur
        }

        foreach (var reward in selectionCounts)
        {
            Debug.Log($"Reward: {reward.Key}, Selected: {reward.Value} times");
        }
    }
}
