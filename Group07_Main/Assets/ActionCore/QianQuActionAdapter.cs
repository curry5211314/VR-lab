using Group07.Story;
using UnityEngine;

namespace Group07.ActionCore
{
    /// <summary>
    /// 前屈动作适配器：启动 MainControl 的前屈目标生成，计数仍由 Star → StoryActionBus 完成。
    /// 前屈为双侧目标，这里沿用 Left 作为轮次侧别占位，命中侧别以 StarHitInfo.side 为准。
    /// </summary>
    public class QianQuActionAdapter : ActionAdapterBase
    {
        [SerializeField] private int endIndex = 7;

        public override ActionAnim Action => ActionAnim.QianQu;
        public override StorySide Side => StorySide.Left;

        protected override void OnBeforeBegin(int difficulty)
        {
            MainControl main = MainControl._instance;
            if (main == null)
            {
                Debug.LogError("[QianQuActionAdapter] 场景中没有 MainControl。");
                return;
            }

            ClearPreviousTargets(main);
            main.OnSceneInit(ActionAnim.QianQu, difficulty, endIndex, null, string.Empty, 0);
            main.OnPosCreater();
        }
    }
}
