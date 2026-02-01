using System;
using Game.Utilities;

namespace Game.Managers
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        protected override void Awake()
        {
            base.Awake();
            
            DimensionManager.Instance.OnDimensionChange.AddListener(PlayDimensionChangeSound);
        }

        public void PlayObstacleMoveSound(string obstacleType)
        {
            switch (obstacleType)
            {
                case "Pillar":
                    break;
                case "Crate":
                    break;
            }
        }

        public void PlayObstacleMoveDeniedSound()
        {
            
        }
        
        public void PlayDimensionChangeSound(Timeline from, Timeline to)
        {
            switch (to)
            {
                case Timeline.Past:
                    break;
                case Timeline.Present:
                    break;
                case Timeline.Future:
                    break;
            }
        }
        
        public void PlayDimensionChangeDeniedSound()
        {
            
        }
        
        public void PlayRoomChangeSound()
        {
            
        }

        public void PlayWalkSound()
        {
            
        }
    }
}