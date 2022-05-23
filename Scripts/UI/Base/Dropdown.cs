using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Armageddon.UI.Base
{
    /// <summary>
    ///     An enhanced TMP_Dropdown
    /// </summary>
    public class Dropdown : TMP_Dropdown
    {
        public UnityEvent OnDropdownClicked { get; } = new();
        public float ScrollbarValue { set; get; }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            OnDropdownClicked?.Invoke();

            Transform content = transform.Find("Dropdown List/Viewport/Content");
            var rectTransform = content.GetComponent<RectTransform>();

            float positionY = ScrollbarValue * rectTransform.rect.size.y;
            rectTransform.anchoredPosition = new Vector2(0, positionY);
        }
    }
}
