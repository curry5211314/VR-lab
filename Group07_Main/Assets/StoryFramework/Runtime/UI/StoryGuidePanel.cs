using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Group07.Story.UI
{
    /// <summary>
    /// 链路可选实现：由链路提供自己的分步引导文案，未实现时链路菜单使用默认两页。
    /// </summary>
    public interface IStoryGuideProvider
    {
        List<StoryGuidePanel.GuideStep> BuildGuideSteps();
    }

    /// <summary>
    /// 动作开始前的分步引导面板，由主题场景复用，链路只提供文案。
    /// </summary>
    public class StoryGuidePanel : MonoBehaviour
    {
        [Serializable]
        public class GuideStep
        {
            public string title;

            [TextArea(2, 5)]
            public string body;

            // 可选：该步骤的语音，未配置时保持纯文字引导
            public AudioClip voice;
        }

        [SerializeField] private GameObject root;
        [SerializeField] private Text titleText;
        [SerializeField] private Text bodyText;
        [SerializeField] private Text pageText;
        [SerializeField] private Button prevButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button startButton;
        [SerializeField] private List<GuideStep> steps = new List<GuideStep>();
        [SerializeField] private AudioSource voiceSource;

        private int stepIndex;

        public event Action StartRequested;

        public int StepIndex => stepIndex;
        public int StepCount => steps.Count;

        private void Awake()
        {
            if (prevButton != null)
            {
                prevButton.onClick.AddListener(Prev);
            }

            if (nextButton != null)
            {
                nextButton.onClick.AddListener(Next);
            }

            if (startButton != null)
            {
                startButton.onClick.AddListener(OnStartClicked);
            }

            if (voiceSource == null)
            {
                voiceSource = GetComponent<AudioSource>();
            }

            if (voiceSource == null)
            {
                voiceSource = gameObject.AddComponent<AudioSource>();
            }

            voiceSource.playOnAwake = false;
            voiceSource.spatialBlend = 0f;

            Refresh();
        }

        public void Show()
        {
            if (root != null)
            {
                root.SetActive(true);
            }

            stepIndex = 0;
            Refresh();
        }

        public void Hide()
        {
            if (voiceSource != null)
            {
                voiceSource.Stop();
            }

            if (root != null)
            {
                root.SetActive(false);
            }
        }

        public void SetSteps(List<GuideStep> newSteps)
        {
            steps = newSteps ?? new List<GuideStep>();
            stepIndex = 0;
            Refresh();
        }

        public void Next()
        {
            if (stepIndex >= steps.Count - 1)
            {
                return;
            }

            stepIndex++;
            Refresh();
        }

        public void Prev()
        {
            if (stepIndex <= 0)
            {
                return;
            }

            stepIndex--;
            Refresh();
        }

        private void OnStartClicked()
        {
            Hide();
            StartRequested?.Invoke();
        }

        private void Refresh()
        {
            bool hasSteps = steps.Count > 0;
            int safeIndex = hasSteps ? Mathf.Clamp(stepIndex, 0, steps.Count - 1) : 0;

            if (titleText != null)
            {
                titleText.text = hasSteps ? steps[safeIndex].title : string.Empty;
            }

            if (bodyText != null)
            {
                bodyText.text = hasSteps ? steps[safeIndex].body : string.Empty;
            }

            if (pageText != null)
            {
                pageText.text = hasSteps ? (safeIndex + 1) + " / " + steps.Count : string.Empty;
            }

            if (prevButton != null)
            {
                prevButton.interactable = hasSteps && safeIndex > 0;
            }

            if (nextButton != null)
            {
                nextButton.interactable = hasSteps && safeIndex < steps.Count - 1;
            }

            if (startButton != null)
            {
                startButton.interactable = !hasSteps || safeIndex >= steps.Count - 1;
            }

            bool panelVisible = root == null || root.activeInHierarchy;
            PlayStepVoice(panelVisible && hasSteps ? steps[safeIndex].voice : null);
        }

        /// <summary>切换步骤时播放该步骤语音；未配置语音时保持静音。</summary>
        private void PlayStepVoice(AudioClip clip)
        {
            if (voiceSource == null)
            {
                return;
            }

            voiceSource.Stop();

            if (clip != null)
            {
                voiceSource.clip = clip;
                voiceSource.Play();
            }
        }
    }
}
