using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Prime31;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

[RequireComponent(typeof(FrameStopwatchScript))]
internal sealed class Switcher : MonoBehaviour
{
	internal const string AbuseMethodKey = "AbuseMethod";

	public static Dictionary<string, int> sceneNameToGameNum;

	public static Dictionary<string, int> counCreateMobsInLevel;

	public static string LoadingInResourcesPath;

	public static string[] loadingNames;

	public GameObject coinsShopPrefab;

	public GameObject nickStackPrefab;

	public GameObject skinsManagerPrefab;

	public GameObject ExperienceControllerPrefab;

	public GameObject weaponManagerPrefab;

	public GameObject flurryPrefab;

	public GameObject experienceGuiPrefab;

	public GameObject bankGuiPrefab;

	public GameObject freeAwardGuiPrefab;

	public GameObject buttonClickSoundPrefab;

	public GameObject faceBookControllerPrefab;

	public GameObject licenseVerificationControllerPrefab;

	public GameObject potionsControllerPrefab;

	public GameObject protocolListGetterPrefab;

	public GameObject updateCheckerPrefab;

	public GameObject promoActionsManagerPrefab;

	public GameObject starterPackManagerPrefab;

	public GameObject tempItemsControllerPrefab;

	public GameObject remotePushNotificationControllerPrefab;

	public GameObject premiumAccountControllerPrefab;

	public GameObject appsFlyerTrackerCallbacksPrefab;

	public GameObject twitterControllerPrefab;

	public GameObject sponsorPayPluginHolderPrefab;

	public GameObject giftControllerPrefab;

	public GameObject disabler;

	public GameObject sceneInfoController;

	private Rect plashkaCoinsRect;

	private Texture fonToDraw;

	private bool _newLaunchingApproach;

	public static Stopwatch timer;

	private static bool _initialAppVersionInitialized;

	private static string _InitialAppVersion;

	public static GameObject comicsSound;

	private float _progress;

	private static AbuseMetod? _abuseMethod;

	public static string InitialAppVersion
	{
		get
		{
			if (!_initialAppVersionInitialized)
			{
				_InitialAppVersion = PlayerPrefs.GetString(Defs.InitialAppVersionKey);
				_initialAppVersionInitialized = true;
			}
			return _InitialAppVersion;
		}
		private set
		{
			_InitialAppVersion = value;
			_initialAppVersionInitialized = true;
		}
	}

	internal static AbuseMetod AbuseMethod
	{
		get
		{
			if (!_abuseMethod.HasValue)
			{
				_abuseMethod = (AbuseMetod)Storager.getInt("AbuseMethod", false);
			}
			return _abuseMethod.Value;
		}
	}

	static Switcher()
	{
		sceneNameToGameNum = new Dictionary<string, int>();
		counCreateMobsInLevel = new Dictionary<string, int>();
		LoadingInResourcesPath = "LevelLoadings";
		loadingNames = new string[17]
		{
			"Loading_coliseum", "loading_Cementery", "Loading_Maze", "Loading_City", "Loading_Hospital", "Loading_Jail", "Loading_end_world_2", "Loading_Arena", "Loading_Area52", "Loading_Slender",
			"Loading_Hell", "Loading_bloody_farm", "Loading_most", "Loading_school", "Loading_utopia", "Loading_sky", "Loading_winter"
		};
		timer = new Stopwatch();
		_initialAppVersionInitialized = false;
		_InitialAppVersion = string.Empty;
		sceneNameToGameNum.Add("Training", 0);
		sceneNameToGameNum.Add("Cementery", 1);
		sceneNameToGameNum.Add("Maze", 2);
		sceneNameToGameNum.Add("City", 3);
		sceneNameToGameNum.Add("Hospital", 4);
		sceneNameToGameNum.Add("Jail", 5);
		sceneNameToGameNum.Add("Gluk_2", 6);
		sceneNameToGameNum.Add("Arena", 7);
		sceneNameToGameNum.Add("Area52", 8);
		sceneNameToGameNum.Add("Slender", 9);
		sceneNameToGameNum.Add("Castle", 10);
		sceneNameToGameNum.Add("Farm", 11);
		sceneNameToGameNum.Add("Bridge", 12);
		sceneNameToGameNum.Add("School", 13);
		sceneNameToGameNum.Add("Utopia", 14);
		sceneNameToGameNum.Add("Sky_islands", 15);
		sceneNameToGameNum.Add("Winter", 16);
		sceneNameToGameNum.Add("Swamp_campaign3", 17);
		sceneNameToGameNum.Add("Castle_campaign3", 18);
		sceneNameToGameNum.Add("Space_campaign3", 19);
		sceneNameToGameNum.Add("Parkour", 20);
		sceneNameToGameNum.Add("Code_campaign3", 21);
		counCreateMobsInLevel.Add("Farm", 10);
		counCreateMobsInLevel.Add("Cementery", 15);
		counCreateMobsInLevel.Add("City", 20);
		counCreateMobsInLevel.Add("Hospital", 25);
		counCreateMobsInLevel.Add("Bridge", 25);
		counCreateMobsInLevel.Add("Jail", 30);
		counCreateMobsInLevel.Add("Slender", 30);
		counCreateMobsInLevel.Add("Area52", 35);
		counCreateMobsInLevel.Add("School", 35);
		counCreateMobsInLevel.Add("Utopia", 25);
		counCreateMobsInLevel.Add("Maze", 30);
		counCreateMobsInLevel.Add("Sky_islands", 30);
		counCreateMobsInLevel.Add("Winter", 30);
		counCreateMobsInLevel.Add("Castle", 35);
		counCreateMobsInLevel.Add("Gluk_2", 35);
		counCreateMobsInLevel.Add("Swamp_campaign3", 30);
		counCreateMobsInLevel.Add("Castle_campaign3", 35);
		counCreateMobsInLevel.Add("Space_campaign3", 25);
		counCreateMobsInLevel.Add("Parkour", 15);
		counCreateMobsInLevel.Add("Code_campaign3", 35);
	}

	internal static IEnumerable<float> InitializeStorager()
	{
		float progress = 0f;
		if (Application.isEditor)
		{
			if (!PlayerPrefs.HasKey(Defs.initValsInKeychain15))
			{
				Storager.setString(Defs.CapeEquppedSN, Defs.CapeNoneEqupped, false);
				Storager.setString(Defs.HatEquppedSN, Defs.HatNoneEqupped, false);
				yield return progress;
			}
			if (!PlayerPrefs.HasKey(Defs.initValsInKeychain46))
			{
				Storager.setString("MaskEquippedSN", "MaskNoneEquipped", false);
				yield return progress;
			}
		}
		if (!Storager.hasKey(Defs.initValsInKeychain15))
		{
			Storager.setInt(Defs.initValsInKeychain15, 0, false);
			Storager.setInt(Defs.LobbyLevelApplied, 1, false);
			Storager.setString(Defs.CapeEquppedSN, Defs.CapeNoneEqupped, false);
			Storager.setString(Defs.HatEquppedSN, Defs.HatNoneEqupped, false);
			Storager.setInt(Defs.IsFirstLaunchFreshInstall, 1, false);
			yield return progress;
		}
		else if (Storager.getInt(Defs.LobbyLevelApplied, false) == 0)
		{
			Storager.setInt(Defs.ShownLobbyLevelSN, 3, false);
		}
		if (!Storager.hasKey(Defs.IsFirstLaunchFreshInstall))
		{
			Storager.setInt(Defs.IsFirstLaunchFreshInstall, 0, false);
		}
		progress = 0.25f;
		if (Application.isEditor || (Application.platform == RuntimePlatform.IPhonePlayer && UnityEngine.Debug.isDebugBuild) || (Application.platform == RuntimePlatform.IPhonePlayer && !Storager.hasKey(Defs.initValsInKeychain17)))
		{
			Storager.setInt(Defs.initValsInKeychain17, 0, false);
			PlayerPrefs.SetFloat(value: SecondsFrom1970(), key: Defs.TimeFromWhichShowEnder_SN);
		}
		if (Application.isEditor && !PlayerPrefs.HasKey(Defs.initValsInKeychain27))
		{
			Storager.setString(Defs.BootsEquppedSN, Defs.BootsNoneEqupped, false);
		}
		if (!Storager.hasKey(Defs.initValsInKeychain27))
		{
			Storager.setInt(Defs.initValsInKeychain27, 0, false);
			Storager.setString(Defs.BootsEquppedSN, Defs.BootsNoneEqupped, false);
			yield return progress;
		}
		progress = 0.5f;
		yield return progress;
		if (!Storager.hasKey(Defs.initValsInKeychain40))
		{
			Storager.setInt(Defs.initValsInKeychain40, 0, false);
			Storager.setString(Defs.ArmorNewEquppedSN, Defs.ArmorNewNoneEqupped, false);
			Storager.setInt("GrenadeID", 5, false);
			yield return progress;
		}
		if (!Storager.IsInitialized(Defs.initValsInKeychain41))
		{
			Storager.setInt(Defs.initValsInKeychain41, 0, false);
			string hatBought = null;
			string visualHatArmor = null;
			if (Storager.getInt("hat_Almaz_1", false) > 0)
			{
				hatBought = "hat_Army_3";
				Storager.setInt("hat_Almaz_1", 0, false);
				Storager.setInt("hat_Royal_1", 0, false);
				Storager.setInt("hat_Steel_1", 0, false);
				visualHatArmor = "hat_Almaz_1";
				yield return progress;
			}
			else if (Storager.getInt("hat_Royal_1", false) > 0)
			{
				hatBought = "hat_Army_2";
				Storager.setInt("hat_Royal_1", 0, false);
				Storager.setInt("hat_Steel_1", 0, false);
				visualHatArmor = "hat_Royal_1";
				yield return progress;
			}
			else if (Storager.getInt("hat_Steel_1", false) > 0)
			{
				hatBought = "hat_Army_1";
				Storager.setInt("hat_Steel_1", 0, false);
				visualHatArmor = "hat_Steel_1";
				yield return progress;
			}
			if (hatBought != null)
			{
				string hatEquipped = Storager.getString(Defs.HatEquppedSN, false);
				if (hatEquipped.Equals("hat_Almaz_1") || hatEquipped.Equals("hat_Royal_1") || hatEquipped.Equals("hat_Steel_1"))
				{
					Storager.setString(Defs.HatEquppedSN, hatBought, false);
				}
				for (int i = 0; i <= Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].IndexOf(hatBought); i++)
				{
					Storager.setInt(Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0][i], 1, false);
					yield return progress;
				}
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				Storager.setString(Defs.VisualHatArmor, string.Empty, false);
			}
			if (visualHatArmor != null)
			{
				Storager.setString(Defs.VisualHatArmor, visualHatArmor, false);
			}
			if (!Storager.hasKey("LikeID"))
			{
				Storager.setInt("LikeID", 5, false);
			}
			yield return progress;
			string armorBought = null;
			string visualArmor = null;
			if (Storager.getInt("Armor_Almaz_1", false) > 0)
			{
				armorBought = "Armor_Army_3";
				Storager.setInt("Armor_Almaz_1", 0, false);
				Storager.setInt("Armor_Royal_1", 0, false);
				Storager.setInt("Armor_Steel_1", 0, false);
				visualArmor = "Armor_Almaz_1";
				yield return progress;
			}
			else if (Storager.getInt("Armor_Royal_1", false) > 0)
			{
				armorBought = "Armor_Army_2";
				Storager.setInt("Armor_Royal_1", 0, false);
				Storager.setInt("Armor_Steel_1", 0, false);
				visualArmor = "Armor_Royal_1";
				yield return progress;
			}
			else if (Storager.getInt("Armor_Steel_1", false) > 0)
			{
				armorBought = "Armor_Army_1";
				Storager.setInt("Armor_Steel_1", 0, false);
				visualArmor = "Armor_Steel_1";
				yield return progress;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				Storager.setString(Defs.VisualArmor, string.Empty, false);
			}
			if (visualArmor != null)
			{
				Storager.setString(Defs.VisualArmor, visualArmor, false);
			}
			yield return progress;
			if (armorBought != null)
			{
				string armorEquipped = Storager.getString(Defs.ArmorNewEquppedSN, false);
				if (armorEquipped.Equals("Armor_Almaz_1") || armorEquipped.Equals("Armor_Royal_1") || armorEquipped.Equals("Armor_Steel_1"))
				{
					Storager.setString(Defs.ArmorNewEquppedSN, armorBought, false);
					yield return progress;
				}
				for (int j = 0; j <= Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0].IndexOf(armorBought); j++)
				{
					Storager.setInt(Wear.wear[ShopNGUIController.CategoryNames.ArmorCategory][0][j], 1, false);
					yield return progress;
				}
			}
		}
		progress = 0.75f;
		if (!Storager.IsInitialized(Defs.initValsInKeychain43))
		{
			Storager.SetInitialized(Defs.initValsInKeychain43);
			PlayerPrefs.SetString(Defs.StartTimeShowBannersKey, DateTimeOffset.UtcNow.ToString("s"));
			PlayerPrefs.Save();
			yield return progress;
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				Storager.setInt(Defs.NeedTakeMarathonBonus, 0, false);
				Storager.setInt(Defs.NextMarafonBonusIndex, 0, false);
				yield return progress;
			}
		}
		if (!Storager.hasKey(GearManager.MusicBox))
		{
			Storager.setInt(GearManager.MusicBox, 2, false);
			Storager.setInt(GearManager.Wings, 2, false);
			Storager.setInt(GearManager.Bear, 2, false);
			Storager.setInt(GearManager.BigHeadPotion, 2, false);
		}
		Defs.StartTimeShowBannersString = PlayerPrefs.GetString(Defs.StartTimeShowBannersKey, string.Empty);
		UnityEngine.Debug.Log(" StartTimeShowBannersString=" + Defs.StartTimeShowBannersString);
		if (!Storager.IsInitialized(Defs.initValsInKeychain44))
		{
			Storager.SetInitialized(Defs.initValsInKeychain44);
			if (Storager.hasKey(Defs.NextMarafonBonusIndex) && Storager.getInt(Defs.NextMarafonBonusIndex, false) == -1)
			{
				Storager.setInt(Defs.NextMarafonBonusIndex, 0, false);
			}
			yield return progress;
		}
		if (!Storager.IsInitialized(Defs.initValsInKeychain45))
		{
			Storager.SetInitialized(Defs.initValsInKeychain45);
			Storager.setInt(Defs.PremiumEnabledFromServer, 0, false);
			if (Storager.getInt("currentLevel2", true) == 0)
			{
				PlayerPrefs.SetString(Defs.DateOfInstallAppForInAppPurchases041215, DateTime.UtcNow.ToString("s"));
			}
			yield return progress;
		}
		if (!Storager.IsInitialized(Defs.initValsInKeychain46))
		{
			Storager.SetInitialized(Defs.initValsInKeychain46);
			Storager.setString("MaskEquippedSN", "MaskNoneEquipped", false);
			yield return progress;
		}
		if (!Storager.hasKey("Win Count Timestamp"))
		{
			Storager.setString("Win Count Timestamp", "{ \"1970-01-01\": 0 }", false);
		}
		if (!Storager.hasKey("StartTimeShowStarterPack"))
		{
			Storager.setString("StartTimeShowStarterPack", string.Empty, false);
			yield return progress;
		}
		if (!Storager.hasKey("TimeEndStarterPack"))
		{
			Storager.setString("TimeEndStarterPack", string.Empty, false);
			yield return progress;
		}
		if (!Storager.hasKey("NextNumberStarterPack"))
		{
			Storager.setInt("NextNumberStarterPack", 0, false);
			yield return progress;
		}
		if (!Storager.hasKey(Defs.ArmorEquppedSN))
		{
			Storager.setString(Defs.ArmorEquppedSN, Defs.ArmorNoneEqupped, false);
		}
		if (!Storager.hasKey(Defs.ShowSorryWeaponAndArmor))
		{
			Storager.setInt(Defs.ShowSorryWeaponAndArmor, 0, false);
		}
		if (Storager.getInt(Defs.IsFirstLaunchFreshInstall, false) > 0)
		{
			Storager.setInt(Defs.IsFirstLaunchFreshInstall, 0, false);
		}
		if (!Storager.hasKey(Defs.NewbieEventX3StartTime))
		{
			Storager.setString(Defs.NewbieEventX3StartTime, 0L.ToString(), false);
			Storager.setString(Defs.NewbieEventX3StartTimeAdditional, 0L.ToString(), false);
			Storager.setString(Defs.NewbieEventX3LastLoggedTime, 0L.ToString(), false);
			PlayerPrefs.SetInt(Defs.WasNewbieEventX3, 0);
		}
		if (!PlayerPrefs.HasKey(Defs.LastTimeUpdateAvailableShownSN))
		{
			DateTime myDate1 = new DateTime(1970, 1, 9, 0, 0, 0);
			DateTimeOffset _1970 = new DateTimeOffset(myDate1);
			PlayerPrefs.SetString(Defs.LastTimeUpdateAvailableShownSN, _1970.ToString("s"));
			PlayerPrefs.Save();
		}
		string lastTimeUpdateShownString = PlayerPrefs.GetString(Defs.LastTimeUpdateAvailableShownSN);
		DateTimeOffset lastTimeUpdateShown = default(DateTimeOffset);
		if (!DateTimeOffset.TryParse(lastTimeUpdateShownString, out lastTimeUpdateShown) && UnityEngine.Debug.isDebugBuild)
		{
			UnityEngine.Debug.LogWarning("Cannot parse " + lastTimeUpdateShownString);
		}
		if (DateTimeOffset.Now - lastTimeUpdateShown > TimeSpan.FromHours(12.0))
		{
			PlayerPrefs.SetInt(Defs.UpdateAvailableShownTimesSN, 3);
			PlayerPrefs.SetString(Defs.LastTimeUpdateAvailableShownSN, DateTimeOffset.Now.ToString("s"));
			yield return progress;
		}
		float eventX3ShowTimeoutHours = 12f;
		if (!PlayerPrefs.HasKey(Defs.EventX3WindowShownLastTime))
		{
			PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, 1);
			PlayerPrefs.SetString(Defs.EventX3WindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
			yield return progress;
		}
		long eventX3WindowShownLastTime;
		long.TryParse(PlayerPrefs.GetString(Defs.EventX3WindowShownLastTime), out eventX3WindowShownLastTime);
		if (PromoActionsManager.CurrentUnixTime - eventX3WindowShownLastTime > (long)TimeSpan.FromHours(eventX3ShowTimeoutHours).TotalSeconds)
		{
			PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, 1);
			PlayerPrefs.SetString(Defs.EventX3WindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
		}
		PlayerPrefs.Save();
		yield return progress;
		float advertShowTimeoutHours = 12f;
		if (!PlayerPrefs.HasKey(Defs.AdvertWindowShownLastTime))
		{
			PlayerPrefs.SetInt(Defs.AdvertWindowShownCount, 3);
			PlayerPrefs.SetString(Defs.AdvertWindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
		}
		long advertWindowShownLastTime;
		long.TryParse(PlayerPrefs.GetString(Defs.AdvertWindowShownLastTime), out advertWindowShownLastTime);
		if (PromoActionsManager.CurrentUnixTime - advertWindowShownLastTime > (long)TimeSpan.FromHours(advertShowTimeoutHours).TotalSeconds)
		{
			PlayerPrefs.SetInt(Defs.AdvertWindowShownCount, 3);
			PlayerPrefs.SetString(Defs.AdvertWindowShownLastTime, PromoActionsManager.CurrentUnixTime.ToString());
		}
		yield return progress;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			if (!Storager.hasKey(Defs.LevelsWhereGetCoinS))
			{
				Storager.setString(Defs.LevelsWhereGetCoinS, string.Empty, false);
			}
			if (!Storager.hasKey(Defs.RatingFlag))
			{
				Storager.setInt(Defs.RatingDeathmatch, 0, false);
				Storager.setInt(Defs.RatingCOOP, 0, false);
				Storager.setInt(Defs.RatingTeamBattle, 0, false);
				Storager.setInt(Defs.RatingHunger, 0, false);
				Storager.setInt(Defs.RatingFlag, 0, false);
			}
			if (!Storager.hasKey(Defs.RatingCapturePoint))
			{
				Storager.setInt(Defs.RatingCapturePoint, 0, false);
			}
		}
		PlayerPrefs.Save();
		yield return 1f;
	}

	private static double Hypot(double x, double y)
	{
		x = Math.Abs(x);
		y = Math.Abs(y);
		double num = Math.Max(x, y);
		double num2 = Math.Min(x, y) / num;
		return num * Math.Sqrt(1.0 + num2 * num2);
	}

	private IEnumerator Start()
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			PlayComicsSound();
		}
		foreach (float item in InitializeSwitcher())
		{
			float step2 = item;
			ActivityIndicator.LoadingProgress = _progress;
			yield return step2;
		}
		foreach (float item2 in LoadMainMenu())
		{
			float step = item2;
			ActivityIndicator.LoadingProgress = _progress;
			yield return step;
		}
	}

	public static string LoadingCupTexture(int number)
	{
		return "loading_cups_" + number + ((!Device.isRetinaAndStrong) ? string.Empty : "-hd");
	}

	public IEnumerable<float> InitializeSwitcher()
	{
		Stopwatch _stopwatch = new Stopwatch();
		ProgressBounds bounds = new ProgressBounds();
		Action logBounds = delegate
		{
		};
		Action<string> log = delegate
		{
		};
		Func<float, long, string> format = delegate(float progress, long ms)
		{
			int num = Mathf.RoundToInt(progress * 100f);
			return string.Format(" << {0}%: {1}", num, ms);
		};
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			string bgTextureName2 = LoadingCupTexture(1);
			fonToDraw = Resources.Load<Texture>(bgTextureName2);
			ActivityIndicator.instance.legendLabel.text = LocalizationStore.Get("Key_1925");
			ActivityIndicator.instance.legendLabel.gameObject.SetActive(true);
		}
		else
		{
			string bgTextureName = ConnectSceneNGUIController.MainLoadingTexture();
			fonToDraw = Resources.Load<Texture>(bgTextureName);
		}
		ActivityIndicator.SetLoadingFon(fonToDraw);
		ActivityIndicator.IsShowWindowLoading = true;
		ActivityIndicator.instance.panelProgress.SetActive(true);
		bounds.SetBounds(0f, 0.09f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (!PlayerPrefs.HasKey("First Launch (Advertisement)"))
		{
			PlayerPrefs.SetString("First Launch (Advertisement)", DateTimeOffset.UtcNow.ToString("s"));
		}
		if (!PlayerPrefs.HasKey(Defs.InitialAppVersionKey))
		{
			if (!PlayerPrefs.HasKey("NamePlayer"))
			{
				PlayerPrefs.SetString(Defs.InitialAppVersionKey, GlobalGameController.AppVersion);
			}
			else
			{
				PlayerPrefs.SetString(Defs.InitialAppVersionKey, "1.0.0");
			}
		}
		InitialAppVersion = PlayerPrefs.GetString(Defs.InitialAppVersionKey);
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			AbstractManager.initialize(typeof(GoogleIABManager));
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			try
			{
				if (Defs.IsDeveloperBuild)
				{
					UnityEngine.Debug.Log("Switcher: Trying to initialize Google Play Games...");
				}
				PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
				PlayGamesPlatform.InitializeInstance(config);
				PlayGamesPlatform.DebugLogEnabled = Defs.IsDeveloperBuild && BuildSettings.BuildTargetPlatform == RuntimePlatform.Android;
				PlayGamesPlatform.Activate();
			}
			catch (Exception ex3)
			{
				Exception ex = ex3;
				UnityEngine.Debug.LogException(ex);
			}
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (sponsorPayPluginHolderPrefab != null)
		{
			UnityEngine.Object.Instantiate(sponsorPayPluginHolderPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		GlobalGameController.LeftHanded = PlayerPrefs.GetInt(Defs.LeftHandedSN, 1) == 1;
		if (!PlayerPrefs.HasKey(Defs.SwitchingWeaponsSwipeRegimSN))
		{
			double diagonalInPixels = Hypot(Screen.width, Screen.height);
			int switchingWeaponMode = 0;
			if (Screen.dpi > 0f)
			{
				double diagonalInInches = diagonalInPixels / (double)Screen.dpi;
				if (UnityEngine.Debug.isDebugBuild)
				{
					UnityEngine.Debug.Log(string.Format("Device dpi: {0},    diagonal: {1} px ({2}\")", Screen.dpi, diagonalInPixels, diagonalInInches));
				}
				switchingWeaponMode = ((!(diagonalInInches < 6.8)) ? 1 : 0);
			}
			else if (UnityEngine.Debug.isDebugBuild)
			{
				UnityEngine.Debug.Log(string.Format("Device dpi: {0},    diagonal: {1} px", Screen.dpi, diagonalInPixels));
			}
			PlayerPrefs.SetInt(Defs.SwitchingWeaponsSwipeRegimSN, switchingWeaponMode);
		}
		GlobalGameController.switchingWeaponSwipe = PlayerPrefs.GetInt(Defs.SwitchingWeaponsSwipeRegimSN, 0) == 1;
		GlobalGameController.ShowRec = PlayerPrefs.GetInt(Defs.ShowRecSN, 1) == 1;
		string oldV = Load.LoadString("keyOldVersion");
		string curV = GlobalGameController.AppVersion;
		if (oldV != curV)
		{
			PlayerPrefs.SetInt("countSessionDayOnStartCorrentVersion", PlayerPrefs.GetInt(Defs.SessionDayNumberKey, 1));
			ReviewController.IsSendReview = false;
			ReviewController.ExistReviewForSend = false;
			ReviewController.CheckActiveReview();
			Save.SaveString("keyOldVersion", curV);
		}
		Tools.AddSessionNumber();
		if (!Storager.hasKey(Defs.WeaponsGotInCampaign))
		{
			Storager.setString(Defs.WeaponsGotInCampaign, string.Empty, false);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		Screen.sleepTimeout = 180;
		if (PromoActionsManager.sharedManager == null && promoActionsManagerPrefab != null)
		{
			UnityEngine.Object.Instantiate(promoActionsManagerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (nickStackPrefab == null)
		{
			UnityEngine.Debug.LogError("Switcher.InitializeSwitcher():    nickStackPrefab == null");
		}
		else if (NickLabelStack.sharedStack == null)
		{
			UnityEngine.Object nicklabelStack = UnityEngine.Object.Instantiate(nickStackPrefab, Vector3.zero, Quaternion.identity);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (sceneInfoController == null)
		{
			UnityEngine.Debug.LogError("Switcher.InitializeSwitcher():    sceneInfoController == null");
		}
		else
		{
			UnityEngine.Object.Instantiate(sceneInfoController, Vector3.zero, Quaternion.identity);
		}
		if (ExperienceControllerPrefab == null)
		{
			UnityEngine.Debug.LogError("Switcher.InitializeSwitcher():    ExperienceControllerPrefab == null");
		}
		else if (ExperienceController.sharedController == null)
		{
			UnityEngine.Object experienceController = UnityEngine.Object.Instantiate(ExperienceControllerPrefab, Vector3.zero, Quaternion.identity);
			_progress = bounds.Lerp(_progress, 0.6f);
			yield return _progress;
			foreach (float item2 in ExperienceController.sharedController.InitController())
			{
				float step5 = item2;
				_progress = bounds.Clamp(_progress + 0.01f);
				yield return _progress;
			}
		}
		bounds.SetBounds(0.1f, 0.19f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (experienceGuiPrefab != null)
		{
			if (ExpController.Instance == null)
			{
				UnityEngine.Object expGui = UnityEngine.Object.Instantiate(experienceGuiPrefab, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(expGui);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("ExperienceGuiPrefab == null");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (bankGuiPrefab != null)
		{
			if (BankController.Instance == null)
			{
				UnityEngine.Object bankGui = UnityEngine.Object.Instantiate(bankGuiPrefab, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(bankGui);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("BankGuiPrefab == null");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (freeAwardGuiPrefab != null)
		{
			if (FreeAwardController.Instance == null)
			{
				UnityEngine.Object freeAwardGui = UnityEngine.Object.Instantiate(freeAwardGuiPrefab, Vector3.zero, Quaternion.identity);
				UnityEngine.Object.DontDestroyOnLoad(freeAwardGui);
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("freeAwardGuiPrefab == null");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (!GameObject.FindGameObjectWithTag("Flurry") && (bool)flurryPrefab)
		{
			UnityEngine.Object.Instantiate(flurryPrefab, Vector3.zero, Quaternion.identity);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (RemotePushNotificationController.Instance == null && (bool)remotePushNotificationControllerPrefab)
		{
			UnityEngine.Object.Instantiate(remotePushNotificationControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (Application.platform == RuntimePlatform.Android)
		{
			try
			{
				float startLoadingDex = Time.realtimeSinceStartup;
				if (Defs.IsDeveloperBuild)
				{
					UnityEngine.Debug.LogFormat("[Rilisoft] Loading classes.dex, frame: {0}, time: {1:0.000}s", Time.frameCount, startLoadingDex);
				}
				loaddex.loadDex();
				if (Defs.IsDeveloperBuild)
				{
					UnityEngine.Debug.LogFormat("[Rilisoft] Loaded classes.dex in {0:0.000}s", Time.realtimeSinceStartup - startLoadingDex);
				}
			}
			catch (Exception ex4)
			{
				Exception ex2 = ex4;
				UnityEngine.Debug.LogWarningFormat("[Rilisoft] Failed loading classes.dex: {0}", ex2.Message);
			}
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (ShopNGUIController.sharedShop == null)
		{
			ResourceRequest shopTask = Resources.LoadAsync("ShopNGUI");
			while (!shopTask.isDone)
			{
				yield return _progress;
			}
			UnityEngine.Object shopP = shopTask.asset;
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			UnityEngine.Object.Instantiate(shopP, Vector3.zero, Quaternion.identity);
		}
		bounds.SetBounds(0.2f, 0.29f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (SkinsController.sharedController == null && (bool)skinsManagerPrefab)
		{
			UnityEngine.Object.Instantiate(skinsManagerPrefab, Vector3.zero, Quaternion.identity);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			foreach (float item3 in SkinsController.sharedController.LoadSkinsInTexture())
			{
				float step4 = item3;
				yield return _progress;
			}
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (FriendsController.sharedController == null)
		{
			ResourceRequest friendsControllerTask = Resources.LoadAsync("FriendsController");
			while (!friendsControllerTask.isDone)
			{
				yield return _progress;
			}
			UnityEngine.Object fcp = friendsControllerTask.asset;
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			UnityEngine.Object.Instantiate(fcp, Vector3.zero, Quaternion.identity);
			yield return _progress;
			foreach (float item4 in FriendsController.sharedController.InitController())
			{
				float step3 = item4;
				yield return _progress;
			}
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		object token = new object();
		Storager.Initialize(token != null);
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			fonToDraw = Resources.Load<Texture>(LoadingCupTexture(2));
			foreach (float item5 in ActivityIndicator.instance.ReplaceLoadingFon(fonToDraw, 0.3f))
			{
				float step2 = item5;
				yield return _progress;
			}
			ActivityIndicator.instance.legendLabel.text = LocalizationStore.Get("Key_1926");
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		_stopwatch.Start();
		foreach (float item6 in InitializeStorager())
		{
			float storagerInitProgress = item6;
			if (_stopwatch.ElapsedMilliseconds > 100)
			{
				_stopwatch.Reset();
				_stopwatch.Start();
				yield return _progress;
			}
		}
		_stopwatch.Reset();
		BankController.GiveInitialNumOfCoins();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (disabler != null)
		{
			UnityEngine.Object.Instantiate(disabler);
		}
		bounds.SetBounds(0.3f, 0.39f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			List<string> weaponsForWhichSetRememberedTier = new List<string>();
			bool armorArmy1Comes;
			Storager.SynchronizeIosWithCloud(ref weaponsForWhichSetRememberedTier, out armorArmy1Comes);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			Storager.SyncWithCloud(Defs.SkinsMakerInProfileBought);
			Storager.SyncWithCloud(Defs.code010110_Key);
			Storager.SyncWithCloud(Defs.smallAsAntKey);
			Storager.SyncWithCloud(Defs.UnderwaterKey);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			foreach (KeyValuePair<ShopNGUIController.CategoryNames, List<List<string>>> item7 in Wear.wear)
			{
				foreach (List<string> ll in item7.Value)
				{
					foreach (string item in ll)
					{
						Storager.SyncWithCloud(item);
					}
				}
			}
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			foreach (KeyValuePair<string, string> kvp in InAppData.inAppData.Values)
			{
				if (Storager.getInt(kvp.Value, true) > 0)
				{
					Storager.setInt(kvp.Value, Storager.getInt(kvp.Value, true), true);
				}
			}
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			WeaponManager.RefreshLevelAndSetRememberedTiersFromCloud(weaponsForWhichSetRememberedTier);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			List<string> canBuyWeaponStorageIds = ItemDb.GetCanBuyWeaponStorageIds(true);
			_progress = bounds.Clamp(_progress + 0.01f);
			yield return _progress;
			_stopwatch.Start();
			for (int j = 0; j < canBuyWeaponStorageIds.Count; j++)
			{
				string storageId = canBuyWeaponStorageIds[j];
				if (!string.IsNullOrEmpty(storageId))
				{
					Storager.SyncWithCloud(storageId);
				}
				if (j % 100 == 0)
				{
					_progress = bounds.Clamp(_progress + 0.01f);
					yield return _progress;
					_stopwatch.Reset();
					_stopwatch.Start();
				}
				if (_stopwatch.ElapsedMilliseconds > 100)
				{
					yield return _progress;
					_stopwatch.Reset();
					_stopwatch.Start();
				}
			}
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			FlurryEvents.LogTutorial("1_First Launch");
		}
		bounds.SetBounds(0.4f, 0.49f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		CountMoneyForRemovedGear();
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		CountMoneyForArmorHats();
		if (Storager.hasKey(Defs.HatEquppedSN) && Storager.getString(Defs.HatEquppedSN, false) == "hat_ManiacMask")
		{
			Storager.setString(Defs.HatEquppedSN, ShopNGUIController.NoneEquippedForWearCategory(ShopNGUIController.CategoryNames.HatsCategory), false);
			Storager.setString("MaskEquippedSN", "hat_ManiacMask", false);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.hatName = ShopNGUIController.NoneEquippedForWearCategory(ShopNGUIController.CategoryNames.HatsCategory);
				FriendsController.sharedController.maskName = "hat_ManiacMask";
			}
		}
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		string[] _arr = Storager.getString(Defs.WeaponsGotInCampaign, false).Split('#');
		List<string> weaponsGotInCampaign = new List<string>();
		string[] array = _arr;
		foreach (string s2 in array)
		{
			weaponsGotInCampaign.Add(s2);
		}
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		foreach (string boxName in CampaignProgress.boxesLevelsAndStars.Keys)
		{
			foreach (string map in CampaignProgress.boxesLevelsAndStars[boxName].Keys)
			{
				string weaponFromBoss;
				if (LevelBox.weaponsFromBosses.TryGetValue(map, out weaponFromBoss) && !weaponsGotInCampaign.Contains(weaponFromBoss))
				{
					weaponsGotInCampaign.Add(weaponFromBoss);
				}
			}
		}
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		if (weaponsGotInCampaign.Contains(WeaponManager.ShotgunWN))
		{
			weaponsGotInCampaign[weaponsGotInCampaign.IndexOf(WeaponManager.ShotgunWN)] = WeaponManager.UZI_WN;
		}
		_progress = bounds.Clamp(_progress + 0.001f);
		yield return _progress;
		Storager.setString(val: string.Join("#", weaponsGotInCampaign.ToArray()), key: Defs.WeaponsGotInCampaign, useICloud: false);
		_progress = bounds.Clamp(0.41f);
		yield return _progress;
		if (coinsShop.thisScript == null && (bool)coinsShopPrefab)
		{
			UnityEngine.Object.Instantiate(coinsShopPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (FacebookController.sharedController == null && FacebookController.FacebookSupported && faceBookControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(faceBookControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (ButtonClickSound.Instance == null && buttonClickSoundPrefab != null)
		{
			UnityEngine.Object.Instantiate(buttonClickSoundPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.005f);
		yield return _progress;
		if (appsFlyerTrackerCallbacksPrefab != null)
		{
			UnityEngine.Object.Instantiate(appsFlyerTrackerCallbacksPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (licenseVerificationControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(licenseVerificationControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (TempItemsController.sharedController == null && tempItemsControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(tempItemsControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (updateCheckerPrefab != null)
		{
			UnityEngine.Object.Instantiate(updateCheckerPrefab);
		}
		bounds.SetBounds(0.5f, 0.52f);
		logBounds();
		_progress = bounds.LowerBound;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			yield return _progress;
			fonToDraw = Resources.Load<Texture>(LoadingCupTexture(3));
			foreach (float item8 in ActivityIndicator.instance.ReplaceLoadingFon(fonToDraw, 0.3f))
			{
				float step = item8;
				yield return _progress;
			}
			ActivityIndicator.instance.legendLabel.text = LocalizationStore.Get("Key_1927");
		}
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		if (TwitterController.Instance == null && twitterControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(twitterControllerPrefab);
		}
		yield return _progress;
		_progress = bounds.Clamp(_progress + 0.01f);
		GameObject o = (GameObject)UnityEngine.Object.Instantiate(weaponManagerPrefab, Vector3.zero, Quaternion.identity);
		WeaponManager wm = o.GetComponent<WeaponManager>();
		bounds.SetBounds(0.52f, 0.88f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		if (wm != null)
		{
			int i = 0;
			while (!wm.Initialized)
			{
				_progress = bounds.Clamp(_progress + 0.01f);
				yield return _progress;
				if (Launcher.UsingNewLauncher)
				{
					yield return -1f;
				}
				i++;
			}
		}
		yield return _progress;
		bounds.SetBounds(0.89f, 0.99f);
		logBounds();
		_progress = bounds.LowerBound;
		yield return _progress;
		SetUpPhoton();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		CheckHugeUpgrade();
		PerformEssentialInitialization("Coins", AbuseMetod.Coins);
		PerformEssentialInitialization("GemsCurrency", AbuseMetod.Gems);
		PerformExpendablesInitialization();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		CampaignProgress.OpenNewBoxIfPossible();
		CampaignProgress.SaveCampaignProgress();
		if (StarterPackController.Get == null && starterPackManagerPrefab != null)
		{
			UnityEngine.Object.Instantiate(starterPackManagerPrefab);
		}
		if (PotionsController.sharedController == null && potionsControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(potionsControllerPrefab);
		}
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		QuestSystem.Instance.Initialize();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		if (PremiumAccountController.Instance == null && premiumAccountControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(premiumAccountControllerPrefab);
		}
		Version initialAppVersion2 = new Version(1, 0, 0);
		try
		{
			initialAppVersion2 = new Version(PlayerPrefs.GetString(Defs.InitialAppVersionKey, "1.0.0"));
		}
		catch
		{
			goto IL_2cec;
		}
		if (!(initialAppVersion2 < new Version(8, 0, 0)))
		{
			DateTimeOffset now2 = DateTimeOffset.Now;
			DateTimeOffset lastLaunch;
			if (!PlayerPrefs.HasKey("Retention.LastLaunch") || !DateTimeOffset.TryParse(PlayerPrefs.GetString("Retention.LastLaunch"), out lastLaunch))
			{
				lastLaunch = now2;
			}
			string timeInRankKey = "Statistics.TimeInRank.Level" + 0;
			if (!PlayerPrefs.HasKey(timeInRankKey))
			{
				PlayerPrefs.SetString(timeInRankKey, DateTime.UtcNow.ToString("s"));
			}
			int rating;
			string ratingString = ((!Device.TryGetGpuRating(out rating)) ? "?" : rating.ToString());
			Dictionary<string, string> parameters15 = new Dictionary<string, string>
			{
				{
					"GPU",
					Device.FormatGpuModelMemoryRating()
				},
				{
					"Device",
					Device.FormatDeviceModelMemoryRating()
				},
				{ "Rating", ratingString }
			};
			if (UnityEngine.Debug.isDebugBuild || Defs.IsDeveloperBuild)
			{
				UnityEngine.Debug.Log("System Info : " + Rilisoft.MiniJson.Json.Serialize(parameters15));
			}
			else
			{
				FlurryPluginWrapper.LogEventAndDublicateToConsole("System Info", parameters15);
			}
			string launchAnalyticsPayingEvent = FlurryPluginWrapper.GetEventName("User Retention (Otval)");
			if (!PlayerPrefs.HasKey("Retention.FirstLaunch"))
			{
				PlayerPrefs.SetString("Retention.FirstLaunch", DateTimeOffset.Now.ToString("s"));
				Dictionary<string, string> parameters2 = new Dictionary<string, string> { { "On Day N", "0" } };
				FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters2);
				FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters2);
			}
			else
			{
				DateTimeOffset firstLaunch;
				if (!DateTimeOffset.TryParse(PlayerPrefs.GetString("Retention.FirstLaunch"), out firstLaunch))
				{
					firstLaunch = lastLaunch;
				}
				Func<int, int, bool> nowInRange = (int left, int right) => firstLaunch + TimeSpan.FromDays(left) <= now2 && now2 < firstLaunch + TimeSpan.FromDays(right);
				Func<int, bool> lastLaunchIsBefore = (int right) => lastLaunch < firstLaunch + TimeSpan.FromDays(right);
				Func<string, Dictionary<string, string>> formatParameters = (string value) => new Dictionary<string, string>
				{
					{ "On Day N", value },
					{
						string.Format("Levels on Day {0}", value),
						ExperienceController.sharedController.currentLevel.ToString()
					}
				};
				if (lastLaunch <= firstLaunch && nowInRange(0, 1))
				{
					Dictionary<string, string> parameters9 = formatParameters("0-1");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters9);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters9);
				}
				else if (lastLaunchIsBefore(1) && nowInRange(1, 3))
				{
					Dictionary<string, string> parameters10 = formatParameters("1-3");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters10);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters10);
				}
				else if (lastLaunchIsBefore(3) && nowInRange(3, 7))
				{
					Dictionary<string, string> parameters11 = formatParameters("3-7");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters11);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters11);
				}
				else if (lastLaunchIsBefore(7) && nowInRange(7, 14))
				{
					Dictionary<string, string> parameters12 = formatParameters("7-14");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters12);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters12);
				}
				else if (lastLaunchIsBefore(14) && nowInRange(14, 30))
				{
					Dictionary<string, string> parameters13 = formatParameters("14-30");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters13);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters13);
				}
				else if (lastLaunchIsBefore(30) && firstLaunch + TimeSpan.FromDays(30.0) <= now2)
				{
					Dictionary<string, string> parameters14 = formatParameters("30+");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval)", parameters14);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEvent, parameters14);
				}
				string launchAnalyticsPayingEventCumulative = FlurryPluginWrapper.GetEventName("User Retention (Otval, Cumulative)");
				if (nowInRange(0, 1))
				{
					Dictionary<string, string> parameters8 = formatParameters("0-1");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval, Cumulative)", parameters8);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEventCumulative, parameters8);
				}
				if (nowInRange(0, 3))
				{
					Dictionary<string, string> parameters7 = formatParameters("0-3");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval, Cumulative)", parameters7);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEventCumulative, parameters7);
				}
				if (nowInRange(0, 7))
				{
					Dictionary<string, string> parameters6 = formatParameters("0-7");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval, Cumulative)", parameters6);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEventCumulative, parameters6);
				}
				if (nowInRange(0, 14))
				{
					Dictionary<string, string> parameters5 = formatParameters("0-14");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval, Cumulative)", parameters5);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEventCumulative, parameters5);
				}
				if (nowInRange(0, 30))
				{
					Dictionary<string, string> parameters4 = formatParameters("0-30");
					FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval, Cumulative)", parameters4);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEventCumulative, parameters4);
				}
				Dictionary<string, string> parameters3 = formatParameters("0+");
				FlurryPluginWrapper.LogEventAndDublicateToConsole("User Retention (Otval, Cumulative)", parameters3);
				FlurryPluginWrapper.LogEventAndDublicateToConsole(launchAnalyticsPayingEventCumulative, parameters3);
			}
			PlayerPrefs.SetString("Retention.LastLaunch", now2.ToString("s"));
		}
		goto IL_2cec;
		IL_2cec:
		string lastLoggedDateString = PlayerPrefs.GetString("Statistics.WeaponPopularityTimestamp", "1970-01-01");
		DateTime lastLoggedDate;
		if (!DateTime.TryParse(lastLoggedDateString, out lastLoggedDate))
		{
			lastLoggedDate = new DateTime(1970, 1, 1);
		}
		DateTime now = DateTime.UtcNow.Date;
		if (now > lastLoggedDate)
		{
			string[] mostPopularWeapons = Statistics.Instance.GetMostPopularWeapons();
			if (mostPopularWeapons.Length > 0)
			{
				string eventName = FlurryPluginWrapper.GetEventName("Weapon Popularity");
				string[] array2 = mostPopularWeapons;
				foreach (string w in array2)
				{
					Dictionary<string, string> parameters = new Dictionary<string, string> { { "Favorite Weapon", w } };
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
				}
				PlayerPrefs.SetString("Statistics.WeaponPopularityTimestamp", now.ToString("yyyy-MM-dd"));
			}
		}
		LogWeaponAndArmorPopularityToFlurry();
		_progress = bounds.Clamp(_progress + 0.01f);
		yield return _progress;
		Storager.SyncWithCloud("PayingUser");
		Storager.SyncWithCloud(Defs.IsFacebookLoginRewardaGained);
		Storager.SyncWithCloud(Defs.IsTwitterLoginRewardaGained);
		foreach (string gochaGun in WeaponManager.GotchaGuns)
		{
			Storager.SyncWithCloud(gochaGun);
		}
		if (GiftController.instance == null && giftControllerPrefab != null)
		{
			UnityEngine.Object.Instantiate(giftControllerPrefab);
		}
		Screen.sleepTimeout = 180;
		_progress = 0.95f;
		yield return _progress;
	}

	private void SetUpPhoton()
	{
		string text = SelectPhotonAppId();
		if (Defs.IsDeveloperBuild)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("appId", text);
			dictionary.Add("defaultAppId", PhotonNetwork.PhotonServerSettings.AppID);
			Dictionary<string, string> dictionary2 = dictionary;
		}
		PhotonNetwork.PhotonServerSettings.AppID = text;
	}

	private static string SelectPhotonAppId()
	{
		HiddenSettings hiddenSettings = Resources.Load<HiddenSettings>("HiddenSettings");
		byte[] bytes = Convert.FromBase64String(hiddenSettings.PhotonAppIdSignaturePad);
		byte[] array = Convert.FromBase64String(hiddenSettings.PhotonAppIdSignatureEncoded);
		byte[] signatureHash = AndroidSystem.Instance.GetSignatureHash();
		byte[] bytes2 = Enumerable.Repeat(signatureHash, int.MaxValue).SelectMany((byte[] bs) => bs).Take(array.Length)
			.ToArray();
		byte[] array2 = new byte[36];
		new BitArray(bytes).Xor(new BitArray(array)).Xor(new BitArray(bytes2)).CopyTo(array2, 0);
		return Encoding.UTF8.GetString(array2, 0, array2.Length);
	}

	public static void PlayComicsSound()
	{
		if (!(comicsSound != null))
		{
			GameObject gameObject = Resources.Load<GameObject>("BackgroundMusic/Background_Comics");
			if (gameObject == null)
			{
				UnityEngine.Debug.LogWarning("ComicsSoundPrefab is null.");
				return;
			}
			comicsSound = UnityEngine.Object.Instantiate(gameObject);
			UnityEngine.Object.DontDestroyOnLoad(comicsSound);
		}
	}

	private static void CheckHugeUpgrade()
	{
		bool flag = Storager.hasKey("Coins");
		bool flag2 = Storager.hasKey(Defs.ArmorNewEquppedSN);
		if (flag && !flag2)
		{
			AppendAbuseMethod(AbuseMetod.UpgradeFromVulnerableVersion);
			UnityEngine.Debug.LogError("Upgrade tampering detected: " + AbuseMethod);
		}
	}

	private static void PerformEssentialInitialization(string currencyKey, AbuseMetod abuseMethod)
	{
		if (!Storager.hasKey(currencyKey))
		{
			return;
		}
		int @int = Storager.getInt(currencyKey, false);
		if (DigestStorager.Instance.ContainsKey(currencyKey))
		{
			if (!DigestStorager.Instance.Verify(currencyKey, @int))
			{
				AppendAbuseMethod(abuseMethod);
				UnityEngine.Debug.LogError("Currency tampering detected: " + AbuseMethod);
			}
		}
		else
		{
			DigestStorager.Instance.Set(currencyKey, @int);
		}
	}

	[Obsolete("Because of issues with CryptoPlayerPrefs")]
	private static void PerformWeaponInitialization()
	{
		IEnumerable<string> source = WeaponManager.storeIDtoDefsSNMapping.Values.Where((string w) => Storager.getInt(w, false) == 1);
		int value = source.Count();
		if (DigestStorager.Instance.ContainsKey("WeaponsCount"))
		{
			if (!DigestStorager.Instance.Verify("WeaponsCount", value))
			{
				AppendAbuseMethod(AbuseMetod.Weapons);
				UnityEngine.Debug.LogError("Weapon tampering detected: " + AbuseMethod);
			}
		}
		else
		{
			DigestStorager.Instance.Set("WeaponsCount", value);
		}
	}

	private static void PerformExpendablesInitialization()
	{
		string[] source = new string[4]
		{
			GearManager.InvisibilityPotion,
			GearManager.Jetpack,
			GearManager.Turret,
			GearManager.Mech
		};
		byte[] value = source.SelectMany((string key) => BitConverter.GetBytes(Storager.getInt(key, false))).ToArray();
		if (DigestStorager.Instance.ContainsKey("ExpendablesCount"))
		{
			if (!DigestStorager.Instance.Verify("ExpendablesCount", value))
			{
				AppendAbuseMethod(AbuseMetod.Expendables);
				UnityEngine.Debug.LogError("Expendables tampering detected: " + AbuseMethod);
			}
		}
		else
		{
			DigestStorager.Instance.Set("ExpendablesCount", value);
		}
	}

	private static void ClearProgress()
	{
	}

	public IEnumerable<float> LoadMainMenu()
	{
		if (!TrainingController.TrainingCompleted && ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel > 1)
		{
			Storager.setInt(Defs.TrainingCompleted_4_4_Sett, 1, false);
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			Defs.isFlag = false;
			Defs.isCOOP = false;
			Defs.isMulti = false;
			Defs.isHunger = false;
			Defs.isCompany = false;
			Defs.IsSurvival = false;
			Defs.isCapturePoints = false;
			GlobalGameController.Score = 0;
			WeaponManager.sharedManager.CurrentWeaponIndex = WeaponManager.sharedManager.playerWeapons.OfType<Weapon>().ToList().FindIndex((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 1);
		}
		string sceneName = ((TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0) ? DetermineSceneName() : Defs.TrainingSceneName);
		_progress = 0.96f;
		yield return _progress;
		AsyncOperation loadLevelTask = Application.LoadLevelAsync(sceneName);
		while (!loadLevelTask.isDone)
		{
			_progress = 0.96f + loadLevelTask.progress / 50f;
			yield return _progress;
		}
	}

	private static void LogWeaponAndArmorPopularityToFlurry()
	{
		LogPopularityToFlurry("Statistics.WeaponPopularityTimestamp", () => Statistics.Instance.GetMostPopularWeapons(), LogWeaponPopularityToFlurry);
		LogPopularityToFlurry("Statistics.WeaponPopularityForTierTimestamp", () => Statistics.Instance.GetMostPopularWeaponsForTier(ExpController.Instance.OurTier), LogWeaponPopularityForTierToFlurry);
		LogPopularityToFlurry("Statistics.ArmorPopularityTimestamp", () => Statistics.Instance.GetMostPopularArmors(), LogArmorPopularityToFlurry);
		LogPopularityToFlurry("Statistics.ArmorPopularityForTierTimestamp", () => Statistics.Instance.GetMostPopularArmorsForTier(ExpController.Instance.OurTier), LogArmorPopularityForTierToFlurry);
		LogPopularityToFlurry("Statistics.ArmorPopularityForLevelTimestamp", () => Statistics.Instance.GetMostPopularArmorsForLevel(ExperienceController.sharedController.currentLevel), LogArmorPopularityForLevelToFlurry);
	}

	private static void LogPopularityToFlurry(string loggedDateTimestampKey, Func<string[]> getMostPopular, Action<string[]> logMostPopular)
	{
		DateTime date = DateTime.UtcNow.Date;
		if (IsLastLoggedDateExpired(loggedDateTimestampKey, date))
		{
			string[] array = getMostPopular();
			if (array.Length > 0)
			{
				logMostPopular(array);
				PlayerPrefs.SetString(loggedDateTimestampKey, date.ToString("yyyy-MM-dd"));
			}
		}
	}

	private static bool IsLastLoggedDateExpired(string timestampKey, DateTime nowDate)
	{
		string @string = PlayerPrefs.GetString(timestampKey, "1970-01-01");
		DateTime result;
		if (!DateTime.TryParse(@string, out result))
		{
			result = new DateTime(1970, 1, 1);
		}
		return nowDate > result;
	}

	private static void LogWeaponPopularityToFlurry(string[] mostPopular)
	{
		string eventName = FlurryPluginWrapper.GetEventName("Weapon Popularity");
		foreach (string value in mostPopular)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Favorite Weapon", value);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
		}
	}

	private static void LogWeaponPopularityForTierToFlurry(string[] mostPopular)
	{
		int ourTier = ExpController.Instance.OurTier;
		string eventName = FlurryPluginWrapper.GetEventName("Weapon Popularity Tier");
		foreach (string value in mostPopular)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Tier " + ourTier, value);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
		}
	}

	private static void LogArmorPopularityToFlurry(string[] mostPopular)
	{
		string eventName = FlurryPluginWrapper.GetEventName("Armor Popularity");
		foreach (string value in mostPopular)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Name", value);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
		}
	}

	private static void LogArmorPopularityForTierToFlurry(string[] mostPopular)
	{
		int ourTier = ExpController.Instance.OurTier;
		string eventName = FlurryPluginWrapper.GetEventName("Armor Popularity Tier");
		foreach (string value in mostPopular)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Tier " + ourTier, value);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
		}
	}

	private static void LogArmorPopularityForLevelToFlurry(string[] mostPopular)
	{
		int currentLevel = ExperienceController.sharedController.currentLevel;
		string payingSuffix = FlurryPluginWrapper.GetPayingSuffix();
		int num = (currentLevel - 1) / 9;
		string arg = string.Format("[{0}, {1})", num * 9 + 1, (num + 1) * 9 + 1);
		string eventName = string.Format("Armor Popularity Level {0}{1}", arg, payingSuffix);
		foreach (string value in mostPopular)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Level " + currentLevel, value);
			Dictionary<string, string> parameters = dictionary;
			FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
		}
	}

	private static bool IsWeaponBought(string weaponTag)
	{
		string value;
		string value2;
		return WeaponManager.tagToStoreIDMapping.TryGetValue(weaponTag, out value) && value != null && WeaponManager.storeIDtoDefsSNMapping.TryGetValue(value, out value2) && value2 != null && Storager.hasKey(value2) && Storager.getInt(value2, true) > 0;
	}

	private static void CountMoneyForRemovedGear()
	{
		Storager.hasKey(Defs.GemsGivenRemovedGear);
		if (Storager.getInt(Defs.GemsGivenRemovedGear, false) != 0)
		{
			return;
		}
		Storager.hasKey(Defs.GearRemoved_GemsCountToCompensate);
		int num = 0;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add(GearManager.Turret, 5);
		dictionary.Add(GearManager.Mech, 7);
		dictionary.Add(GearManager.InvisibilityPotion, 3);
		dictionary.Add(GearManager.Jetpack, 4);
		Dictionary<string, int> dictionary2 = dictionary;
		foreach (string key in dictionary2.Keys)
		{
			num += Storager.getInt(key, false) * dictionary2[key];
		}
		Storager.setInt(Defs.GearRemoved_GemsCountToCompensate, Mathf.Max(0, num), false);
		Storager.setInt(Defs.GemsGivenRemovedGear, 1, false);
		foreach (string key2 in dictionary2.Keys)
		{
			Storager.setInt(key2, 0, false);
		}
	}

	private static void CountMoneyForGunsFrom831To901()
	{
		Storager.hasKey(Defs.CoinsCountToCompensate);
		Storager.hasKey(Defs.GemsCountToCompensate);
		Storager.hasKey(Defs.MoneyGiven831to901);
		Storager.SyncWithCloud(Defs.MoneyGiven831to901);
		Storager.hasKey(Defs.Weapons831to901);
		if (Storager.getInt(Defs.Weapons831to901, false) != 0)
		{
			return;
		}
		bool flag = Storager.getInt(Defs.MoneyGiven831to901, true) == 0;
		int num = 0;
		int num2 = 0;
		if (flag)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add(WeaponTags.CrossbowTag, 120);
			dictionary.Add(WeaponTags.CrystalCrossbowTag, 155);
			dictionary.Add(WeaponTags.SteelCrossbowTag, 120);
			dictionary.Add(WeaponTags.Bow_3_Tag, 185);
			dictionary.Add(WeaponTags.WoodenBowTag, 100);
			dictionary.Add(WeaponTags.Staff2Tag, 200);
			dictionary.Add(WeaponTags.Staff_3_Tag, 235);
			Dictionary<string, int> dictionary2 = dictionary;
			foreach (KeyValuePair<string, int> item in dictionary2)
			{
				string key = item.Key;
				int value = item.Value;
				if (IsWeaponBought(key))
				{
					num += value;
				}
			}
			dictionary = new Dictionary<string, int>();
			dictionary.Add(WeaponTags.AutoShotgun_Tag, 255);
			dictionary.Add(WeaponTags.TwoRevolvers_Tag, 267);
			dictionary.Add(WeaponTags.TwoBolters_Tag, 249);
			dictionary.Add(WeaponTags.SnowballGun_Tag, 281);
			Dictionary<string, int> dictionary3 = dictionary;
			foreach (KeyValuePair<string, int> item2 in dictionary3)
			{
				string key2 = item2.Key;
				int value2 = item2.Value;
				if (IsWeaponBought(key2))
				{
					num2 += value2;
				}
			}
			dictionary = new Dictionary<string, int>();
			dictionary.Add("cape_EliteCrafter", 50);
			dictionary.Add("cape_Archimage", 65);
			dictionary.Add("cape_BloodyDemon", 50);
			dictionary.Add("cape_SkeletonLord", 75);
			dictionary.Add("cape_RoyalKnight", 65);
			Dictionary<string, int> dictionary4 = dictionary;
			foreach (KeyValuePair<string, int> item3 in dictionary4)
			{
				string key3 = item3.Key;
				int value3 = item3.Value;
				if (Storager.hasKey(key3) && Storager.getInt(key3, false) != 0)
				{
					num += value3;
				}
			}
			dictionary = new Dictionary<string, int>();
			dictionary.Add("boots_gray", 50);
			dictionary.Add("boots_red", 50);
			dictionary.Add("boots_black", 100);
			dictionary.Add("boots_blue", 50);
			dictionary.Add("boots_green", 75);
			Dictionary<string, int> dictionary5 = dictionary;
			foreach (KeyValuePair<string, int> item4 in dictionary5)
			{
				string key4 = item4.Key;
				int value4 = item4.Value;
				if (Storager.hasKey(key4) && Storager.getInt(key4, false) != 0)
				{
					num += value4;
				}
			}
		}
		Storager.setInt(Defs.CoinsCountToCompensate, num, false);
		Storager.setInt(Defs.GemsCountToCompensate, num2, false);
		Storager.setInt(Defs.Weapons831to901, 1, false);
		Storager.setInt(Defs.MoneyGiven831to901, 1, true);
	}

	private static void CountMoneyForArmorHats()
	{
		Storager.hasKey("MoneyGivenRemovedArmorHat");
		Storager.SyncWithCloud("MoneyGivenRemovedArmorHat");
		Storager.hasKey("RemovedArmorHatMethodExecuted");
		if (Storager.getInt("RemovedArmorHatMethodExecuted", false) != 0)
		{
			return;
		}
		Storager.hasKey("RemovedArmorHat_CoinsCountToCompensate");
		bool flag = Storager.getInt("MoneyGivenRemovedArmorHat", true) == 0;
		int num = 0;
		if (flag)
		{
			foreach (string item2 in Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0])
			{
				if (Storager.getInt(item2, true) > 0)
				{
					num += VirtualCurrencyHelper.Price(item2).Price;
				}
			}
		}
		Storager.hasKey(Defs.HatEquppedSN);
		string item = Storager.getString(Defs.HatEquppedSN, false) ?? string.Empty;
		if (Wear.wear[ShopNGUIController.CategoryNames.HatsCategory][0].Contains(item))
		{
			Storager.setString(Defs.HatEquppedSN, ShopNGUIController.NoneEquippedForWearCategory(ShopNGUIController.CategoryNames.HatsCategory), false);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.hatName = ShopNGUIController.NoneEquippedForWearCategory(ShopNGUIController.CategoryNames.HatsCategory);
			}
		}
		Storager.setInt("RemovedArmorHat_CoinsCountToCompensate", num, false);
		Storager.setInt("RemovedArmorHatMethodExecuted", 1, false);
		Storager.setInt("MoneyGivenRemovedArmorHat", 1, true);
	}

	public static float SecondsFrom1970()
	{
		DateTime dateTime = new DateTime(1970, 1, 9, 0, 0, 0);
		DateTime now = DateTime.Now;
		return (float)(now - dateTime).TotalSeconds;
	}

	private void OnDestroy()
	{
		ActivityIndicator.IsShowWindowLoading = false;
	}

	private static string DetermineSceneName()
	{
		switch (GlobalGameController.currentLevel)
		{
		case -1:
			return Defs.MainMenuScene;
		case 0:
			return "Cementery";
		case 1:
			return "Maze";
		case 2:
			return "City";
		case 3:
			return "Hospital";
		case 4:
			return "Jail";
		case 5:
			return "Gluk_2";
		case 6:
			return "Arena";
		case 7:
			return "Area52";
		case 101:
			return "Training";
		case 8:
			return "Slender";
		case 9:
			return "Castle";
		default:
			return Defs.MainMenuScene;
		}
	}

	internal static void AppendAbuseMethod(AbuseMetod f)
	{
		_abuseMethod = AbuseMethod | f;
		AbuseMetod? abuseMethod = _abuseMethod;
		Storager.setInt("AbuseMethod", (int)abuseMethod.Value, false);
	}
}
