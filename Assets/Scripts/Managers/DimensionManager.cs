using System;
using Game.Utilities;
using UnityEngine.Events;

namespace Game.Managers
{
    public class DimensionManager : SingletonBehaviour<DimensionManager>
    {
        public UnityEvent<int> OnDimensionChange { get; private set; }
        private int m_CurrentDimension = 0;
        public int CurrentDimension => m_CurrentDimension;
        
        protected override void Awake()
        {
            base.Awake();
            OnDimensionChange = new UnityEvent<int>();
        }

        public void SetDimension(int index)
        {
            m_CurrentDimension = index;
            OnDimensionChange?.Invoke(index);
        }
    }
}
