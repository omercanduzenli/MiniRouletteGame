using System.Collections.Generic;
using UnityEngine;
//BU CLASS DIÞARIYA KARIÞTIRILMIÞ REWARDQUEUE DÖNER VE AMOUNTUNU DÖNER.
public class RewardManager : MonoBehaviour
{
    [SerializeField] private List<RewardData> rewards;

    private Queue<(RewardData reward, int amount)> rewardQueue;

    public Queue<(RewardData reward, int amount)> GetRewardQueue() => rewardQueue; //karýþtýrýlmýþ reward queuesunu dýþarý gönderir.

    public void InitializeRewardQueue()
    {
        rewardQueue = new Queue<(RewardData reward, int amount)>(GenerateWeightedRewards()); //aðýrlýklý oluþan listeyi queue olarak atar.
    }

    private List<(RewardData reward, int amount)> GenerateWeightedRewards()
    {
        var weightedRewards = new List<(RewardData reward, int amount)>();

        foreach (var reward in rewards)  //bu döngüde aðýrlýklara göre 3 ise örneðin 3 tane üretir ve amount deðerini de 3 tane rastgele yanýna ekler 14 reward için de aynýný yapýp liisteye ekler
        {
            int weightMultiplier = Mathf.Max(1, Mathf.RoundToInt(reward.weight)); //sayýyý yuvarlar yakýn ondalýktan Max ise iki sayýdan büyüðü seçer yani weightMultiplier en az 1 olur.
            for (int i = 0; i < weightMultiplier; i++)
            {
                int randomAmount = Random.Range(reward.minAmount, reward.maxAmount + 1);
                weightedRewards.Add((reward, randomAmount));
            }
        }

        for (int i = weightedRewards.Count - 1; i > 0; i--) //burada tüm listeyi karýþtýrýr rastgele.
        {
            int randomIndex = Random.Range(0, i + 1);
            (weightedRewards[i], weightedRewards[randomIndex]) = (weightedRewards[randomIndex], weightedRewards[i]);
        }

        return weightedRewards;
    }

    public void ResetRewards()
    {
        rewardQueue.Clear();
        InitializeRewardQueue();
    }
}



