using System;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class LevelBehaviour : MonoBehaviour
    {
        public TimelineGameObjectDictionary Timelines;

        private void Awake()
        {
            LevelManager.Instance.LocateLevel(this);
            
            //DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChanged);
        }

        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            Timelines[from].SetActive(false);
            Timelines[to].SetActive(true);
        }

        [Serializable]
        public class TimelineGameObjectDictionary : SerializableDictionary<Timeline, GameObject> { }
    }
}
