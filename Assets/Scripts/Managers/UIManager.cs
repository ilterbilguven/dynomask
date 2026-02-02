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

#if !UNITY_EDITOR && UNITY_WEBGL

        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern bool IsMobileBrowser();
      
        [System.Runtime.InteropServices.DllImport("__Internal")]
        public static extern bool IsPreferredDesktopPlatform();
#else
        public static bool IsMobileBrowser() => false;
        public static bool IsPreferredDesktopPlatform() => true;
#endif
        
        public void ToggleMainMenu(bool value)
        {
            if (value) _mainMenu.Open();
            else _mainMenu.Close();
        }

        protected override void Awake()
        {
            base.Awake();
            LevelManager.Instance.OnLevelLoaded.AddListener(OnLevelChanged);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || IsMobileBrowser())
            {
                Screen.autorotateToPortrait = false;
                Screen.orientation = ScreenOrientation.LandscapeLeft;
                
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
