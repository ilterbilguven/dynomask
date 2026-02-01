using System;
using Game.Managers;
using UnityEngine;

namespace Game.UI
{
    public class MainMenuPanelBehaviour : UIPanelBehaviour
    {
        private void Start()
        {
            GameManager.Instance.StartGame();
        }
    }
}