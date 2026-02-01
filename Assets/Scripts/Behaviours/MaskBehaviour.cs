using System;
using Game.Behaviours;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class MaskBehaviour : MonoBehaviour
    {
        public Timeline Timeline;

        private void Awake()
        {
            if (PlayerBehaviour.AvailableTimelines.HasFlag(Timeline))
            {
                Destroy(gameObject);
            }
        }
    }
}
