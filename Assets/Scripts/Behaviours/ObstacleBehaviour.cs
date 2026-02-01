using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Utilities;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;

namespace Game.Behaviours
{
    public class ObstacleBehaviour : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        public float Movement = 1;

        [ReadOnly, SerializeField] private bool _canMove;
        
        [SerializeField, ReadOnly] private Vector3 _destination;
        
        private Sequence _sequence;
        
        private Color _gizmoColor = Color.red;

        [SerializeField] private DimensionObjectBehaviour _dimensionObject;

        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] private Color _passthroughColor = Color.white;

        [SerializeField] private TimelineSpriteDictionary _timelineSpriteDictionary = new();

        private static Dictionary<string, Vector3?> _positionCache = new();

        [ReadOnly, SerializeField] private string _id;

        private void Reset()
        {
            GenerateID();
        }

        [Button]
        private void GenerateID()
        {
            _id = Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            if (_positionCache.TryGetValue(_id, out var position))
            {
                if (position.HasValue)
                {
                    transform.position = position.Value;
                    
                }
                else
                {
                    Destroy(gameObject);
                }
            }            
            
            DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChanged);
            _dimensionObject.LocateObstacleBehaviour(this);
            if (_timelineSpriteDictionary.TryGetValue(DimensionManager.Instance.CurrentDimension, out var sprite))
            {
                _renderer.sprite = sprite;
            }
        }

        private void Start()
        {
            _canMove = true;
            
            _destination = transform.position;
        }

        public bool TryMove(Vector3 delta)
        {
            // if (!_canMove)
            // {
            //     return false;
            // }
            
            // raycast from the bounds of the collider
            // if there is an obstacle, return false
            // else return true    
            
            _destination += delta;

            var results = new List<Collider2D>();
            
            var size = Physics2D.OverlapBox(_destination, Vector2.one * Movement, 0, new ()
            {
                layerMask = LayerMask.GetMask("Obstacle"),
                useTriggers = false
            }, results);

            if (size != 0)
            {
                _destination -= delta;
                _gizmoColor = Color.red;
                return false;
            }
            
            if (!_sequence.isAlive)
            {
                _sequence = Sequence.Create();
            }

            _sequence.Chain(Tween.RigidbodyMovePosition(_rigidbody2D, _destination, 0.2f));
            GamepadRumble.Instance.Rumble(0.2f, 0.5f, 0.2f);
            _gizmoColor = Color.green;

            // transform.Translate(direction);

            return true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_destination, Vector3.one * Movement);
        }

        private void OnDimensionChanged(Timeline from, Timeline to)
        {
            _destination = transform.position;
            if (_timelineSpriteDictionary.TryGetValue(to, out var sprite))
            {
                _renderer.sprite = sprite;
            }

            if (to == Timeline.Present)
            {
                _positionCache[_id] = transform.position;
            }
        }

        public void ResetDestination(Vector3 position)
        {
            _destination = position;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tween.Color(_renderer, Color.white, _passthroughColor, 0.2f);
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Tween.Color(_renderer, _passthroughColor, Color.white, 0.2f);
            }
        }
        
        [Serializable]
        public class TimelineSpriteDictionary : SerializableDictionary<Timeline, Sprite> { }
    }
}