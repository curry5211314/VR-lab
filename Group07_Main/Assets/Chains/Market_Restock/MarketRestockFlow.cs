using Group07.ActionCore;
using Group07.Story;
using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 超市 - 补货上架（前屈链路）。
    /// </summary>
    public class MarketRestockFlow : StoryFlowBase
    {
        [SerializeField] private QianQuActionAdapter actionAdapter;

        public override string ChainId => "Market_Restock";
        public override string DisplayName => "补货";
        public override int TargetCount => 16; // 左右各 8 个前屈目标

        public override void OnRoundStart(StorySide side, int difficulty)
        {
            if (actionAdapter == null)
            {
                actionAdapter = GetComponent<QianQuActionAdapter>();
            }

            if (actionAdapter != null)
            {
                actionAdapter.BeginAction(difficulty);
            }
        }
    }
}
