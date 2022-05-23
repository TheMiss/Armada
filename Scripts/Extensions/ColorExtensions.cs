using UnityEngine;

namespace Armageddon.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color WithAlpha(this Color color, int alpha)
        {
            return new Color(color.r, color.g, color.b, alpha / 255f);
        }
    }
}
