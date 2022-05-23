using UnityEngine;

namespace Armageddon.Extensions
{
    public static class GameObjectExtensions
    {
        public static void SetLayer(this GameObject gameObject, LayerMask layerMask, bool applyToChildren = true)
        {
            gameObject.layer = layerMask;

            if (applyToChildren)
            {
                foreach (Transform childTransform in gameObject.transform)
                {
                    childTransform.gameObject.SetLayer(layerMask);
                }
            }
        }
    }
}
