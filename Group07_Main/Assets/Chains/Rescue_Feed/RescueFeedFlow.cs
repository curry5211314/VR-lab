using Group07.ActionCore;
using Group07.Story;
using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 野生动物救助站 - 投喂（前屈链路）。
    /// </summary>
    public class RescueFeedFlow : StoryFlowBase
    {
        [SerializeField] private QianQuActionAdapter actionAdapter;

        public override string ChainId => "Rescue_Feed";
        public override string DisplayName => "投喂";
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
