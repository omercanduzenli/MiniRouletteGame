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
    public IReadOnlyList<Slot> Slots => slots; //bu liste blueprefab highlight prefab iþlenmemiþ ve rewardprefaba image ve amount miktarý doldurulmuþ gönderilir.
    
    //eleman ekleme, çýkarma veya düzenleme gibi iþlemleri yapamaz dýþarýdan alanlar.ancak Listedeki elemanlara eriþebilir,Listenin uzunluðunu alabilir.
    //IReadOnlyList, dýþarýya yalnýzca okuma izni vererek veri güvenliðini artýrýr.
    //readonly List yalnýzca referansý korurken, dýþ dünyadan listenin içeriði deðiþtirilebilir.
    public async Task CreateSlots(List<Transform> panels, Dictionary<string, int> panelSlotCounts, Queue<(RewardData reward, int amount)> rewardQueue)
    {
        slots.Clear(); //slot varsa temizle.

        int slotCounter = 0;
        foreach (var panel in panels)  //sýrasýyla tüm paneller içinde dönülür.
        {
            if (panelSlotCounts.TryGetValue(panel.tag, out int slotCount))
            {
                for (int i = 0; i < slotCount; i++)
                {
                    var slotPrefab = await LoadPrefabAsync(slotPrefabReference, panel); //slot prefabý panel childý olarak instantiate edilir.
                    if (slotPrefab)
                    {
                        slotPrefab.name = $"Slot{slotCounter++}"; //slotun ismi üretildikçe numaralandýrýlýr.
                        var slot = slotPrefab.GetComponent<Slot>(); //slotun içindeki Slot scripti alýnýr.

                        
                        var BlueHighlightPrefab = await LoadPrefabAsync(BlueHighlightPrefabReference, slotPrefab.transform); //BlueHighlightPrefab slotPrefab childý olarak instantiate edilir.
                        if (BlueHighlightPrefab)
                        {
                            BlueHighlightPrefab.transform.SetSiblingIndex(0); //childlar arasýnda en üste koy 
                            BlueHighlightPrefab.SetActive(false);             // ilk oluþtuðunda aktif deðil.     
                        }

                        var highlight = await LoadPrefabAsync(highlightPrefabReference, slotPrefab.transform);  //highlight slotPrefab childý olarak instantiate edilir.
                        if (highlight && BlueHighlightPrefab)
                        {     
                            highlight.transform.SetSiblingIndex(1); //highlýghtý 1. indexe yerleþtir.

                            var highlightImage = highlight.GetComponentInChildren<Image>(); //highlightýn image bileþenini al.
                            if (highlightImage)
                            {
                                highlightImage.color = new Color(1, 1, 1, 0);  //highlýghtý baþlangýçta görünmez yap.
                            }
                        }

                        var reward = await LoadPrefabAsync(rewardPrefabReference, slotPrefab.transform); //reward slotPrefab childý olarak instantiate edilir.
                        if (reward)
                        {
                            reward.transform.SetSiblingIndex(2); //highlýghtý 2. indexe yerleþtir.
                        }

                        slot.Initialize(reward, highlight, BlueHighlightPrefab); //slotprefab altýndaki slot scriptine burada üretilen prefablar gönderilir.

                        if (rewardQueue.Count > 0)
                        {
                            var rewardDataWithAmount = rewardQueue.Dequeue(); //reward datadaki karýþtýrýlmýþ rewarddata, int amount bileþeni sondan çýkarýlýr
                            slot.SetReward(rewardDataWithAmount.reward.rewardSprite, rewardDataWithAmount.reward.rewardID, rewardDataWithAmount.amount); //oluþturulan reward prefabýna queueden çýkan elemanýn sprite ý reward id si ve sayýsý gönderilir ve sahnede yerleþtirilir.
                        }

                        slotPrefab.SetActive(false); //slotlar ilk üretildiðinde inaktif yapýlýr.
                        slots.Add(slot); //üretilen slot slot listesine eklenir.
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
        foreach (var slot in slots) //tüm slotlarý döner
        {
            slot.gameObject.SetActive(true); //0.1 saniyede bir her slotu aktif eder sýrayla
            slot.transform.localScale = Vector3.zero; //slotlar baþta boyutsuz olur ve.
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

        //Eðer Prefab daha önce yüklenmemiþse, þu satýr devreye girer:
        var handle = reference.LoadAssetAsync<GameObject>(); //Prefab asenkron olarak yüklenir 
        await handle.Task; //ve iþlem tamamlanana kadar beklenir.

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            return Instantiate(handle.Result, parent);
        }

        Debug.LogError($"Failed to load AssetReference: {reference.RuntimeKey}");
        return null;
    }

    //reference.OperationHandle.IsValid() bu kýsým assetin belleðe daha önce yüklenip yüklenmediðini kontrol eder yani ilk defa oluþuyorsa addressable diskten çekeceði için false ancak belleðe geldikten sonra prefab silinmedikçe bellekte kalacak ve true dönecek.
    //reference.OperationHandle.Status == AsyncOperationStatus.Succeeded bu satýr da yükleme iþlemi baþarýyla yapýlmýþ mý? bunu kontrol eder.
}
