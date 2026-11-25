using System.IO;
using UnityEditor;
using UnityEngine;
using Group07.Story;

namespace Group07.EditorTools
{
    public static class StoryFrameworkMenu
    {
        private const string CatalogPath = "Assets/StoryFramework/StoryChainCatalog.asset";

        [MenuItem("Group07/公共框架/创建链路目录")]
        public static void CreateCatalog()
        {
            StoryChainCatalog existing = AssetDatabase.LoadAssetAtPath<StoryChainCatalog>(CatalogPath);
            if (existing != null)
            {
                Selection.activeObject = existing;
                return;
            }

            string directory = Path.GetDirectoryName(CatalogPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }

            StoryChainCatalog catalog = ScriptableObject.CreateInstance<StoryChainCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
            AssetDatabase.SaveAssets();
            Selection.activeObject = catalog;
        }
    }
}
