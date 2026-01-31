using System;
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
        
        [ReadOnly, SerializeField]
        private CinemachineCamera _camera;
        
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
        }
        
        public void LocateCamera(CinemachineCamera camera)
        {
            _camera = camera;
            _camera.Follow = _player.transform;
        }
    }
}
