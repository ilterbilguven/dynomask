using System;
using System.Collections;
using Game.Managers;
using UnityEngine;

namespace Game.UI
{
    public class MainMenuPanelBehaviour : UIPanelBehaviour
    {
        private IEnumerator Start()
        {
            if (!FMODUnity.RuntimeManager.HasBankLoaded("Master"))
            {
                yield return FMODUnity.RuntimeManager.HasBankLoaded("Master");
                Debug.Log("Master Bank Loaded");
            }
            GameManager.Instance.StartGame();
        }
    }
}