using System;
using UnityEngine;

namespace Game.Utilities
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T Instance { get; protected set; }

        [SerializeField] private bool _isPersistent = true;

        protected virtual void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(this);
                throw new Exception("An instance of this singleton already exists.");
            }

            Instance = (T)this;
            if (_isPersistent) DontDestroyOnLoad(gameObject);
        }
    }
}
