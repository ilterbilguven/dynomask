using System;
using System.Collections.Generic;
using Game.Behaviours;
using Game.Managers;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class DimensionObjectBehaviour : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void ResetContext()
        {
            m_TimelineCache.Clear();
        }
        
        [SerializeField] private Timeline m_TimelineExistance = Timeline.Past | Timeline.Present | Timeline.Future;

        private Dictionary<Timeline, Vector2> m_TimelinePositions; // <Timeline, Position>

        [SerializeField] private ObstacleBehaviour m_ObstacleBehaviour;

        [SerializeField] private bool m_UpdatePosition = true;
        
        private static Dictionary<string, Dictionary<Timeline, Vector2>> m_TimelineCache = new();

        [field: ReadOnly, SerializeField] public string id { get; private set; }

        [Button]
        private void GenerateID()
        {
            id = Guid.NewGuid().ToString();
        }

        private void Reset()
        {
            GenerateID();
        }
        
        private void Awake()
        {
            DimensionManager.Instance.OnDimensionChange?.AddListener(OnDimensionChanged);
            gameObject.SetActive(m_TimelineExistance.HasFlag(DimensionManager.Instance.CurrentDimension));
            
            if (!m_UpdatePosition) return;
            m_TimelinePositions = new();
            if (m_TimelineExistance.HasFlag(Timeline.Past)) m_TimelinePositions.Add(Timeline.Past, transform.position);
            if (m_TimelineExistance.HasFlag(Timeline.Present)) m_TimelinePositions.Add(Timeline.Present, transform.position);
            if (m_TimelineExistance.HasFlag(Timeline.Future)) m_TimelinePositions.Add(Timeline.Future, transform.position);

            if (m_TimelineCache.TryGetValue(id, out var positions))
            {
                m_TimelinePositions = positions;
                transform.position = m_TimelinePositions[Timeline.Present];
                
                if (m_ObstacleBehaviour) m_ObstacleBehaviour.ResetDestination(transform.position);
            }
            else
            {
                m_TimelineCache[id] = m_TimelinePositions;
            }
        }

        private void OnDestroy()
        {
            DimensionManager.Instance.OnDimensionChange?.RemoveListener(OnDimensionChanged);
        }

        private void Update()
        {
            UpdateDimensions();
        }

        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            gameObject.SetActive(m_TimelineExistance.HasFlag(to));
            if (!m_UpdatePosition) return;
            if (!m_TimelineExistance.HasFlag(to)) return;
            
            var destination = m_TimelinePositions[to];

            if (GameManager.Instance.CheckIfPlayerExists(destination))
            {
                var e = new Exception("Player is in the way of the dimension transition");
                Debug.LogException(e, this);
                throw e;
            }
            
            transform.position = destination;

            if (m_ObstacleBehaviour)
            {
                m_ObstacleBehaviour.ResetDestination(transform.position);
            }
        }

        private void UpdateDimensions()
        {
            if (!m_UpdatePosition) return;
            
            switch (DimensionManager.Instance.CurrentDimension)
            {
                case Timeline.Past:
                    if (m_TimelineExistance.HasFlag(Timeline.Past))
                        m_TimelinePositions[Timeline.Past] = transform.position;
                    if (m_TimelineExistance.HasFlag(Timeline.Present))
                        m_TimelinePositions[Timeline.Present] = transform.position;
                    if (m_TimelineExistance.HasFlag(Timeline.Future))
                        m_TimelinePositions[Timeline.Future] = transform.position;
                    break;
                case Timeline.Present:
                    if (m_TimelineExistance.HasFlag(Timeline.Present))
                        m_TimelinePositions[Timeline.Present] = transform.position;
                    if (m_TimelineExistance.HasFlag(Timeline.Future))
                        m_TimelinePositions[Timeline.Future] = transform.position;
                    break;
                case Timeline.Future:
                    if (m_TimelineExistance.HasFlag(Timeline.Future))
                        m_TimelinePositions[Timeline.Future] = transform.position;
                    break;
            }
            
            m_TimelineCache[id] = m_TimelinePositions;
        }

        private void OnDrawGizmos()
        {
            if (!m_UpdatePosition) return;
            if (!m_TimelineExistance.HasFlag(DimensionManager.Instance.CurrentDimension)) return;
            
            Gizmos.color = Color.tomato;
            
            Gizmos.DrawWireCube(m_TimelinePositions[DimensionManager.Instance.CurrentDimension], Vector3.one * 0.5f);
        }
    }
}
