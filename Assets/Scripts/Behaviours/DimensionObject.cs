using System;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class DimensionObject : MonoBehaviour
    {
        [SerializeField] private Timeline m_TimelineExistance = Timeline.Past | Timeline.Present | Timeline.Future;
        [SerializeField] private bool m_ValidateMovements = true;
        [SerializeField] private bool m_DisableOnWrongTimeline = false;

        private Vector2 m_LastValidPosition;
        
        void Start()
        {
            DimensionManager.Instance.OnDimensionChange?.AddListener(OnDimensionChanged);
            m_LastValidPosition = transform.position;
        }

        private void OnDestroy()
        {
            DimensionManager.Instance.OnDimensionChange?.RemoveListener(OnDimensionChanged);
        }

        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            if(m_ValidateMovements)
                ValidatePosition(from, to);
            if(m_DisableOnWrongTimeline)
                gameObject.SetActive(m_TimelineExistance.HasFlag(to));
        }

        private void ValidatePosition(Timeline from, Timeline to)
        {
            bool valid = (int)from <= (int)to;
            if (valid)
                m_LastValidPosition = transform.position;
            else
                transform.position = m_LastValidPosition;
        }
    }
}
