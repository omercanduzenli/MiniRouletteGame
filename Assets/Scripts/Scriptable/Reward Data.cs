using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "ScriptableObjects/RewardData", order = 1)]
public class RewardData : ScriptableObject
{
    public string rewardName;
    public Sprite rewardSprite;
    public int rewardValue;
    public float weight;
}
