using System;
using System.Collections.Generic;
using Game.Managers;
using Game.Utilities;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Behaviours
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int X = Animator.StringToHash("X");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsPushing = Animator.StringToHash("IsPushing");
        private static readonly int IsSwimming = Animator.StringToHash("IsSwimming");
        private GameActions _gameActions;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _animator;
        
        [ReadOnly, SerializeField]
        private Vector2 _rawInput = Vector2.zero;
        
        [ReadOnly, SerializeField]
        private Vector3 _delta = Vector3.zero;
        
        [ReadOnly, SerializeField]
        private bool _isMoving;

        [SerializeField] private int _movement = 1;

        [SerializeField, ReadOnly] private Vector3 _destination;
        
        private Sequence _sequence;
        
        private void Awake()
        {
            GameManager.Instance.SetPlayer(this);
        }

        private void Start()
        {
            _gameActions = new GameActions();
            _gameActions.Player.Move.performed += OnMovePerformed;
            _gameActions.Player.Move.canceled += OnMoveCanceled;
            
            _destination = transform.position;
            _animator.SetFloat(X, 0);
            _animator.SetFloat(Y, -1);
            _animator.SetBool(IsMoving, false);
        }
        
        public void EnableInput()
        {
            _gameActions.Enable();
        }
        
        public void DisableInput()
        {
            _gameActions.Disable();
        }

        private void OnDestroy()
        {
            _gameActions.Player.Move.performed -= OnMovePerformed;
            _gameActions.Player.Move.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (_isMoving)
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

            _destination += _delta;
            
            _animator.SetBool(IsMoving, true);

            if (!_sequence.isAlive)
            {
                _sequence = Sequence.Create();
                _sequence.OnComplete(() =>
                {
                    _animator.SetBool(IsMoving, false);
                    _animator.SetBool(IsPushing, false);
                });
            }
            
            _sequence.Chain(Tween.RigidbodyMovePosition(_rigidbody2D, _destination, 0.2f));
            // transform.Translate(_delta);
            
            _animator.SetFloat(X, _delta.x);
            _animator.SetFloat(Y, _delta.y);
        }

        private bool CheckAndTryMoveObstacle()
        {
            var hit = Physics2D.Raycast(transform.position, _delta, _movement, LayerMask.GetMask("Obstacle"));
            if (!hit.collider)
            {
                _animator.SetBool(IsPushing, false);
                return true;
            }
            Debug.Log($"PlayerHit: {hit.collider.gameObject.name}");
            if (!hit.collider.gameObject.TryGetComponent(out ObstacleBehaviour obstacle)) return false;
            if (!obstacle.TryMove(_delta)) return false;
            
            _animator.SetBool(IsPushing, true);
            return true;
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _isMoving = false;
            _delta = Vector3.zero;
            _rawInput = Vector2.zero;
        }
        
        
        [Serializable]
        public class Vector2IntCollider2DDictionary : SerializableDictionary<Vector2Int, Collider2D> { }
    }
}