using System;
using System.Collections.Generic;
using Game.Managers;
using UnityEngine;

namespace Game.UI
{
    public class EndPanelBehaviour : UIPanelBehaviour
    {
        [SerializeField] private TimelineGameObjectDictionary _dictionary;
        
        private void Start()
        {
            DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChanged);
            
            OnDimensionChanged(0, DimensionManager.Instance.CurrentDimension);
        }

        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            foreach (var dictionaryKey in _dictionary.Keys)
            {
                foreach (var go in _dictionary[dictionaryKey])
                {
                    go.SetActive(dictionaryKey == to);
                }
            }
        }
        
        [Serializable] public class TimelineGameObjectDictionary : SerializableDictionary<Timeline, List<GameObject>, GameObjectListStorage> { }

        [Serializable]
        public class GameObjectListStorage : SerializableDictionary.Storage<List<GameObject>> {}
    }
}