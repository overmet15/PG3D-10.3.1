using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rilisoft
{
	internal static class FlurryEvents
	{
		public enum LogTrafficForwardingMode
		{
			Show = 0,
			Press = 1
		}

		private const string WeaponsSpecialOffersEvent = "Weapons Special Offers";

		public const string PurchasesPointEventName = "Purchases Points";

		public const string CoinsGained = "Coins Gained";

		public const string FeatureEnabled = "Feature Enabled";

		public const string ShopPurchasesFormat = "Shop Purchases {0}";

		public const string TimeWeaponsFormat = "Time Weapons {0}";

		public const string TimeWeaponsRedFormat = "Time Weapons (red test) {0}";

		public const string TimeArmorAndHatFormat = "Time Armor and Hat {0}";

		public const string GemsFormat = "Purchase for Gems {0}";

		public const string GemsTempArmorFormat = "Purchase for Gems TempArmor {0}";

		public const string CoinsFormat = "Purchase for Coins {0}";

		public const string CoinsTempArmorFormat = "Purchase for Coins TempArmor {0}";

		public const string TrainingProgress = "Training Progress";

		public const string PurchaseAfterPayment = "Purchase After Payment";

		public const string PurchaseAfterPaymentCumulative = "Purchase After Payment Cumulative";

		public const string FastPurchase = "Fast Purchase";

		public const string AfterTraining = "After Training";

		public static Dictionary<ShopNGUIController.CategoryNames, string> shopCategoryToLogSalesNamesMapping = new Dictionary<ShopNGUIController.CategoryNames, string>
		{
			{
				ShopNGUIController.CategoryNames.SkinsCategory,
				"Skins"
			},
			{
				ShopNGUIController.CategoryNames.PrimaryCategory,
				"Primary"
			},
			{
				ShopNGUIController.CategoryNames.BackupCategory,
				"Back Up"
			},
			{
				ShopNGUIController.CategoryNames.MeleeCategory,
				"Melee"
			},
			{
				ShopNGUIController.CategoryNames.SpecilCategory,
				"Special"
			},
			{
				ShopNGUIController.CategoryNames.SniperCategory,
				"Sniper"
			},
			{
				ShopNGUIController.CategoryNames.PremiumCategory,
				"Premium"
			},
			{
				ShopNGUIController.CategoryNames.ArmorCategory,
				"Armor"
			},
			{
				ShopNGUIController.CategoryNames.BootsCategory,
				"Boots"
			},
			{
				ShopNGUIController.CategoryNames.CapesCategory,
				"Capes"
			},
			{
				ShopNGUIController.CategoryNames.HatsCategory,
				"Hats"
			},
			{
				ShopNGUIController.CategoryNames.GearCategory,
				"Gear"
			},
			{
				ShopNGUIController.CategoryNames.MaskCategory,
				"Masks"
			}
		};

		public static float? PaymentTime { get; set; }

		public static void LogWEaponsSpecialOffers_MoneySpended(string packId)
		{
			try
			{
				if (string.IsNullOrEmpty(packId))
				{
					Debug.LogError("LogWEaponsSpecialOffers_MoneySpended: string.IsNullOrEmpty(packId)");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Money Spended", ReadableNameForInApp(packId));
				Dictionary<string, string> dictionary2 = dictionary;
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogWEaponsSpecialOffers_MoneySpended = Weapons Special Offers Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogWEaponsSpecialOffers_MoneySpended = Weapons Special Offers" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Weapons Special Offers Total", dictionary2, false);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Weapons Special Offers" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2, false);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogWEaponsSpecialOffers_MoneySpended: " + ex);
			}
		}

		public static void LogWEaponsSpecialOffers_Conversion(bool show, string weaponId = null)
		{
			try
			{
				if (!show && string.IsNullOrEmpty(weaponId))
				{
					Debug.LogError("LogWEaponsSpecialOffers_Conversion: string.IsNullOrEmpty(weaponId)");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Conversion", (!show) ? "Buy" : "Show");
				Dictionary<string, string> dictionary2 = dictionary;
				try
				{
					float killRate = KillRateCheck.instance.GetKillRate();
					string arg = ((killRate <= 0.5f) ? "Weak" : ((!(killRate <= 1.2f)) ? "Strong" : "Normal"));
					string key = string.Format("Conversion {0} Players", arg);
					if (!show)
					{
						dictionary2.Add("Currency Spended", weaponId);
						dictionary2.Add("Buy (Tier)", (ExpController.OurTierForAnyPlace() + 1).ToString());
						dictionary2.Add("Buy (Level)", ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel).ToString());
						dictionary2.Add(key, "Buy");
					}
					else
					{
						dictionary2.Add("Show (Tier)", (ExpController.OurTierForAnyPlace() + 1).ToString());
						dictionary2.Add("Show (Level)", ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel).ToString());
						dictionary2.Add(key, "Show");
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in LogWEaponsSpecialOffers_Conversion adding paramters: " + ex);
				}
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogWEaponsSpecialOffers_Conversion = Weapons Special Offers Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogWEaponsSpecialOffers_Conversion = Weapons Special Offers" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Weapons Special Offers Total", dictionary2, false);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Weapons Special Offers" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2, false);
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in LogWEaponsSpecialOffers_Conversion: " + ex2);
			}
		}

		public static void LogTrafficForwarding(LogTrafficForwardingMode mode)
		{
			try
			{
				string text = ((mode != 0) ? "Button Pressed" : "Button Show");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Conversion", text);
				dictionary.Add(text + " Levels", string.Empty + ((!(ExperienceController.sharedController != null)) ? 1 : ExperienceController.sharedController.currentLevel));
				dictionary.Add(text + " Tiers", string.Empty + (ExpController.OurTierForAnyPlace() + 1));
				dictionary.Add(text + " Paying", (!FlurryPluginWrapper.IsPayingUser()) ? "FALSE" : "TRUE");
				Dictionary<string, string> dictionary2 = dictionary;
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogTrafficForwarding = Pereliv Button</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Pereliv Button", dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogTrafficForwarding: " + ex);
			}
		}

		public static void LogSpecialOffersPanel(string efficiencyPArameter, string efficiencyValue, string additionalParameter = null, string additionalValue = null)
		{
			try
			{
				if (string.IsNullOrEmpty(efficiencyPArameter) || string.IsNullOrEmpty(efficiencyValue))
				{
					Debug.LogError("LogSpecialOffersPanel:  string.IsNullOrEmpty(efficiencyPArameter) || string.IsNullOrEmpty(efficiencyValue)");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(efficiencyPArameter, efficiencyValue);
				Dictionary<string, string> dictionary2 = dictionary;
				if (additionalParameter != null && additionalValue != null)
				{
					dictionary2.Add(additionalParameter, additionalValue);
				}
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogSpecialOffersPanel = Special Offers Panel Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogSpecialOffersPanel = Special Offers Panel" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Special Offers Panel Total", dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Special Offers Panel" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogSpecialOffersPanel: " + ex);
			}
		}

		public static void LogRateUsFake(bool rate, int stars = 0, bool sendNegativFeedback = false)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Efficiency", (!rate) ? "Later" : "Rate");
			if (rate)
			{
				dictionary.Add("Rating (Stars)", stars.ToString());
			}
			if (stars > 0 && stars < 4)
			{
				dictionary.Add("Negative Feedback", (!sendNegativFeedback) ? "Not sended" : "Sended");
			}
			if (Debug.isDebugBuild)
			{
				Debug.Log("<color=green>LogRateUsFake = Rate Us Fake</color>\n<color=white>parameters = " + dictionary.ToStringFull() + "</color>");
			}
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Rate Us Fake", dictionary);
		}

		public static void LogTutorial(string _progress)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Progress", _progress);
			if (Debug.isDebugBuild)
			{
				Debug.Log("<color=green>LogRateUsFake = Tutorial</color>\n<color=white>parameters = " + dictionary.ToStringFull() + "</color>");
			}
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Tutorial", dictionary);
		}

		public static void LogSales(string itemId, string categoryParameterName, bool isDaterWeapon = false)
		{
			try
			{
				if (string.IsNullOrEmpty(itemId))
				{
					Debug.LogError("LogSales: string.IsNullOrEmpty(itemId)");
					return;
				}
				if (string.IsNullOrEmpty(categoryParameterName))
				{
					Debug.LogError("LogSales: string.IsNullOrEmpty(categoryParameterName)");
					return;
				}
				string[] source = new string[11]
				{
					"Stickers", "Premium Maps", "Gear", "Premium Account", "Skins", "Armor", "Boots", "Capes", "Hats", "Starter Pack",
					"Masks"
				};
				string[] array = new string[6] { "Primary", "Back Up", "Melee", "Special", "Sniper", "Premium" };
				string text = ((!source.Contains(categoryParameterName)) ? "Weapons Sales" : "Equipment Sales");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(categoryParameterName, itemId);
				Dictionary<string, string> dictionary2 = dictionary;
				if (isDaterWeapon)
				{
					dictionary2.Add("Dater Weapons", itemId);
				}
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogSales = Sales Report Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogSales = Sales Report" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogSales_Splitted = " + text + " Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogSales_Splitted = " + text + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Sales Report Total", dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Sales Report" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole(text + " Total", dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole(text + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogSales: " + ex);
			}
		}

		public static void LogPurchaseStickers(string stickersPackId)
		{
			try
			{
				if (string.IsNullOrEmpty(stickersPackId))
				{
					Debug.LogError("LogPurchaseStickers: string.IsNullOrEmpty(stickersPackId)");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Purchases", stickersPackId);
				dictionary.Add("Points", BuySmileBannerController.GetCurrentBuySmileContextName());
				Dictionary<string, string> dictionary2 = dictionary;
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogPurchaseStickers = Purchases Stickers Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogPurchaseStickers = Purchases Stickers" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchases Stickers Total", dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Purchases Stickers" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogPurchaseStickers: " + ex);
			}
		}

		public static void LogDailyGiftPurchases(string packId)
		{
			try
			{
				if (string.IsNullOrEmpty(packId))
				{
					Debug.LogError("LogDailyGiftPurchases: string.IsNullOrEmpty(packId)");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Purchases", ReadableNameForInApp(packId));
				Dictionary<string, string> dictionary2 = dictionary;
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogDailyGiftPurchases = Daily Gift Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogDailyGiftPurchases = Daily Gift" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Daily Gift Total", dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Daily Gift" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogDailyGiftPurchases: " + ex);
			}
		}

		public static void LogDailyGift(string giftId, int count, bool isForMoneyGift)
		{
			try
			{
				if (string.IsNullOrEmpty(giftId))
				{
					Debug.LogError("LogDailyGift: string.IsNullOrEmpty(giftId)");
					return;
				}
				if (SkinsController.shopKeyFromNameSkin.ContainsKey(giftId))
				{
					giftId = "Skin";
				}
				giftId = giftId + "_" + count;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Chance", giftId);
				dictionary.Add("Spins", (!isForMoneyGift) ? "Free" : "Paid");
				Dictionary<string, string> dictionary2 = dictionary;
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogDailyGiftPurchases = Daily Gift Total</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
					Debug.Log("<color=green>LogDailyGiftPurchases = Daily Gift" + FlurryPluginWrapper.GetPayingSuffixNo10() + "</color>\n<color=white>parameters = " + dictionary2.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Daily Gift Total", dictionary2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Daily Gift" + FlurryPluginWrapper.GetPayingSuffixNo10(), dictionary2);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogDailyGift: " + ex);
			}
		}

		private static int NumberOfDaysForBecomePaying(DateTime earlyDate, DateTime futureDate)
		{
			return (int)(Math.Ceiling((futureDate - earlyDate).TotalDays) + 0.5);
		}

		public static string ReadableNameForInApp(string purchaseId)
		{
			return (!StoreKitEventListener.inAppsReadableNames.ContainsKey(purchaseId)) ? purchaseId : StoreKitEventListener.inAppsReadableNames[purchaseId];
		}

		private static string CurrentContextForNonePlaceInBecomePaying()
		{
			string text = string.Empty;
			try
			{
				if (Defs.inRespawnWindow)
				{
					text += " Killcam";
				}
				if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
				{
					text += " PlayerExists";
				}
				if (NetworkStartTableNGUIController.IsEndInterfaceShown())
				{
					text += " NetworkStartTable_End";
				}
				if (NetworkStartTableNGUIController.IsStartInterfaceShown())
				{
					text += " NetworkStartTable_Start";
				}
				if (ShopNGUIController.GuiActive)
				{
					text += " InShop";
				}
				string text2 = (FlurryPluginWrapper.ModeNameForPurchasesAnalytics() ?? string.Empty).Replace(" ", string.Empty);
				if (text2 != string.Empty)
				{
					text = text + " " + text2;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in CurrentContextForNonePlaceInBecomePaying: " + ex);
			}
			return text;
		}

		public static void LogBecomePaying(string purchaseId)
		{
			try
			{
				string @string = PlayerPrefs.GetString(Defs.DateOfInstallAppForInAppPurchases041215, string.Empty);
				DateTime result;
				if (string.IsNullOrEmpty(@string) || !DateTime.TryParse(@string, out result))
				{
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				string string2 = PlayerPrefs.GetString(Defs.FirstInAppPurchaseDate_041215, string.Empty);
				DateTime result2;
				if (string.IsNullOrEmpty(string2) || !DateTime.TryParse(string2, out result2))
				{
					DateTime utcNow = DateTime.UtcNow;
					PlayerPrefs.SetString(Defs.FirstInAppPurchaseDate_041215, utcNow.ToString("s"));
					dictionary.Add("First Purchase (Day)", NumberOfDaysForBecomePaying(result, utcNow).ToString());
					string empty = string.Empty;
					if (Application.loadedLevelName == Defs.MainMenuScene && MainMenuController.sharedController != null && MainMenuController.sharedController.InAdventureScreen)
					{
						empty = "Connect Scene Adventure";
					}
					else if (Application.loadedLevelName == "ConnectScene")
					{
						empty = "Connect Scene Multiplayer";
					}
					else if (Application.loadedLevelName == "ConnectSceneSandbox")
					{
						empty = "Connect Scene Sandbox";
					}
					else if (Application.loadedLevelName == "Clans")
					{
						empty = "Clans Premium Map";
					}
					else if (Initializer.Instance != null && Defs.isInet && Defs.isMulti && !PhotonNetwork.connected && !NetworkStartTableNGUIController.IsStartInterfaceShown() && !NetworkStartTableNGUIController.IsEndInterfaceShown())
					{
						empty = "Disconnected";
					}
					else
					{
						empty = FlurryPluginWrapper.PlaceForPurchasesAnalytics();
						if (empty == "None")
						{
							empty = empty + " " + CurrentContextForNonePlaceInBecomePaying();
						}
					}
					dictionary.Add("Point First Purchase", empty);
					dictionary.Add("First Purchase Total", ReadableNameForInApp(purchaseId));
				}
				else
				{
					string string3 = PlayerPrefs.GetString(Defs.SecondInAppPurchaseDate_041215, string.Empty);
					DateTime result3;
					if (string.IsNullOrEmpty(string3) || !DateTime.TryParse(string3, out result3))
					{
						DateTime utcNow2 = DateTime.UtcNow;
						PlayerPrefs.SetString(Defs.SecondInAppPurchaseDate_041215, utcNow2.ToString("s"));
						dictionary.Add("Second Purchase (Day After First)", NumberOfDaysForBecomePaying(result2, utcNow2).ToString());
						dictionary.Add("Second Purchase (Day After Start)", NumberOfDaysForBecomePaying(result, utcNow2).ToString());
					}
				}
				dictionary.Add("Purchase Day Total", NumberOfDaysForBecomePaying(result, DateTime.UtcNow).ToString());
				if (Debug.isDebugBuild)
				{
					Debug.Log("<color=green>LogBecomePaying = Become Paying</color>\n<color=white>parameters = " + dictionary.ToStringFull() + "</color>");
				}
				FlurryPluginWrapper.LogEventAndDublicateToConsole("Become Paying", dictionary);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in LogBecomePaying: " + ex);
			}
		}

		public static void LogAfterTraining(string action, bool trainingState)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add((!trainingState) ? "Skipped" : "Completed", action ?? string.Empty);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("After Training", parameters);
		}

		public static void LogCoinsGained(string mode, int coinCount)
		{
			mode = mode ?? string.Empty;
			string value = ((!(ExperienceController.sharedController != null)) ? "Unknown" : ExperienceController.sharedController.currentLevel.ToString());
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Total", mode);
			dictionary.Add(mode + " (Rank)", value);
			Dictionary<string, string> dictionary2 = dictionary;
			if (coinCount >= 1000)
			{
				string eventName = "Coins Gained Suspiciously Large Amount";
				dictionary2.Add("Amount", coinCount.ToString());
				FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, dictionary2);
				return;
			}
			int num = coinCount;
			int num2 = 1;
			while (num > 0 && num2 < 100)
			{
				int num3 = num % 10;
				string eventName2 = string.Format("{0} x{1}", "Coins Gained", num2);
				for (int i = 0; i < num3; i++)
				{
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName2, dictionary2);
				}
				num /= 10;
				num2 *= 10;
			}
			if (num > 0)
			{
				string eventName3 = string.Format("{0} x{1}", "Coins Gained", 100);
				for (int j = 0; j < num; j++)
				{
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName3, dictionary2);
				}
			}
		}

		public static void LogGemsGained(string mode, int gemsCount)
		{
		}

		public static void LogTrainingProgress(string kind)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Kind", kind);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Training Progress", parameters);
		}

		public static string GetPlayingMode()
		{
			if (Application.loadedLevelName == Defs.MainMenuScene)
			{
				return "Main Menu";
			}
			if (Application.loadedLevelName.Equals("ConnectScene", StringComparison.OrdinalIgnoreCase))
			{
				return "Connect Scene";
			}
			if (Application.loadedLevelName.Equals("ConnectSceneSandbox", StringComparison.OrdinalIgnoreCase))
			{
				return "Connect Scene Sandbox";
			}
			if (!Defs.IsSurvival && !Defs.isMulti)
			{
				return "Campaign";
			}
			if (Defs.IsSurvival)
			{
				return (!Defs.isMulti) ? "Survival" : "Time Survival";
			}
			if (Defs.isCompany)
			{
				return "Team Battle";
			}
			if (Defs.isFlag)
			{
				return "Flag Capture";
			}
			if (Defs.isHunger)
			{
				return "Deadly Games";
			}
			if (Defs.isCapturePoints)
			{
				return "Capture Points";
			}
			return (!Defs.isInet) ? "Deathmatch Local" : "Deathmatch Worldwide";
		}

		internal static void StartLoggingGameModeEvent()
		{
			string gameModeEventName = GetGameModeEventName(GetPlayingMode());
			StartLoggingGameModeEvent(gameModeEventName);
		}

		internal static void StopLoggingGameModeEvent()
		{
			string[] source = new string[10] { "Main Menu", "Connect Scene", "Campaign", "Time Survival", "Survival", "Team Battle", "Flag Capture", "Deadly Games", "Deathmatch Worldwide", "Deathmatch Local" };
            IEnumerable<string> enumerable = source.Select<string, string>(GetGameModeEventName);
            foreach (string item in enumerable)
			{
				StopLoggingGameModeEvent(item);
			}
		}

		private static void StartLoggingGameModeEvent(string eventName)
		{
			FlurryPluginWrapper.LogTimedEventAndDublicateToConsole(eventName);
		}

		internal static void StopLoggingGameModeEvent(string eventName)
		{
			FlurryPluginWrapper.EndTimedEvent(eventName);
		}

		private static string GetGameModeEventName(string gameMode)
		{
			return "Game Mode " + gameMode;
		}
	}
}
