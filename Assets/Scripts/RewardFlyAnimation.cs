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

        for (int i = 0; i < rewardCount; i++) // rewardCount kadar simge olu�turur.
        {
            GameObject icon = Instantiate(rewardIconPrefab, centerRect.parent); // Simge prefab'�n� sahneye ekler.
            var img = icon.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = rewardSprite; // Simgelere �d�l sprite'�n� atar.
                img.color = Color.white; // Simgelerin rengini beyaz yapar.
            }

            RectTransform iconRect = icon.GetComponent<RectTransform>();
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Rastgele bir konum hesaplar.
            float r = Random.Range(0f, spawnRadius);
            Vector2 randomOffset = new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);

            iconRect.anchoredPosition = centerRect.anchoredPosition + randomOffset;// Simgeyi rastgele konuma yerle�tirir ve ba�lang�� boyutunu s�f�r yapar.
            iconRect.localScale = Vector3.zero;
            iconRect.DOScale(1, 0.5f).SetEase(Ease.OutBack);// Simgeyi b�y�tme animasyonu oynat�r.

            icons.Add(icon);// Simgeyi listeye ekler.
        }

        StartCoroutine(WaitAndMoveIcons()); // Hareket animasyonunu ba�latmak i�in coroutine �a��r�r.
    }

    private IEnumerator WaitAndMoveIcons()
    {
        yield return new WaitForSeconds(0.5f); // 0.5 saniye bekler.

        int completedAnimations = 0; // Tamamlanan animasyon say�s�n� takip eder.

        foreach (var icon in icons) // Her simge i�in i�lem yapar.
        {
            if (icon)
            {
                var iconRect = icon.GetComponent<RectTransform>();
                var img = icon.GetComponent<Image>();

                // Simgeyi kendi etraf�nda d�nd�rme animasyonu.
                iconRect.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Restart);

                // Simgeyi c�zdana do�ru hareket ettirme animasyonu.
                iconRect.DOAnchorPos(walletIconRect.anchoredPosition, flyDuration).SetEase(Ease.InOutCubic).OnComplete(() =>
                {
                    if (img)
                    { // Simgeyi yava��a yok etme animasyonu.
                        img.DOFade(0, 0.5f).OnComplete(() =>
                        {
                            iconRect.DOKill();
                            Destroy(icon);// Simgeyi sahneden siler.
                            completedAnimations++;
                            if (completedAnimations == icons.Count && onCompleteCallback != null)
                            {
                                onCompleteCallback.Invoke();// T�m animasyonlar tamamland���nda callback �a��r�r.
                            }
                        });
                    }
                });
            }
        }
    }
}
