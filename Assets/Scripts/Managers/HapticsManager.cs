using System.Collections;
using Game.Managers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Utilities
{
    public class HapticsManager : SingletonBehaviour<HapticsManager>
    {
        [Range(0,1)]
        public float TestHighFrequency = 0.5f;
        [Range(0,1)]
        public float TestLowFrequency = 0.5f;
        [Range(0,5)] public float TestDuration = 0.1f; // [ 0.01f, 10.0f]
        
        
        [Button]
        public void Test()
        {
            Rumble(TestLowFrequency, TestHighFrequency, 0.5f);
        }
        
        
        Coroutine _routine;
        

        private void OnLevelChangeRequested()
        {
            InputSystem.ResetHaptics();
        }

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
