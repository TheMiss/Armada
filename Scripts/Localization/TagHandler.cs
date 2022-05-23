using System;
using Armageddon.Backend.Payloads;
using Armageddon.Games;
using UnityEngine;

namespace Armageddon.Localization
{
    public static class TagHandler
    {
        private static Game m_game;

        public static void InjectGame(Game game)
        {
            m_game = game;
        }

        public static string Execute(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                Debug.Log("s == null or empty.");
                return string.Empty;
            }

            // const string sprite = "<sprite>";
            //
            // if (s.Contains(sprite))
            // {
            //     s = s.Replace(sprite, "<sprite name=");
            //     s = s.Replace("]}", ">]}");
            // }
            // s = s.Replace("{[", string.Empty);
            // s = s.Replace("]}", string.Empty);

            const string spriteWithParentheses = "{[<sprite";
            // const string sprite = "<sprite";

            int counter = 0;
            while (s.Contains(spriteWithParentheses) || counter < 100)
            {
                int index = s.IndexOf(spriteWithParentheses, StringComparison.Ordinal);

                if (index > -1)
                {
                    // Remove {[
                    s = s.Remove(index, 2);

                    index = s.IndexOf("]}", index + 2, StringComparison.Ordinal);

                    if (index > -1)
                    {
                        s = s.Remove(index, 2);
                    }
                }

                counter++;
            }

            s = s.Replace("{[\\n]}", "\\n");
            s = s.Replace("\\n", "\n");

            return s;
        }

        public static string ReplacePrice(string text, CurrencyType currencyType, long price)
        {
            string replacingText;

            switch (currencyType)
            {
                case CurrencyType.EvilHeart:
                    replacingText = "{[Crystal Heart Price]}";
                    break;
                case CurrencyType.RedGem:
                    replacingText = "{[Red Gem Price]}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currencyType), currencyType, null);
            }

            return text.Replace(replacingText, $"{price}");
        }
    }
}
