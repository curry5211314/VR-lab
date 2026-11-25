using System;
using UnityEngine;
using UnityEngine.UI;

namespace Group07.Story.UI
{
    /// <summary>
    /// 单轮结算面板：星级、命中/失误、用时，并提供重玩与返回按钮。
    /// </summary>
    public class StorySettlementPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text titleText;
        [SerializeField] private Text summaryText;
        [SerializeField] private Image[] starImages;
        [SerializeField] private Sprite starOnSprite;
        [SerializeField] private Sprite starOffSprite;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button backButton;

        public event Action RestartRequested;
        public event Action BackRequested;

        public StoryRoundResult LastResult { get; private set; }
        public bool IsVisible => root != null && root.activeSelf;

        private void Awake()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(() => RestartRequested?.Invoke());
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(() => BackRequested?.Invoke());
            }
        }

        public void Show(StoryRoundResult result)
        {
            LastResult = result;

            if (root != null)
            {
                root.SetActive(true);
            }

            if (titleText != null)
            {
                titleText.text = result.completed ? "完成：" + result.displayName : "未完成：" + result.displayName;
            }

            if (summaryText != null)
            {
                summaryText.text = string.Format(
                    "命中 {0} 次\n失误 {1} 次\n用时 {2:F1} 秒",
                    result.hitCount,
                    result.missCount,
                    result.durationSeconds);
            }

            RefreshStars(result.starRating);
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        private void RefreshStars(int starRating)
        {
            if (starImages == null)
            {
                return;
            }

            for (int i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] == null)
                {
                    continue;
                }

                bool on = i < starRating;
                if (on && starOnSprite != null)
                {
                    starImages[i].sprite = starOnSprite;
                }
                else if (!on && starOffSprite != null)
                {
                    starImages[i].sprite = starOffSprite;
                }
            }
        }
    }
}
