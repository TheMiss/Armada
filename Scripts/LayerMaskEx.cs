using System;
using UnityEngine;

namespace Armageddon
{
    public static class LayerMaskEx
    {
        /// <summary>
        ///     A faster version of LayerMask.GetMask(params string[] layers)
        /// </summary>
        public static LayerMask GetMask(params int[] layers)
        {
            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers));
            }

            int num = 0;

            for (int i = 0; i < layers.Length; i++)
            {
                int layer = layers[i];
                num |= 1 << layer;
            }

            return num;
        }
    }
}
