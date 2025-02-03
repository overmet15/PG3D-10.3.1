using System.IO;
using System.Text;
using Rilisoft;
using UnityEngine;

public class URLs
{
	public const string UrlForTwitterPost = "http://goo.gl/dQMf4n";

	public static string BanURL = "http://pixelserver.com/pixelgun3d-config/getBanList.php";

	private static readonly Lazy<string> _trafficForwardingConfigUrl = new Lazy<string>(InitializeTrafficForwardingConfigUrl);

	public static string Friends = "http://pixelserver.com/action";

	public static string PromoActions
	{
		get
		{
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				return (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "http://pixelserver.com/pixelgun3d-config/PromoActions/promo_actions_android.json" : "http://pixelserver.com/pixelgun3d-config/PromoActions/promo_actions_amazon.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/PromoActions/promo_actions_wp8.json";
			}
			return "http://pixelserver.com/pixelgun3d-config/PromoActions/promo_actions.json";
		}
	}

	public static string PromoActionsTest
	{
		get
		{
			return "http://pixelserver.com/pixelgun3d-config/PromoActions/promo_actions_test.json";
		}
	}

	public static string AmazonEvent
	{
		get
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return "http://pixelserver.com/pixelgun3d-config/amazonEvent/amazon-event-test.json";
			}
			if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
			{
				return "http://pixelserver.com/pixelgun3d-config/amazonEvent/amazon-event-test.json";
			}
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/amazonEvent/amazon-event-test.json";
			}
			return "http://pixelserver.com/pixelgun3d-config/amazonEvent/amazon-event.json";
		}
	}

	public static string QuestConfig
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/questConfig/quest-config-test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/questConfig/quest-config-ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/questConfig/quest-config-android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/questConfig/quest-config-amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/questConfig/quest-config-wp8.json";
			}
			return string.Empty;
		}
	}

	public static string TutorialQuestConfig
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return string.Format("http://pixelserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "test");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return string.Format("http://pixelserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "ios");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return string.Format("http://pixelserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "android");
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return string.Format("http://pixelserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "amazon");
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return string.Format("http://pixelserver.com/pixelgun3d-config/tutorial-quests/tutorial-quests-{0}.json", "wp8");
			}
			return string.Empty;
		}
	}

	public static string EventX3
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/event_x3_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/event_x3_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/event_x3_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/event_x3_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/event_x3_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string FilterBadWord
	{
		get
		{
			return "http://pixelserver.com/pixelgun3d-config/FilterBadWord.json";
		}
	}

	internal static string TrafficForwardingConfigUrl
	{
		get
		{
			return _trafficForwardingConfigUrl.Value;
		}
	}

	public static string PixelbookSettings
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_androd.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/PixelBookSettings/PixelBookSettings_wp.json";
			}
			return string.Empty;
		}
	}

	public static string BuffSettings
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/BuffSettings/BuffSettings_test" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/BuffSettings/BuffSettings_ios" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/BuffSettings/BuffSettings_android" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/BuffSettings/BuffSettings_amazon" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/BuffSettings/BuffSettings_WP" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
			}
			return string.Empty;
		}
	}

	public static string BuffSettings1031
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_test" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_ios" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_android" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_amazon" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/BuffSettings1031/BuffSettings_WP" + ((!FlurryPluginWrapper.IsPayingUser()) ? ".json" : "_paying.json");
			}
			return string.Empty;
		}
	}

	public static string LobbyNews
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/lobbyNews/LobbyNews_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/lobbyNews/LobbyNews_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/lobbyNews/LobbyNews_androd.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/lobbyNews/LobbyNews_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/lobbyNews/LobbyNews_wp.json";
			}
			return string.Empty;
		}
	}

	public static string Advert
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
				{
					return "http://pixelserver.com/pixelgun3d-config/advert/advert_ios_TEST.json";
				}
				if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
				{
					if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
					{
						return "http://pixelserver.com/pixelgun3d-config/advert/advert_amazon_TEST.json";
					}
					return "http://pixelserver.com/pixelgun3d-config/advert/advert_android_TEST.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/advert/advert_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/advert/advert_amazon.json";
				}
				return "http://pixelserver.com/pixelgun3d-config/advert/advert_android.json";
			}
			return string.Empty;
		}
	}

	public static string BestBuy
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/bestBuy/best_buy_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/bestBuy/best_buy_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/bestBuy/best_buy_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/bestBuy/best_buy_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/bestBuy/best_buy_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string DayOfValor
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/daysOfValor/days_of_valor_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/daysOfValor/days_of_valor_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/daysOfValor/days_of_valor_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/daysOfValor/days_of_valor_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/daysOfValor/days_of_valor_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string PremiumAccount
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "http://pixelserver.com/pixelgun3d-config/premiumAccount/premium_account_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "http://pixelserver.com/pixelgun3d-config/premiumAccount/premium_account_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "http://pixelserver.com/pixelgun3d-config/premiumAccount/premium_account_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "http://pixelserver.com/pixelgun3d-config/premiumAccount/premium_account_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "http://pixelserver.com/pixelgun3d-config/premiumAccount/premium_account_wp8.json";
			}
			return string.Empty;
		}
	}

	public static string PopularityMapUrl
	{
		get
		{
			int num = ExpController.GetOurTier() + 1;
			return "http://pixelserver.com/mapstats/" + GlobalGameController.MultiplayerProtocolVersion + "_" + (int)(ConnectSceneNGUIController.myPlatformConnect - 1) + "_" + num + "_mapstat.json";
		}
	}

	private static string InitializeTrafficForwardingConfigUrl()
	{
		if (Defs.IsDeveloperBuild)
		{
			return string.Format("http://pixelserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "test");
		}
		switch (BuildSettings.BuildTargetPlatform)
		{
		case RuntimePlatform.IPhonePlayer:
			return string.Format("http://pixelserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "ios");
		case RuntimePlatform.Android:
			switch (Defs.AndroidEdition)
			{
			case Defs.RuntimeAndroidEdition.GoogleLite:
				return string.Format("http://pixelserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "android");
			case Defs.RuntimeAndroidEdition.Amazon:
				return string.Format("http://pixelserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "amazon");
			default:
				return string.Empty;
			}
		case RuntimePlatform.MetroPlayerX64:
			return string.Format("http://pixelserver.com/pixelgun3d-config/trafficForwarding/traffic_forwarding_{0}.json", "wp8");
		default:
			return string.Empty;
		}
	}

	internal static string Sanitize(WWW request)
	{
		if (request == null)
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(request.error))
		{
			return string.Empty;
		}
		UTF8Encoding uTF8Encoding = new UTF8Encoding(false);
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64)
		{
			return uTF8Encoding.GetString(request.bytes, 0, request.bytes.Length).Trim();
		}
		using (StreamReader streamReader = new StreamReader(new MemoryStream(request.bytes), uTF8Encoding))
		{
			return streamReader.ReadToEnd().Trim();
		}
	}
}
