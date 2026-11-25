using UnityEngine;

namespace Group07.Story.UI
{
    /// <summary>
    /// 主题场景里的 UI 总装配：把引导、进度、结算与计数层串起来。
    /// 链路 prefab 不需要认识任何 UI。
    /// </summary>
    public class StoryHudBinder : MonoBehaviour
    {
        [SerializeField] private StoryGuidePanel guidePanel;
        [SerializeField] private StoryProgressPanel progressPanel;
        [SerializeField] private StorySettlementPanel settlementPanel;
        [SerializeField] private StoryRoundController roundController;
        [SerializeField] private StoryChainMenu chainMenu;
        [SerializeField] private StorySide defaultSide = StorySide.Right;
        [SerializeField] private int defaultDifficulty = 1;

        private StorySide lastSide;
        private int lastDifficulty;

        private void Awake()
        {
            lastSide = defaultSide;
            lastDifficulty = defaultDifficulty;

            if (guidePanel != null)
            {
                guidePanel.StartRequested += StartRound;
            }

            if (roundController != null)
            {
                roundController.RoundCompleted += HandleRoundCompleted;
            }

            if (settlementPanel != null)
            {
                settlementPanel.RestartRequested += RestartRound;
                settlementPanel.BackRequested += BackToGuide;
            }

            if (progressPanel != null)
            {
                progressPanel.SetVisible(false);
            }

            if (settlementPanel != null)
            {
                settlementPanel.Hide();
            }
        }

        private void OnDestroy()
        {
            if (guidePanel != null)
            {
                guidePanel.StartRequested -= StartRound;
            }

            if (roundController != null)
            {
                roundController.RoundCompleted -= HandleRoundCompleted;
            }

            if (settlementPanel != null)
            {
                settlementPanel.RestartRequested -= RestartRound;
                settlementPanel.BackRequested -= BackToGuide;
            }
        }

        public void ShowGuide()
        {
            if (progressPanel != null)
            {
                progressPanel.SetVisible(false);
            }

            if (settlementPanel != null)
            {
                settlementPanel.Hide();
            }

            if (guidePanel != null)
            {
                guidePanel.Show();
            }
        }

        public void BeginWithSide(StorySide side, int difficulty)
        {
            lastSide = side;
            lastDifficulty = difficulty;
            StartRound();
        }

        private void StartRound()
        {
            if (roundController == null)
            {
                Debug.LogError("[StoryHudBinder] 未指定 StoryRoundController。");
                return;
            }

            if (settlementPanel != null)
            {
                settlementPanel.Hide();
            }

            if (guidePanel != null)
            {
                guidePanel.Hide();
            }

            if (progressPanel != null)
            {
                progressPanel.SetVisible(true);
            }

            roundController.Begin(lastSide, lastDifficulty);
        }

        private void RestartRound()
        {
            StartRound();
        }

        private void BackToGuide()
        {
            if (progressPanel != null)
            {
                progressPanel.SetVisible(false);
            }

            if (settlementPanel != null)
            {
                settlementPanel.Hide();
            }

            if (chainMenu != null)
            {
                chainMenu.ShowMenu();
                return;
            }

            ShowGuide();
        }

        private void HandleRoundCompleted(StoryRoundResult result)
        {
            if (progressPanel != null)
            {
                progressPanel.SetVisible(false);
            }

            if (settlementPanel != null)
            {
                settlementPanel.Show(result);
            }
        }
    }
}
