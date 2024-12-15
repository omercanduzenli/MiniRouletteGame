using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    // Child object references
    private GameObject _rewardObject;
    private GameObject _highlightObject;
    private GameObject _blueHighlightObject;
    private RectTransform _maskContainer;

    private RewardData _rewardData;
    private TextMeshProUGUI _rewardAmountText;
    private Sprite _initialBlueHighlightSprite;

    private string _rewardID;
    private int _rewardAmount;
    private bool _isEliminated;

    // Constants for child object indices
    private const int HighlightChildIndex = 0;
    private const int BlueHighlightChildIndex = 1;
    private const int RewardChildIndex = 2;
    private const int MaskContainerChildIndex = 3;

    // Public Getters
    public string GetRewardID() => _rewardID;
    public int GetRewardAmount() => _rewardAmount;
    public bool IsEliminated() => _isEliminated;
    public RewardData GetRewardData() => _rewardData;
    public GameObject GetRewardObject() => _rewardObject;
    public RectTransform GetMaskContainerObject() => _maskContainer;
    public GameObject GetBlueHighlightObject() => _blueHighlightObject;
    public Sprite GetRewardSprite() => _rewardObject?.GetComponentInChildren<Image>()?.sprite;
    public Image GetHighlightImage() => _highlightObject?.GetComponentInChildren<Image>();

    // Initializes the slot and assigns child object references
    public void Initialize()
    {
        _rewardObject = transform.GetChild(RewardChildIndex).gameObject;
        _highlightObject = transform.GetChild(HighlightChildIndex).gameObject;
        _blueHighlightObject = transform.GetChild(BlueHighlightChildIndex).gameObject;
        _maskContainer = transform.GetChild(MaskContainerChildIndex).GetComponent<RectTransform>();

        if (_blueHighlightObject)
        {
            var blueHighlightImage = _blueHighlightObject.GetComponentInChildren<Image>();
            if (blueHighlightImage)
                _initialBlueHighlightSprite = blueHighlightImage.sprite;

            _blueHighlightObject.SetActive(false);
        }

        if (_rewardObject)
        {
            _rewardAmountText = _rewardObject.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    // Assigns reward details to the slot
    public void SetReward(Sprite rewardSprite, string rewardID, int amount, RewardData rewardData)
    {
        _rewardID = rewardID;
        _rewardAmount = amount;
        _rewardData = rewardData;

        var image = _rewardObject?.GetComponentInChildren<Image>();
        if (image) image.sprite = rewardSprite;

        if (_rewardAmountText)
        {
            _rewardAmountText.text = amount > 1 ? amount.ToString() : "";
        }
    }

    // Marks the slot as eliminated
    public void Eliminate()
    {
        _isEliminated = true;
    }

    // Activates the blue highlight for the slot
    public void ActivateBlueSlot()
    {
        if (_blueHighlightObject) _blueHighlightObject.SetActive(true);

        var highlightImage = GetHighlightImage();
        highlightImage?.DOKill();
        if (highlightImage) highlightImage.color = new Color(1, 1, 1, 0);
    }

    // Resets the slot to its initial state
    public void CompletelyResetToInitialState()
    {
        _isEliminated = false;
        ResetHighlight();
        ResetBlueHighlight();
        ResetRewardVisuals();
        ResetMaskSize();
    }

    // Private Methods
    // Resets the highlight state
    private void ResetHighlight()
    {
        var highlightImage = GetHighlightImage();
        highlightImage?.DOKill();
        if (highlightImage)
            highlightImage.color = new Color(1, 1, 1, 0);
    }

    // Resets the blue highlight state
    private void ResetBlueHighlight()
    {
        if (_blueHighlightObject)
        {
            _blueHighlightObject.SetActive(false);
            var blueHighlightImage = _blueHighlightObject.GetComponentInChildren<Image>();
            if (blueHighlightImage)
                blueHighlightImage.sprite = _initialBlueHighlightSprite;
        }
    }

    // Resets the mask container size
    private void ResetMaskSize()
    {
        if (_maskContainer)
        {
            _maskContainer.sizeDelta = new Vector2(0, _maskContainer.sizeDelta.y);
        }
    }

    // Resets reward visuals
    private void ResetRewardVisuals()
    {
        if (_rewardObject)
        {
            var rewardImageObj = _rewardObject.transform.GetChild(0);
            if (rewardImageObj) rewardImageObj.gameObject.SetActive(true);

            if (_rewardAmountText) _rewardAmountText.text = "";
        }
    }
}
