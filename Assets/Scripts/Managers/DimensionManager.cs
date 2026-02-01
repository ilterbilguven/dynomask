using System;
using Game.Utilities;
using UnityEngine.Events;

namespace Game.Managers
{
    [Flags]
    public enum Timeline { Past = 2, Present = 4, Future = 8 }
    
    public class DimensionManager : SingletonBehaviour<DimensionManager>
    {
        public UnityEvent<Timeline, Timeline> OnDimensionChange { get; private set; }
        private Timeline m_CurrentDimension = Timeline.Present;
        public Timeline CurrentDimension => m_CurrentDimension;
        
        protected override void Awake()
        {
            base.Awake();
            OnDimensionChange = new UnityEvent<Timeline, Timeline>();
        }

        // public Timeline test;
        // public bool change;
        // private void Update()
        // {
        //     if (change)
        //     {
        //         change = false;
        //         SetDimension(test);
        //     }
        // }

        public void SetDimension(Timeline to)
        {
            if (m_CurrentDimension == to) return;
            Timeline fromTimeline = m_CurrentDimension;
            m_CurrentDimension = to;
            OnDimensionChange?.Invoke(fromTimeline, to);
        }
    }
}
