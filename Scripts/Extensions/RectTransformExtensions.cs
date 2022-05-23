using Armageddon.Assistance.Designs;
using UnityEngine;

namespace Armageddon.Extensions
{
    public static class RectTransformExtensions
    {
        public static void DestroyDesignRemnant(this RectTransform rectTransform, bool destroyImmediate = false)
        {
            rectTransform.gameObject.DestroyDesignRemnant(destroyImmediate);
        }
    }
}
