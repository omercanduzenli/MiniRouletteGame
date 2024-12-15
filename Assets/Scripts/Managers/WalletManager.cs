using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletManager : MonoBehaviour, IWalletManager
{
    [Header("Wallet Management")]
    [SerializeField] private List<RewardData> _rewards;
    [SerializeField] private Transform _walletGridLayout;
    [SerializeField] private GameObject _walletRewardImgPrefab;
    [SerializeField] private GameObject _walletPanel;

    private readonly Dictionary<string, (int amount, TextMeshProUGUI text)> _walletEntries = new();
    private const string SaveFileName = "walletData.json";

    // Unity lifecycle method: Initializes the wallet on start.
    private void Start()
    {
        if (_walletPanel) _walletPanel.SetActive(false);
        LoadWalletData();
        InitializeWalletUI();
    }

    // Adds the given reward to the wallet.
    public void AddReward(string rewardID, int amount)
    {
        if (_walletEntries.TryGetValue(rewardID, out var entry))
        {
            int updatedAmount = entry.amount + amount;
            UpdateWalletEntry(rewardID, updatedAmount, entry.text);
        }
        else
        {
            Debug.LogWarning($"Reward ID '{rewardID}' does not exist in the wallet.");
        }
    }

    // Opens the wallet panel with animation.
    public void OpenWalletPanel()
    {
        if (!_walletPanel) return;

        _walletPanel.SetActive(true);
        AnimatePanel(_walletPanel.transform);
        
    }

    // Closes the wallet panel.
    public void CloseWalletPanel()
    {
        if (_walletPanel) _walletPanel.SetActive(false);
    }

    // Saves the wallet data to a JSON file.
    public void SaveWalletData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        var saveData = new WalletSaveData(_walletEntries);
        var jsonData = JsonUtility.ToJson(saveData);
        File.WriteAllText(filePath, jsonData);
    }

    // Loads the wallet data from a JSON file.
    public void LoadWalletData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(filePath)) return;

        var jsonData = File.ReadAllText(filePath);
        var saveData = JsonUtility.FromJson<WalletSaveData>(jsonData);

        if (saveData != null && saveData.keys != null && saveData.values != null && saveData.keys.Count == saveData.values.Count)
        {
            _walletEntries.Clear();
            for (int i = 0; i < saveData.keys.Count; i++)
            {
                _walletEntries[saveData.keys[i]] = (saveData.values[i], null);
            }
        }
    }

    // Initializes the wallet UI with the available rewards.
    private void InitializeWalletUI()
    {
        if (_rewards == null || _rewards.Count == 0) return;

        foreach (var reward in _rewards)
        {
            if (reward == null || string.IsNullOrEmpty(reward.rewardID)) continue;
            CreateWalletItem(reward);
        }
    }

    // Creates a wallet UI item for the given reward.
    private void CreateWalletItem(RewardData rewardData)
    {
        var walletItem = Instantiate(_walletRewardImgPrefab, _walletGridLayout);
        var image = walletItem.GetComponent<Image>();
        if (image) image.sprite = rewardData.rewardSprite;

        var text = walletItem.GetComponentInChildren<TextMeshProUGUI>();
        int currentAmount = _walletEntries.ContainsKey(rewardData.rewardID) ? _walletEntries[rewardData.rewardID].amount : 0;
        text.text = currentAmount.ToString();

        _walletEntries[rewardData.rewardID] = (currentAmount, text);
    }

    // Updates the wallet entry for a specific reward.
    private void UpdateWalletEntry(string rewardID, int updatedAmount, TextMeshProUGUI text)
    {
        text.text = updatedAmount.ToString();
        _walletEntries[rewardID] = (updatedAmount, text);
        SaveWalletData();
    }

    // Animates the wallet panel when opened.
    private void AnimatePanel(Transform panelTransform)
    {
        panelTransform.localScale = Vector3.zero;
        panelTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    // Serializable class for saving wallet data in JSON format.
    [Serializable]
    private class WalletSaveData
    {
        public List<string> keys;
        public List<int> values;

        public WalletSaveData(Dictionary<string, (int amount, TextMeshProUGUI)> dictionary)
        {
            keys = new List<string>();
            values = new List<int>();

            foreach (var kvp in dictionary)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value.amount);
            }
        }
    }
}
