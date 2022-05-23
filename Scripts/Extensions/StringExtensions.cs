using System;
using UnityEngine;

namespace Armageddon.Extensions
{
    public static class StringExtensions
    {
        public static long ToInt64(this string hex)
        {
            try
            {
                long value = Convert.ToInt64(hex, 16);
                return value;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}: {e.Data}");
                return -1;
            }
        }
    }
}
