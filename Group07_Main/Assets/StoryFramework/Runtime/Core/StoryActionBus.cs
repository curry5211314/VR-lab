using System;
using UnityEngine;

namespace Group07.Story
{
    /// <summary>
    /// 动作包与故事层之间的唯一事件入口。
    /// 动作脚本只负责 Raise，不依赖任何故事层类型。
    /// </summary>
    public static class StoryActionBus
    {
        public static event Action<StoryHitInfo> TargetHit;
        public static event Action<int> TargetMiss;
        public static event Action<int> StageChanged;
        public static event Action<StoryRoundResult> RoundCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay()
        {
            ClearAll();
        }

        public static void RaiseTargetHit(int index, bool isLeft)
        {
            StorySide side = isLeft ? StorySide.Left : StorySide.Right;
            RaiseTargetHit(new StoryHitInfo(index, side, Time.time));
        }

        public static void RaiseTargetHit(StoryHitInfo hit)
        {
            TargetHit?.Invoke(hit);
        }

        public static void RaiseTargetMiss(int index)
        {
            TargetMiss?.Invoke(index);
        }

        public static void RaiseStageChanged(int stage)
        {
            StageChanged?.Invoke(stage);
        }

        public static void RaiseRoundCompleted(StoryRoundResult result)
        {
            RoundCompleted?.Invoke(result);
        }

        public static void ClearAll()
        {
            TargetHit = null;
            TargetMiss = null;
            StageChanged = null;
            RoundCompleted = null;
        }
    }
}
