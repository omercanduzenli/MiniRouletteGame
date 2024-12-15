using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "ScriptableObjects/RewardData", order = 50)]
public class RewardData : ScriptableObject
{
    public string rewardID;
    public Sprite rewardSprite;
    public float weight;
    public int minAmount;
    public int maxAmount;
}
