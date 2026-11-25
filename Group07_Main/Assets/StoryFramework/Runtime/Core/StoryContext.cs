using UnityEngine;

namespace Group07.Story
{
    /// <summary>
    /// 主题场景向链路 prefab 注入外部引用，避免链路直接 Find 场景对象。
    /// </summary>
    public class StoryContext : MonoBehaviour
    {
        public StoryTheme theme;
        public Camera mainCamera;
        public Transform leftHand;
        public Transform rightHand;
        public Transform bodyRoot;
        public Transform chainMount;
        public Transform uiRoot;
        public AudioSource audioRoot;
    }
}
