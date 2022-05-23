﻿using System;

namespace Armageddon.Extensions
{
    public static class GenericExtensions
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }

            return val.CompareTo(max) > 0 ? max : val;
        }
    }
}
