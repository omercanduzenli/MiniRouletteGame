using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletManager : MonoBehaviour
{
    [Header("Wallet Management")]
    [SerializeField] private List<RewardData> rewards; //14 reward SO buraya atanýr.
    [SerializeField] private GameObject WalletRewardImgPrefab; //walleta yerleþecek reward simgeleri ve texti bu prefabda.
    [SerializeField] private Transform walletGridLayout; //wallet paneldeki layput gamobjecti ödülleri sýrayla yerleþtirmek için.
    [SerializeField] private GameObject walletPanel; //wallet panaeli atanýr.

    private readonly Dictionary<string, TextMeshProUGUI> rewardTextLookup = new(); //readonly bu deðiþkene baþka dictionary atanamaz demek içeriði deðiþebilir.

    private void Start()
    {
        walletPanel.SetActive(false);
        InitializeWalletUI();
    }

    private void InitializeWalletUI()
    {
        foreach (var reward in rewards)  //tüm reward SO lar içinde gezilir.
        {
            GameObject WalletRewardImg = Instantiate(WalletRewardImgPrefab, walletGridLayout); //14 adet WalletRewardImgPrefab walletGridLayoutun altna yerleþtirilir.

            var image = WalletRewardImg.GetComponent<Image>();
            var text = WalletRewardImg.GetComponentInChildren<TextMeshProUGUI>();

            if (image) image.sprite = reward.rewardSprite; //wallettaki WalletRewardlara önce spritelar atanýr.
            if (text)
            {
                text.text = "0"; // þuanda iþlenen rewardýn textine 0 atanýr.
                rewardTextLookup[reward.rewardID] = text; //rewardID deðerine göre textler sýrasýyla atanýr .
            }
        }
    }

    public void AddReward(string rewardID, int amount) //highlighmanagerden seçilen slotun id si ve amountý alýnýr.
    {
        if (rewardTextLookup.TryGetValue(rewardID, out var text)) //Amaç: rewardID ile eþleþen bir metin bileþeni (TextMeshProUGUI) var mý kontrol eder. Eðer varsa, text deðiþkenine atar.
        {
            int currentAmount = string.IsNullOrEmpty(text.text) ? 0 : int.Parse(text.text); //Amaç: Mevcut miktarý alýr.Eðer text.text boþsa 0, deðilse tam sayýya çevirerek alýr.
            currentAmount += amount; //Amaç: Gelen miktarý mevcut miktara ekler.
            text.text = currentAmount.ToString(); //gelen amount texte yazdýrýlýr. 
            
        }
        else
        {
            Debug.LogWarning($"Reward ID '{rewardID}' için bir UI paneli bulunamadý!");
        }
    }

    public void OpenWalletPanel() //wallet butonuna týklandýðýnda
    {
        walletPanel.SetActive(true);
    }

    public void CloseWalletPanel() //close tuþuna basýldýðýnda.
    {
        walletPanel.SetActive(false);
    }
}
