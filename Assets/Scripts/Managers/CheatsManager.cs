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
            else if (Keyboard.current.iKey.wasPressedThisFrame)
                DimensionManager.Instance.SetDimension(Timeline.Past);
            else if (Keyboard.current.oKey.wasPressedThisFrame)
                DimensionManager.Instance.SetDimension(Timeline.Present);
            else if (Keyboard.current.pKey.wasPressedThisFrame)
                DimensionManager.Instance.SetDimension(Timeline.Future);
        }
#endif
    }
}
