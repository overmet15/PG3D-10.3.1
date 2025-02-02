using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Rilisoft;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

public sealed class Initializer : MonoBehaviour
{
	public GameObject tc;

	public GameObject tempCam;

	public bool isDisconnect;

	public int countConnectToRoom;

	public float timerShowNotConnectToRoom;

	public UIButton buttonCancel;

	public UILabel descriptionLabel;

	public bool isCancelReConnect;

	private GameObject _playerPrefab;

	private UnityEngine.Object networkTablePref;

	private bool _isMultiplayer;

	public bool isNotConnectRoom;

	private Vector2 scrollPosition = Vector2.zero;

	private List<Vector3> _initPlayerPositions = new List<Vector3>();

	private List<float> _rots = new List<float>();

	public static List<Player_move_c> players = new List<Player_move_c>();

	public static List<Player_move_c> bluePlayers = new List<Player_move_c>();

	public static List<Player_move_c> redPlayers = new List<Player_move_c>();

	public static List<GameObject> playersObj = new List<GameObject>();

	public static List<GameObject> enemiesObj = new List<GameObject>();

	public static List<GameObject> turretsObj = new List<GameObject>();

	public static List<GameObject> chestsObj = new List<GameObject>();

	public static List<GameObject> damagedObj = new List<GameObject>();

	private float koofScreen = (float)Screen.height / 768f;

	public WeaponManager _weaponManager;

	public float timerShow = -1f;

	public Transform playerPrefab;

	public Texture fonLoadingScene;

	private bool showLoading;

	private bool isMulti;

	private bool isInet;

	private PauseONGuiDrawer _onGUIDrawer;

	private readonly IDictionary<string, int> _killedWithWeaponMap = new Dictionary<string, int>();

	public static int GameModeCampaign = 100;

	public static int GameModeSurvival = 101;

	public static int lastGameMode = -1;

	private Stopwatch _gameSessionStopwatch = new Stopwatch();

	public string goMapName = string.Empty;

	private bool _needReconnectShow;

	private bool _roomNotExistShow;

	public LoadingNGUIController _loadingNGUIController;

	private static readonly Lazy<string> _separator = new Lazy<string>(InitialiseSeparatorWrapper);

	public static Initializer Instance { get; private set; }

	internal static string Separator
	{
		get
		{
			return _separator.Value;
		}
	}

	public static event Action PlayerAddedEvent;

	private void ReportWeaponStatistics()
	{
		if (UnityEngine.Debug.isDebugBuild)
		{
			UnityEngine.Debug.Log("Killed with weapon:    " + Json.Serialize(_killedWithWeaponMap));
		}
		string eventName = FlurryPluginWrapper.GetEventName("Killed With Weapon Worldwide");
		string eventName2 = FlurryPluginWrapper.GetEventName("Killed With Weapon Worldwide (Tier 1-3)");
		string eventName3 = FlurryPluginWrapper.GetEventName("Killed With Weapon Worldwide (Tier 4-5)");
		int ourTier = ExpController.GetOurTier();
		string eventName4 = ((ourTier + 1 <= 3) ? eventName2 : eventName3);
		foreach (KeyValuePair<string, int> item in _killedWithWeaponMap)
		{
			int num = item.Value;
			int num2 = 1;
			while (num > 0 && num2 < 10)
			{
				int num3 = num % 10;
				string text = string.Format("{0}x", num2);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(text, item.Key);
				Dictionary<string, string> parameters = dictionary;
				dictionary = new Dictionary<string, string>();
				dictionary.Add(text, item.Key);
				dictionary.Add(string.Format("{0} (Tier {1})", text, ourTier), item.Key);
				Dictionary<string, string> parameters2 = dictionary;
				for (int i = 0; i < num3; i++)
				{
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName4, parameters2);
				}
				num /= 10;
				num2 *= 10;
			}
			if (num > 0)
			{
				string text2 = string.Format("{0}x", 10);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add(text2, item.Key);
				Dictionary<string, string> parameters3 = dictionary;
				dictionary = new Dictionary<string, string>();
				dictionary.Add(text2, item.Key);
				dictionary.Add(string.Format("{0} (Tier {1})", text2, ourTier), item.Key);
				Dictionary<string, string> parameters4 = dictionary;
				for (int j = 0; j < num; j++)
				{
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters3);
					FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName4, parameters4);
				}
			}
		}
	}

	public bool IncrementKillCountForWeapon(string weapon)
	{
		if (string.IsNullOrEmpty(weapon))
		{
			UnityEngine.Debug.LogError("Weapon must not be null or empty.");
			return false;
		}
		int value;
		bool flag = _killedWithWeaponMap.TryGetValue(weapon, out value);
		if (flag)
		{
			_killedWithWeaponMap[weapon] = value + 1;
		}
		else
		{
			_killedWithWeaponMap.Add(weapon, 1);
		}
		return flag;
	}

	private void Awake()
	{
		Instance = this;
		isMulti = Defs.isMulti;
		isInet = Defs.isInet;
		lastGameMode = -1;
		if (Device.IsLoweMemoryDevice)
		{
			ActivityIndicator.ClearMemory();
		}
		if (!Defs.IsSurvival && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted))
		{
			networkTablePref = Resources.Load("NetworkTable");
		}
		Defs.typeDisconnectGame = Defs.DisconectGameType.Reconnect;
		GameObject gameObject = null;
		if (Defs.isMulti)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(Application.loadedLevelName);
			if (infoScene != null)
			{
				GlobalGameController.currentLevel = infoScene.indexMap;
			}
			gameObject = Resources.Load("BackgroundMusic/BackgroundMusic_Level" + GlobalGameController.currentLevel) as GameObject;
		}
		else if (CurrentCampaignGame.currentLevel == 0)
		{
			string path = "BackgroundMusic/" + ((!Defs.IsSurvival) ? "Background_Training" : "BackgroundMusic_Level0");
			gameObject = Resources.Load(path) as GameObject;
		}
		else
		{
			gameObject = Resources.Load("BackgroundMusic/BackgroundMusic_Level" + CurrentCampaignGame.currentLevel) as GameObject;
		}
		if ((bool)gameObject)
		{
			UnityEngine.Object.Instantiate(gameObject);
		}
		if (!Defs.isMulti && !Defs.IsSurvival)
		{
			FlurryPluginWrapper.LogEventWithParameterAndValue("Campaign Progress", "Level Started", Application.loadedLevelName);
			StoreKitEventListener.State.PurchaseKey = "In game";
			StoreKitEventListener.State.Parameters.Clear();
			StoreKitEventListener.State.Parameters.Add("Level", Application.loadedLevelName + " In game");
			string[] array = Storager.getString(Defs.LevelsWhereGetCoinS, false).Split('#');
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string item in array2)
			{
				list.Add(item);
			}
			if (!list.Contains(Application.loadedLevelName) || (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None))
			{
				GameObject gameObject2 = GameObject.FindGameObjectWithTag("Configurator");
				CoinConfigurator component = gameObject2.GetComponent<CoinConfigurator>();
				if (component.CoinIsPresent)
				{
					Vector3 pos = ((!(component.coinCreatePoint == null)) ? component.coinCreatePoint.position : component.pos);
					CreateCoinAtPos(pos);
				}
			}
			lastGameMode = GameModeCampaign;
		}
		else if (!Defs.isMulti && Defs.IsSurvival)
		{
			FlurryPluginWrapper.LogEvent("Survival Started");
			lastGameMode = GameModeSurvival;
		}
		else if (Defs.isMulti)
		{
			FlurryPluginWrapper.LogEventWithParameterAndValue("Multiplayer Started", "Level", Application.loadedLevelName);
			lastGameMode = (int)ConnectSceneNGUIController.regim;
		}
		string abuseKey_d4d3cbab = GetAbuseKey_d4d3cbab(3570650027u);
		if (Storager.hasKey(abuseKey_d4d3cbab))
		{
			string @string = Storager.getString(abuseKey_d4d3cbab, false);
			if (!string.IsNullOrEmpty(@string) && @string != "0")
			{
				long num = DateTime.UtcNow.Ticks >> 1;
				long result = num;
				if (long.TryParse(@string, out result))
				{
					result = Math.Min(num, result);
					Storager.setString(abuseKey_d4d3cbab, result.ToString(), false);
				}
				else
				{
					Storager.setString(abuseKey_d4d3cbab, num.ToString(), false);
				}
				TimeSpan timeSpan = TimeSpan.FromTicks(num - result);
				bool anotherNeedApply = (Player_move_c.NeedApply = ((!Defs.IsDeveloperBuild) ? (timeSpan.TotalDays >= 1.0) : (timeSpan.TotalMinutes >= 3.0)));
				Player_move_c.AnotherNeedApply = anotherNeedApply;
			}
		}
		PhotonObjectCacher.AddObject(base.gameObject);
	}

	private void OnCompletedQuest(object sender, QuestCompletedEventArgs e)
	{
		InfoWindowController.ShowAchievementBox(string.Empty, QuestConstants.GetAccumulativeQuestDescriptionByType(e.Quest as AccumulativeQuestBase));
	}

	private static string GetAbuseKey_d4d3cbab(uint pad)
	{
		return (0x1039BA92u ^ pad).ToString("x");
	}

	public static GameObject CreateCoinAtPos(Vector3 pos)
	{
		GameObject original = Resources.Load("coin") as GameObject;
		return UnityEngine.Object.Instantiate(original, pos, Quaternion.Euler(270f, 0f, 0f)) as GameObject;
	}

	private void CheckRoom()
	{
		if (PhotonNetwork.room != null && (PhotonNetwork.room.maxPlayers < 2 || PhotonNetwork.room.maxPlayers > ((!Defs.isCOOP) ? 10 : 4)))
		{
			goToConnect();
		}
	}

	private void Start()
	{
		FriendsController.sharedController.profileInfo.Clear();
		FriendsController.sharedController.notShowAddIds.Clear();
		FacebookController.LogEvent("Campaign_ACHIEVED_LEVEL");
		Defs.inRespawnWindow = false;
		PlayerPrefs.SetInt("StartAfterDisconnect", 0);
		PhotonNetwork.isMessageQueueRunning = true;
		_isMultiplayer = Defs.isMulti;
		_weaponManager = WeaponManager.sharedManager;
		_weaponManager.players.Clear();
		CheckRoom();
		if (PhotonNetwork.room != null)
		{
			SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty].ToString()));
			goMapName = infoScene.NameScene;
			try
			{
				string text = ConnectSceneNGUIController.regim.ToString();
				string translateEngShortName = infoScene.TranslateEngShortName;
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Mode", text);
				dictionary.Add("Map", translateEngShortName);
				dictionary.Add(string.Format("Mode ({0})", text), translateEngShortName);
				dictionary.Add("Mode, Map", text + ", " + translateEngShortName);
				Dictionary<string, string> parameters = dictionary;
				string eventName = FlurryPluginWrapper.GetEventName("Maps Popularity Worldwide");
				FlurryPluginWrapper.LogEventAndDublicateToConsole(eventName, parameters);
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
			}
		}
		if (!_isMultiplayer)
		{
			_initPlayerPositions.Add(new Vector3(12f, 1f, 9f));
			_initPlayerPositions.Add(new Vector3(17f, 1f, -15f));
			_initPlayerPositions.Add(new Vector3(-42f, 1f, -10.487f));
			_initPlayerPositions.Add(new Vector3(0f, 1f, 19.5f));
			_initPlayerPositions.Add(new Vector3(-33f, 1.2f, -13f));
			_initPlayerPositions.Add(new Vector3(-2.67f, 1f, 2.67f));
			_initPlayerPositions.Add(new Vector3(0f, 1f, 0f));
			_initPlayerPositions.Add(new Vector3(19f, 1f, -0.8f));
			_initPlayerPositions.Add(new Vector3(-28.5f, 1.75f, -3.73f));
			_initPlayerPositions.Add(new Vector3(-2.5f, 1.75f, 0f));
			_initPlayerPositions.Add(new Vector3(-1.596549f, 2.5f, 2.684792f));
			_initPlayerPositions.Add(new Vector3(-6.611357f, 1.5f, -105.2573f));
			_initPlayerPositions.Add(new Vector3(-20.3f, 2f, 17.6f));
			_initPlayerPositions.Add(new Vector3(5f, 2.5f, 0f));
			_initPlayerPositions.Add(new Vector3(0f, 2.5f, 0f));
			_initPlayerPositions.Add(new Vector3(-7.3f, 3.6f, 6.46f));
			_initPlayerPositions.Add(new Vector3(17f, 1f, -15f));
			_initPlayerPositions.Add(new Vector3(17f, 1f, 0f));
			_initPlayerPositions.Add(new Vector3(0.2f, 11.2f, -0.28f));
			_initPlayerPositions.Add(new Vector3(-1.76f, 100.9f, 20.8f));
			_initPlayerPositions.Add(new Vector3(20f, -0.4f, 17f));
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(180f);
			_rots.Add(180f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(270f);
			_rots.Add(270f);
			_rots.Add(270f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(90f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(90f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(0f);
			_rots.Add(0f);
			int @int = Storager.getInt(Defs.EarnedCoins, false);
			if (@int > 0)
			{
				GameObject original = Resources.Load("MessageCoinsObject") as GameObject;
				UnityEngine.Object.Instantiate(original);
			}
			AddPlayer();
		}
		else
		{
			tc = UnityEngine.Object.Instantiate(tempCam, Vector3.zero, Quaternion.identity) as GameObject;
			if (!Defs.isInet)
			{
				if (PlayerPrefs.GetString("TypeGame").Equals("client"))
				{
					bool useNat = !Network.HavePublicAddress();
					Network.useNat = useNat;
					UnityEngine.Debug.Log(Defs.ServerIp + " " + Network.Connect(Defs.ServerIp, 25002));
				}
				else
				{
					_weaponManager.myTable = (GameObject)Network.Instantiate(networkTablePref, base.transform.position, base.transform.rotation, 0);
					_weaponManager.myNetworkStartTable = _weaponManager.myTable.GetComponent<NetworkStartTable>();
				}
			}
			else
			{
				_weaponManager.myTable = PhotonNetwork.Instantiate("NetworkTable", base.transform.position, base.transform.rotation, 0);
				if (_weaponManager.myTable != null)
				{
					_weaponManager.myNetworkStartTable = _weaponManager.myTable.GetComponent<NetworkStartTable>();
				}
				else
				{
					OnConnectionFail(DisconnectCause.TimeoutDisconnect);
				}
			}
		}
		FlurryEvents.StartLoggingGameModeEvent();
		_gameSessionStopwatch.Start();
	}

	[RPC]
	[PunRPC]
	private void SpawnOnNetwork(Vector3 pos, Quaternion rot, int id1, PhotonPlayer np)
	{
		if (networkTablePref != null)
		{
			Transform transform = UnityEngine.Object.Instantiate(networkTablePref, pos, rot) as Transform;
			PhotonView component = transform.GetComponent<PhotonView>();
			component.viewID = id1;
		}
	}

	private void AddPlayer()
	{
		_playerPrefab = Resources.Load<GameObject>("Player");
		Vector3 position;
		float y;
		if (Defs.IsSurvival)
		{
			if (Application.loadedLevelName.Equals("Arena_Underwater"))
			{
				position = new Vector3(0f, 3.5f, 0f);
				y = 0f;
			}
			else if (Application.loadedLevelName.Equals("Pizza"))
			{
				position = new Vector3(-32.48f, 2.46f, 2.01f);
				y = 90f;
			}
			else
			{
				position = new Vector3(0f, 2.5f, 0f);
				y = 0f;
			}
		}
		else if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			TrainingController trainingController = UnityEngine.Object.FindObjectOfType<TrainingController>();
			position = ((!(trainingController != null)) ? TrainingController.PlayerDefaultPosition : trainingController.PlayerDesiredPosition);
			y = 0f;
		}
		else
		{
			int index = Mathf.Max(0, CurrentCampaignGame.currentLevel - 1);
			position = ((CurrentCampaignGame.currentLevel != 0) ? _initPlayerPositions[index] : new Vector3(-0.72f, 1.75f, -13.23f));
			y = ((CurrentCampaignGame.currentLevel != 0) ? _rots[index] : 0f);
			GameObject gameObject = GameObject.FindGameObjectWithTag("PlayerRespawnPoint");
			if (gameObject != null)
			{
				position = gameObject.transform.position;
				y = gameObject.transform.rotation.eulerAngles.y;
			}
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(_playerPrefab, position, Quaternion.Euler(0f, y, 0f)) as GameObject;
		NickLabelController.currentCamera = gameObject2.GetComponent<SkinName>().camPlayer.GetComponent<Camera>();
		Invoke("SetupObjectThatNeedsPlayer", 0.01f);
	}

	[Obfuscation(Exclude = true)]
	public void SetupObjectThatNeedsPlayer()
	{
		if (Defs.isMulti)
		{
			if (Initializer.PlayerAddedEvent != null)
			{
				Initializer.PlayerAddedEvent();
			}
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("CoinBonus");
		if ((bool)gameObject)
		{
			CoinBonus component = gameObject.GetComponent<CoinBonus>();
			if ((bool)component)
			{
				component.SetPlayer();
			}
		}
		if (TrainingController.TrainingCompleted)
		{
			ZombieCreator.sharedCreator.BeganCreateEnemies();
		}
		GetComponent<BonusCreator>().BeginCreateBonuses();
		if (Initializer.PlayerAddedEvent != null)
		{
			Initializer.PlayerAddedEvent();
		}
	}

	private void ShowDescriptionLabel(string text)
	{
		descriptionLabel.gameObject.SetActive(true);
		descriptionLabel.text = text;
	}

	public void HideReconnectInterface()
	{
		descriptionLabel.gameObject.SetActive(false);
		buttonCancel.gameObject.SetActive(false);
	}

	[Obfuscation(Exclude = true)]
	public void OnCancelButtonClick()
	{
		isCancelReConnect = true;
		goToConnect();
	}

	private void ReconnectGUI()
	{
		bool guiActive = ShopNGUIController.GuiActive;
		bool interfaceEnabled = BankController.Instance.InterfaceEnabled;
		bool flag = ProfileController.Instance.Map((ProfileController p) => p.InterfaceEnabled);
		if (guiActive || interfaceEnabled || flag || !isDisconnect)
		{
			return;
		}
		if (timerShowNotConnectToRoom > 0f)
		{
			timerShowNotConnectToRoom -= Time.deltaTime;
			if (timerShowNotConnectToRoom > 0f)
			{
				if (!_needReconnectShow)
				{
					_needReconnectShow = true;
					ShowDescriptionLabel(LocalizationStore.Get("Key_1005"));
					buttonCancel.gameObject.SetActive(false);
				}
			}
			else
			{
				SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(goMapName);
				if (infoScene != null)
				{
					isDisconnect = false;
					JoinRandomRoom(infoScene);
				}
				else
				{
					goToConnect();
				}
			}
		}
		else if (!_roomNotExistShow)
		{
			_roomNotExistShow = true;
			ShowDescriptionLabel(LocalizationStore.Get("Key_1004"));
			bool active = !ShopNGUIController.GuiActive && !ProfileController.Instance.InterfaceEnabled;
			buttonCancel.gameObject.SetActive(active);
		}
	}

	private void Update()
	{
		if ((bool)_onGUIDrawer)
		{
			_onGUIDrawer.gameObject.SetActive(isDisconnect || showLoading);
		}
		if (timerShow > 0f)
		{
			timerShow -= Time.deltaTime;
			showLoading = true;
			fonLoadingScene = null;
			Invoke("goToConnect", 0.1f);
		}
		ReconnectGUI();
	}

	private void OnConnectedToServer()
	{
		_weaponManager.myTable = (GameObject)Network.Instantiate(networkTablePref, base.transform.position, base.transform.rotation, 0);
		_weaponManager.myNetworkStartTable = _weaponManager.myTable.GetComponent<NetworkStartTable>();
	}

	private void OnFailedToConnect(NetworkConnectionError error)
	{
		if (error == NetworkConnectionError.TooManyConnectedPlayers)
		{
			ShowDescriptionLabel(LocalizationStore.Get("Key_0992"));
		}
		if (error == NetworkConnectionError.ConnectionFailed)
		{
			ShowDescriptionLabel(LocalizationStore.Get("Key_0993"));
		}
		timerShow = 5f;
		if (!(_weaponManager == null) && !(_weaponManager.myTable == null))
		{
			_weaponManager.myTable.GetComponent<NetworkStartTable>().isShowNickTable = false;
			_weaponManager.myTable.GetComponent<NetworkStartTable>().showTable = false;
		}
	}

	private void OnDestroy()
	{
		QuestSystem.Instance.SaveQuestProgressIfDirty();
		Instance = null;
		players.Clear();
		bluePlayers.Clear();
		redPlayers.Clear();
		Defs.showTableInNetworkStartTable = false;
		Defs.showNickTableInNetworkStartTable = false;
		if ((bool)_onGUIDrawer)
		{
			_onGUIDrawer.act = null;
		}
		_gameSessionStopwatch.Stop();
		if (lastGameMode == GameModeCampaign || lastGameMode == GameModeSurvival)
		{
			NetworkStartTable.IncreaseTimeInMode(lastGameMode, _gameSessionStopwatch.Elapsed.TotalMinutes);
		}
		FlurryEvents.StopLoggingGameModeEvent();
		ReportWeaponStatistics();
		ExperienceController.sharedController.isShowRanks = false;
		if (ReviewController.IsNeedActive)
		{
			ConnectSceneNGUIController.NeedShowReviewInConnectScene = true;
		}
		else if (Defs.isMulti)
		{
			bool flag = ReplaceAdmobPerelivController.ReplaceAdmobWithPerelivApplicable() && ReplaceAdmobPerelivController.sharedController != null;
			if (flag)
			{
				ReplaceAdmobPerelivController.IncreaseTimesCounter();
			}
			if (flag && ReplaceAdmobPerelivController.ShouldShowAtThisTime)
			{
				if (!ReplaceAdmobPerelivController.sharedController.DataLoading)
				{
					ReplaceAdmobPerelivController.sharedController.LoadPerelivData();
				}
				ConnectSceneNGUIController.ReplaceAdmobWithPerelivRequest = true;
			}
			else if (MobileAdManager.AdIsApplicable(MobileAdManager.Type.Image, Defs.IsDeveloperBuild))
			{
				if (Defs.IsDeveloperBuild)
				{
					UnityEngine.Debug.Log("Setting request for interstitial advertisement.");
				}
				ConnectSceneNGUIController.InterstitialRequest = true;
			}
		}
		PhotonObjectCacher.RemoveObject(base.gameObject);
		Defs.inComingMessagesCounter = 0;
	}

	[Obfuscation(Exclude = true)]
	public void goToConnect()
	{
		UnityEngine.Debug.Log("goToConnect()");
		ConnectSceneNGUIController.Local();
	}

	private void GoToRandomRoom()
	{
	}

	private void ShowLoadingGUI(string _mapName)
	{
		_loadingNGUIController = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LoadingGUI")).GetComponent<LoadingNGUIController>();
		_loadingNGUIController.SceneToLoad = _mapName;
		_loadingNGUIController.loadingNGUITexture.mainTexture = Resources.Load("LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + _mapName) as Texture2D;
		_loadingNGUIController.transform.localPosition = Vector3.zero;
		_loadingNGUIController.Init();
		ExperienceController.sharedController.isShowRanks = false;
	}

	public void OnLeftRoom()
	{
		UnityEngine.Debug.Log("OnLeftRoom (local) init");
		if (Defs.typeDisconnectGame == Defs.DisconectGameType.Exit)
		{
			showLoading = true;
			fonLoadingScene = null;
			Invoke("goToConnect", 0.1f);
			if (_weaponManager == null || _weaponManager.myTable == null)
			{
				return;
			}
			_weaponManager.myTable.GetComponent<NetworkStartTable>().isShowNickTable = false;
			_weaponManager.myTable.GetComponent<NetworkStartTable>().showTable = false;
		}
		if (Defs.typeDisconnectGame == Defs.DisconectGameType.SelectNewMap)
		{
			bool guiActive = ShopNGUIController.GuiActive;
			bool interfaceEnabled = BankController.Instance.InterfaceEnabled;
			bool interfaceEnabled2 = ProfileController.Instance.InterfaceEnabled;
			if (!guiActive && !interfaceEnabled && !interfaceEnabled2)
			{
				ActivityIndicator.IsActiveIndicator = true;
			}
			ShowLoadingGUI(goMapName);
		}
	}

	public void OnDisconnectedFromPhoton()
	{
		UnityEngine.Debug.Log("OnDisconnectedFromPhotoninit");
		OnConnectionFail((DisconnectCause)0);
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		BankController.canShowIndication = true;
		Defs.inRespawnWindow = false;
		QuestSystem.Instance.SaveQuestProgressIfDirty();
		if (Defs.typeDisconnectGame == Defs.DisconectGameType.SelectNewMap)
		{
			if (_loadingNGUIController != null)
			{
				UnityEngine.Object.Destroy(_loadingNGUIController.gameObject);
				_loadingNGUIController = null;
			}
			Defs.typeDisconnectGame = Defs.DisconectGameType.Reconnect;
		}
		if (Defs.typeDisconnectGame == Defs.DisconectGameType.Exit)
		{
			goToConnect();
			return;
		}
		GameObject gameObject = GameObject.FindGameObjectWithTag("NetworkStartTableNGUI");
		if (gameObject != null)
		{
			NetworkStartTableNGUIController component = gameObject.GetComponent<NetworkStartTableNGUIController>();
			if (component != null)
			{
				component.shopAnchor.SetActive(false);
			}
		}
		timerShowNotConnectToRoom = -1f;
		isCancelReConnect = false;
		isNotConnectRoom = false;
		countConnectToRoom = 0;
		PlayerPrefs.SetString("TypeGame", "client");
		UnityEngine.Debug.Log("OnConnectionFail " + cause);
		tc.SetActive(true);
		BonusController.sharedController.ClearBonuses();
		for (int i = 0; i < enemiesObj.Count; i++)
		{
			UnityEngine.Object.Destroy(enemiesObj[i]);
		}
		GameObject gameObject2 = GameObject.FindGameObjectWithTag("InGameGUI");
		if (gameObject2 != null)
		{
			UnityEngine.Object.Destroy(gameObject2);
		}
		GameObject gameObject3 = GameObject.FindGameObjectWithTag("ChatViewer");
		if (gameObject3 != null)
		{
			UnityEngine.Object.Destroy(gameObject3);
		}
		isDisconnect = true;
		Invoke("ConnectToPhoton", 3f);
		Invoke("OnCancelButtonClick", 60f);
		bool guiActive = ShopNGUIController.GuiActive;
		bool interfaceEnabled = BankController.Instance.InterfaceEnabled;
		bool interfaceEnabled2 = ProfileController.Instance.InterfaceEnabled;
		if (!guiActive && !interfaceEnabled && !interfaceEnabled2 && !ExpController.Instance.experienceView.levelUpPanel.gameObject.activeSelf && !ExpController.Instance.experienceView.levelUpPanelTier.gameObject.activeSelf && (!(NetworkStartTableNGUIController.sharedController != null) || !(NetworkStartTableNGUIController.sharedController.rewardWindow != null) || !NetworkStartTableNGUIController.sharedController.rewardWindow.gameObject.activeSelf))
		{
			ActivityIndicator.IsActiveIndicator = true;
			ExperienceController.sharedController.isShowRanks = false;
		}
	}

	[Obfuscation(Exclude = true)]
	private void ConnectToPhoton()
	{
		if (!isCancelReConnect)
		{
			bool guiActive = ShopNGUIController.GuiActive;
			bool interfaceEnabled = BankController.Instance.InterfaceEnabled;
			bool interfaceEnabled2 = ProfileController.Instance.InterfaceEnabled;
			if (guiActive || interfaceEnabled || interfaceEnabled2 || ExpController.Instance.experienceView.levelUpPanel.gameObject.activeSelf || ExpController.Instance.experienceView.levelUpPanelTier.gameObject.activeSelf || (NetworkStartTableNGUIController.sharedController != null && NetworkStartTableNGUIController.sharedController.rewardWindow.gameObject.activeSelf))
			{
				Invoke("ConnectToPhoton", 3f);
				return;
			}
			UnityEngine.Debug.Log("ConnectToPhoton ");
			ActivityIndicator.IsActiveIndicator = true;
			ExperienceController.sharedController.isShowRanks = false;
			PhotonNetwork.autoJoinLobby = false;
			PhotonNetwork.ConnectUsingSettings(Separator + ConnectSceneNGUIController.regim.ToString() + ((!Defs.isDaterRegim) ? ConnectSceneNGUIController.gameTier.ToString() : "Dater") + "v" + GlobalGameController.MultiplayerProtocolVersion);
		}
	}

	private static string InitialiseSeparatorWrapper()
	{
		return InitializeSeparator();
	}

	private static string InitializeSeparator()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			return "bada8a20";
		}
		AndroidJavaObject currentActivity = AndroidSystem.Instance.CurrentActivity;
		if (currentActivity == null)
		{
			return "deadac71";
		}
		AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0]);
		if (androidJavaObject == null)
		{
			return "dead3a9a";
		}
		AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[2] { "com.pixel.gun3d", 64 });
		if (androidJavaObject2 == null)
		{
			return "dead6ac5";
		}
		AndroidJavaObject[] array = androidJavaObject2.Get<AndroidJavaObject[]>("signatures");
		if (array == null)
		{
			return "deadc199";
		}
		if (array.Length != 1)
		{
			return "dead139c";
		}
		AndroidJavaObject androidJavaObject3 = array[0];
		byte[] buffer = androidJavaObject3.Call<byte[]>("toByteArray", new object[0]);
		using (SHA1Managed sHA1Managed = new SHA1Managed())
		{
			byte[] source = sHA1Managed.ComputeHash(buffer);
			return BitConverter.ToString(source.Take(4).ToArray()).Replace("-", string.Empty).ToLower();
		}
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("NetworkStartTableNGUI");
		if (gameObject != null)
		{
			NetworkStartTableNGUIController component = gameObject.GetComponent<NetworkStartTableNGUIController>();
			if (component != null)
			{
				component.shopAnchor.SetActive(false);
			}
		}
		UnityEngine.Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
		if (!isCancelReConnect)
		{
			Invoke("ConnectToPhoton", 3f);
		}
	}

	public void OnConnectedToMaster()
	{
		ConnectToRoom();
	}

	public void OnJoinedLobby()
	{
		UnityEngine.Debug.Log("OnJoinedLobby()");
		ConnectToRoom();
	}

	[Obfuscation(Exclude = true)]
	private void ConnectToRoom()
	{
		CancelInvoke("OnCancelButtonClick");
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(goMapName);
		if (Defs.typeDisconnectGame == Defs.DisconectGameType.RandomGameInHunger)
		{
			UnityEngine.Debug.Log("JoinRandomRoom");
			isCancelReConnect = true;
			int num = UnityEngine.Random.Range(0, SceneInfoController.instance.GetCountScenesForMode(TypeModeGame.DeadlyGames));
			PlayerPrefs.SetString("TypeGame", "client");
			PlayerPrefs.SetInt("CustomGame", 0);
			ConnectSceneNGUIController.JoinRandomGameRoom(infoScene.indexMap, ConnectSceneNGUIController.RegimGame.DeadlyGames, false);
			ActivityIndicator.IsActiveIndicator = true;
		}
		else if (Defs.typeDisconnectGame == Defs.DisconectGameType.SelectNewMap)
		{
			UnityEngine.Debug.Log("ConnectToRoom() " + goMapName);
			JoinRandomRoom(infoScene);
		}
		else
		{
			UnityEngine.Debug.Log("ConnectToRoom " + PlayerPrefs.GetString("RoomName"));
			if (!isCancelReConnect)
			{
				PhotonNetwork.JoinRoom(PlayerPrefs.GetString("RoomName"));
			}
		}
	}

	private void OnPhotonJoinRoomFailed()
	{
		countConnectToRoom++;
		UnityEngine.Debug.Log("OnPhotonJoinRoomFailed - init");
		isNotConnectRoom = true;
		if (countConnectToRoom < 6)
		{
			Invoke("ConnectToRoom", 3f);
		}
		else
		{
			timerShowNotConnectToRoom = 3f;
		}
	}

	private void JoinRandomRoom(SceneInfo _map)
	{
		if (Defs.typeDisconnectGame != Defs.DisconectGameType.SelectNewMap)
		{
			goMapName = _map.NameScene;
		}
		UnityEngine.Debug.Log("JoinRandomRoom " + goMapName);
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		FlurryPluginWrapper.LogEnteringMap(0, goMapName);
		FlurryPluginWrapper.LogMultiplayerWayStart();
		ActivityIndicator.IsActiveIndicator = true;
		ConnectSceneNGUIController.JoinRandomGameRoom(_map.indexMap, ConnectSceneNGUIController.regim, false);
	}

	private void OnPhotonRandomJoinFailed()
	{
		UnityEngine.Debug.Log("OnPhotonJoinRoomFailed");
		PlayerPrefs.SetString("TypeGame", "server");
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(goMapName);
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		int playerLimit = (Defs.isCOOP ? 4 : (Defs.isCompany ? 10 : ((!Defs.isHunger) ? 10 : 6)));
		int num = ((!(ExperienceController.sharedController != null) || ExperienceController.sharedController.currentLevel > 2) ? ((ExperienceController.sharedController != null && ExperienceController.sharedController.currentLevel <= 5) ? 1 : 2) : 0);
		int maxKill = ((Defs.filterMaps.ContainsKey(goMapName) && Defs.filterMaps[goMapName] == 3) ? 5 : ((num == 0) ? (Defs.isHunger ? 15 : ((ConnectSceneNGUIController.regim != 0 && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.TeamFight && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.FlagCapture && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints) ? 10 : 3)) : ((ConnectSceneNGUIController.regim != 0 && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.TeamFight && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.FlagCapture && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints) ? 15 : 3)));
		ConnectSceneNGUIController.CreateGameRoom(null, playerLimit, infoScene.indexMap, maxKill, string.Empty, ConnectSceneNGUIController.regim);
	}

	[Obfuscation(Exclude = true)]
	private void StartGameAfterDisconnectInvoke()
	{
		if (ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.TimeBattle && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.FlagCapture && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.TeamFight && ConnectSceneNGUIController.regim != ConnectSceneNGUIController.RegimGame.CapturePoints && !Defs.showTableInNetworkStartTable && !Defs.showNickTableInNetworkStartTable)
		{
			PlayerPrefs.SetInt("StartAfterDisconnect", 1);
		}
		_weaponManager.myTable = PhotonNetwork.Instantiate("NetworkTable", base.transform.position, base.transform.rotation, 0);
		_weaponManager.myNetworkStartTable = _weaponManager.myTable.GetComponent<NetworkStartTable>();
		ActivityIndicator.IsActiveIndicator = false;
	}

	private void OnJoinedRoom()
	{
		CheckRoom();
		UnityEngine.Debug.Log("OnJoinedRoom - init");
		PlayerPrefs.SetString("RoomName", PhotonNetwork.room.name);
		SceneInfo infoScene = SceneInfoController.instance.GetInfoScene(int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty].ToString()));
		Instance.goMapName = infoScene.NameScene;
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(Instance.goMapName) ? Defs.filterMaps[Instance.goMapName] : 0);
		}
		if (isDisconnect && (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Deathmatch || ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.DeadlyGames))
		{
			Invoke("StartGameAfterDisconnectInvoke", 3f);
		}
		else
		{
			GlobalGameController.healthMyPlayer = 0f;
			PlayerPrefs.SetInt("StartAfterDisconnect", 0);
			PhotonNetwork.isMessageQueueRunning = false;
			StartCoroutine(MoveToGameScene());
		}
		isDisconnect = false;
		_roomNotExistShow = false;
		_needReconnectShow = false;
		HideReconnectInterface();
	}

	private IEnumerator MoveToGameScene()
	{
		if (Defs.typeDisconnectGame != Defs.DisconectGameType.SelectNewMap && WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.Reset(Defs.filterMaps.ContainsKey(goMapName) ? Defs.filterMaps[goMapName] : 0);
		}
		UnityEngine.Debug.Log("MoveToGameScene");
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		PhotonNetwork.isMessageQueueRunning = false;
		SceneInfo scInfo = SceneInfoController.instance.GetInfoScene(int.Parse(PhotonNetwork.room.customProperties[ConnectSceneNGUIController.mapProperty].ToString()));
		UnityEngine.Debug.Log(scInfo.NameScene);
		LoadConnectScene.textureToShow = Resources.Load("LevelLoadings" + ((!Device.isRetinaAndStrong) ? string.Empty : "/Hi") + "/Loading_" + scInfo.NameScene) as Texture2D;
		UnityEngine.Debug.Log("LoadConnectScene.textureToShow " + LoadConnectScene.textureToShow.name);
		LoadConnectScene.sceneToLoad = scInfo.NameScene;
		LoadConnectScene.noteToShow = null;
		AsyncOperation async = Application.LoadLevelAsync("PromScene");
		FlurryPluginWrapper.LogEvent("Play_" + scInfo.NameScene);
		FriendsController.sharedController.GetFriendsData();
		yield return async;
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
	}

	public void OnReceivedRoomList()
	{
	}

	public void OnReceivedRoomListUpdate()
	{
	}

	public void OnConnectedToPhoton()
	{
		UnityEngine.Debug.Log("OnConnectedToPhotoninit");
	}

	public void OnFailedToConnectToPhoton()
	{
		UnityEngine.Debug.Log("OnFailedToConnectToPhotoninit");
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		UnityEngine.Debug.Log("OnPhotonInstantiate init" + info.sender);
	}
}
