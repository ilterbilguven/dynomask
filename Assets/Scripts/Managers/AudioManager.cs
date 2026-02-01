using System;
using Game.Utilities;

namespace Game.Managers
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
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
        
        public void PlayDimensionChangeSound(Timeline timeline)
        {
            switch (timeline)
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