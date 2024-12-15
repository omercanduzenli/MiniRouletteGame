using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


    public interface ISlotManager
    {
        void ResetSlots(Queue<(RewardData reward, int amount)> rewardQueue);
        void StartSlotActivationAnimation();                                 
        int SelectFinalSlotForSpin();                                      
        IReadOnlyList<Slot> Slots { get; }                                 
        Task CreateSlots(List<Transform> panels, Dictionary<string, int> panelSlotCounts, Queue<(RewardData reward, int amount)> rewardQueue); 
    }
