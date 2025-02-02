using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Facebook.Unity;
using Rilisoft;
using UnityEngine;

public sealed class NetworkStartTableNGUIController : MonoBehaviour
{
	public static NetworkStartTableNGUIController sharedController;

	public GameObject facebookButton;

	public GameObject twitterButton;

	public Transform rentScreenPoint;

	public GameObject ranksInterface;

	public RanksTable ranksTable;

	public GameObject shopAnchor;

	public GameObject finishedInterface;

	public UILabel[] finishedInterfaceLabels;

	public GameObject startInterfacePanel;

	public GameObject winnerPanelCom1;

	public GameObject winnerPanelCom2;

	public GameObject endInterfacePanel;

	public Animator interfaceAnimator;

	public GameObject allInterfacePanel;

	public GameObject randomBtn;

	public GameObject socialPnl;

	public GameObject spectratorModePnl;

	public GameObject spectatorModeBtnPnl;

	public GameObject spectatorModeOnBtn;

	public GameObject spectatorModeOffBtn;

	public UILabel[] winerLabel;

	public GameObject MapSelectPanel;

	public string winner;

	public int winnerCommand;

	public UILabel HungerStartLabel;

	private int addCoins;

	private int addExperience;

	private bool isCancelHideAvardPanel;

	private bool updateRealTableAfterActionPanel = true;

	public GameObject SexualButton;

	public GameObject InAppropriateActButton;

	public GameObject OtherButton;

	public GameObject ReasonsPanel;

	public GameObject ActionPanel;

	public GameObject AddButton;

	public GameObject ReportButton;

	public UILabel[] actionPanelNicklabel;

	public string pixelbookID;

	public string nick;

	public GoMapInEndGame[] goMapInEndGameButtons = new GoMapInEndGame[3];

	public int CountAddFriens;

	public UILabel totalBlue;

	public UILabel totalRed;

	private GameObject cameraObj;

	public GameObject changeMapLabel;

	private readonly Lazy<string> _versionString = new Lazy<string>(() => typeof(NetworkStartTableNGUIController).Assembly.GetName().Version.ToString());

	private IDisposable _backSubscription;

	public RewardWindowBase rewardWindow { get; set; }

	private void Awake()
	{
		sharedController = this;
	}

	private void OnDestroy()
	{
		sharedController = null;
	}

	private void Start()
	{
		cameraObj = base.transform.GetChild(0).gameObject;
		if (SexualButton != null)
		{
			ButtonHandler component = SexualButton.GetComponent<ButtonHandler>();
			if (component != null)
			{
				component.Clicked += SexualButtonHandler;
			}
		}
		if (InAppropriateActButton != null)
		{
			ButtonHandler component2 = InAppropriateActButton.GetComponent<ButtonHandler>();
			if (component2 != null)
			{
				component2.Clicked += InAppropriateActButtonHandler;
			}
		}
		if (OtherButton != null)
		{
			ButtonHandler component3 = OtherButton.GetComponent<ButtonHandler>();
			if (component3 != null)
			{
				component3.Clicked += OtherButtonHandler;
			}
		}
		if (ReportButton != null)
		{
			ButtonHandler component4 = ReportButton.GetComponent<ButtonHandler>();
			if (component4 != null)
			{
				component4.Clicked += ShowReasonPanel;
			}
		}
		if (AddButton != null)
		{
			ButtonHandler component5 = AddButton.GetComponent<ButtonHandler>();
			if (component5 != null)
			{
				component5.Clicked += AddButtonHandler;
			}
		}
	}

	private void Update()
	{
		if (ExpController.Instance != null && ExpController.Instance.experienceView != null)
		{
			bool flag = ExpController.Instance.experienceView.levelUpPanel.gameObject.activeInHierarchy || ExpController.Instance.experienceView.levelUpPanelTier.gameObject.activeInHierarchy;
			if (cameraObj.activeSelf == flag)
			{
				cameraObj.SetActive(!flag);
			}
		}
		if ((Defs.isHunger || Defs.isRegimVidosDebug) && spectatorModeBtnPnl.activeSelf && Initializer.players.Count == 0)
		{
			spectatorModeBtnPnl.SetActive(false);
			spectratorModePnl.SetActive(false);
			ShowTable(false);
		}
		facebookButton.SetActive(FacebookController.FacebookSupported && FacebookController.FacebookPost_Old_Supported && FB.IsLoggedIn);
		twitterButton.SetActive(TwitterController.TwitterSupported && TwitterController.TwitterSupported_OldPosts && TwitterController.IsLoggedIn);
		bool flag2 = facebookButton.activeSelf || twitterButton.activeSelf;
		if (socialPnl.activeSelf != flag2)
		{
			socialPnl.SetActive(flag2);
		}
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Network Start Table GUI");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	public void HandleEscape()
	{
		if (ReasonsPanel != null && ReasonsPanel.activeInHierarchy)
		{
			BackFromReasonPanel();
		}
		else if (ActionPanel != null && ActionPanel.activeInHierarchy)
		{
			CancelFromActionPanel();
		}
		else if (ShopNGUIController.GuiActive && WeaponManager.sharedManager != null && WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().HandleResumeFromShop();
		}
	}

	public void ShowActionPanel(string _pixelbookID, string _nick)
	{
		pixelbookID = _pixelbookID;
		nick = _nick;
		HideTable();
		for (int i = 0; i < actionPanelNicklabel.Length; i++)
		{
			actionPanelNicklabel[i].text = nick;
		}
		ActionPanel.SetActive(true);
		spectatorModeBtnPnl.SetActive(false);
		if (FriendsController.sharedController.IsShowAdd(pixelbookID) && CountAddFriens < 3)
		{
			AddButton.GetComponent<UIButton>().isEnabled = true;
		}
		else
		{
			AddButton.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void HideActionPanel()
	{
		ActionPanel.SetActive(false);
		ShowTable(updateRealTableAfterActionPanel);
		if ((Defs.isHunger || Defs.isRegimVidosDebug) && Initializer.players.Count > 0)
		{
			spectatorModeBtnPnl.SetActive(true);
		}
	}

	public void ShowReasonPanel(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Debug.Log("ShowReasonPanel");
			ReasonsPanel.SetActive(true);
			ActionPanel.SetActive(false);
		}
	}

	public void HideReasonPanel()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			ReasonsPanel.SetActive(false);
			ActionPanel.SetActive(true);
		}
	}

	public bool CheckHideInternalPanel()
	{
		if (ActionPanel.activeInHierarchy)
		{
			CancelFromActionPanel();
			return true;
		}
		if (ReasonsPanel.activeInHierarchy)
		{
			BackFromReasonPanel();
			return true;
		}
		return false;
	}

	public void AddButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Debug.Log("[Add] " + pixelbookID);
			CountAddFriens++;
			string value = ((!Defs.isDaterRegim) ? "Multiplayer Battle" : "Sandbox (Dating)");
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Added Friends", value);
			dictionary.Add("Deleted Friends", "Add");
			Dictionary<string, string> socialEventParameters = dictionary;
			FriendsController.sharedController.SendInvitation(pixelbookID, socialEventParameters);
			if (!FriendsController.sharedController.notShowAddIds.Contains(pixelbookID))
			{
				FriendsController.sharedController.notShowAddIds.Add(pixelbookID);
			}
			AddButton.GetComponent<UIButton>().isEnabled = false;
		}
	}

	public void CancelFromActionPanel()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			HideActionPanel();
		}
	}

	public void BackFromReasonPanel()
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			HideReasonPanel();
		}
	}

	public void InAppropriateActButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Action handler = delegate
			{
				string value = _versionString.Value;
				string text = string.Concat("mailto:", Defs.SupportMail, "?subject=INAPPROPRIATE ACT ", nick, "(", pixelbookID, ")&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", value, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20INAPPROPRIATE ACT ", nick, "(", pixelbookID, ")%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
				text = text.Replace(" ", "%20");
				FlurryPluginWrapper.LogEventWithParameterAndValue("User Feedback", "Menu", "In Game Menu_inappropriate");
				Application.OpenURL(text);
			};
			FeedbackMenuController.ShowDialogWithCompletion(handler);
		}
	}

	public void SexualButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Action handler = delegate
			{
				string value = _versionString.Value;
				string text = string.Concat("mailto:", Defs.SupportMail, "?subject=CHEATING ", nick, "(", pixelbookID, ")&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", value, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20CHEATING ", nick, "(", pixelbookID, ")%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
				text = text.Replace(" ", "%20");
				FlurryPluginWrapper.LogEventWithParameterAndValue("User Feedback", "Menu", "In Game Menu_cheater");
				Application.OpenURL(text);
			};
			FeedbackMenuController.ShowDialogWithCompletion(handler);
		}
	}

	public void OtherButtonHandler(object sender, EventArgs e)
	{
		if ((!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && !ShopNGUIController.GuiActive && !ExperienceController.sharedController.isShowNextPlashka)
		{
			Action handler = delegate
			{
				string value = _versionString.Value;
				string text = string.Concat("mailto:", Defs.SupportMail, "?subject=Report ", nick, "(", pixelbookID, ")&body=%0D%0A%0D%0A%0D%0A%0D%0A%0D%0A------------%20DO NOT DELETE%20------------%0D%0AUTC%20Time:%20", DateTime.Now.ToString(), "%0D%0AGame:%20PixelGun3D%0D%0AVersion:%20", value, "%0D%0APlayerID:%20", FriendsController.sharedController.id, "%0D%0ACategory:%20Report ", nick, "(", pixelbookID, ")%0D%0ADevice%20Type:%20", SystemInfo.deviceType, "%20", SystemInfo.deviceModel, "%0D%0AOS%20Version:%20", SystemInfo.operatingSystem, "%0D%0A------------------------");
				text = text.Replace(" ", "%20");
				FlurryPluginWrapper.LogEventWithParameterAndValue("User Feedback", "Menu", "In Game Menu_other");
				Application.OpenURL(text);
			};
			FeedbackMenuController.ShowDialogWithCompletion(handler);
		}
	}

	public void StartSpectatorMode()
	{
		UILabel[] array = winerLabel;
		foreach (UILabel uILabel in array)
		{
			uILabel.gameObject.SetActive(false);
		}
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.aimPanel.SetActive(true);
		}
		spectatorModeOnBtn.SetActive(true);
		spectatorModeOffBtn.SetActive(false);
		spectratorModePnl.SetActive(true);
		socialPnl.SetActive(false);
		MapSelectPanel.SetActive(false);
		HideTable();
		if (WeaponManager.sharedManager.myTable != null)
		{
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isRegimVidos = true;
		}
	}

	public void EndSpectatorMode()
	{
		UILabel[] array = winerLabel;
		foreach (UILabel uILabel in array)
		{
			uILabel.gameObject.SetActive(true);
		}
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.aimPanel.SetActive(false);
		}
		spectatorModeOnBtn.SetActive(false);
		spectatorModeOffBtn.SetActive(true);
		spectratorModePnl.SetActive(false);
		MapSelectPanel.SetActive(true);
		if (WeaponManager.sharedManager.myTable != null)
		{
			if (WeaponManager.sharedManager.myNetworkStartTable.currentGameObjectPlayer != null)
			{
				Player_move_c.SetLayerRecursively(WeaponManager.sharedManager.myNetworkStartTable.currentGameObjectPlayer.transform.GetChild(0).gameObject, 0);
			}
			WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isRegimVidos = false;
		}
		ShowTable();
	}

	[Obfuscation(Exclude = true)]
	public void HideAvardPanel()
	{
		if (!isCancelHideAvardPanel)
		{
			rewardWindow = null;
			ShowEndInterface(winner, winnerCommand);
			if (WeaponManager.sharedManager.myTable != null)
			{
				WeaponManager.sharedManager.myTable.GetComponent<NetworkStartTable>().isShowAvard = false;
			}
			else
			{
				UnityEngine.Object.Destroy(sharedController.gameObject);
			}
			isCancelHideAvardPanel = true;
		}
	}

	public static RewardWindowBase ShowRewardWindow(bool win, Transform par)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("NguiWindows/WinWindowNGUI"));
		RewardWindowBase component = gameObject.GetComponent<RewardWindowBase>();
		FacebookController.StoryPriority priority = FacebookController.StoryPriority.Red;
		component.priority = priority;
		component.twitterPriority = FacebookController.StoryPriority.MultyWinLimit;
		component.shareAction = delegate
		{
			FacebookController.PostOpenGraphStory("win", "battle", priority, new Dictionary<string, string> { 
			{
				"battle",
				ConnectSceneNGUIController.regim.ToString().ToLower()
			} });
		};
		component.HasReward = true;
		component.CollectOnlyNoShare = !win;
		component.twitterStatus = () => "I've won a battle in @PixelGun3D! Join the fight now! #pixelgun3d #pixelgun #3d #pg3d #mobile #fps #shooter http://goo.gl/8fzL9u";
		component.EventTitle = "Won Batlle";
		gameObject.transform.parent = par;
		Player_move_c.SetLayerRecursively(gameObject, LayerMask.NameToLayer("NGUITable"));
		gameObject.transform.localPosition = new Vector3(0f, 0f, -130f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		return component;
	}

	public void ShowFinishedInterface()
	{
		finishedInterface.SetActive(true);
		string text = LocalizationStore.Get((!Defs.isDaterRegim) ? "Key_1976" : "Key_1987");
		for (int i = 0; i < finishedInterfaceLabels.Length; i++)
		{
			finishedInterfaceLabels[i].text = text;
		}
	}

	public void showAvardPanel(string _winner, int _addCoin, int _addExpierence, bool _isCustom, bool firstPlace, int _winnerCommand)
	{
		isCancelHideAvardPanel = false;
		if (_isCustom)
		{
			addCoins = 0;
			addExperience = 0;
		}
		else
		{
			addCoins = _addCoin;
			addExperience = _addExpierence;
		}
		string text = string.Format("+ {0} {1}", _addCoin, LocalizationStore.Key_0275);
		string text2 = string.Format("+ {0} {1}", _addExpierence, LocalizationStore.Key_0204);
		ConnectSceneNGUIController.RegimGame regim = ConnectSceneNGUIController.regim;
		PremiumAccountController instance = PremiumAccountController.Instance;
		bool flag = regim == ConnectSceneNGUIController.RegimGame.Deathmatch || regim == ConnectSceneNGUIController.RegimGame.FlagCapture || regim == ConnectSceneNGUIController.RegimGame.TeamFight || regim == ConnectSceneNGUIController.RegimGame.CapturePoints;
		bool flag2 = PromoActionsManager.sharedManager.IsDayOfValorEventActive && flag;
		bool flag3 = instance.IsActiveOrWasActiveBeforeStartMatch();
		int num = 1;
		int num2 = 1;
		if (flag3 || flag2)
		{
			num = ((!Defs.isCOOP && !Defs.isHunger) ? AdminSettingsController.GetMultiplyerRewardWithBoostEvent(false) : PremiumAccountController.Instance.RewardCoeff);
			num2 = ((!Defs.isCOOP && !Defs.isHunger) ? AdminSettingsController.GetMultiplyerRewardWithBoostEvent(true) : PremiumAccountController.Instance.RewardCoeff);
		}
		rewardWindow = ShowRewardWindow(firstPlace, sharedController.allInterfacePanel.transform.parent);
		rewardWindow.customHide = delegate
		{
			rewardWindow.CancelInvoke("Hide");
			HideAvardPanel();
		};
		RewardWindowAfterMatch component = rewardWindow.GetComponent<RewardWindowAfterMatch>();
		component.victory.SetActive(true);
		component.lose.SetActive(false);
		if (flag3 && flag2)
		{
			component.daysAndPremiumBack.SetActive(true);
			component.premiumBackground.SetActive(false);
			component.daysOfValorBackground.SetActive(false);
			component.normlaBeckground.SetActive(false);
		}
		else if (flag3)
		{
			component.daysAndPremiumBack.SetActive(false);
			component.premiumBackground.SetActive(true);
			component.daysOfValorBackground.SetActive(false);
			component.normlaBeckground.SetActive(false);
		}
		else if (flag2)
		{
			component.daysAndPremiumBack.SetActive(false);
			component.premiumBackground.SetActive(false);
			component.daysOfValorBackground.SetActive(true);
			component.normlaBeckground.SetActive(false);
		}
		else
		{
			component.daysAndPremiumBack.SetActive(false);
			component.premiumBackground.SetActive(false);
			component.daysOfValorBackground.SetActive(false);
			component.normlaBeckground.SetActive(true);
		}
		component.coinsMultiplierContainer.SetActive(num2 > 1 && _addCoin > 0);
		component.coinsMultiplier.text = "x" + num2;
		component.expMultiplierContainer.SetActive(num > 1);
		component.expMilyiplier.text = "x" + num;
		foreach (UILabel coin in component.coins)
		{
			coin.text = text;
		}
		foreach (UILabel item in component.exp)
		{
			item.text = text2;
		}
		if (_addCoin == 0)
		{
			component.coinsContainer.SetActive(false);
			component.expContainer.transform.localPosition = new Vector3(0f, component.expContainer.transform.localPosition.y, component.expContainer.transform.localPosition.z);
		}
		endInterfacePanel.SetActive(true);
		finishedInterface.SetActive(false);
		MapSelectPanel.SetActive(false);
		socialPnl.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64);
		UILabel[] array = winerLabel;
		foreach (UILabel uILabel in array)
		{
			uILabel.gameObject.SetActive(true);
		}
		winnerCommand = _winnerCommand;
		winner = _winner;
		UILabel[] array2 = winerLabel;
		foreach (UILabel uILabel2 in array2)
		{
			uILabel2.text = winner;
		}
		if (Defs.isDaterRegim)
		{
			UILabel[] array3 = winerLabel;
			foreach (UILabel uILabel3 in array3)
			{
				uILabel3.text = LocalizationStore.Get("Key_1427");
			}
		}
		if (addExperience > 0)
		{
			ExperienceController.sharedController.addExperience(addExperience);
		}
		if (addCoins > 0)
		{
			int @int = Storager.getInt("Coins", false);
			Storager.setInt("Coins", @int + addCoins, false);
			FlurryEvents.LogCoinsGained(FlurryEvents.GetPlayingMode(), addCoins);
		}
	}

	public void ShowStartInterface()
	{
		allInterfacePanel.SetActive(true);
		startInterfacePanel.SetActive(true);
		ShowTable();
		StartCoroutine("TryToShowExpiredBanner");
	}

	public void HideStartInterface()
	{
		Debug.Log("HideStartInterface");
		allInterfacePanel.SetActive(false);
		startInterfacePanel.SetActive(false);
		ReasonsPanel.SetActive(false);
		ActionPanel.SetActive(false);
		updateRealTableAfterActionPanel = true;
		HideTable();
		StopCoroutine("TryToShowExpiredBanner");
	}

	public void ShowEndInterface(string _winner, int _winnerCommand)
	{
		KillRateCheck.instance.CheckKillRate();
		if (WeaponManager.sharedManager != null)
		{
			WeaponManager.sharedManager.DecreaseTryGunsMatchesCount();
		}
		if (Defs.isCompany || Defs.isFlag || Defs.isCompany)
		{
			if (_winnerCommand == 1)
			{
				winnerPanelCom1.SetActive(true);
				winnerPanelCom2.SetActive(false);
			}
			if (_winnerCommand == 2)
			{
				winnerPanelCom1.SetActive(false);
				winnerPanelCom2.SetActive(true);
			}
			if (_winnerCommand == 0)
			{
				winnerPanelCom1.SetActive(false);
				winnerPanelCom2.SetActive(false);
			}
		}
		if (Defs.isMulti)
		{
			MapSelectPanel.SetActive(true);
		}
		finishedInterface.SetActive(false);
		startInterfacePanel.SetActive(false);
		if (InGameGUI.sharedInGameGUI != null)
		{
			InGameGUI.sharedInGameGUI.aimPanel.SetActive(false);
		}
		endInterfacePanel.SetActive(true);
		socialPnl.SetActive(BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64);
		UILabel[] array = winerLabel;
		foreach (UILabel uILabel in array)
		{
			uILabel.gameObject.SetActive(true);
		}
		winner = _winner;
		UILabel[] array2 = winerLabel;
		foreach (UILabel uILabel2 in array2)
		{
			uILabel2.text = winner;
		}
		allInterfacePanel.SetActive(true);
		ranksTable.UpdateRanksFromOldSpisok();
		if (Defs.isHunger || Defs.isRegimVidosDebug)
		{
			if (Defs.isHunger)
			{
				randomBtn.SetActive(true);
			}
			spectatorModeBtnPnl.SetActive(true);
			updateRealTableAfterActionPanel = (_winner.Equals(string.Empty) ? true : false);
			if (!ActionPanel.activeSelf && !ReasonsPanel.activeSelf)
			{
				ShowTable(string.IsNullOrEmpty(_winner));
			}
		}
		else
		{
			updateRealTableAfterActionPanel = false;
			ShowTable(false);
		}
		StartCoroutine("TryToShowExpiredBanner");
	}

	private IEnumerator TryToShowExpiredBanner()
	{
		while (FriendsController.sharedController == null || TempItemsController.sharedController == null)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(FriendsController.sharedController.MyWaitForSeconds(1f));
			try
			{
				if (!ShopNGUIController.GuiActive && (!(BankController.Instance != null) || !BankController.Instance.InterfaceEnabled) && (!(ExpController.Instance != null) || !ExpController.Instance.WaitingForLevelUpView) && (!(ExpController.Instance != null) || !ExpController.Instance.IsLevelUpShown) && rentScreenPoint.childCount == 0 && (Storager.getInt(Defs.PremiumEnabledFromServer, false) != 1 || !ShopNGUIController.ShowPremimAccountExpiredIfPossible(rentScreenPoint, "NGUITable", string.Empty)))
				{
					ShopNGUIController.ShowTryGunIfPossible(startInterfacePanel.activeSelf, rentScreenPoint, "NGUITable");
				}
			}
			catch (Exception ex)
			{
				Exception e = ex;
				Debug.LogWarning("exception in NetworkTableNGUI  TryToShowExpiredBanner: " + e);
			}
		}
	}

	public static bool IsStartInterfaceShown()
	{
		return sharedController != null && sharedController.startInterfacePanel != null && sharedController.startInterfacePanel.activeSelf;
	}

	public static bool IsEndInterfaceShown()
	{
		return sharedController != null && sharedController.endInterfacePanel != null && sharedController.endInterfacePanel.activeSelf;
	}

	public void HideEndInterface()
	{
		Debug.Log("HideEndInterface");
		socialPnl.SetActive(false);
		UILabel[] array = winerLabel;
		foreach (UILabel uILabel in array)
		{
			uILabel.gameObject.SetActive(false);
		}
		allInterfacePanel.SetActive(false);
		endInterfacePanel.SetActive(false);
		HideTable();
		ReasonsPanel.SetActive(false);
		ActionPanel.SetActive(false);
		updateRealTableAfterActionPanel = true;
		StopCoroutine("TryToShowExpiredBanner");
	}

	private void ShowTable(bool _isRealUpdate = true)
	{
		ranksTable.isShowRanks = _isRealUpdate;
		ranksTable.tekPanel.SetActive(true);
	}

	public void HideTable()
	{
		ranksTable.isShowRanks = false;
		ranksTable.tekPanel.SetActive(false);
	}

	public void ShowRanksTable()
	{
		ShowTable();
		ranksInterface.SetActive(true);
	}

	public void HideRanksTable(bool isHideTable = true)
	{
		if (isHideTable)
		{
			HideTable();
		}
		ranksInterface.SetActive(false);
	}

	public void BackPressFromRanksTable(bool isHideTable = true)
	{
		if (!CheckHideInternalPanel())
		{
			HideRanksTable(isHideTable);
			ReasonsPanel.SetActive(false);
			ActionPanel.SetActive(false);
			if (WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.BackRanksPressed();
			}
		}
	}

	public void UpdateGoMapButtons()
	{
		if (ConnectSceneNGUIController.gameTier != ExpController.Instance.OurTier)
		{
			for (int i = 0; i < goMapInEndGameButtons.Length; i++)
			{
				goMapInEndGameButtons[i].gameObject.SetActive(false);
			}
			changeMapLabel.SetActive(false);
			return;
		}
		AllScenesForMode listScenesForMode = SceneInfoController.instance.GetListScenesForMode(ConnectSceneNGUIController.curSelectMode);
		SceneInfo[] array = new SceneInfo[3];
		for (int j = 0; j < 3; j++)
		{
			int num = 0;
			bool flag = true;
			int num2 = UnityEngine.Random.Range(0, listScenesForMode.avaliableScenes.Count);
			while (flag)
			{
				flag = false;
				SceneInfo sceneInfo = listScenesForMode.avaliableScenes[num2];
				for (int k = 0; k < j; k++)
				{
					if (array[k] == sceneInfo)
					{
						flag = true;
						break;
					}
				}
				if (!flag && (sceneInfo.NameScene.Equals(Application.loadedLevelName) || sceneInfo.AvaliableWeapon == ModeWeapon.dater || (sceneInfo.isPremium && Storager.getInt(sceneInfo.NameScene + "Key", true) == 0 && !PremiumAccountController.MapAvailableDueToPremiumAccount(sceneInfo.NameScene))))
				{
					flag = true;
				}
				if (!flag)
				{
					array[j] = sceneInfo;
				}
				else
				{
					num2++;
					num++;
					if (num2 > listScenesForMode.avaliableScenes.Count - 1)
					{
						num2 = 0;
					}
				}
				if (num > listScenesForMode.avaliableScenes.Count)
				{
					Debug.LogWarning("no map");
					break;
				}
			}
		}
		goMapInEndGameButtons[0].SetMap(array[0].NameScene);
		goMapInEndGameButtons[1].SetMap(array[1].NameScene);
		goMapInEndGameButtons[2].SetMap(array[2].NameScene);
	}
}
