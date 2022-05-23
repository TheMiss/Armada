using System;
using System.Globalization;
using Armageddon.Backend.Payloads;
using Armageddon.Mechanics;
using I2.Loc;

namespace Armageddon.Localization
{
    public static class Lexicon
    {
        public static string InsufficientCurrency(CurrencyType currencyType)
        {
            return Texts.Message.InsufficientCurrency.Replace("{[Currency]}", Currency(currencyType));
        }

        public static string InsufficientCurrencyDetails(CurrencyType currencyType)
        {
            return Texts.Message.InsufficientCurrencyDetails.Replace("{[Currency]}", Currency(currencyType));
        }

        public static string StageNumber(int stageNumber)
        {
            return Texts.UI.Stage.Replace("{[Stage]}", $"{stageNumber}");
        }

        public static string MapName(int mapId)
        {
            // This is a good example of not always using Texts.Xxx
            string mapName = LocalizationManager.GetTranslation($"Name/Map{mapId}");
            return mapName;
        }

        public static string IncorrectEmailFormat(string email)
        {
            return Texts.Message.IncorrectEmailFormat.Replace("{[Email]}", email);
        }

        public static string ConfirmResetShop(ShopType shopType, Currency price)
        {
            string text = Texts.Message.ConfirmResetShop.Replace("{[Currency]}", Currency(price))
                .Replace("{[Shop]}", Shop(shopType));

            return text;
        }

        public static string Currency(CurrencyType currencyType)
        {
            return $"<sprite name=\"{currencyType.ToCurrencyCode()}\">";
        }

        public static string Currency(CurrencyType currencyType, int amount)
        {
            return $"{Currency(currencyType)}{Amount(amount)}";
        }

        public static string Currency(Currency currency)
        {
            return Currency(currency.Type, currency.Amount);
        }

        public static string Amount(int amount)
        {
            var us = new CultureInfo("en-US");
            return amount.ToString("N0", us);
        }

        public static string Shop(ShopType shopTyped)
        {
            return shopTyped switch
            {
                ShopType.Daily => Texts.UI.DailyShop,
                ShopType.Weekly => Texts.UI.WeeklyShop,
                ShopType.Special => Texts.UI.SpecialShop,
                ShopType.Ads => Texts.UI.AdsShop,
                _ => throw new ArgumentOutOfRangeException(nameof(shopTyped), shopTyped, null)
            };
        }

        public static string ExpiresAfter(DateTime expiredDateTime)
        {
            TimeSpan expiresAfterTime = expiredDateTime - DateTime.UtcNow;
            return ExpiresAfter(expiresAfterTime);
        }

        public static string ExpiresAfter(TimeSpan timeSpan)
        {
            int days = timeSpan.Days;
            if (days > 0)
            {
                return Texts.UI.ExpiresAfterDays.Replace("{[days]}", $"{days}");
            }

            int hours = timeSpan.Hours;
            if (hours > 0)
            {
                return Texts.UI.ExpiresAfterHours.Replace("{[hours]}", $"{hours}");
            }

            int minutes = timeSpan.Minutes;
            if (minutes > 0)
            {
                return Texts.UI.ExpiresAfterMinutes.Replace("{[minutes]}", $"{minutes}");
            }

            int seconds = timeSpan.Seconds;
            if (seconds > 0)
            {
                return Texts.UI.ExpiresAfterSeconds.Replace("{[seconds]}", $"{seconds}");
            }

            return Texts.UI.Expired;
        }

        public static string TimePassedAgo(DateTime startedDateTime)
        {
            TimeSpan passingTime = DateTime.UtcNow - startedDateTime;
            return TimePassedAgo(passingTime);
        }

        public static string TimePassedAgo(TimeSpan timeSpan)
        {
            int days = timeSpan.Days;
            if (days > 0)
            {
                return Texts.UI.TimePassedDaysAgo.Replace("{[days]}", $"{days}");
            }

            int hours = timeSpan.Hours;
            if (hours > 0)
            {
                return Texts.UI.TimePassedHoursAgo.Replace("{[hours]}", $"{hours}");
            }

            int minutes = timeSpan.Minutes;
            if (minutes > 0)
            {
                return Texts.UI.TimePassedMinutesAgo.Replace("{[minutes]}", $"{minutes}");
            }

            int seconds = timeSpan.Seconds;
            if (seconds > 0)
            {
                return Texts.UI.TimePassedSecondsAgo.Replace("{[seconds]}", $"{seconds}");
            }

            return string.Empty;
        }

        public static string ConfirmChangeLanguage(string language)
        {
            return Texts.Message.ConfirmChangeLanguage.Replace("{[language]}", language);
        }
    }
}
