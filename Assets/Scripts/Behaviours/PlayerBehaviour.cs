using System;
using System.Collections;
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
        private bool _isMoving, _dimensionChanged;

        [SerializeField] private int _movement = 1;

        [SerializeField, ReadOnly] private Vector3 _destination;

        public static Timeline AvailableTimelines { get; private set; } = Timeline.Present;
        private Stack<Timeline> _timelineStack = new();
        private Dictionary<int, Vector2> _previousPositionsInScenes = new();
        
        private Sequence _sequence;
        
        private void Awake()
        {
            GameManager.Instance.SetPlayer(this);

            _destination = transform.position;

            SetAnimatorDefaults();
            
            _gameActions = new GameActions();
            _gameActions.Player.Move.performed += OnMovePerformed;
            _gameActions.Player.Move.canceled += OnMoveCanceled;
            _gameActions.Player.Previous.performed += OnPreviousPerformed;
            _gameActions.Player.Previous.canceled += OnPreviousCanceled;
            _gameActions.Player.Next.performed += OnNextPerformed;
            _gameActions.Player.Next.canceled += OnNextCanceled;
        }

        private void SetAnimatorDefaults()
        {
            _animator.SetFloat(X, -1);
            _animator.SetFloat(Y, 0);
            _animator.SetBool(IsMoving, false);
            _animator.SetBool(IsPushing, false);
            _animator.SetBool(IsSwimming, false);
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

        private void OnNextPerformed(InputAction.CallbackContext context)
        {
            if (!AvailableTimelines.HasFlag(Timeline.Future)) return;
            if (_timelineStack.Contains(Timeline.Future)) return;
            _timelineStack.Push(Timeline.Future);
            Debug.Log("Future pushed");

            if (!_dimensionChanged)
            {
                ApplyStackedTimelines();
            }
        }

        private void OnNextCanceled(InputAction.CallbackContext context)
        {
            if (!AvailableTimelines.HasFlag(Timeline.Future)) return;
            
            _timelineStack.Push(Timeline.Present);
            Debug.Log("Present pushed");
            
            ApplyStackedTimelines();
        }

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            if (!AvailableTimelines.HasFlag(Timeline.Past)) return;
            if (_timelineStack.Contains(Timeline.Past)) return;
            
            _timelineStack.Push(Timeline.Past);
            Debug.Log("Past pushed");

            if (!_dimensionChanged)
            {
                ApplyStackedTimelines();
            }
        }
        
        private void OnPreviousCanceled(InputAction.CallbackContext context)
        {
            if (!AvailableTimelines.HasFlag(Timeline.Past)) return;
            
            _timelineStack.Push(Timeline.Present);
            Debug.Log("Present pushed");
            
            ApplyStackedTimelines();
        }
        
        private void ApplyStackedTimelines()
        {
            if (_timelineStack.Count == 0) return;
            _dimensionChanged = true;
            var timeline = _timelineStack.Pop();
            Debug.Log($"Applying timeline: {timeline}");
            DimensionManager.Instance.SetDimension(timeline);
            _dimensionChanged = timeline != Timeline.Present;
            if (_timelineStack.Count > 0)
            {
                Debug.Log("Applying next timeline");
                ApplyStackedTimelines();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Mask") && other.TryGetComponent(out MaskBehaviour mask))
            {
                AvailableTimelines |= mask.Timeline;
                Destroy(mask.gameObject);
            }
        }
    }
}