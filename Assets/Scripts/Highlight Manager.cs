using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HighlightManager : MonoBehaviour
{
    private List<Slot> slots = new(); // readonly kaldırıldı, çünkü yeniden oluşturacağız
    private Slot currentSlot;
    private bool isSpinning = false;

    [SerializeField] private WalletManager walletManager;
    [SerializeField] private Sprite desiredGreySprite;

    private List<Image> highlightTrail = new List<Image>();

    public void Initialize(IReadOnlyList<Slot> slotList)  //bu liste blueprefab highlight prefab işlenmemiş ve rewardprefaba image ve amount miktarı doldurulmuş çağırılır.
    {
        slots.Clear(); //slotları temizle 

        slots.AddRange(slotList); //AddRange, bir koleksiyona (örnekte: slots listesini), başka bir koleksiyonun (örnekte: slotList listesini) tüm elemanlarını bir defada ekler.

        foreach (var slot in slots) //tüm sllotlraı dön
        {
            var highlightRenderer = slot.GetHighlightImage(); //highlightRenderer e highlight prefabdaki image atanır.
            if (highlightRenderer)
            {
                highlightRenderer.color = new Color(1, 1, 1, 0); //burada tekrardan alpha değeri 0 yapılır bence gereksiz.
            }
        }
    }

    public void StartSpin() //bu metod spin tuşuna basınca çalışır.
    {
        if (isSpinning || slots.Count == 0) return; //eğer şuanda dönüyorsa veya slot kalmadıysa spin başlatma
        StartCoroutine(SpinRoulette()); //spinrulet coroutine başlat.
    }

    public void ResetHighlights()
    {
        foreach (var slot in slots)
        {
            var highlightRenderer = slot.GetHighlightImage();
            if (highlightRenderer != null)
            {
                highlightRenderer.color = new Color(1, 1, 1, 0);
            }
        }
        highlightTrail.Clear();
        isSpinning = false;
        currentSlot = null;

        Debug.Log("Highlights have been reset.");
    }

    private IEnumerator SpinRoulette()
    {
        isSpinning = true; //rulet dönüyor.

        int totalSlots = slots.Count; //blueprefab highlight prefab işlenmemiş ve rewardprefaba image ve amount miktarı doldurulmuş slotlistin sayısını alır.
        int currentIndex = 0; //highlightın ilk indeksi 0 ayarlanır.
        int finalIndex; //durduğu son indeks değeri buna atanacak

        do
        {
            finalIndex = Random.Range(0, totalSlots); //tüm slotlar içinde rastgele bir indeks al.
        } while (slots[finalIndex].IsEliminated()); //finalindeks elenmiş bir indeks seçilirse devam eder yani elenmemiş bir slot bulana kadar do kısmı tekrar tekrar çalışır.

        int stepsToTarget = Random.Range(1, 3) * totalSlots + finalIndex - currentIndex; //random range 1 veya 2 seçilir * totalslots ile de seçili slota ulaşana kadar highlightın kaç tur aatacağıdır. ve seçilen slotun imndeksi (finalindexten) slotun başlangıç indeksi çıkarılarak toplamda kaç adım atacağı belirlenir.

        for (int i = 0; i <= stepsToTarget; i++) //highlightın yanacağı adım kadar çalışır.
        {
            currentSlot = slots[currentIndex]; //Bu satır, şu anki currentIndex değeri ile ilişkili slotu (slots[currentIndex]) alır ve bunu currentHighlight değişkenine atar. Amacı: Döngüde, hangi slotun aktif olarak highlight edileceğini belirler.
            var currentHighlightRenderer = currentSlot.GetHighlightImage(); //slotun şuanki indeksindeki highlight resmi currentHighlightRenderer a atanır.

            if (currentHighlightRenderer)
            {
                highlightTrail.Insert(0, currentHighlightRenderer); //Bu satır, şu anki currentIndex değeri ile ilişkili slotun highlight resmini alır ve highlightTrail listesinin en başına atar.
                currentHighlightRenderer.DOFade(1, 0.3f); //şeffaf olan highlightın alphasını 0.3 saniye sürede 1 yapar ve görünür olur.
            }

            for (int h = 0; h < highlightTrail.Count; h++) //Bu döngü, highlightTrail içindeki tüm highlight resimlerini sırasıyla işler.
            {
                var hr = highlightTrail[h]; //0 dan başlayarak highlightTrail listesindeki tüm image elemanlarını sırayla hr değişkenine atar. 0. indeksteki en parlak 1,2.. diye index arttıkça arkası sönüyor görüntüsü için aşağıda switchte alpha değerleri verilir.
                float targetAlpha = h switch
                {
                    0 => 1f, //şuanki hgihlight alphası 1
                    1 => 0.8f,//arkasındaki hgihlight alphası 0.8
                    2 => 0.5f,//arkasındaki hgihlight alphası 0.5
                    3 => 0.2f,//arkasındaki hgihlight alphası 0.2
                    _ => 0f //3.den sonrakiler söndürülür 4 tane yanan highlight gözüksün.
                };

                hr.DOKill(); //Bu, DOTween ile daha önce bu highlight için başlatılmış animasyonları durdurur. Amacı: Çakışan veya üst üste binen animasyonları engellemek.
                hr.DOFade(targetAlpha, 0.3f);  // listede indeksine gören işlenen highlight imagein 0.4 saniyede switch ettiği değere alphasını yarlar
            }

            if (highlightTrail.Count > 5) //Kontrol: Eğer highlightTrail içindeki highlight resimlerinin sayısı 5'ten fazlaysa, en eski highlight görüntüsünü listeden çıkarır.Amacı: Highlight izi efekti için maksimum 5 görseli sınırlamak(görsel karmaşayı ve performans sorunlarını önlemek).
            {
                highlightTrail.RemoveAt(highlightTrail.Count - 1);
            }

            currentIndex = (currentIndex + 1) % totalSlots; //Bu satır, currentIndex değerini bir artırır ve totalSlots sayısına göre döngüsel hale getirir.% totalSlots:Eğer currentIndex, totalSlots değerine ulaşırsa, tekrar 0'dan başlar. Bu, highlight'ın sonsuz bir döngüde ilerlemesini sağlar.

            float t = (float)i / stepsToTarget; //t Hesabı:i(şu anki adım) ile stepsToTarget(toplam adım sayısı) arasında bir ilerleme oranı hesaplar.t:0.0: Döngünün başında.1.0: Döngünün sonunda.
            float waitTime = Mathf.Lerp(0.1f, 0.25f, t); //float waitTime = Mathf.Lerp(0.1f, 0.25f, t);Mathf.Lerp(start, end, t):t değerine bağlı olarak, başlangıç ve bitiş arasında bir değer döndürür.0.1f → 0.25f:Döngünün başında(t = 0), bekleme süresi 0.1 saniyedir. Döngünün sonunda(t = 1), bekleme süresi 0.25 saniyeye çıkar.Amacı: Döngü ilerledikçe highlight'ın yavaşlamasını simüle eder (örneğin, rulet yavaşça duruyor gibi).
            yield return new WaitForSeconds(waitTime); //Her adım arasında waitTime kadar bekler. Bu, highlight'ın bir sonraki slota hareket etmeden önceki bekleme süresini belirler.Amacı: Ruletin dönüş hızını kontrol etmek.
        }
        FadeOutHighlightTrail(); // Highlight izi (highlightTrail) yok edilmesini sağlamak

        currentSlot.Eliminate(); //Amaç: Seçilen slotu "elenmiş" olarak işaretlemek ve ödül eklemek.
        walletManager.AddReward(currentSlot.GetRewardID(), currentSlot.GetRewardAmount()); //seçilen slottaki rewardın id sini ve sayısını walletmanagere addreward metoduna gönderir.

        Debug.Log($"Seçilen Ödül: {currentSlot.GetRewardID()}, Miktar: {currentSlot.GetRewardAmount()}");

        StartCoroutine(HighlightBlinkAndFadeOut());
        //isSpinning = false; //Ruletin dönme işleminin sona erdiğini belirtmek.
    }

    private void FadeOutHighlightTrail()
    {
        for (int i = 0; i < highlightTrail.Count; i++) // Tüm highlight izlerini döngüyle işle
        {
            var highlight = highlightTrail[i]; // highlightTrail listesindeki i. elemanı al

            if (highlight == currentSlot.GetHighlightImage()) // Eğer bu eleman seçili highlight'ın görüntüsü ise:
            {
                highlight.DOKill();// Önceki animasyonları durdur
                highlight.DOFade(1, 0.3f); // Tamamen görünür yap (şeffaflık = 1), 0.3 saniyede
            }
            else // Eğer bu eleman seçili highlight değilse:
            {    
                float fadeOutDelay = 0.1f * i;  // Her eleman için bir gecikme hesapla (i’ye bağlı olarak artan)
                highlight.DOKill();// Önceki animasyonları durdur
                highlight.DOFade(0, 0.3f).SetDelay(fadeOutDelay);// Elemanı görünmez yap (şeffaflık = 0), gecikmeyle başlar
            }
        }
        highlightTrail.Clear(); // Tüm highlightTrail listesini temizle
    }


    private IEnumerator HighlightBlinkAndFadeOut()
    {
        var finalHighlightRenderer = currentSlot?.GetHighlightImage(); //seçili slottaki highlight resmini finalHighlightRenderer a atadık.
        if (finalHighlightRenderer)
        {
            finalHighlightRenderer.color = new Color(1, 1, 1, 0); //alpha kanalı sıfırlandı görünmez
            finalHighlightRenderer.DOKill(); //önceki animasyonları sil.
            finalHighlightRenderer.DOFade(1, 0.1f) //0.1 saniye süreyle tamamen opak yap.
                .SetLoops(10, LoopType.Yoyo) //10 kez yanıp sönecek
                .OnComplete(() => { finalHighlightRenderer.color = new Color(1, 1, 1, 0); }); //tamamlandığında highlightı görünmez yap.

            yield return new WaitForSeconds(1f); //1 saniye bekle

            this.currentSlot.ActivateBlueSlot(); 

            yield return new WaitForSeconds(0.5f); //yarım saniye bekle

            if (currentSlot)
            {
                var rewardObj = currentSlot.GetRewardObject(); //seçili slottaki reward objesini al.
                if (rewardObj)
                {
                    var maskContainer = rewardObj.transform.Find("MaskContainer")?.GetComponent<RectTransform>(); //Eğer ödül nesnesi varsa, içinde MaskContainer adında bir RectTransform nesnesi arar.
                    if (maskContainer)
                    {
                        float finalWidth = 100f;
                        maskContainer.sizeDelta = new Vector2(0, maskContainer.sizeDelta.y);

                        maskContainer.DOSizeDelta(new Vector2(finalWidth, maskContainer.sizeDelta.y), 1.5f).SetEase(Ease.OutCubic) //DOTween ile sizeDelta genişliğini 100 birime kadar açar (1.5 saniyede Ease.OutCubic hareketiyle).Tamamlandığında: Sonraki işlemleri başlatır.
                            .OnComplete(() =>
                            {
                                var rewardFlyAnimation = FindFirstObjectByType<RewardFlyAnimation>();
                                if (rewardFlyAnimation)
                                {
                                    Sprite rewardSpr = currentSlot.GetRewardSprite(); //currentSlot (şu an seçilen slot) nesnesinden ödülün sprite'ını alır. rewardSpr a atar.
                                    rewardFlyAnimation.PlayRewardFlyAnimation(rewardSpr, () => //PlayRewardFlyAnimation metodunu çağırır.Bu metod, ödül sprite'ı (rewardSpr) ile bir animasyon oynatır.
                                    {
                                        var rewardImageObj = rewardObj.transform.Find("Image"); //reward imageini ismiyle bulur childlardan.
                                        if (rewardImageObj)
                                        {
                                            rewardImageObj.gameObject.SetActive(false); //ve setactive false yapar.
                                        }

                                        var blueHighlightObj = currentSlot.GetBlueSlotObject();
                                        if (blueHighlightObj && desiredGreySprite)
                                        {
                                            var blueHighlightImg = blueHighlightObj.transform.Find("BlueHighlightImage")?.GetComponent<Image>();
                                            if (blueHighlightImg)
                                            {
                                                blueHighlightImg.sprite = desiredGreySprite;
                                            }
                                        }
                                        CheckAndShowWinPopup();
                                        isSpinning = false;
                                    });
                                }
                            });
                    }
                    else
                    {
                        Debug.LogWarning("MaskContainer not found under RewardPrefab!");
                    }
                }
                else
                {
                    Debug.LogWarning("Reward object not found in current slot!");
                }
            }
        }
    }

    private void CheckAndShowWinPopup()
    {
        if (AllRewardsCollected())
        {
            ShowWinPopup();
        }
    }

    private bool AllRewardsCollected() //tüm rewardlar toplandı mı toplanmadı mı eğer hepsi iseliminated çağrısına true dönerse bu metod true döner eğer hesi toplanmadıysa false döner.
    {
        foreach (var slot in slots)
        {
            if (!slot.IsEliminated()) return false;
        }
        return true;
    }

    private void ShowWinPopup() 
    {
        var winPopupManager = FindFirstObjectByType<WinPopupManager>();
        if (winPopupManager)
        {
            winPopupManager.ShowPopup();
        }
    }
}
