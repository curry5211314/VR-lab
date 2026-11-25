using UnityEngine;

namespace Group07.Story.UI
{
    /// <summary>
    /// 启动场景的主题选择入口，按钮只需要绑定这两个方法。
    /// </summary>
    public class StoryThemeMenu : MonoBehaviour
    {
        [SerializeField] private StoryAppController appController;

        public void SetController(StoryAppController controller)
        {
            appController = controller;
        }

        public void SelectRescue()
        {
            OpenTheme(StoryTheme.Rescue);
        }

        public void SelectMarket()
        {
            OpenTheme(StoryTheme.Market);
        }

        private void OpenTheme(StoryTheme theme)
        {
            if (appController == null)
            {
                Debug.LogError("[StoryThemeMenu] 未指定 StoryAppController。");
                return;
            }

            appController.LoadTheme(theme);
        }
    }
}
