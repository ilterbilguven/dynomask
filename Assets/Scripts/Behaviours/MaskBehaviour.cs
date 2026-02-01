using System;
using Game.Behaviours;
using Game.Managers;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class MaskBehaviour : MonoBehaviour
    {
        public Timeline Timeline;

        public static UnityEvent OnMaskWasAlreadyCollected = new UnityEvent();
        
        private void Awake()
        {
            if (PlayerBehaviour.AvailableTimelines.HasFlag(Timeline))
            {
                OnMaskWasAlreadyCollected?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
