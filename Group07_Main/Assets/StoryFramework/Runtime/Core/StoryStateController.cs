using System;
using UnityEngine;

namespace Group07.Story
{
    /// <summary>
    /// 管理故事阶段（例如：脏污 -> 清洁中 -> 基本恢复 -> 完成）。
    /// </summary>
    public class StoryStateController : MonoBehaviour
    {
        [SerializeField] private int maxStage = 3;
        [SerializeField] private bool raiseBusEvent = true;

        public int CurrentStage { get; private set; }
        public int MaxStage => Mathf.Max(0, maxStage);

        public event Action<int> StageChanged;

        private void Awake()
        {
            CurrentStage = 0;
        }

        public void ResetToStart()
        {
            CurrentStage = -1;
            SetStage(0);
        }

        public void SetStage(int stage)
        {
            int clamped = Mathf.Clamp(stage, 0, MaxStage);
            if (clamped == CurrentStage)
            {
                return;
            }

            CurrentStage = clamped;
            StageChanged?.Invoke(CurrentStage);

            if (raiseBusEvent)
            {
                StoryActionBus.RaiseStageChanged(CurrentStage);
            }
        }

        public void Advance()
        {
            SetStage(CurrentStage + 1);
        }
    }
}
