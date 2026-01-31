using System;
using System.Collections.Generic;
using Game.Utilities;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Behaviours
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private GameActions _gameActions;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        [ReadOnly, SerializeField]
        private Vector2 _rawInput = Vector2.zero;
        
        [ReadOnly, SerializeField]
        private Vector3 _delta = Vector3.zero;
        
        [ReadOnly, SerializeField]
        private bool _isMoving, _canMove;

        [SerializeField] private int _movement = 1;
        
        
        private void Start()
        {
            _gameActions = new GameActions();
            _gameActions.Player.Move.performed += OnMovePerformed;
            _gameActions.Player.Move.canceled += OnMoveCanceled;
            _gameActions.Enable();
            // _canMove = true;
        }

        private void OnDestroy()
        {
            _gameActions.Player.Move.performed -= OnMovePerformed;
            _gameActions.Player.Move.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (_isMoving /*&& !_canMove*/)
            {
                return;
            }
            
            _isMoving = true;
            _rawInput = context.ReadValue<Vector2>();

            if (Mathf.Abs(_rawInput.x) > Mathf.Abs(_rawInput.y))
            {
                _delta.x = _movement * (int)Mathf.Sign(_rawInput.x);
                _delta.y = 0;
            }
            else if (Mathf.Abs(_rawInput.x) < Mathf.Abs(_rawInput.y))
            {
                _delta.x = 0;
                _delta.y = _movement * (int)Mathf.Sign(_rawInput.y);
            }
            
            if (!CheckAndTryMoveObstacle())
            {
                return;
            }

            // _canMove = false;
            // Tween.RigidbodyMovePosition(_rigidbody2D, transform.position + _delta, 0.2f).OnComplete(() => _canMove = true);
            transform.Translate(_delta);
            
            _delta = Vector3.zero;
        }

        private bool CheckAndTryMoveObstacle()
        {
            var hit = Physics2D.Raycast(transform.position, _delta, _movement, LayerMask.GetMask("Obstacle"));
            if (!hit.collider)
            {
                return true;
            }
            Debug.Log($"PlayerHit: {hit.collider.gameObject.name}");
            return hit.collider.gameObject.TryGetComponent(out ObstacleBehaviour obstacle) && obstacle.TryMove(_delta);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _isMoving = false;
            _rawInput = Vector2.zero;
        }
        
        
        [Serializable]
        public class Vector2IntCollider2DDictionary : SerializableDictionary<Vector2Int, Collider2D> { }
    }
}