using Group07.Story;
using UnityEngine;

namespace Group07.ActionCore
{
    /// <summary>
    /// 动作包适配层：把“体侧内旋 / 前屈”包装成故事层可用的一次动作。
    /// 只负责启动一轮计数会话，碰撞判定仍然在 Star.cs 里完成。
    /// </summary>
    public abstract class ActionAdapterBase : MonoBehaviour
    {
        [SerializeField] protected StoryRoundController roundController;

        public abstract ActionAnim Action { get; }
        public abstract StorySide Side { get; }

        /// <summary>由故事链路在轮次开始时调用：只启动动作，不重复启动计数会话。</summary>
        public void BeginAction(int difficulty)
        {
            ConfigureMissDetection(difficulty);
            OnBeforeBegin(difficulty);
        }

        public virtual void BeginRound(int difficulty)
        {
            if (roundController == null)
            {
                Debug.LogError("[ActionAdapter] 未指定 StoryRoundController。");
                return;
            }

            ConfigureMissDetection(difficulty);
            OnBeforeBegin(difficulty);
            roundController.Begin(Side, difficulty);
        }

        public virtual void AbortRound()
        {
            if (roundController != null)
            {
                roundController.Abort();
            }
        }

        protected virtual void OnBeforeBegin(int difficulty)
        {
        }

        protected virtual void OnDestroy()
        {
            // 离开故事链路后恢复默认行为，避免影响原演示场景。
            Star.EnableMissDetection = false;
        }

        /// <summary>重开一轮前清掉上一轮残留的目标，避免星星叠加。</summary>
        protected static void ClearPreviousTargets(MainControl main)
        {
            for (int i = 0; i < main.StarLs.Count; i++)
            {
                if (main.StarLs[i] != null)
                {
                    Destroy(main.StarLs[i].gameObject);
                }
            }

            for (int i = 0; i < main.StarRs.Count; i++)
            {
                if (main.StarRs[i] != null)
                {
                    Destroy(main.StarRs[i].gameObject);
                }
            }

            main.StarLs.Clear();
            main.StarRs.Clear();
        }

        /// <summary>开启超时失误判定，难度越高留给玩家的反应时间越短。</summary>
        private static void ConfigureMissDetection(int difficulty)
        {
            Star.EnableMissDetection = true;
            switch (difficulty)
            {
                case 0:
                    Star.MissTimeoutSeconds = 8f;
                    break;
                case 2:
                    Star.MissTimeoutSeconds = 4f;
                    break;
                default:
                    Star.MissTimeoutSeconds = 6f;
                    break;
            }
        }
    }
}
