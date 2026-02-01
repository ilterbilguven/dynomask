using UnityEngine;
using NaughtyAttributes;

namespace Game.Behaviours
{
    public class RecipientBehaviour : MonoBehaviour
    {
        [ReadOnly, SerializeField] private bool _activated;
        
        public void Activate()
        {
            _activated = true;
            gameObject.SetActive(false);
        }

        public void Deactivate()
        {
            _activated = false;
            gameObject.SetActive(true);
        }
    }
}