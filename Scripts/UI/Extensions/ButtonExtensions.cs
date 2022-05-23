using Purity.Common.Extensions;
using TMPro;
using UnityEngine.UI;

namespace Armageddon.UI.Extensions
{
    public static class ButtonExtensions
    {
        public static void SetText(this Button button, string text)
        {
            var textMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            textMesh.Set(text);
        }
    }
}
