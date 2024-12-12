using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletManager : MonoBehaviour
{
    [Header("Wallet Management")]
    [SerializeField] private List<RewardData> rewards; //14 reward SO buraya atan�r.
    [SerializeField] private GameObject WalletRewardImgPrefab; //walleta yerle�ecek reward simgeleri ve texti bu prefabda.
    [SerializeField] private Transform walletGridLayout; //wallet paneldeki layput gamobjecti �d�lleri s�rayla yerle�tirmek i�in.
    [SerializeField] private GameObject walletPanel; //wallet panaeli atan�r.

    private readonly Dictionary<string, TextMeshProUGUI> rewardTextLookup = new(); //readonly bu de�i�kene ba�ka dictionary atanamaz demek i�eri�i de�i�ebilir.

    private void Start()
    {
        walletPanel.SetActive(false);
        InitializeWalletUI();
    }

    private void InitializeWalletUI()
    {
        foreach (var reward in rewards)  //t�m reward SO lar i�inde gezilir.
        {
            GameObject WalletRewardImg = Instantiate(WalletRewardImgPrefab, walletGridLayout); //14 adet WalletRewardImgPrefab walletGridLayoutun altna yerle�tirilir.

            var image = WalletRewardImg.GetComponent<Image>();
            var text = WalletRewardImg.GetComponentInChildren<TextMeshProUGUI>();

            if (image) image.sprite = reward.rewardSprite; //wallettaki WalletRewardlara �nce spritelar atan�r.
            if (text)
            {
                text.text = "0"; // �uanda i�lenen reward�n textine 0 atan�r.
                rewardTextLookup[reward.rewardID] = text; //rewardID de�erine g�re textler s�ras�yla atan�r .
            }
        }
    }

    public void AddReward(string rewardID, int amount) //highlighmanagerden se�ilen slotun id si ve amount� al�n�r.
    {
        if (rewardTextLookup.TryGetValue(rewardID, out var text)) //Ama�: rewardID ile e�le�en bir metin bile�eni (TextMeshProUGUI) var m� kontrol eder. E�er varsa, text de�i�kenine atar.
        {
            int currentAmount = string.IsNullOrEmpty(text.text) ? 0 : int.Parse(text.text); //Ama�: Mevcut miktar� al�r.E�er text.text bo�sa 0, de�ilse tam say�ya �evirerek al�r.
            currentAmount += amount; //Ama�: Gelen miktar� mevcut miktara ekler.
            text.text = currentAmount.ToString(); //gelen amount texte yazd�r�l�r. 
            
        }
        else
        {
            Debug.LogWarning($"Reward ID '{rewardID}' i�in bir UI paneli bulunamad�!");
        }
    }

    public void OpenWalletPanel() //wallet butonuna t�kland���nda
    {
        walletPanel.SetActive(true);
    }

    public void CloseWalletPanel() //close tu�una bas�ld���nda.
    {
        walletPanel.SetActive(false);
    }
}
