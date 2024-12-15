using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour, IRewardManager
{
    [SerializeField] private List<RewardData> _rewards;

    private Queue<(RewardData reward, int amount)> _rewardQueue;

    // Returns the current reward queue.
    public Queue<(RewardData reward, int amount)> GetRewardQueue() => _rewardQueue;

    // Initializes the reward queue by generating weighted rewards.
    public void InitializeRewardQueue()
    {
        _rewardQueue = new Queue<(RewardData reward, int amount)>(GenerateWeightedRewards());
    }

    // Clears and reinitializes the reward queue.
    public void ResetRewards()
    {
        _rewardQueue.Clear();
        InitializeRewardQueue();
    }

    // Generates a list of rewards weighted by their weight property.
    private List<(RewardData reward, int amount)> GenerateWeightedRewards()
    {
        var weightedRewards = new List<(RewardData reward, int amount)>();

        foreach (var reward in _rewards)
        {
            if (reward == null) continue;

            int weightMultiplier = Mathf.Max(1, Mathf.RoundToInt(reward.weight));
            for (int i = 0; i < weightMultiplier; i++)
            {
                int randomAmount = Random.Range(reward.minAmount, reward.maxAmount + 1);
                weightedRewards.Add((reward, randomAmount));
            }
        }

        ShuffleList(weightedRewards);
        return weightedRewards;
    }

    // Shuffles the list of rewards randomly.
    private void ShuffleList(List<(RewardData reward, int amount)> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}
