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

        [SerializeField] private bool _canMove = true;
        
        [SerializeField, ReadOnly] private Vector3 _destination;
        
        private Sequence _sequence;
        
        private Color _gizmoColor = Color.red;

        [SerializeField] private DimensionObjectBehaviour _dimensionObject;

        [SerializeField] private SpriteRenderer _renderer;

        [SerializeField] private Color _passthroughColor = Color.white;

        [SerializeField] private TimelineSpriteDictionary _timelineSpriteDictionary = new();

        
        private void Awake()
        {
            DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChanged);
            if (_timelineSpriteDictionary.TryGetValue(DimensionManager.Instance.CurrentDimension, out var sprite))
            {
                _renderer.sprite = sprite;
            }
        }

        private void OnDestroy()
        {
            DimensionManager.Instance.OnDimensionChange.RemoveListener(OnDimensionChanged);
        }

        private void Start()
        {
            _destination = transform.position;
            _renderer.sortingOrder = (int)-_destination.y;
        }

        public bool TryMove(Vector3 delta, bool dontOverrideMovement = false, float tweenTime = 0.2f)
        {
            if (!_canMove)
            {
                return false;
            }
            
            if (_sequence.isAlive && dontOverrideMovement)
                return false;
            
            // raycast from the bounds of the collider
            // if there is an obstacle, return false
            // else return true    
            
            _destination += delta;

            var results = new List<Collider2D>();
            var filters = new ContactFilter2D();
            filters.SetLayerMask(LayerMask.GetMask("Obstacle"));
            filters.useTriggers = false;
            
            var size = Physics2D.OverlapBox(_destination, Vector2.one * Movement, 0, filters, results);

            if (size != 0)
            {
                _destination -= delta;
                _gizmoColor = Color.red;
                AudioManager.Instance.PlayObstacleMoveDeniedSound();
                HapticsManager.Instance.Rumble(0.40f, 0.55f, 0.2f);
                return false;
            }
            
            if (!_sequence.isAlive)
            {
                _sequence = Sequence.Create();
            }

            _renderer.sortingOrder = (int)-_destination.y;
            _sequence.Chain(Tween.RigidbodyMovePosition(_rigidbody2D, _destination, tweenTime));
            HapticsManager.Instance.Rumble(0.2f, 0.5f, 0.2f);
            AudioManager.Instance.PlayObstacleMoveSound(tag);
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
        }

        public void ResetDestination(Vector3 position)
        {
            _destination = position;
            _renderer.sortingOrder = (int)-_destination.y;
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