using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PanelConfig", menuName = "ScriptableObjects/PanelConfig", order = 50)]
public class PanelConfig : ScriptableObject
{
    [SerializeField] private int topPanelSlots = 4;
    [SerializeField] private int bottomPanelSlots = 4;
    [SerializeField] private int rightPanelSlots = 3;
    [SerializeField] private int leftPanelSlots = 3;

    public Dictionary<string, int> GetPanelSlotCounts()
    {
        return new Dictionary<string, int>
        {
            { "TopPanel", topPanelSlots },
            { "BottomPanel", bottomPanelSlots },
            { "RightPanel", rightPanelSlots },
            { "LeftPanel", leftPanelSlots }
        };
    }
}
