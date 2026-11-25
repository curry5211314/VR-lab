using UnityEngine;

namespace Group07.Story
{
    /// <summary>
    /// 每条链路实现一个子类，只关心自己的故事表现，不关心动作如何判定。
    /// </summary>
    public abstract class StoryFlowBase : MonoBehaviour
    {
        public abstract string ChainId { get; }
        public abstract string DisplayName { get; }
        public virtual int TargetCount => 5;

        public virtual void BindContext(StoryContext context)
        {
        }

        public virtual void OnRoundStart(StorySide side, int difficulty)
        {
        }

        public virtual void OnTargetHit(StoryHitInfo hit)
        {
        }

        public virtual void OnTargetMiss(int index)
        {
        }

        public virtual void OnRoundComplete(StoryRoundResult result)
        {
        }

        public virtual void OnRoundAbort()
        {
        }
    }
}
