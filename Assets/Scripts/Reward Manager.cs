using System.Collections.Generic;
using UnityEngine;
//BU CLASS DI�ARIYA KARI�TIRILMI� REWARDQUEUE D�NER VE AMOUNTUNU D�NER.
public class RewardManager : MonoBehaviour
{
    [SerializeField] private List<RewardData> rewards;

    private Queue<(RewardData reward, int amount)> rewardQueue;

    public Queue<(RewardData reward, int amount)> GetRewardQueue() => rewardQueue; //kar��t�r�lm�� reward queuesunu d��ar� g�nderir.

    public void InitializeRewardQueue()
    {
        rewardQueue = new Queue<(RewardData reward, int amount)>(GenerateWeightedRewards()); //a��rl�kl� olu�an listeyi queue olarak atar.
    }

    private List<(RewardData reward, int amount)> GenerateWeightedRewards()
    {
        var weightedRewards = new List<(RewardData reward, int amount)>();

        foreach (var reward in rewards)  //bu d�ng�de a��rl�klara g�re 3 ise �rne�in 3 tane �retir ve amount de�erini de 3 tane rastgele yan�na ekler 14 reward i�in de ayn�n� yap�p liisteye ekler
        {
            int weightMultiplier = Mathf.Max(1, Mathf.RoundToInt(reward.weight)); //say�y� yuvarlar yak�n ondal�ktan Max ise iki say�dan b�y��� se�er yani weightMultiplier en az 1 olur.
            for (int i = 0; i < weightMultiplier; i++)
            {
                int randomAmount = Random.Range(reward.minAmount, reward.maxAmount + 1);
                weightedRewards.Add((reward, randomAmount));
            }
        }

        for (int i = weightedRewards.Count - 1; i > 0; i--) //burada t�m listeyi kar��t�r�r rastgele.
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



