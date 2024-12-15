using System.Collections.Generic;


    public interface IRewardManager
    {
        void InitializeRewardQueue();                       
        Queue<(RewardData reward, int amount)> GetRewardQueue(); 
        void ResetRewards();                                
    }
