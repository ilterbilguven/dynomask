using Game.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class CheatsManager : MonoBehaviour
    {
#if CHEATS_ENABLED
        // Update is called once per frame
        void Update()
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                LevelManager.Instance.ChangeLevel(1);
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
                LevelManager.Instance.ChangeLevel(2);
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
                LevelManager.Instance.ChangeLevel(3);
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
                LevelManager.Instance.ChangeLevel(4);
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
                LevelManager.Instance.ChangeLevel(5);
            else if (Keyboard.current.digit0Key.wasPressedThisFrame)
                GameManager.Instance.Player.CollectMasks();
        }
#endif
    }
}
