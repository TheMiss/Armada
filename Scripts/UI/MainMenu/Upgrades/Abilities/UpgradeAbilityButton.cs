using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Armageddon.UI.MainMenu.Upgrades.Abilities
{
    public class UpgradeAbilityButton : Button, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsPointerOver { get; private set; }

        public UnityEvent PointerExited { get; } = new();

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            IsPointerOver = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            IsPointerOver = false;
            PointerExited?.Invoke();
        }
    }
}
