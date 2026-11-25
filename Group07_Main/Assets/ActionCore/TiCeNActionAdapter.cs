using Group07.Story;
using UnityEngine;

namespace Group07.ActionCore
{
    /// <summary>
    /// 体侧内旋动作适配器：同时生成左右两侧内旋目标（每侧 5 个，共 10 个）。
    /// 命中与失误都由 Star → StoryActionBus 上报，适配器不参与碰撞判定。
    /// </summary>
    public class TiCeNActionAdapter : ActionAdapterBase
    {
        [SerializeField] private int endIndex = 4;

        public override ActionAnim Action => ActionAnim.TiCeNXuan_Both;

        // 双侧链路沿用 Left 作为轮次侧别占位，实际命中侧别以 StoryHitInfo.side 为准。
        public override StorySide Side => StorySide.Left;

        protected override void OnBeforeBegin(int difficulty)
        {
            MainControl main = MainControl._instance;
            if (main == null)
            {
                Debug.LogError("[TiCeNActionAdapter] 场景中没有 MainControl。");
                return;
            }

            if (main.starCreater_L_tcNx == null || main.starCreater_R_tcNx == null)
            {
                Debug.LogError("[TiCeNActionAdapter] MainControl 未同时绑定左右内旋生成器。");
                return;
            }

            ClearPreviousTargets(main);
            main.OnSceneInit(ActionAnim.TiCeNXuan_Both, difficulty, endIndex, null, string.Empty, 0);
            main.OnPosCreater();
        }
    }
}
