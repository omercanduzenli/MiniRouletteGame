using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework;

public class SlotManager : MonoBehaviour
{
    [SerializeField] private AssetReference slotPrefabReference;
    [SerializeField] private AssetReference highlightPrefabReference;
    [SerializeField] private AssetReference rewardPrefabReference;
    [SerializeField] private AssetReference BlueHighlightPrefabReference;

    private readonly List<Slot> slots = new();
    public IReadOnlyList<Slot> Slots => slots; //bu liste blueprefab highlight prefab i�lenmemi� ve rewardprefaba image ve amount miktar� doldurulmu� g�nderilir.
    
    //eleman ekleme, ��karma veya d�zenleme gibi i�lemleri yapamaz d��ar�dan alanlar.ancak Listedeki elemanlara eri�ebilir,Listenin uzunlu�unu alabilir.
    //IReadOnlyList, d��ar�ya yaln�zca okuma izni vererek veri g�venli�ini art�r�r.
    //readonly List yaln�zca referans� korurken, d�� d�nyadan listenin i�eri�i de�i�tirilebilir.
    public async Task CreateSlots(List<Transform> panels, Dictionary<string, int> panelSlotCounts, Queue<(RewardData reward, int amount)> rewardQueue)
    {
        slots.Clear(); //slot varsa temizle.

        int slotCounter = 0;
        foreach (var panel in panels)  //s�ras�yla t�m paneller i�inde d�n�l�r.
        {
            if (panelSlotCounts.TryGetValue(panel.tag, out int slotCount))
            {
                for (int i = 0; i < slotCount; i++)
                {
                    var slotPrefab = await LoadPrefabAsync(slotPrefabReference, panel); //slot prefab� panel child� olarak instantiate edilir.
                    if (slotPrefab)
                    {
                        slotPrefab.name = $"Slot{slotCounter++}"; //slotun ismi �retildik�e numaraland�r�l�r.
                        var slot = slotPrefab.GetComponent<Slot>(); //slotun i�indeki Slot scripti al�n�r.

                        
                        var BlueHighlightPrefab = await LoadPrefabAsync(BlueHighlightPrefabReference, slotPrefab.transform); //BlueHighlightPrefab slotPrefab child� olarak instantiate edilir.
                        if (BlueHighlightPrefab)
                        {
                            BlueHighlightPrefab.transform.SetSiblingIndex(0); //childlar aras�nda en �ste koy 
                            BlueHighlightPrefab.SetActive(false);             // ilk olu�tu�unda aktif de�il.     
                        }

                        var highlight = await LoadPrefabAsync(highlightPrefabReference, slotPrefab.transform);  //highlight slotPrefab child� olarak instantiate edilir.
                        if (highlight && BlueHighlightPrefab)
                        {     
                            highlight.transform.SetSiblingIndex(1); //highl�ght� 1. indexe yerle�tir.

                            var highlightImage = highlight.GetComponentInChildren<Image>(); //highlight�n image bile�enini al.
                            if (highlightImage)
                            {
                                highlightImage.color = new Color(1, 1, 1, 0);  //highl�ght� ba�lang��ta g�r�nmez yap.
                            }
                        }

                        var reward = await LoadPrefabAsync(rewardPrefabReference, slotPrefab.transform); //reward slotPrefab child� olarak instantiate edilir.
                        if (reward)
                        {
                            reward.transform.SetSiblingIndex(2); //highl�ght� 2. indexe yerle�tir.
                        }

                        slot.Initialize(reward, highlight, BlueHighlightPrefab); //slotprefab alt�ndaki slot scriptine burada �retilen prefablar g�nderilir.

                        if (rewardQueue.Count > 0)
                        {
                            var rewardDataWithAmount = rewardQueue.Dequeue(); //reward datadaki kar��t�r�lm�� rewarddata, int amount bile�eni sondan ��kar�l�r
                            slot.SetReward(rewardDataWithAmount.reward.rewardSprite, rewardDataWithAmount.reward.rewardID, rewardDataWithAmount.amount); //olu�turulan reward prefab�na queueden ��kan eleman�n sprite � reward id si ve say�s� g�nderilir ve sahnede yerle�tirilir.
                        }

                        slotPrefab.SetActive(false); //slotlar ilk �retildi�inde inaktif yap�l�r.
                        slots.Add(slot); //�retilen slot slot listesine eklenir.
                    }
                }
            }
        }
    }

    public void ResetSlots(Queue<(RewardData reward, int amount)> rewardQueue)
    {
        foreach (var slot in slots)
        {
            slot.ResetSlot();
        }

        foreach (var slot in slots)
        {
            if (rewardQueue.Count > 0)
            {
                var rewardDataWithAmount = rewardQueue.Dequeue();
                slot.SetReward(rewardDataWithAmount.reward.rewardSprite, rewardDataWithAmount.reward.rewardID, rewardDataWithAmount.amount);
            }
        }
        foreach (var slot in slots)
        {
            slot.gameObject.SetActive(false);
        }

        Debug.Log("Slots have been reset.");
    }

    public IEnumerator ActivateSlotsWithAnimation()
    {
        foreach (var slot in slots) //t�m slotlar� d�ner
        {
            slot.gameObject.SetActive(true); //0.1 saniyede bir her slotu aktif eder s�rayla
            slot.transform.localScale = Vector3.zero; //slotlar ba�ta boyutsuz olur ve.
            slot.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack); //0.3 saniyede bir 1.1.1 boyutuna gelirler Ease.OutBack animasyonuyla
            yield return new WaitForSeconds(0.1f);
        }
    }

    private async Task<GameObject> LoadPrefabAsync(AssetReference reference, Transform parent = null)
    {

        if (reference.OperationHandle.IsValid() &&
            reference.OperationHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return Instantiate(reference.OperationHandle.Result as GameObject, parent);
        }

        //E�er Prefab daha �nce y�klenmemi�se, �u sat�r devreye girer:
        var handle = reference.LoadAssetAsync<GameObject>(); //Prefab asenkron olarak y�klenir 
        await handle.Task; //ve i�lem tamamlanana kadar beklenir.

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return Instantiate(handle.Result, parent);
        }

        Debug.LogError($"Failed to load AssetReference: {reference.RuntimeKey}");
        return null;
    }

    //reference.OperationHandle.IsValid() bu k�s�m assetin belle�e daha �nce y�klenip y�klenmedi�ini kontrol eder yani ilk defa olu�uyorsa addressable diskten �ekece�i i�in false ancak belle�e geldikten sonra prefab silinmedik�e bellekte kalacak ve true d�necek.
    //reference.OperationHandle.Status == AsyncOperationStatus.Succeeded bu sat�r da y�kleme i�lemi ba�ar�yla yap�lm�� m�? bunu kontrol eder.
}
