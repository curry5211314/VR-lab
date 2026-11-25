using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Group07.Story.UI
{
    /// <summary>
    /// 主题场景内的链路选择入口：按当前主题从 Catalog 取链路，选中后加载 prefab 并进入引导。
    /// </summary>
    public class StoryChainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private StoryContext context;
        [SerializeField] private StoryChainCatalog catalog;
        [SerializeField] private StoryChainLoader chainLoader;
        [SerializeField] private StoryHudBinder hudBinder;
        [SerializeField] private StoryGuidePanel guidePanel;
        [SerializeField] private Button[] chainButtons;
        [SerializeField] private Text[] chainLabels;

        private readonly List<StoryChainEntry> entries = new List<StoryChainEntry>();

        private void Awake()
        {
            if (chainButtons == null)
            {
                return;
            }

            for (int i = 0; i < chainButtons.Length; i++)
            {
                int index = i;
                if (chainButtons[i] != null)
                {
                    chainButtons[i].onClick.AddListener(() => Select(index));
                }
            }
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void ShowMenu()
        {
            if (root != null)
            {
                root.SetActive(true);
            }

            Refresh();
        }

        public void Hide()
        {
            if (root != null)
            {
                root.SetActive(false);
            }
        }

        /// <summary>返回 Bootstrap 主题选择。</summary>
        public void BackToThemes()
        {
            SceneManager.LoadScene("Bootstrap");
        }

        private void Refresh()
        {
            entries.Clear();

            if (catalog != null && context != null)
            {
                List<StoryChainEntry> themeChains = catalog.GetChains(context.theme);
                for (int i = 0; i < themeChains.Count; i++)
                {
                    if (themeChains[i].chainPrefab != null)
                    {
                        entries.Add(themeChains[i]);
                    }
                }
            }

            if (chainButtons == null)
            {
                return;
            }

            for (int i = 0; i < chainButtons.Length; i++)
            {
                bool hasEntry = i < entries.Count;
                if (chainButtons[i] != null)
                {
                    chainButtons[i].gameObject.SetActive(hasEntry);
                }

                if (hasEntry && chainLabels != null && i < chainLabels.Length && chainLabels[i] != null)
                {
                    chainLabels[i].text = entries[i].displayName + "\n<size=24>" + entries[i].actionLabel + "</size>";
                }
            }
        }

        private void Select(int index)
        {
            if (index < 0 || index >= entries.Count)
            {
                return;
            }

            StoryChainEntry entry = entries[index];
            StoryFlowBase flow = null;
            if (chainLoader != null)
            {
                flow = chainLoader.Load(entry.chainId);
            }

            if (flow == null)
            {
                Debug.LogError("[StoryChainMenu] 链路加载失败：" + entry.chainId);
                return;
            }

            if (guidePanel != null)
            {
                guidePanel.SetSteps(new List<StoryGuidePanel.GuideStep>
                {
                    new StoryGuidePanel.GuideStep { title = entry.displayName, body = entry.description },
                    new StoryGuidePanel.GuideStep { title = "操作提示", body = BuildHint(entry.actionLabel) }
                });
            }

            Hide();

            if (hudBinder != null)
            {
                hudBinder.ShowGuide();
            }
        }

        private static string BuildHint(string actionLabel)
        {
            if (actionLabel == "前屈")
            {
                return "站到场地中央，身体前屈，让双手依次触碰身前出现的目标星星。";
            }

            if (actionLabel == "体侧内旋")
            {
                return "抬起手臂，从体侧向内旋转，依次触碰身体两侧出现的目标。";
            }

            return "按画面提示完成动作。";
        }
    }
}
