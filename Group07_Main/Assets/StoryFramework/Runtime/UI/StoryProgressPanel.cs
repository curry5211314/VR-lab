using UnityEngine;
using UnityEngine.UI;

namespace Group07.Story.UI
{
    /// <summary>
    /// 游戏中的计数与阶段显示，只读取公共计数层，不关心具体动作。
    /// </summary>
    public class StoryProgressPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text countText;
        [SerializeField] private Text stageText;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private StoryRoundController roundController;
        [SerializeField] private StoryStateController stateController;

        private void OnEnable()
        {
            if (roundController != null)
            {
                roundController.ProgressChanged += HandleProgressChanged;
            }

            if (stateController != null)
            {
                stateController.StageChanged += HandleStageChanged;
            }
        }

        private void OnDisable()
        {
            if (roundController != null)
            {
                roundController.ProgressChanged -= HandleProgressChanged;
            }

            if (stateController != null)
            {
                stateController.StageChanged -= HandleStageChanged;
            }
        }

        public void SetVisible(bool visible)
        {
            if (root != null)
            {
                root.SetActive(visible);
            }
        }

        private void HandleProgressChanged(int current, int target)
        {
            int safeTarget = Mathf.Max(1, target);

            if (countText != null)
            {
                countText.text = current + " / " + safeTarget;
            }

            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = safeTarget;
                progressSlider.value = Mathf.Clamp(current, 0, safeTarget);
            }
        }

        private void HandleStageChanged(int stage)
        {
            if (stageText != null)
            {
                stageText.text = "阶段 " + stage;
            }
        }
    }
}
