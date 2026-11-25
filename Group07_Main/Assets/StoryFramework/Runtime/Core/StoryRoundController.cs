using System;
using UnityEngine;

namespace Group07.Story
{
    /// <summary>
    /// 单轮计数与结算，是动作事件到故事流程的转发中心。
    /// </summary>
    public class StoryRoundController : MonoBehaviour
    {
        [SerializeField] private StoryFlowBase flow;
        [SerializeField] private bool autoCompleteOnTargetCount = true;

        private int hitCount;
        private int missCount;
        private float startedAt;
        private bool running;

        public StoryFlowBase Flow => flow;
        public int HitCount => hitCount;
        public int MissCount => missCount;
        public bool IsRunning => running;
        public int TargetCount => flow != null ? Mathf.Max(1, flow.TargetCount) : 1;

        public event Action<int, int> ProgressChanged;
        public event Action<StoryRoundResult> RoundCompleted;
        public event Action RoundAborted;

        private void OnEnable()
        {
            StoryActionBus.TargetHit += HandleTargetHit;
            StoryActionBus.TargetMiss += HandleTargetMiss;
        }

        private void OnDisable()
        {
            StoryActionBus.TargetHit -= HandleTargetHit;
            StoryActionBus.TargetMiss -= HandleTargetMiss;
        }

        public void SetFlow(StoryFlowBase targetFlow)
        {
            flow = targetFlow;
        }

        public void Begin(StorySide side, int difficulty)
        {
            hitCount = 0;
            missCount = 0;
            startedAt = Time.time;
            running = true;

            flow?.OnRoundStart(side, difficulty);
            ProgressChanged?.Invoke(0, TargetCount);
        }

        private void HandleTargetHit(StoryHitInfo hit)
        {
            if (!running)
            {
                return;
            }

            hitCount++;
            flow?.OnTargetHit(hit);
            ProgressChanged?.Invoke(hitCount, TargetCount);

            if (autoCompleteOnTargetCount && hitCount + missCount >= TargetCount)
            {
                Complete();
            }
        }

        private void HandleTargetMiss(int index)
        {
            if (!running)
            {
                return;
            }

            missCount++;
            flow?.OnTargetMiss(index);

            // 失误同样消耗一个目标：漏掉最后一颗时轮次也能正常结算。
            if (autoCompleteOnTargetCount && hitCount + missCount >= TargetCount)
            {
                Complete();
            }
        }

        public StoryRoundResult Complete()
        {
            if (!running)
            {
                return BuildResult(false);
            }

            running = false;
            StoryRoundResult result = BuildResult(true);
            flow?.OnRoundComplete(result);
            RoundCompleted?.Invoke(result);
            StoryActionBus.RaiseRoundCompleted(result);
            return result;
        }

        public void Abort()
        {
            if (!running)
            {
                return;
            }

            running = false;
            flow?.OnRoundAbort();
            RoundAborted?.Invoke();
        }

        private StoryRoundResult BuildResult(bool completed)
        {
            int stars = 1;
            if (completed)
            {
                stars = missCount == 0 ? 3 : (missCount <= 2 ? 2 : 1);
            }

            return new StoryRoundResult
            {
                chainId = flow != null ? flow.ChainId : string.Empty,
                displayName = flow != null ? flow.DisplayName : string.Empty,
                hitCount = hitCount,
                missCount = missCount,
                durationSeconds = Mathf.Max(0f, Time.time - startedAt),
                starRating = stars,
                completed = completed
            };
        }
    }
}
