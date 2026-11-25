using UnityEngine;
using UnityEngine.SceneManagement;

namespace Group07.Story
{
    /// <summary>
    /// 总工程入口：主题场景切换与链路选择。
    /// </summary>
    public class StoryAppController : MonoBehaviour
    {
        [SerializeField] private StoryChainCatalog catalog;
        [SerializeField] private StoryChainLoader chainLoader;
        [SerializeField] private string bootstrapSceneName = "Bootstrap";
        [SerializeField] private string rescueSceneName = "Theme_Rescue";
        [SerializeField] private string marketSceneName = "Theme_Market";

        public void LoadTheme(StoryTheme theme)
        {
            string sceneName = theme == StoryTheme.Rescue ? rescueSceneName : marketSceneName;
            SceneManager.LoadScene(sceneName);
        }

        public void LoadBootstrap()
        {
            SceneManager.LoadScene(bootstrapSceneName);
        }

        public void SelectChain(string chainId)
        {
            if (chainLoader == null)
            {
                Debug.LogError("[StoryAppController] 未指定 StoryChainLoader。");
                return;
            }

            chainLoader.Load(chainId);
        }

        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
