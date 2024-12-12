using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class RewardFlyAnimation : MonoBehaviour
{
    [SerializeField] private GameObject rewardIconPrefab;
    [SerializeField] private RectTransform walletIconRect;
    [SerializeField] private RectTransform centerRect;
    [SerializeField] private int rewardCount = 7;
    [SerializeField] private float flyDuration = 1.5f;
    [SerializeField] private float spawnRadius = 100f;

    private System.Action onCompleteCallback;
    private List<GameObject> icons = new();

    public void PlayRewardFlyAnimation(Sprite rewardSprite, System.Action onComplete = null)
    {
        onCompleteCallback = onComplete;
        SpawnAndFlyIcons(rewardSprite);
    }

    private void SpawnAndFlyIcons(Sprite rewardSprite)
    {
        icons.Clear(); // Eski simgeleri temizler.

        for (int i = 0; i < rewardCount; i++) // rewardCount kadar simge oluþturur.
        {
            GameObject icon = Instantiate(rewardIconPrefab, centerRect.parent); // Simge prefab'ýný sahneye ekler.
            var img = icon.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = rewardSprite; // Simgelere ödül sprite'ýný atar.
                img.color = Color.white; // Simgelerin rengini beyaz yapar.
            }

            RectTransform iconRect = icon.GetComponent<RectTransform>();
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Rastgele bir konum hesaplar.
            float r = Random.Range(0f, spawnRadius);
            Vector2 randomOffset = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);

            iconRect.anchoredPosition = centerRect.anchoredPosition + randomOffset;// Simgeyi rastgele konuma yerleþtirir ve baþlangýç boyutunu sýfýr yapar.
            iconRect.localScale = Vector3.zero;
            iconRect.DOScale(1, 0.5f).SetEase(Ease.OutBack);// Simgeyi büyütme animasyonu oynatýr.

            icons.Add(icon);// Simgeyi listeye ekler.
        }

        StartCoroutine(WaitAndMoveIcons()); // Hareket animasyonunu baþlatmak için coroutine çaðýrýr.
    }

    private IEnumerator WaitAndMoveIcons()
    {
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekler.

        int completedAnimations = 0; // Tamamlanan animasyon sayýsýný takip eder.

        foreach (var icon in icons) // Her simge için iþlem yapar.
        {
            if (icon)
            {
                var iconRect = icon.GetComponent<RectTransform>();
                var img = icon.GetComponent<Image>();

                // Simgeyi kendi etrafýnda döndürme animasyonu.
                iconRect.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Restart);

                // Simgeyi cüzdana doðru hareket ettirme animasyonu.
                iconRect.DOAnchorPos(walletIconRect.anchoredPosition, flyDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    if (img)
                    { // Simgeyi yavaþça yok etme animasyonu.
                        img.DOFade(0, 0.5f).OnComplete(() =>
                        {
                            iconRect.DOKill();
                            Destroy(icon);// Simgeyi sahneden siler.
                            completedAnimations++;
                            if (completedAnimations == icons.Count && onCompleteCallback != null)
                            {
                                onCompleteCallback.Invoke();// Tüm animasyonlar tamamlandýðýnda callback çaðýrýr.
                            }
                        });
                    }
                });
            }
        }
    }
}
