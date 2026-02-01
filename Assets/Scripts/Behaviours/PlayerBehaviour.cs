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
        [SerializeField] private SpriteRenderer _renderer;
        
        [ReadOnly, SerializeField]
        private Vector2 _rawInput = Vector2.zero;
        
        [ReadOnly, SerializeField]
        private Vector3 _delta = Vector3.zero;
        
        [ReadOnly, SerializeField]
        private bool _isMoving, _dimensionChanged;

        [SerializeField] private int _movement = 1;

        [SerializeField, ReadOnly] private Vector3 _destination;

        
        public static Timeline AvailableTimelines { get; private set; } = Timeline.Present;
        
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
            _gameActions.Player.Next.performed += OnNextPerformed;
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
            Debug.Log("Future pushed");
            
            var currentDimension = DimensionManager.Instance.CurrentDimension;

            try
            {
                DimensionManager.Instance.SetDimension(DimensionManager.Instance.CurrentDimension != Timeline.Present
                    ? Timeline.Present
                    : Timeline.Future);
                HapticsManager.Instance.Rumble(0.25f, 0.45f, 0.2f);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                DimensionManager.Instance.SetDimension(currentDimension);
                ViewError();
            }
        }

        private void OnPreviousPerformed(InputAction.CallbackContext context)
        {
            if (!AvailableTimelines.HasFlag(Timeline.Past)) return;
            Debug.Log("Past pushed");
            
            var currentDimension = DimensionManager.Instance.CurrentDimension;

            try
            {
                DimensionManager.Instance.SetDimension(DimensionManager.Instance.CurrentDimension != Timeline.Present
                    ? Timeline.Present
                    : Timeline.Past);
                HapticsManager.Instance.Rumble(0.25f, 0.45f, 0.2f);
                
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                DimensionManager.Instance.SetDimension(currentDimension);
                ViewError();
            }
        }

        private void ViewError()
        {
            HapticsManager.Instance.Rumble(0.10f, 0.70f, 0.2f);
            Tween.ShakeLocalPosition(_renderer.transform, Vector3.one * 0.2f, 0.2f);
            Tween.Color(_renderer, Color.red, 0.2f).OnComplete(() => Tween.Color(_renderer, Color.white, 0.2f));
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