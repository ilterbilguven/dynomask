using System;
using Game.Behaviours;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class ActivatorBehaviour : MonoBehaviour
    {
        [ReadOnly, SerializeField] private bool _activated;

        public RecipientBehaviour Recipient;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Pillar"))
            {
                _activated = true;
                Recipient.Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Pillar"))
            {
                _activated = false;
                Recipient.Deactivate();
            }
        }
    }
}
