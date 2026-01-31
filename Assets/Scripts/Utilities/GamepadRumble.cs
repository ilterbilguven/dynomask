using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Utilities
{
    public class GamepadRumble : MonoBehaviour
    {
        Coroutine _routine;

        public void Rumble(float low, float high, float seconds)
        {
            var pad = Gamepad.current;
            if (pad == null) return;

            low = Mathf.Clamp01(low);
            high = Mathf.Clamp01(high);

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(RumbleRoutine(pad, low, high, seconds));
        }

        IEnumerator RumbleRoutine(Gamepad pad, float low, float high, float seconds)
        {
            pad.SetMotorSpeeds(low, high); 
            yield return new WaitForSeconds(seconds);
            pad.ResetHaptics(); 
        }

        void OnDisable()
        {
            InputSystem.ResetHaptics();
        }
    }
}
