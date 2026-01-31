using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public abstract class UIPanelBehaviour : MonoBehaviour
    {
        public UnityAction BeforeOpen;
        public UnityAction BeforeClose;
        public UnityAction AfterOpen;
        public UnityAction AfterClose;

        public bool IsOpen;
        public bool IsClosing;
        public bool IsOpening;

        public virtual void Open()
        {
            BeforeOpen?.Invoke();
            IsOpen = true;
            gameObject.SetActive(true);
            AfterOpen?.Invoke();
        }

        public virtual void Close()
        {
            BeforeClose?.Invoke();
            IsOpen = false;
            gameObject.SetActive(false);
            AfterClose?.Invoke();
        }
    }
}