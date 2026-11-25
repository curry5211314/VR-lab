using System;
using UnityEngine;

namespace Group07.Story
{
    public enum StorySide
    {
        Left,
        Right
    }

    public enum StoryTheme
    {
        Rescue,
        Market
    }

    [Serializable]
    public struct StoryHitInfo
    {
        public int index;
        public StorySide side;
        public float time;

        public StoryHitInfo(int index, StorySide side, float time)
        {
            this.index = index;
            this.side = side;
            this.time = time;
        }
    }

    [Serializable]
    public struct StoryRoundResult
    {
        public string chainId;
        public string displayName;
        public int hitCount;
        public int missCount;
        public float durationSeconds;
        public int starRating;
        public bool completed;
    }

    [Serializable]
    public class StoryChainEntry
    {
        public string chainId;
        public string displayName;
        public StoryTheme theme;
        public string actionLabel;
        public GameObject chainPrefab;
        public Sprite icon;

        [TextArea(2, 4)]
        public string description;
    }
}
