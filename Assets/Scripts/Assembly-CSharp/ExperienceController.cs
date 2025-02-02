using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class ExperienceController : MonoBehaviour
{
	public static readonly int[] MaxExpLevels = new int[32]
	{
		0, 15, 35, 50, 90, 100, 110, 115, 120, 125,
		130, 135, 140, 150, 160, 170, 180, 200, 220, 250,
		290, 340, 400, 470, 550, 640, 740, 850, 970, 1100,
		1240, 100000
	};

	public static readonly float[] HealthByLevel = new float[32]
	{
		0f, 9f, 10f, 10f, 11f, 11f, 12f, 13f, 13f, 14f,
		14f, 15f, 16f, 16f, 17f, 17f, 18f, 19f, 19f, 20f,
		20f, 21f, 22f, 22f, 23f, 23f, 24f, 25f, 25f, 26f,
		26f, 27f
	};

	public bool isMenu;

	public bool isConnectScene;

	public int currentLevelForEditor;

	public static int maxLevel = 31;

	public int[,] limitsLeveling = new int[6, 2]
	{
		{ 1, 6 },
		{ 7, 11 },
		{ 12, 16 },
		{ 17, 21 },
		{ 22, 26 },
		{ 27, 31 }
	};

	public static int[,] accessByLevels = new int[maxLevel, maxLevel];

	public Texture2D[] marks;

	private SaltedInt currentExperience = new SaltedInt(12512238, 0);

	private int[] _addCoinsFromLevels = new int[32]
	{
		0, 5, 10, 15, 20, 25, 25, 25, 30, 30,
		30, 35, 35, 40, 40, 40, 45, 45, 50, 50,
		50, 55, 55, 60, 60, 60, 65, 65, 70, 70,
		70, 0
	};

	private int[] _addGemsFromLevels = new int[32]
	{
		0, 4, 4, 5, 5, 6, 6, 7, 7, 8,
		8, 9, 9, 10, 10, 11, 11, 12, 12, 13,
		13, 14, 14, 15, 15, 16, 16, 17, 17, 18,
		18, 0
	};

	private static bool _storagerKeysInitialized = false;

	private bool _isShowRanks = true;

	public bool isShowNextPlashka;

	public Vector2 posRanks = Vector2.zero;

	private int oldCurrentExperience;

	public int oldCurrentLevel;

	public bool isShowAdd;

	public AudioClip exp_1;

	public AudioClip exp_2;

	public AudioClip exp_3;

	public AudioClip Tierup;

	public static ExperienceController sharedController;

	public int AddHealthOnCurLevel
	{
		get
		{
			int num = currentLevel;
			if (HealthByLevel.Length > num && num > 0)
			{
				return (int)(HealthByLevel[num] - HealthByLevel[num - 1]);
			}
			return 0;
		}
	}

	public int currentLevel
	{
		get
		{
			return currentLevelForEditor;
		}
		private set
		{
			bool flag = false;
			if (currentLevelForEditor != value)
			{
				flag = true;
			}
			currentLevelForEditor = value;
			if (value >= 4)
			{
				ReviewController.CheckActiveReview();
			}
			if (flag && ExperienceController.onLevelChange != null)
			{
				ExperienceController.onLevelChange();
			}
		}
	}

	public int[] addCoinsFromLevels
	{
		get
		{
			return _addCoinsFromLevels;
		}
	}

	public int[] addGemsFromLevels
	{
		get
		{
			return _addGemsFromLevels;
		}
	}

	public int CurrentExperience
	{
		get
		{
			return currentExperience.Value;
		}
	}

	public bool isShowRanks
	{
		get
		{
			return _isShowRanks;
		}
		set
		{
			_isShowRanks = value;
			if (ExpController.Instance != null)
			{
				ExpController.Instance.InterfaceEnabled = value;
			}
		}
	}

	public static event Action onLevelChange;

	public ExperienceController()
	{
		currentLevel = 1;
	}

	private static void InitializeStoragerKeysIfNeeded()
	{
		if (!_storagerKeysInitialized)
		{
			if (!Storager.hasKey("currentLevel1"))
			{
				Storager.setInt("currentLevel1", 1, true);
			}
			_storagerKeysInitialized = true;
		}
	}

	public static int GetCurrentLevelWithUpdateCorrection()
	{
		InitializeStoragerKeysIfNeeded();
		int num = GetCurrentLevel();
		if (num < maxLevel && Storager.getInt("currentExperience", false) >= MaxExpLevels[num])
		{
			num++;
		}
		return num;
	}

	public static int GetCurrentLevel()
	{
		int result = 1;
		for (int i = 1; i <= maxLevel; i++)
		{
			string key = "currentLevel" + i;
			if (Storager.getInt(key, true) == 1)
			{
				result = i;
				Storager.setInt(key, 1, true);
			}
		}
		return result;
	}

	public void Refresh()
	{
		currentExperience.Value = Storager.getInt("currentExperience", false);
		currentLevel = GetCurrentLevel();
	}

	private void AddCurrenciesForLevelUP()
	{
		int num = addGemsFromLevels[currentLevel - 1];
		BankController.canShowIndication = false;
		BankController.AddGems(num, false);
		FlurryEvents.LogGemsGained(FlurryEvents.GetPlayingMode(), num);
		StartCoroutine(BankController.WaitForIndicationGems(true));
		int num2 = addCoinsFromLevels[currentLevel - 1];
		BankController.AddCoins(num2, false);
		FlurryEvents.LogCoinsGained(FlurryEvents.GetPlayingMode(), num2);
		StartCoroutine(BankController.WaitForIndicationGems(false));
	}

	private void Awake()
	{
		sharedController = this;
	}

	public IEnumerable<float> InitController()
	{
		for (int i = 0; i < maxLevel; i++)
		{
			for (int d2 = 0; d2 < maxLevel; d2++)
			{
				accessByLevels[i, d2] = 0;
			}
		}
		for (int j = 0; j < maxLevel; j++)
		{
			for (int k = limitsLeveling.GetLowerBound(0); k <= limitsLeveling.GetUpperBound(0); k++)
			{
				int min = limitsLeveling[k, 0] - 1;
				int max = limitsLeveling[k, 1] - 1;
				if (j >= min && j <= max)
				{
					for (int d = min; d <= max; d++)
					{
						accessByLevels[j, d] = 1;
					}
					break;
				}
			}
		}
		yield return 0f;
		try
		{
			InitializeStoragerKeysIfNeeded();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			Refresh();
			if (currentLevel < maxLevel && currentExperience.Value >= MaxExpLevels[currentLevel])
			{
				currentExperience.Value = 0;
				currentLevel++;
				Storager.setInt("currentLevel" + currentLevel, 1, true);
				Storager.setInt("currentExperience", currentExperience.Value, false);
				BankController.GiveInitialNumOfCoins();
				AddCurrenciesForLevelUP();
			}
			isShowRanks = false;
		}
		catch (Exception ex2)
		{
			Exception ex = ex2;
			Debug.LogError("<<< ExperienceController.Start() failed.");
			Debug.LogException(ex);
		}
	}

	public void addExperience(int experience)
	{
		if (currentLevel == maxLevel)
		{
			return;
		}
		oldCurrentLevel = currentLevel;
		oldCurrentExperience = currentExperience.Value;
		if (currentLevel < maxLevel && experience >= MaxExpLevels[currentLevel] - currentExperience.Value + MaxExpLevels[currentLevel + 1])
		{
			experience = MaxExpLevels[currentLevel + 1] + MaxExpLevels[currentLevel] - currentExperience.Value - 5;
		}
		string key = "Statistics.ExpInMode.Level" + sharedController.currentLevel;
		if (PlayerPrefs.HasKey(key) && Initializer.lastGameMode != -1)
		{
			string key2 = Initializer.lastGameMode.ToString();
			string @string = PlayerPrefs.GetString(key, "{}");
			try
			{
				Dictionary<string, object> dictionary = (Json.Deserialize(@string) as Dictionary<string, object>) ?? new Dictionary<string, object>();
				object value;
				if (dictionary.TryGetValue(key2, out value))
				{
					int num = Convert.ToInt32(value) + experience;
					dictionary[key2] = num;
				}
				else
				{
					dictionary.Add(key2, experience);
				}
				string value2 = Json.Serialize(dictionary);
				PlayerPrefs.SetString(key, value2);
			}
			catch (OverflowException exception)
			{
				Debug.LogError("Cannot deserialize exp-in-mode: " + @string);
				Debug.LogException(exception);
			}
			catch (Exception exception2)
			{
				Debug.LogError("Unknown exception: " + @string);
				Debug.LogException(exception2);
			}
		}
		currentExperience.Value += experience;
		Storager.setInt("currentExperience", currentExperience.Value, false);
		if (currentLevel < maxLevel && currentExperience.Value >= MaxExpLevels[currentLevel])
		{
			DateTime utcNow = DateTime.UtcNow;
			string key3 = "Statistics.TimeInRank.Level" + (currentLevel + 1);
			PlayerPrefs.SetString(key3, utcNow.ToString("s"));
			string key4 = "Statistics.MatchCount.Level" + sharedController.currentLevel;
			int @int = PlayerPrefs.GetInt(key4, 0);
			string key5 = "Statistics.WinCount.Level" + sharedController.currentLevel;
			int int2 = PlayerPrefs.GetInt(key5, 0);
			FlurryPluginWrapper.LogEventToAppsFlyer("af_level_achieved", new Dictionary<string, string> { 
			{
				"af_level",
				currentLevel.ToString()
			} });
			currentExperience.Value -= MaxExpLevels[currentLevel];
			currentLevel++;
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted && currentLevel > 1)
			{
				TrainingController.CompletedTrainingStage = TrainingController.NewTrainingCompletedStage.FirstMatchCompleted;
				FlurryEvents.LogTutorial("24_Level Up");
				try
				{
					if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.playerWeapons != null)
					{
						IEnumerable<Weapon> source = WeaponManager.sharedManager.playerWeapons.OfType<Weapon>();
						if (source.FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 3) == null)
						{
							WeaponManager.sharedManager.EquipWeapon(WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>().First((Weapon w) => w.weaponPrefab.tag == WeaponTags.BASIC_FLAMETHROWER_Tag));
						}
						if (source.FirstOrDefault((Weapon w) => w.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1 == 5) == null)
						{
							WeaponManager.sharedManager.EquipWeapon(WeaponManager.sharedManager.allAvailablePlayerWeapons.OfType<Weapon>().First((Weapon w) => w.weaponPrefab.tag == WeaponTags.SignalPistol_Tag));
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception in gequipping flamethrower and rocketniza: " + ex);
				}
			}
			Storager.setInt("currentLevel" + currentLevel, 1, true);
			Storager.setInt("currentExperience", currentExperience.Value, false);
			ShopNGUIController.SynchronizeAndroidPurchases("Current level: " + currentLevel);
			BankController.GiveInitialNumOfCoins();
			AddCurrenciesForLevelUP();
			FriendsController.sharedController.rank = currentLevel;
			FriendsController.sharedController.SendOurData();
			FriendsController.sharedController.UpdatePopularityMaps();
		}
		if (Defs.isSoundFX)
		{
			NGUITools.PlaySound(exp_1);
		}
		if (ExpController.Instance != null)
		{
			ExpController.Instance.AddExperience(oldCurrentLevel, oldCurrentExperience, experience, exp_2, exp_3, Tierup);
		}
	}

	private void HideNextPlashka()
	{
		isShowNextPlashka = false;
		isShowAdd = false;
	}

	private void DoOnGUI()
	{
	}

	public static void SetEnable(bool enable)
	{
		if (!(sharedController == null))
		{
			sharedController.isShowRanks = enable;
		}
	}
}
