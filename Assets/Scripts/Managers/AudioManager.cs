using System;
using Game.Utilities;
using System.Collections;
using UnityEngine;

namespace Game.Managers
{
    public class AudioManager : SingletonBehaviour<AudioManager>
    {
        private IEnumerator Start()
        {
            if (!FMODUnity.RuntimeManager.HasBankLoaded("Master"))
            {
                yield return new WaitUntil(() => FMODUnity.RuntimeManager.HasBankLoaded("Master"));
                Debug.Log("Master Bank Loaded");
            }
            
            var instanceMusic = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Music_Gameplay");
            instanceMusic.start();

            var instanceAmbience = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Ambience");
            instanceAmbience.start();
        }

        public void PlayObstacleMoveSound(string obstacleType)
        {
            switch (obstacleType)
            {
                case "Pillar":
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Stone_Move");
                    break;
                case "Crate":
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Wood_Move");
                    break;
            }
        }

        public void PlayObstacleMoveDeniedSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Push_Error");
        }
        
        public void PlayDimensionChangeSound(Timeline to)
        {
            switch (to)
            {
                case Timeline.Past:
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Portal_Open");
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Time", 0);
                    break;
                case Timeline.Present:
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Portal_Close");
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Time", 1);
                    break;
                case Timeline.Future:
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Portal_Open");
                FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Time", 2);
                    break;
            }
        }
        
        public void PlayDimensionChangeDeniedSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Hero_Error");
        }

        public void PlayDinoRoarSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Dinosaur_Roar");
        }
        
        public void PlayDeathSound()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Hero_Death");
        }
        
        public void PlayRoomChangeSound()
        {
            
        }

        public void PlayWalkSound()
        {
            
        }
    }
}