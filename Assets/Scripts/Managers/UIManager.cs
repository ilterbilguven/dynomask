using Game.UI;
using Game.Utilities;
using UnityEngine;

namespace Game.Managers
{
    public class UIManager : SingletonBehaviour<UIManager>
    {
        [SerializeField] private MainMenuPanelBehaviour _mainMenu;

        public void ToggleMainMenu(bool value)
        {
            if (value) _mainMenu.Open();
            else _mainMenu.Close();
        }
    }
}
