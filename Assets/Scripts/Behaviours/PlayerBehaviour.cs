using System;
using System.Collections;
using System.Collections.Generic;
using Game.Managers;
using Game.Utilities;
using NaughtyAttributes;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Game.Behaviours
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int X = Animator.StringToHash("X");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int IsPushing = Animator.StringToHash("IsPushing");
        private static readonly int Dimension = Animator.StringToHash("Dimension");
        private GameActions _gameActions;

        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _renderer;
        
        [ReadOnly, SerializeField]
        private Vector2 _rawInput = Vector2.zero;
        
        [ReadOnly, SerializeField]
        private Vector2 _animInput = Vector2.left;
        
        [ReadOnly, SerializeField]
        private Vector3 _delta = Vector3.zero;
        
        [ReadOnly, SerializeField]
        private bool _isMoving, _dimensionChanged;

        [SerializeField] private int _movement = 1;

        [SerializeField, ReadOnly] private Vector3 _destination;

        [SerializeField] private FullScreenPassRendererFeature _waterBlit;

        public UnityEvent OnMaskCollected = new UnityEvent();
        
        public static Timeline AvailableTimelines { get; private set; } = Timeline.Present;
        
        private Sequence _sequence;

        private static bool _firstSession = true;
        
        [SerializeField] private float _cooldownMovement = 0.2f;

        private Coroutine _moveCoroutine;
        private bool _isHoldingMove;
        
        private void Awake()
        {
            if(_firstSession)
                _animInput = Vector2.left;
            
            _animator.SetBool("FirstSession", _firstSession);
            _waterBlit.SetActive(false);
            GameManager.Instance.SetPlayer(this);

            _destination = transform.position;

            SetAnimatorDefaults();
            
            _gameActions = new GameActions();
            _gameActions.Player.Move.performed += OnMovePerformed;
            _gameActions.Player.Move.canceled += OnMoveCanceled;
            _gameActions.Player.Previous.performed += OnPreviousPerformed;
            _gameActions.Player.Next.performed += OnNextPerformed;
        }

        private async void Start()
        {
            await Awaitable.WaitForSecondsAsync(2f);
            if (_firstSession)
            {
                _animator.SetTrigger("TutorialLevelStart");
                
                _firstSession = false;
                _animator.SetBool("FirstSession", _firstSession);
                
            }
        }

        private void SetAnimatorDefaults()
        {
            _animator.SetFloat(X, -1);
            _animator.SetFloat(Y, 0);
            _animator.SetBool(IsMoving, false);
            _animator.SetBool(IsPushing, false);
            _animator.SetInteger(Dimension, (int) DimensionManager.Instance.CurrentDimension);
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
            _waterBlit.SetActive(false);
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            _rawInput = context.ReadValue<Vector2>();
            _isHoldingMove = true;

            _animator.SetBool(IsMoving, true);
            
            if (_moveCoroutine == null)
            {
                _moveCoroutine = StartCoroutine(MoveWhilePressed());
            }
        }

        private IEnumerator MoveWhilePressed()
        {
            while (_isHoldingMove)
            {
                TryMoveStep();
                yield return new WaitForSeconds(_cooldownMovement);
            }

            _moveCoroutine = null;
        }

        private void Update()
        {
            _animator.SetFloat(X, _animInput.x);
            _animator.SetFloat(Y, _animInput.y);
        }

        private void TryMoveStep()
        {
            if (_isMoving) return;

            _isMoving = true;

            _delta = Vector3.zero;

            if (Mathf.Abs(_rawInput.x) > Mathf.Abs(_rawInput.y))
            {
                _delta.x = _movement * Mathf.Sign(_rawInput.x);
            }
            else if (Mathf.Abs(_rawInput.y) > 0)
            {
                _delta.y = _movement * Mathf.Sign(_rawInput.y);
            }
            else
            {
                _isMoving = false;
                return;
            }

            if (Physics2D.Raycast(transform.position, _delta, _movement, LayerMask.GetMask("Water")))
            {
                AudioManager.Instance.PlayObstacleMoveDeniedSound();
                _isMoving = false;
                return;
            }

            if (!CheckAndTryMoveObstacle())
            {
                _isMoving = false;
                return;
            }

            _destination += _delta;
            _animInput = _rawInput;

            _sequence = Sequence.Create()
                .Chain(Tween.RigidbodyMovePosition(_rigidbody2D, _destination, _cooldownMovement))
                .OnComplete(() =>
                {
                    //_animator.SetBool(IsMoving, false);
                    _animator.SetBool(IsPushing, false);
                    _isMoving = false;
                });
        }

        
        private bool CheckAndTryMoveObstacle()
        {
            var hit = Physics2D.Raycast(transform.position, _delta, _movement, LayerMask.GetMask("Obstacle", "Water"));
            if (!hit.collider)
            {
                _animator.SetBool(IsPushing, false);
                return true;
            }
            Debug.Log($"PlayerHit: {hit.collider.gameObject.name}");
            if (!hit.collider.gameObject.TryGetComponent(out ObstacleBehaviour obstacle))
            {
                AudioManager.Instance.PlayObstacleMoveDeniedSound();
                return false;
            }
            if (!obstacle.TryMove(_delta)) return false;
            
            _animator.SetBool(IsPushing, true);
            return true;
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _isHoldingMove = false;
            _rawInput = Vector2.zero;
            _delta = Vector3.zero;
            
            _animator.SetBool(IsMoving, false);
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
                _animator.SetInteger(Dimension, (int) DimensionManager.Instance.CurrentDimension);
                _waterBlit.SetActive(DimensionManager.Instance.CurrentDimension == Timeline.Future);
                AudioManager.Instance.PlayDimensionChangeSound(DimensionManager.Instance.CurrentDimension);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                AudioManager.Instance.PlayDimensionChangeDeniedSound();
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
                _animator.SetInteger(Dimension, (int) DimensionManager.Instance.CurrentDimension);
                AudioManager.Instance.PlayDimensionChangeSound(DimensionManager.Instance.CurrentDimension);
                _waterBlit.SetActive(false);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                AudioManager.Instance.PlayDimensionChangeDeniedSound();
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
                OnMaskCollected?.Invoke();
                AvailableTimelines |= mask.Timeline;
                Destroy(mask.gameObject);
            }
        }
    }
}