using System;
using System.Collections.Generic;
using Game.Behaviours;
using Game.Utilities;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;

namespace Game.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [ReadOnly, SerializeField]
        private PlayerBehaviour _player;
        public PlayerBehaviour Player => _player;
        
        [ReadOnly, SerializeField]
        private CinemachineCamera _camera;
        
        private Dictionary<int, Vector3> _previousPositionsInScenes = new();
        
        protected override void Awake()
        {
            base.Awake();

            LevelManager.Instance.OnLevelChangeRequested.AddListener(OnLevelChangeRequested);
            LevelManager.Instance.OnLevelChanged.AddListener(OnLevelChanged);
        }

        private void Start()
        {
            UIManager.Instance.ToggleMainMenu(true);
        }

        private void OnLevelChangeRequested()
        {
            if (_player)
            {
                _previousPositionsInScenes[LevelManager.Instance.CurrentSceneIndex] = _player.transform.position;
                _player.DisableInput();
            }
        }

        private void OnLevelChanged(int arg0)
        {
            if (_player)
            {
                _player.EnableInput();
            }
        }

        public void StartGame()
        {
            UIManager.Instance.ToggleMainMenu(false);
            LevelManager.Instance.ChangeLevel(1);
        }
        
        public void SetPlayer(PlayerBehaviour player)
        {
            _player = player;
            if (_previousPositionsInScenes.TryGetValue(LevelManager.Instance.CurrentSceneIndex, out var position))
            {
                _player.transform.position = position;
            }
        }
        
        public void LocateCamera(CinemachineCamera camera)
        {
            _camera = camera;
            _camera.Follow = _player.transform;
        }

        public bool CheckIfPlayerExists(Vector3 position)
        {
            var results = new List<Collider2D>();
            var filter = new ContactFilter2D();
            filter.SetLayerMask(LayerMask.GetMask("Player"));
            var size = Physics2D.OverlapBox(position, Vector2.one * 0.5f, 0, filter, results);
            
            return size != 0;
        }
    }
}
