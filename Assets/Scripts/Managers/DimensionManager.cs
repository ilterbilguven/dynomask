using System;
using Game.Utilities;
using UnityEngine.Events;

namespace Game.Managers
{
    public class DimensionManager : SingletonBehaviour<DimensionManager>
    {
        public UnityEvent<int> OnDimensionChange { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            OnDimensionChange = new UnityEvent<int>();
        }
    }
}
