using System;
using Game.Managers;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Behaviours
{
    public class RecipientBehaviour : MonoBehaviour
    {
        [ReadOnly, SerializeField] private bool _activated;
        [ReadOnly, SerializeField] private int _currentActivationCount = 0;

        [SerializeField] private int _requiredActivations = 1;
        private DimensionObjectBehaviour _dimensionObject;
        
        private void Awake()
        {
            DimensionManager.Instance.OnDimensionChange.AddListener(OnDimensionChange);
            TryGetComponent(out _dimensionObject);
        }

        private void OnDimensionChange(Timeline from, Timeline to)
        {
            gameObject.SetActive(!_activated && _dimensionObject.DoesExistInDimension(to));
        }


        public void Activate()
        {
            _currentActivationCount++;
            _activated = _currentActivationCount == _requiredActivations;
            gameObject.SetActive(!_activated && _dimensionObject.DoesExistInDimension(DimensionManager.Instance.CurrentDimension));
        }

        public void Deactivate()
        {
            _currentActivationCount--;
            _activated = _currentActivationCount == _requiredActivations;
            gameObject.SetActive(!_activated && _dimensionObject.DoesExistInDimension(DimensionManager.Instance.CurrentDimension));;
        }
    }
}