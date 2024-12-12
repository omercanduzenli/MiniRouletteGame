using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private GameObject reward;
    private GameObject highlight;
    private GameObject blueHighlight;
    private TextMeshProUGUI amountText;

    private string rewardID;
    private int rewardAmount;
    private bool isEliminated = false;

    public string GetRewardID() => rewardID; //dışarıya rewardID döner
    public int GetRewardAmount() => rewardAmount; //dışarıya reward sayısını döner
    public bool IsEliminated() => isEliminated;  //Seçildi mi seçilmedi mi slot belirten flag.

    public void Initialize(GameObject rewardPrefab, GameObject highlightPrefab, GameObject maviSlotPrefab) //slotmanagerde yüklenen prefabları alır.
    {
        reward = rewardPrefab;
        highlight = highlightPrefab;
        blueHighlight = maviSlotPrefab;
        if (reward)
        {
            amountText = reward.GetComponentInChildren<TextMeshProUGUI>(); //rewarddaki amounttexti alır.
        }
    }

    public Image GetHighlightImage()
    {
        return highlight?.GetComponentInChildren<Image>(); //highlight?. null kontrolüdür if(highlight gibi) ve highlight prefabındaki image i döner.
    }

    public void SetReward(Sprite rewardSprite, string rewardID, int amount) //kuyruktan çıkarılan rewarddatanın spriteı, reward id si ve sayısı buraya gönderilir
    {
        this.rewardID = rewardID;
        this.rewardAmount = amount;

        var image = reward.GetComponentInChildren<Image>(); //oluşan rewarddaki ımage bileşeni alınır.

        if (image) image.sprite = rewardSprite; //rewarddatadan aöınan sprite reward prefabına atanır.

        if (amountText)
        {
            amountText.text = amount > 1 ? amount.ToString() : ""; //queueden gelen amount miktarı 1 den büyükse texte yazılır değilse yazılmaz
        }
    }

    public void Eliminate()
    {
        isEliminated = true;
    }
    public void ActivateBlueSlot() // bu metot çağrıldığında seçim animasyonu
    {
        if (highlight) //highlight varsa kapat 
        {
            highlight.SetActive(false); 
        }

        if (blueHighlight) //bluhighlightı aç
        {
            blueHighlight.SetActive(true);
        }
    }
    public GameObject GetBlueSlotObject()
    {
        return blueHighlight;
    }

    public GameObject GetRewardObject()
    {
        return reward;
    }
    public Sprite GetRewardSprite()
    {
        var image = reward?.GetComponentInChildren<Image>();
        return image != null ? image.sprite : null;
    }
    public void ResetSlot()
    {
        isEliminated = false; //slot yeniden seçilebilir.
        if (blueHighlight) blueHighlight.SetActive(false);
        if (amountText) amountText.text = "";
        Debug.Log($"{name} has been reset.");
    }




}
