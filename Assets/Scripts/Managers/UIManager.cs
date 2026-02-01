using System;
using Game.UI;
using Game.Utilities;
using UnityEngine;

namespace Game.Managers
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [SerializeField] private MainMenuPanelBehaviour _mainMenu;
        [SerializeField] private EndPanelBehaviour _end;
        [SerializeField] private InputPanelBehaviour _inputPanelBehaviour;
        
        
        public void ToggleMainMenu(bool value)
        {
            if (value) _mainMenu.Open();
            else _mainMenu.Close();
        }

        private void Start()
        {
            LevelManager.Instance.OnLevelLoaded.AddListener(OnLevelChanged);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _inputPanelBehaviour.Open();
            }
        }

        private void OnLevelChanged(int index)
        {
            if (index == 6)
            {
                _end.Open();
            }
        }
    }
}
