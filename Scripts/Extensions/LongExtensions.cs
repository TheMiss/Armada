using System;

namespace Armageddon.Extensions
{
    public static class LongExtensions
    {
        public static string ToHex(this long value, bool toUpperCase = true)
        {
            string hex = Convert.ToString(value, 16);
            if (toUpperCase)
            {
                hex = hex.ToUpper();
            }

            return hex;
        }
    }
}
