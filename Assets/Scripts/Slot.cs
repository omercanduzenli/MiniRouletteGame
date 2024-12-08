using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private GameObject reward;
    private GameObject highlight;

    public GameObject GetReward() => reward;
    public GameObject GetHighlight() => highlight;

    public void Initialize(GameObject rewardPrefab, GameObject highlightPrefab)
    {
        reward = rewardPrefab;
        highlight = highlightPrefab;
        highlight.SetActive(false);
    }

    public void SetHighlight(bool isActive)
    {
        if (highlight)
        {
            highlight.SetActive(isActive);
        }
    }

    public void SetReward(Sprite rewardSprite)
    {
        if (reward)
        {
            Image image = reward.GetComponentInChildren<Image>();
            if (image)
            {
                image.sprite = rewardSprite;
            }
        }
    }
}
