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
        public static void ResetContext()
        {
            m_TimelinePositionCache.Clear();
            m_TimelineExistenceCache.Clear();
            m_TimelineInitialValues.Clear();
        }
        
        [SerializeField] private Timeline m_TimelineExistance = Timeline.Past | Timeline.Present | Timeline.Future;

        private Dictionary<Timeline, Vector2> m_TimelinePositions; // <Timeline, Position>

        [SerializeField] private ObstacleBehaviour m_ObstacleBehaviour;

        [SerializeField] private bool m_UpdatePosition = true;
        
        private static Dictionary<string, Dictionary<Timeline, Vector2>> m_TimelinePositionCache = new();
        private static Dictionary<int, Dictionary<string, (Timeline timeline, string tag)>> m_TimelineExistenceCache = new();
        private static Dictionary<string, Timeline> m_TimelineInitialValues = new();
        
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

            if (!string.IsNullOrEmpty(id))
            {
                m_TimelineInitialValues.TryAdd(id, m_TimelineExistance);
                
                if (m_TimelineExistenceCache.TryGetValue(LevelManager.Instance.CurrentSceneIndex, out var timelines))
                {
                    if (timelines.TryGetValue(id, out var existence))
                    {
                        m_TimelineExistance = existence.timeline;

                    }
                    else
                    {
                        timelines[id] = (m_TimelineExistance, gameObject.tag);
                    }
                }
                else
                {
                    m_TimelineExistenceCache[LevelManager.Instance.CurrentSceneIndex] = new();
                    m_TimelineExistenceCache[LevelManager.Instance.CurrentSceneIndex][id] = (m_TimelineExistance, gameObject.tag);
                }
            }
            
            gameObject.SetActive(m_TimelineExistance.HasFlag(DimensionManager.Instance.CurrentDimension));
            
            if (!m_UpdatePosition) return;
            m_TimelinePositions = new();
            if (m_TimelineExistance.HasFlag(Timeline.Past)) m_TimelinePositions.Add(Timeline.Past, transform.position);
            if (m_TimelineExistance.HasFlag(Timeline.Present)) m_TimelinePositions.Add(Timeline.Present, transform.position);
            if (m_TimelineExistance.HasFlag(Timeline.Future)) m_TimelinePositions.Add(Timeline.Future, transform.position);

            if (m_TimelinePositionCache.TryGetValue(id, out var positions))
            {
                m_TimelinePositions = positions;
                if (m_TimelinePositions.TryGetValue(Timeline.Present, out var position))
                {
                    transform.position = position;
                }
                
                if (m_ObstacleBehaviour) m_ObstacleBehaviour.ResetDestination(transform.position);
            }
            else
            {
                m_TimelinePositionCache[id] = m_TimelinePositions;
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

        // black magic
        // it might be illegal in some countries
        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            gameObject.SetActive(m_TimelineExistance.HasFlag(to));
            if (!m_UpdatePosition) return;
            
            if (!m_TimelineExistance.HasFlag(to))
            {
                if (!gameObject.activeSelf && !string.IsNullOrEmpty(id) && m_TimelineExistenceCache.TryGetValue(LevelManager.Instance.CurrentSceneIndex, out var timelinesInThisScene))
                {
                    var alone = true;
                    
                    foreach (var queryId in timelinesInThisScene.Keys)
                    {
                        if (queryId == id) continue;
                        if (!m_TimelinePositionCache.TryGetValue( queryId, out var positions )) continue;
                        if (!positions.TryGetValue(to, out var position)) continue; 
                        if (Vector2.Distance(position, transform.position) < 0.1f)
                        {
                            alone = false;
                        }
                    }

                    if (alone)
                    {
                        m_TimelineExistance = m_TimelineInitialValues[id];
                        gameObject.SetActive(m_TimelineExistance.HasFlag(to));
                        m_TimelineExistenceCache[LevelManager.Instance.CurrentSceneIndex][id] = (m_TimelineExistance, gameObject.tag);
                    }
                }

                return;
            }
            
            var destination = m_TimelinePositions[to];

            if (GameManager.Instance.CheckIfPlayerExists(destination))
            {
                var e = new Exception("Player is in the way of the dimension transition");
                throw e;
            }

            try
            {
                if (!string.IsNullOrEmpty(id) &&
                    m_TimelineExistenceCache.TryGetValue(LevelManager.Instance.CurrentSceneIndex,
                        out var timelinesInScene))
                {
                    foreach (var queryId in timelinesInScene.Keys)
                    {
                        if (queryId == id) continue;
                        if (!m_TimelinePositionCache.TryGetValue(queryId, out var positions)) continue;
                        if (!positions.TryGetValue(to, out var position)) continue;

                        if (Vector2.Distance(position, transform.position) < 0.1f)
                        {
                            switch (tag)
                            {
                                case "Crate" when timelinesInScene[queryId].tag == "Fence":
                                    m_TimelineExistance &= ~to;
                                    gameObject.SetActive(false);
                                    m_TimelineExistenceCache[LevelManager.Instance.CurrentSceneIndex][id] =
                                        (m_TimelineExistance, gameObject.tag);
                                    break;
                                case "Fence" when timelinesInScene[queryId].tag == "Crate":
                                    break;
                                case "Fence" when timelinesInScene[queryId].tag == "Pillar":
                                    m_TimelineExistance &= ~to;
                                    gameObject.SetActive(false);
                                    m_TimelineExistenceCache[LevelManager.Instance.CurrentSceneIndex][id] =
                                        (m_TimelineExistance, gameObject.tag);
                                    break;
                                case "Pillar" when timelinesInScene[queryId].tag == "Fence":
                                    break;
                            }

                            Debug.LogError(timelinesInScene[queryId].tag, this);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
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
            
            m_TimelinePositionCache[id] = m_TimelinePositions;
        }
    }
}
