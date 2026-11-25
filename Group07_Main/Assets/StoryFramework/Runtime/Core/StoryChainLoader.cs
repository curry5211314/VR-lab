using UnityEngine;

namespace Group07.Story
{
    /// <summary>
    /// 按 chainId 从 Catalog 实例化链路 prefab，并绑定到当前主题场景。
    /// </summary>
    public class StoryChainLoader : MonoBehaviour
    {
        [SerializeField] private StoryContext context;
        [SerializeField] private StoryChainCatalog catalog;
        [SerializeField] private StoryRoundController roundController;

        private GameObject currentInstance;

        public StoryFlowBase Current { get; private set; }
        public StoryChainCatalog Catalog => catalog;

        public StoryFlowBase Load(string chainId)
        {
            Unload();

            if (catalog == null)
            {
                Debug.LogError("[StoryChainLoader] 未指定 StoryChainCatalog。");
                return null;
            }

            StoryChainEntry entry = catalog.Find(chainId);
            if (entry == null || entry.chainPrefab == null)
            {
                Debug.LogError("[StoryChainLoader] 找不到链路或 prefab：" + chainId);
                return null;
            }

            Transform parent = context != null && context.chainMount != null ? context.chainMount : transform;
            currentInstance = Instantiate(entry.chainPrefab, parent);

            Current = currentInstance.GetComponentInChildren<StoryFlowBase>(true);
            if (Current == null)
            {
                Debug.LogError("[StoryChainLoader] prefab 中没有 StoryFlowBase：" + chainId);
                Destroy(currentInstance);
                currentInstance = null;
                return null;
            }

            Current.BindContext(context);

            if (roundController != null)
            {
                roundController.SetFlow(Current);
            }

            return Current;
        }

        public void Unload()
        {
            if (currentInstance != null)
            {
                Destroy(currentInstance);
                currentInstance = null;
            }

            Current = null;

            if (roundController != null)
            {
                roundController.SetFlow(null);
            }
        }

        public void SetCatalog(StoryChainCatalog value)
        {
            catalog = value;
        }
    }
}
