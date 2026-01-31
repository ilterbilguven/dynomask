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
        

        private void Awake()
        {
            DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChanged);
            _dimensionObject.LocateObstacleBehaviour(this);
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
            
            var size = Physics2D.OverlapBox(_destination, Vector2.one * Movement, 0, ContactFilter2D.noFilter, results);

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
        }

        public void ResetDestination(Vector3 position)
        {
            _destination = position;
        }
    }
}