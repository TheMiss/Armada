using I2.Loc;

namespace Armageddon.Localization
{
	public static class Texts
	{

		public static class Font
		{
			public static string Regular 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Font/Regular")); } }
			public static string RegularHighlight 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Font/Regular Highlight")); } }
			public static string RegularOutline 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Font/Regular Outline")); } }
			public static string RegularUnderlay 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Font/Regular Underlay")); } }
		}

		public static class Message
		{
			public static string ConfirmChangeLanguage 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/ConfirmChangeLanguage")); } }
			public static string ConfirmResetShop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/ConfirmResetShop")); } }
			public static string CurrentLevel 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/Current Level")); } }
			public static string FirstLevel 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/First Level")); } }
			public static string IncorrectEmailFormat 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/IncorrectEmailFormat")); } }
			public static string InsufficientCurrency 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/InsufficientCurrency")); } }
			public static string InsufficientCurrencyDetails 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/InsufficientCurrencyDetails")); } }
			public static string NextLevel 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/Next Level")); } }
			public static string PasswordHasInvalidLength 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/PasswordHasInvalidLength")); } }
			public static string PasswordsDoNotMatch 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/PasswordsDoNotMatch")); } }
			public static string PurchaseFailed 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/PurchaseFailed")); } }
			public static string UnlockWithCrystalHeart 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/Unlock With Crystal Heart")); } }
			public static string UnlockWithRedGem 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/Unlock With Red Gem")); } }
			public static string UnlockNextChestInShop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/UnlockNextChestInShop")); } }
			public static string YouSentTooManyRequests 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Message/YouSentTooManyRequests")); } }
		}

		public static class Name
		{
			public static string Map0 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Name/Map0")); } }
			public static string Map1 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Name/Map1")); } }
			public static string Map2 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Name/Map2")); } }
			public static string StageWithNumber 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Name/Stage With Number")); } }
		}

		public static class StatusEffectDescription
		{
			public static string StatusEffect1 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("StatusEffect/Description/StatusEffect1")); } }
		}

		public static class StatusEffectName
		{
			public static string StatusEffect1 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("StatusEffect/Name/StatusEffect1")); } }
		}

		public static class Tutorial
		{
			public static string Welcome 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("Tutorial/Welcome")); } }
		}

		public static class UI
		{
			public static string Abilities 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Abilities")); } }
			public static string Account 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Account")); } }
			public static string Activies 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Activies")); } }
			public static string AdsShop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/AdsShop")); } }
			public static string Angel 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Angel")); } }
			public static string AskToExitBattle 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/AskToExitBattle")); } }
			public static string Attention 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Attention")); } }
			public static string Back 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Back")); } }
			public static string Bonus 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Bonus")); } }
			public static string Cancel 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Cancel")); } }
			public static string Cards 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Cards")); } }
			public static string ChestPack1 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ChestPack1")); } }
			public static string ChestPack2 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ChestPack2")); } }
			public static string ChestPack3 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ChestPack3")); } }
			public static string ChestPack4 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ChestPack4")); } }
			public static string Claim 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Claim")); } }
			public static string ClaimAll 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ClaimAll")); } }
			public static string Close 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Close")); } }
			public static string Compare 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Compare")); } }
			public static string DailyLogin 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/DailyLogin")); } }
			public static string DailyShop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/DailyShop")); } }
			public static string DailySignIn 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/DailySignIn")); } }
			public static string Delete 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Delete")); } }
			public static string DeleteAll 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/DeleteAll")); } }
			public static string Equip 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Equip")); } }
			public static string Error 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Error")); } }
			public static string Expired 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Expired")); } }
			public static string ExpiresAfterDays 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ExpiresAfterDays")); } }
			public static string ExpiresAfterHours 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ExpiresAfterHours")); } }
			public static string ExpiresAfterMinutes 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ExpiresAfterMinutes")); } }
			public static string ExpiresAfterSeconds 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ExpiresAfterSeconds")); } }
			public static string Game 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Game")); } }
			public static string Gear 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Gear")); } }
			public static string GemPackTitle1 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GemPackTitle1")); } }
			public static string GemPackTitle2 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GemPackTitle2")); } }
			public static string GemPackTitle3 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GemPackTitle3")); } }
			public static string GemPackTitle4 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GemPackTitle4")); } }
			public static string GemPackTitle5 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GemPackTitle5")); } }
			public static string GemPackTitle6 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GemPackTitle6")); } }
			public static string GoogleFailedToAuthorizeLogin 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/GoogleFailedToAuthorizeLogin")); } }
			public static string GotIt 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Got It")); } }
			public static string Home 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Home")); } }
			public static string Inventory 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Inventory")); } }
			public static string Language 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Language")); } }
			public static string Leaderboards 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Leaderboards")); } }
			public static string Level 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Level")); } }
			public static string Loadout 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Loadout")); } }
			public static string Lock 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Lock")); } }
			public static string Locked 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Locked")); } }
			public static string LogIn 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/LogIn")); } }
			public static string LogOut 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/LogOut")); } }
			public static string Loot 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Loot")); } }
			public static string Mailbox 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Mailbox")); } }
			public static string Music 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Music")); } }
			public static string No 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/No")); } }
			public static string Notifications 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Notifications")); } }
			public static string OK 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/OK")); } }
			public static string Open 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Open")); } }
			public static string OpenX 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/OpenX")); } }
			public static string Overseer 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Overseer")); } }
			public static string Play 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Play")); } }
			public static string PleaseWait 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Please Wait")); } }
			public static string PossibleEncounters 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Possible Encounters")); } }
			public static string Power 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Power")); } }
			public static string Powers 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Powers")); } }
			public static string RedeemCode 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/RedeemCode")); } }
			public static string Retry 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Retry")); } }
			public static string Rewards 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Rewards")); } }
			public static string Select 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Select")); } }
			public static string Selected 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Selected")); } }
			public static string Sell 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Sell")); } }
			public static string Service 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Service")); } }
			public static string Settings 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Settings")); } }
			public static string Shop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Shop")); } }
			public static string ShowDamageText 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ShowDamageText")); } }
			public static string ShowFPS 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/ShowFPS")); } }
			public static string SignIn 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/SignIn")); } }
			public static string SignInWith 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/SignInWith")); } }
			public static string SignOut 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/SignOut")); } }
			public static string SignUpWith 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/SignUpWith")); } }
			public static string SoundEffects 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/SoundEffects")); } }
			public static string SpecialShop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/SpecialShop")); } }
			public static string Stage 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Stage")); } }
			public static string StartingItems 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Starting Items")); } }
			public static string Stats 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Stats")); } }
			public static string Support 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Support")); } }
			public static string Switch 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Switch")); } }
			public static string TimePassedDaysAgo 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/TimePassedDaysAgo")); } }
			public static string TimePassedHoursAgo 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/TimePassedHoursAgo")); } }
			public static string TimePassedMinutesAgo 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/TimePassedMinutesAgo")); } }
			public static string TimePassedSecondsAgo 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/TimePassedSecondsAgo")); } }
			public static string Unequip 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Unequip")); } }
			public static string Unlock 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Unlock")); } }
			public static string Unlocked 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Unlocked")); } }
			public static string Upgrade 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Upgrade")); } }
			public static string Upgrades 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Upgrades")); } }
			public static string Use 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Use")); } }
			public static string Vibration 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Vibration")); } }
			public static string WeeklyShop 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/WeeklyShop")); } }
			public static string World 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/World")); } }
			public static string Yes 		{ get{ return TagHandler.Execute(LocalizationManager.GetTranslation("UI/Yes")); } }
		}
	}

}
