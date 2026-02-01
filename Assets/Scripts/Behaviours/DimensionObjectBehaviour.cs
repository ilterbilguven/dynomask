using System;
using System.Collections.Generic;
using Game.Behaviours;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class DimensionObjectBehaviour : MonoBehaviour
    {
        [SerializeField] private Timeline m_TimelineExistance = Timeline.Past | Timeline.Present | Timeline.Future;

        private Vector2 m_LastValidPosition;
        
        private Dictionary<Timeline, Vector2> m_TimelinePositions; // <Timeline, Position>

        [SerializeField] private ObstacleBehaviour m_ObstacleBehaviour;

        [SerializeField] private bool m_UpdatePosition = true;
        
        private void Awake()
        {
            DimensionManager.Instance.OnDimensionChange?.AddListener(OnDimensionChanged);
            gameObject.SetActive(m_TimelineExistance.HasFlag(DimensionManager.Instance.CurrentDimension));
            m_TimelinePositions = new()
            {
                { Timeline.Past, transform.position },
                { Timeline.Present, transform.position },
                { Timeline.Future, transform.position }
            };
        }

        void Start()
        {
            m_LastValidPosition = transform.position;
        }

        private void OnDestroy()
        {
            DimensionManager.Instance.OnDimensionChange?.RemoveListener(OnDimensionChanged);
        }

        private void Update()
        {
            if (!m_UpdatePosition) return;
            switch (DimensionManager.Instance.CurrentDimension)
            {
                case Timeline.Past:
                    if (m_TimelineExistance.HasFlag(Timeline.Past)) m_TimelinePositions[Timeline.Past] = transform.position;
                    if (m_TimelineExistance.HasFlag(Timeline.Present)) m_TimelinePositions[Timeline.Present] = transform.position;
                    if (m_TimelineExistance.HasFlag(Timeline.Future)) m_TimelinePositions[Timeline.Future] = transform.position;
                    break;
                case Timeline.Present:
                    if (m_TimelineExistance.HasFlag(Timeline.Present)) m_TimelinePositions[Timeline.Present] = transform.position;
                    if (m_TimelineExistance.HasFlag(Timeline.Future)) m_TimelinePositions[Timeline.Future] = transform.position;
                    break;
                case Timeline.Future:
                    if (m_TimelineExistance.HasFlag(Timeline.Future)) m_TimelinePositions[Timeline.Future] = transform.position;
                    break;
            }
        }

        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            gameObject.SetActive(m_TimelineExistance.HasFlag(to));
            if (!m_UpdatePosition) return;
            transform.position = m_TimelinePositions[to];
            if (m_ObstacleBehaviour)
            {
                m_ObstacleBehaviour.ResetDestination(transform.position);
            }
        }
    }
}
