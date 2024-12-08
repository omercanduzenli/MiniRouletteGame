using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private List<RewardData> rewards;

    private Queue<RewardData> rewardQueue;

    public Queue<RewardData> GetRewardQueue()
    {
        return rewardQueue;
    }

    public void InitializeRewardQueue()
    {
        rewardQueue = new Queue<RewardData>(GenerateWeightedRewards());
    }

    private List<RewardData> GenerateWeightedRewards()
    {
        List<RewardData> weightedRewards = new List<RewardData>();

        foreach (RewardData reward in rewards)
        {
            int weightMultiplier = Mathf.Max(1, Mathf.RoundToInt(reward.weight));
            for (int i = 0; i < weightMultiplier; i++)
            {
                weightedRewards.Add(reward);
            }
        }

        for (int i = weightedRewards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (weightedRewards[i], weightedRewards[randomIndex]) = (weightedRewards[randomIndex], weightedRewards[i]);
        }

        return weightedRewards;
    }
}
