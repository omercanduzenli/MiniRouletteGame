using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "ScriptableObjects/RewardData", order = 1)]
public class RewardData : ScriptableObject
{
    public string rewardID; 
    public string rewardName; 
    public Sprite rewardSprite;
    public float weight; 
    public int minAmount; 
    public int maxAmount; 
}
