using System.Collections.Generic;
using UnityEngine;

namespace Group07.Story
{
    [CreateAssetMenu(menuName = "Group07/Story Chain Catalog", fileName = "StoryChainCatalog")]
    public class StoryChainCatalog : ScriptableObject
    {
        [SerializeField] private List<StoryChainEntry> chains = new List<StoryChainEntry>();

        public IReadOnlyList<StoryChainEntry> Chains => chains;

        public List<StoryChainEntry> GetChains(StoryTheme theme)
        {
            List<StoryChainEntry> result = new List<StoryChainEntry>();
            for (int i = 0; i < chains.Count; i++)
            {
                if (chains[i] != null && chains[i].theme == theme)
                {
                    result.Add(chains[i]);
                }
            }

            return result;
        }

        public StoryChainEntry Find(string chainId)
        {
            for (int i = 0; i < chains.Count; i++)
            {
                if (chains[i] != null && chains[i].chainId == chainId)
                {
                    return chains[i];
                }
            }

            return null;
        }
    }
}
