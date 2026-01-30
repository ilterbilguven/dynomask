using Game.Managers;
using UnityEngine;

namespace Game
{
    public class DimensionObject : MonoBehaviour
    {
        [SerializeField] private int m_DimensionIndex = 0;
        
        void Start()
        {
            DimensionManager.Instance.OnDimensionChange?.AddListener(OnDimensionChanged);
        }

        private void OnDestroy()
        {
            DimensionManager.Instance.OnDimensionChange?.RemoveListener(OnDimensionChanged);
        }

        private void OnDimensionChanged(int index)
        {
            gameObject.SetActive(m_DimensionIndex == index);
        }
    }
}
