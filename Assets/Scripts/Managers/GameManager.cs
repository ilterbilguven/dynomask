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

            LevelManager.Instance.OnLevelLoaded.AddListener(OnLevelLoaded);
        }

        private void Start()
        {
            UIManager.Instance.ToggleMainMenu(true);
        }

        private async void OnLevelLoaded(int levelIndex)
        {
            await Awaitable.NextFrameAsync();
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
