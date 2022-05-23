using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Armageddon.UI.Base
{
    public abstract class SelectableElement<T> : Widget, ISelectHandler, IDeselectHandler where T : SelectableElement<T>
    {
        public UnityEvent<T> Selected { get; set; } = new();
        public UnityEvent<T> Deselected { get; set; } = new();

        public abstract object Object { get; }

        public virtual void OnDeselect(BaseEventData eventData)
        {
            Debug.Log($"OnDeselect {Object}");

            Deselected?.Invoke((T)this);
        }

        public virtual void OnSelect(BaseEventData eventData)
        {
            Debug.Log($"OnSelect {Object}");

            Selected?.Invoke((T)this);
        }
    }
}
