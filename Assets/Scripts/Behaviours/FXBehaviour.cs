using System;
using Game.Managers;
using UnityEngine;

namespace Game
{
    public class FXBehaviour : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            MaskBehaviour.OnMaskWasAlreadyCollected.AddListener(ForceStopPlayback); 
            GameManager.Instance.Player.OnMaskCollected.AddListener(StopPlayback);
        }

        private void ForceStopPlayback()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            MaskBehaviour.OnMaskWasAlreadyCollected.RemoveListener(ForceStopPlayback); 
            GameManager.Instance.Player.OnMaskCollected.RemoveListener(StopPlayback);
        }

        private void StopPlayback()
        {
            GetComponent<ParticleSystem>().Stop();
        }
        
    }
}
