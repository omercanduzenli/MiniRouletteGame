using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardFlyAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _rewardIconPrefab;
    [SerializeField] private RectTransform _walletIconRect;
    [SerializeField] private RectTransform _centerRect;

    [SerializeField] private int _rewardCount = 7;
    [SerializeField] private float _flyDuration = 1.5f;
    [SerializeField] private float _spawnRadius = 100f;

    private Action _onCompleteCallback;
    private readonly Queue<GameObject> _pooledIcons = new();

    // Plays the reward fly animation.
    public void PlayRewardFlyAnimation(Sprite rewardSprite, Action onComplete)
    {
        _onCompleteCallback = onComplete;
        SpawnAndFlyIcons(rewardSprite);
    }

    // Spawns and animates icons flying.
    private void SpawnAndFlyIcons(Sprite rewardSprite)
    {
        var activeIcons = new List<GameObject>();

        for (int i = 0; i < _rewardCount; i++)
        {
            var icon = GetOrCreateIcon();
            SetupIconImage(icon, rewardSprite);
            PositionIconRandomly(icon);
            ScaleIconUp(icon);
            activeIcons.Add(icon);
        }

        StartCoroutine(WaitAndMoveIcons(activeIcons));
    }

    // Sets up the sprite for the reward icon.
    private void SetupIconImage(GameObject icon, Sprite sprite)
    {
        var img = icon.GetComponent<Image>();
        if (img)
        {
            img.sprite = sprite;
            img.color = Color.white;
        }
    }

    // Positions the icon randomly within a circular area.
    private void PositionIconRandomly(GameObject icon)
    {
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.SetParent(_centerRect);

        float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = UnityEngine.Random.Range(0f, _spawnRadius);
        Vector2 randomOffset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

        iconRect.anchoredPosition = randomOffset;
    }

    // Animates the scaling up of an icon.
    private void ScaleIconUp(GameObject icon)
    {
        var iconRect = icon.GetComponent<RectTransform>();
        iconRect.localScale = Vector3.zero;
        iconRect.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    // Moves icons to the wallet position.
    private void MoveIconToWallet(GameObject icon, Action onAnimationComplete)
    {
        var iconRect = icon.GetComponent<RectTransform>();
        var img = icon.GetComponent<Image>();

        iconRect.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        iconRect.DOAnchorPos(_walletIconRect.anchoredPosition, _flyDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            if (img)
            {
                img.DOFade(0, 0.5f).OnComplete(() =>
                {
                    iconRect.DOKill();
                    icon.SetActive(false);
                    _pooledIcons.Enqueue(icon);
                    onAnimationComplete?.Invoke();
                });
            }
        });
    }

    // Waits and moves all icons to their target position.
    private IEnumerator WaitAndMoveIcons(List<GameObject> activeIcons)
    {
        yield return new WaitForSeconds(0.5f);

        int completedAnimations = 0;
        foreach (var icon in activeIcons)
        {
            MoveIconToWallet(icon, () =>
            {
                completedAnimations++;
                if (completedAnimations == activeIcons.Count)
                {
                    _onCompleteCallback?.Invoke();
                }
            });
        }
    }

    // Gets an icon from the pool or creates a new one.
    private GameObject GetOrCreateIcon()
    {
        if (_pooledIcons.Count > 0)
        {
            var icon = _pooledIcons.Dequeue();
            icon.SetActive(true);
            return icon;
        }

        return Instantiate(_rewardIconPrefab, _centerRect);
    }

    // Cleans up animations when disabled.
    private void OnDisable()
    {
        DOTween.Kill(gameObject);
    }
}
