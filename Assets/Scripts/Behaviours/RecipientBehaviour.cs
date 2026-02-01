using UnityEngine;
using NaughtyAttributes;

namespace Game.Behaviours
{
    public class RecipientBehaviour : MonoBehaviour
    {
        [ReadOnly, SerializeField] private bool _activated;
        [ReadOnly, SerializeField] private int _currentActivationCount = 0;

        [SerializeField] private int _requiredActivations = 1;
        
        
        public void Activate()
        {
            _currentActivationCount++;
            _activated = _currentActivationCount == _requiredActivations;
            gameObject.SetActive(!_activated);
        }

        public void Deactivate()
        {
            _currentActivationCount--;
            _activated = _currentActivationCount == _requiredActivations;
            gameObject.SetActive(!_activated);
        }
    }
}