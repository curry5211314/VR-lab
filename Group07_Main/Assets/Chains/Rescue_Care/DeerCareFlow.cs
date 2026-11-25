using Group07.ActionCore;
using Group07.Story;
using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 野生动物救助站 - 内旋护理（体侧内旋链路，左右双侧）。
    /// </summary>
    public class DeerCareFlow : StoryFlowBase
    {
        [SerializeField] private TiCeNActionAdapter actionAdapter;

        public override string ChainId => "Rescue_Care";
        public override string DisplayName => "内旋护理";
        public override int TargetCount => 10; // 左右各 5 个内旋目标

        public override void OnRoundStart(StorySide side, int difficulty)
        {
            if (actionAdapter == null)
            {
                actionAdapter = GetComponent<TiCeNActionAdapter>();
            }

            if (actionAdapter != null)
            {
                actionAdapter.BeginAction(difficulty);
            }
        }
    }
}
