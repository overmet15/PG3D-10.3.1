using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class InGameGUI : MonoBehaviour
{
	public delegate float GetFloatVAlue();

	public delegate string GetString();

	public delegate int GetIntVAlue();

	private const string weaponCat = "WeaponCat_";

	public UILabel Wave1_And_Counter;

	public UILabel reloadLabel;

	public UITexture reloadCircularSprite;

	public UITexture fireCircularSprite;

	public UITexture fireAdditionalCrcualrSprite;

	private UITexture[] circularSprites;

	public GameObject centerAnhor;

	public UILabel newWave;

	public UILabel waveDone;

	public UILabel SurvivalWaveNumber;

	public GameObject deathmatchContainer;

	public GameObject daterContainer;

	public GameObject teamBattleContainer;

	public GameObject timeBattleContainer;

	public GameObject deadlygamesContainer;

	public GameObject flagCaptureContainer;

	public GameObject survivalContainer;

	public GameObject CampaignContainer;

	public GameObject CapturePointContainer;

	public GameObject[] hidesPanelInTurrel;

	public GameObject turretPanel;

	public ButtonHandler runTurrelButton;

	public ButtonHandler cancelTurrelButton;

	[Range(1f, 1000f)]
	public float minLength = 300f;

	[Range(1f, 1000f)]
	public float maxLength = 550f;

	[Range(1f, 1000f)]
	public float defaultPanelLength = 486f;

	public UISprite bottomPanelSprite;

	public UISlider bottomPanelSlider;

	public Transform sideObjGearShop;

	public static InGameGUI sharedInGameGUI;

	public GameObject characterDrag;

	public GameObject cameraDrag;

	public GameObject pausePanel;

	public Transform shopPanelForTap;

	public Transform shopPanelForSwipe;

	public Transform shopPanelForTapDater;

	public Transform shopPanelForSwipeDater;

	public Transform swipeWeaponPanel;

	public static Vector3 swipeWeaponPanelPos;

	public static Vector3 shopPanelForTapPos;

	public static Vector3 shopPanelForSwipePos;

	public GameObject blockedCollider;

	public GameObject blockedCollider2;

	public GameObject blockedColliderDater;

	public GameObject zoomButton;

	public GameObject reloadButton;

	public GameObject jumpButton;

	public GameObject fireButton;

	public GameObject fireButtonInJoystick;

	public GameObject joystick;

	public GameObject grenadeButton;

	public UISprite fireButtonSprite;

	public UISprite fireButtonSprite2;

	public GameObject aimPanel;

	public GameObject flagBlueCaptureTexture;

	public GameObject flagRedCaptureTexture;

	public GameObject message_draw;

	public GameObject message_now;

	public GameObject message_wait;

	public float timerShowNow;

	public GameObject interfacePanel;

	public UILabel timerStartHungerLabel;

	public GameObject shopButton;

	public GameObject shopButtonInPause;

	public GameObject enemiesLeftLabel;

	public GameObject duel;

	public GameObject downBloodTexture;

	public GameObject upBloodTexture;

	public GameObject leftBloodTexture;

	public GameObject rightBloodTexture;

	public GameObject aimUp;

	public GameObject aimDown;

	public GameObject aimRight;

	public GameObject aimLeft;

	public GameObject aimCenter;

	public GameObject aimUpLeft;

	public GameObject aimDownLeft;

	public GameObject aimDownRight;

	public GameObject aimUpRight;

	[HideInInspector]
	public UISprite aimUpSprite;

	[HideInInspector]
	public UISprite aimDownSprite;

	[HideInInspector]
	public UISprite aimRightSprite;

	[HideInInspector]
	public UISprite aimLeftSprite;

	[HideInInspector]
	public UISprite aimCenterSprite;

	[HideInInspector]
	public UISprite aimUpLeftSprite;

	[HideInInspector]
	public UISprite aimDownLeftSprite;

	[HideInInspector]
	public UISprite aimDownRightSprite;

	[HideInInspector]
	public UISprite aimUpRightSprite;

	public GameObject topAnchor;

	public GameObject leftAnchor;

	public GameObject rightAnchor;

	public GameObject bottomAnchor;

	public GetFloatVAlue health;

	public GetFloatVAlue armor;

	public GetIntVAlue armorType;

	public GetString killsToMaxKills;

	public GetString timeLeft;

	public UIButton gearToogle;

	public UIButton[] weaponCategoriesButtons;

	public UILabel[] ammoCategoriesLabels;

	public UIButton[] weaponCategoriesButtonsDater;

	public UILabel[] ammoCategoriesLabelsDater;

	public GameObject fonBig;

	public GameObject fonSmall;

	public GameObject settingsController;

	public HeartEffect[] hearts;

	public HeartEffect[] armorShields;

	public HeartEffect[] mechShields;

	public DamageTakenController[] damageTakenControllers;

	private int curDamageTakenController;

	private float timerShowPotion = -1f;

	private float timerShowPotionMax = 10f;

	public SetChatLabelController[] killLabels;

	public GameObject[] chatLabels;

	public UILabel[] messageAddScore;

	public GameObject elixir;

	public GameObject scoreLabel;

	public GameObject enemiesLabel;

	public GameObject timeLabel;

	public GameObject killsLabel;

	public GameObject scopeText;

	public GameObject joystickContainer;

	public GameObject nightVisionEffect;

	public UILabel rulesLabel;

	public Player_move_c playerMoveC;

	private ZombieCreator zombieCreator;

	public UIPanel multyKillPanel;

	public UISprite multyKillSprite;

	private bool isMulti;

	private bool isChatOn;

	private bool isInet;

	private bool isHunger;

	private HungerGameController hungerGameController;

	public GameObject[] upButtonsInShopPanel;

	public GameObject[] upButtonsInShopPanelSwipeRegim;

	public GameObject healthAddButton;

	public GameObject healthAddButtonDater;

	public GameObject ammoAddButton;

	public GameObject ammoAddButtonDater;

	public UITexture[] weaponIcons;

	public UITexture[] weaponIconsDater;

	public GameObject fastShopPanel;

	public UIScrollView changeWeaponScroll;

	public UIWrapContent changeWeaponWrap;

	public GameObject weaponPreviewPrefab;

	public int weaponIndexInScroll;

	public int weaponIndexInScrollOld;

	public int widthWeaponScrollPreview;

	public AudioClip lowResourceBeep;

	public UIPanel joystikPanel;

	public UIPanel shopPanel;

	public UIPanel bloodPanel;

	public UILabel perfectLabels;

	public RespawnWindow respawnWindow;

	public UIPanel offGameGuiPanel;

	public ButtonHandler fastChatButton;

	public UIToggle fastChatToggle;

	public GameObject fastChatPanel;

	public UIButton pauseButton;

	private IEnumerator _lowResourceBeepRoutine;

	private string[] mechShieldsSpriteName = new string[6] { "mech_armor1", "mech_armor2", "mech_armor3", "mech_armor4", "mech_armor5", "mech_armor6" };

	private string[] armSpriteName = new string[6] { "wood_armor", "armor", "gold_armor", "crystal_armor", "red_armor", "adamant_armor" };

	private float timerBlinkNoAmmo;

	private float periodBlink = 2f;

	public UILabel blinkNoAmmoLabel;

	private float timerBlinkNoHeath;

	public UILabel blinkNoHeathLabel;

	public UISprite[] blinkNoHeathFrames;

	private int oldCountHeath;

	public float timerShowScorePict;

	public float maxTimerShowScorePict = 3f;

	public string scorePictName = string.Empty;

	private bool _disabled;

	private float pastHealth;

	private float pastMechHealth;

	private float pastArmor;

	private bool mechWasActive;

	private int currentHealthStep;

	private int currentMechHealthStep;

	private int currentArmorStep;

	private bool healthInAnim;

	private bool armorInAnim;

	private bool mechInAnim;

	public void ShowCircularIndicatorOnReload(float length)
	{
		StopAllCircularIndicators();
		reloadLabel.gameObject.SetActive(true);
		Invoke("ReloadAmmo", length);
		if (playerMoveC != null)
		{
			playerMoveC.isReloading = true;
		}
		RunCircularSpriteOn(reloadCircularSprite, length, delegate
		{
		});
	}

	[Obfuscation(Exclude = true)]
	private void ReloadAmmo()
	{
		reloadLabel.gameObject.SetActive(false);
		WeaponManager.sharedManager.ReloadAmmo();
	}

	public void StartFireCircularIndicators(float length)
	{
		StopAllCircularIndicators();
		RunCircularSpriteOn(fireCircularSprite, length);
		RunCircularSpriteOn(fireAdditionalCrcualrSprite, length);
	}

	private void RunCircularSpriteOn(UITexture sprite, float length, Action onComplete = null)
	{
		sprite.fillAmount = 0f;
		HOTween.To(sprite, length, new TweenParms().Prop("fillAmount", 1f).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.Linear)
			.OnComplete((TweenDelegate.TweenCallback)delegate
			{
				sprite.fillAmount = 0f;
				if (onComplete != null)
				{
					onComplete();
				}
			}));
	}

	public void StopAllCircularIndicators()
	{
		CancelInvoke("ReloadAmmo");
		if (playerMoveC != null)
		{
			playerMoveC.isReloading = false;
		}
		if (circularSprites == null)
		{
			Debug.LogWarning("Circular sprites is null!");
			return;
		}
		UITexture[] array = circularSprites;
		foreach (UITexture uITexture in array)
		{
			HOTween.Kill(uITexture);
			uITexture.fillAmount = 0f;
		}
		reloadLabel.gameObject.SetActive(false);
	}

	public void PlayLowResourceBeep(int count)
	{
		StopPlayingLowResourceBeep();
		_lowResourceBeepRoutine = PlayLowResourceBeepCoroutine(count);
		StartCoroutine(_lowResourceBeepRoutine);
	}

	public void SetEnablePerfectLabel(bool enabled)
	{
		if (!(perfectLabels == null))
		{
			perfectLabels.gameObject.SetActive(enabled);
		}
	}

	public void PlayLowResourceBeepIfNotPlaying(int count)
	{
		if (_lowResourceBeepRoutine == null)
		{
			PlayLowResourceBeep(count);
		}
	}

	public void StopPlayingLowResourceBeep()
	{
		if (_lowResourceBeepRoutine != null)
		{
			StopCoroutine(_lowResourceBeepRoutine);
			_lowResourceBeepRoutine = null;
		}
	}

	private IEnumerator PlayLowResourceBeepCoroutine(int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (Defs.isSoundFX)
			{
				NGUITools.PlaySound(lowResourceBeep);
			}
			yield return new WaitForSeconds(1f);
		}
		_lowResourceBeepRoutine = null;
	}

	public void PanelSlider()
	{
		float num = maxLength - minLength;
		float num2 = minLength;
		bottomPanelSprite.width = (int)(bottomPanelSlider.value * num + num2);
	}

	private void HandleChatSettUpdated()
	{
		isChatOn = Defs.IsChatOn;
	}

	private void Awake()
	{
		sharedInGameGUI = this;
		circularSprites = new UITexture[3] { reloadCircularSprite, fireCircularSprite, fireAdditionalCrcualrSprite };
		changeWeaponScroll.GetComponent<UIPanel>().baseClipRegion = new Vector4(0f, 0f, (float)widthWeaponScrollPreview * 1.3f, (float)widthWeaponScrollPreview * 1.3f);
		changeWeaponWrap.itemSize = widthWeaponScrollPreview;
		bottomPanelSprite.width = (int)defaultPanelLength;
		bottomPanelSlider.value = (defaultPanelLength - minLength) / (maxLength - minLength);
		HandleChatSettUpdated();
		PauseNGUIController.ChatSettUpdated += HandleChatSettUpdated;
		ControlsSettingsBase.ControlsChanged += AdjustToPlayerHands;
		if (Defs.isDaterRegim)
		{
			shopPanelForTap = shopPanelForTapDater;
			shopPanelForSwipe = shopPanelForSwipeDater;
			ammoAddButton = ammoAddButtonDater;
			healthAddButton = healthAddButtonDater;
			for (int i = 0; i < weaponCategoriesButtons.Length; i++)
			{
				weaponCategoriesButtons[i] = weaponCategoriesButtonsDater[i];
			}
			for (int j = 0; j < ammoCategoriesLabels.Length; j++)
			{
				ammoCategoriesLabels[j] = ammoCategoriesLabelsDater[j];
			}
			for (int k = 0; k < weaponIcons.Length; k++)
			{
				weaponIcons[k] = weaponIconsDater[k];
			}
		}
		shopPanelForTap.gameObject.SetActive(true);
		shopPanelForSwipe.gameObject.SetActive(true);
		swipeWeaponPanelPos = swipeWeaponPanel.localPosition;
		shopPanelForTapPos = shopPanelForTap.localPosition;
		shopPanelForSwipePos = shopPanelForSwipe.localPosition;
		SetSwitchingWeaponPanel();
		isMulti = Defs.isMulti;
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			settingsController.SetActive(false);
			centerAnhor.SetActive(false);
		}
		isInet = Defs.isInet;
		isHunger = Defs.isHunger;
		if (isHunger)
		{
			HungerGameController instance = HungerGameController.Instance;
			if (instance == null)
			{
				Debug.LogError("hungerGameControllerObject == null");
			}
			else
			{
				hungerGameController = instance.GetComponent<HungerGameController>();
			}
		}
		aimUpSprite = aimUp.GetComponent<UISprite>();
		aimDownSprite = aimDown.GetComponent<UISprite>();
		aimRightSprite = aimRight.GetComponent<UISprite>();
		aimLeftSprite = aimLeft.GetComponent<UISprite>();
		aimCenterSprite = aimCenter.GetComponent<UISprite>();
		aimUpLeftSprite = aimUpLeft.GetComponent<UISprite>();
		aimDownLeftSprite = aimDownLeft.GetComponent<UISprite>();
		aimDownRightSprite = aimDownRight.GetComponent<UISprite>();
		aimUpRightSprite = aimUpRight.GetComponent<UISprite>();
	}

	public void SetSwipeWeaponPanelVisibility(bool visible)
	{
		swipeWeaponPanel.localPosition = ((!visible) ? (swipeWeaponPanelPos + new Vector3(10000f, 0f, 0f)) : swipeWeaponPanelPos);
	}

	public void SetSwitchingWeaponPanel()
	{
		if (GlobalGameController.switchingWeaponSwipe)
		{
			sharedInGameGUI.swipeWeaponPanel.localPosition = swipeWeaponPanelPos;
			sharedInGameGUI.shopPanelForTap.gameObject.SetActive(false);
			sharedInGameGUI.shopPanelForSwipe.gameObject.SetActive(true);
			return;
		}
		sharedInGameGUI.swipeWeaponPanel.localPosition = new Vector3(10000f, sharedInGameGUI.swipeWeaponPanel.localPosition.y, sharedInGameGUI.swipeWeaponPanel.localPosition.z);
		sharedInGameGUI.shopPanelForTap.gameObject.SetActive(true);
		sharedInGameGUI.shopPanelForSwipe.gameObject.SetActive(false);
		for (int i = 0; i < sharedInGameGUI.upButtonsInShopPanel.Length; i++)
		{
			if (!PotionsController.sharedController.PotionIsActive(sharedInGameGUI.upButtonsInShopPanel[i].GetComponent<ElexirInGameButtonController>().myPotion.name))
			{
				sharedInGameGUI.upButtonsInShopPanel[i].GetComponent<ElexirInGameButtonController>().myLabelTime.gameObject.SetActive(false);
			}
		}
	}

	public void AddDamageTaken(float alpha)
	{
		curDamageTakenController++;
		if (curDamageTakenController >= damageTakenControllers.Length)
		{
			curDamageTakenController = 0;
		}
		damageTakenControllers[curDamageTakenController].reset(alpha);
	}

	public void ResetDamageTaken()
	{
		for (int i = 0; i < damageTakenControllers.Length; i++)
		{
			damageTakenControllers[i].Remove();
		}
	}

	private void AdjustToPlayerHands()
	{
		float num = (GlobalGameController.LeftHanded ? 1 : (-1));
		Vector3[] array = Load.LoadVector3Array(ControlsSettingsBase.JoystickSett);
		if (array == null || array.Length < 7)
		{
			Defs.InitCoordsIphone();
			zoomButton.transform.localPosition = new Vector3((float)Defs.ZoomButtonX * num, Defs.ZoomButtonY, zoomButton.transform.localPosition.z);
			reloadButton.transform.localPosition = new Vector3((float)Defs.ReloadButtonX * num, Defs.ReloadButtonY, reloadButton.transform.localPosition.z);
			jumpButton.transform.localPosition = new Vector3((float)Defs.JumpButtonX * num, Defs.JumpButtonY, jumpButton.transform.localPosition.z);
			fireButton.transform.localPosition = new Vector3((float)Defs.FireButtonX * num, Defs.FireButtonY, fireButton.transform.localPosition.z);
			joystick.transform.localPosition = new Vector3((float)Defs.JoyStickX * num, Defs.JoyStickY, joystick.transform.localPosition.z);
			grenadeButton.transform.localPosition = new Vector3((float)Defs.GrenadeX * num, Defs.GrenadeY, grenadeButton.transform.localPosition.z);
			fireButtonInJoystick.transform.localPosition = new Vector3((float)Defs.FireButton2X * num, Defs.FireButton2Y, fireButtonInJoystick.transform.localPosition.z);
		}
		else
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].x *= num;
			}
			zoomButton.transform.localPosition = array[0];
			reloadButton.transform.localPosition = array[1];
			jumpButton.transform.localPosition = array[2];
			fireButton.transform.localPosition = array[3];
			joystick.transform.localPosition = array[4];
			grenadeButton.transform.localPosition = array[5];
			fireButtonInJoystick.transform.localPosition = array[6];
		}
		UISprite[] array2 = new GameObject[7] { zoomButton, reloadButton, jumpButton, fireButton, joystick, grenadeButton, fireButtonInJoystick }.Select((GameObject go) => go.GetComponent<UISprite>()).ToArray();
		object obj = Json.Deserialize(PlayerPrefs.GetString("Controls.Size", "[]"));
		List<object> list = obj as List<object>;
		if (list == null)
		{
			list = new List<object>(array2.Length);
			Debug.LogWarning(list.GetType().FullName);
		}
		int num2 = Math.Min(list.Count, array2.Length);
		for (int j = 0; j != num2; j++)
		{
			int num3 = Convert.ToInt32(list[j]);
			if (num3 <= 0)
			{
				continue;
			}
			UISprite uISprite = array2[j];
			if (uISprite == null)
			{
				continue;
			}
			array2[j].keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
			array2[j].width = num3;
			if (uISprite.gameObject == joystick)
			{
				UIJoystick component = uISprite.GetComponent<UIJoystick>();
				if (!(component == null))
				{
					float radius = component.radius;
					float num4 = radius / 144f;
					component.ActualRadius = num4 * (float)num3;
				}
			}
		}
	}

	private void Start()
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			SetSwipeWeaponPanelVisibility(false);
		}
		shopButton.SetActive(!Device.isPixelGunLow);
		shopButtonInPause.SetActive(!Device.isPixelGunLow);
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();
		if (!Defs.isMulti && !Defs.IsSurvival)
		{
			CampaignContainer.SetActive(true);
		}
		if (!Defs.isMulti && Defs.IsSurvival)
		{
			survivalContainer.SetActive(true);
		}
		if (Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Deathmatch)
		{
			if (Defs.isDaterRegim)
			{
				daterContainer.SetActive(true);
			}
			else
			{
				deathmatchContainer.SetActive(true);
			}
		}
		if (Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
		{
			timeBattleContainer.SetActive(true);
		}
		if (Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TeamFight)
		{
			teamBattleContainer.SetActive(true);
		}
		if (Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			flagCaptureContainer.SetActive(true);
		}
		if (Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.DeadlyGames)
		{
			deadlygamesContainer.SetActive(true);
		}
		if (Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
		{
			CapturePointContainer.SetActive(true);
		}
		turretPanel.SetActive(false);
		if (runTurrelButton != null)
		{
			runTurrelButton.Clicked += RunTurret;
		}
		if (cancelTurrelButton != null)
		{
			cancelTurrelButton.Clicked += CancelTurret;
		}
		if (isMulti)
		{
			enemiesLeftLabel.SetActive(false);
		}
		else
		{
			zombieCreator = ZombieCreator.sharedCreator;
		}
		AdjustToPlayerHands();
		PauseNGUIController.PlayerHandUpdated += AdjustToPlayerHands;
		PauseNGUIController.SwitchingWeaponsUpdated += SetSwitchingWeaponPanel;
		WeaponManager.WeaponEquipped += HandleWeaponEquipped;
		int num = ((!isMulti) ? WeaponManager.sharedManager.CurrentWeaponIndex : WeaponManager.sharedManager.CurrentIndexOfLastUsedWeaponInPlayerWeapons());
		HandleWeaponEquipped(((Weapon)WeaponManager.sharedManager.playerWeapons[num]).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1);
		if (num < changeWeaponWrap.transform.childCount)
		{
			changeWeaponWrap.GetComponent<MyCenterOnChild>().springStrength = 1E+11f;
			changeWeaponWrap.GetComponent<MyCenterOnChild>().CenterOn(changeWeaponWrap.transform.GetChild(num));
			changeWeaponWrap.GetComponent<MyCenterOnChild>().springStrength = 8f;
		}
		else
		{
			Debug.LogError("InGameGUI: not weapon icon with index " + (((Weapon)WeaponManager.sharedManager.playerWeapons[num]).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1));
		}
		if (gearToogle != null)
		{
			gearToogle.gameObject.GetComponent<ButtonHandler>().Clicked += HandleGearToogleClicked;
		}
		if (weaponCategoriesButtons[0] != null)
		{
			weaponCategoriesButtons[0].gameObject.GetComponent<ButtonHandler>().Clicked += HandlePrimaryToogleClicked;
		}
		if (weaponCategoriesButtons[1] != null)
		{
			weaponCategoriesButtons[1].gameObject.GetComponent<ButtonHandler>().Clicked += HandleBackupToogleClicked;
		}
		if (weaponCategoriesButtons[2] != null)
		{
			weaponCategoriesButtons[2].gameObject.GetComponent<ButtonHandler>().Clicked += HandleMeleeToogleClicked;
		}
		if (weaponCategoriesButtons[3] != null)
		{
			weaponCategoriesButtons[3].gameObject.GetComponent<ButtonHandler>().Clicked += HandleSpecialToogleClicked;
		}
		if (weaponCategoriesButtons[4] != null)
		{
			weaponCategoriesButtons[4].gameObject.GetComponent<ButtonHandler>().Clicked += HandleSniperToogleClicked;
		}
		if (weaponCategoriesButtons[5] != null)
		{
			weaponCategoriesButtons[5].gameObject.GetComponent<ButtonHandler>().Clicked += HandlePremiumToogleClicked;
		}
		fastChatPanel.SetActive(false);
		if (Defs.isMulti)
		{
			fastChatButton.Clicked += FastChatClick;
		}
		else
		{
			fastChatButton.gameObject.SetActive(false);
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			gearToogle.GetComponent<UIToggle>().value = false;
			HandleGearToogleClicked(null, null);
		}
		for (int i = 0; i < upButtonsInShopPanel.Length; i++)
		{
			StartUpdatePotionButton(upButtonsInShopPanel[i]);
		}
		for (int j = 0; j < upButtonsInShopPanelSwipeRegim.Length; j++)
		{
			StartUpdatePotionButton(upButtonsInShopPanelSwipeRegim[j]);
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
		{
			fastShopPanel.transform.localPosition = new Vector3(-1000f, -1000f, -1f);
			gearToogle.isEnabled = false;
		}
		SetNGUITouchDragThreshold(1f);
		if (Device.isPixelGunLow)
		{
			rightAnchor.transform.localPosition -= Vector3.left * 119f;
		}
	}

	public void ShowTurretInterface()
	{
		swipeWeaponPanel.gameObject.SetActive(false);
		shopPanelForSwipe.gameObject.SetActive(false);
		shopPanelForTap.gameObject.SetActive(false);
		runTurrelButton.GetComponent<UIButton>().isEnabled = false;
		turretPanel.SetActive(true);
		if (playerMoveC != null)
		{
			playerMoveC.ChangeWeapon(1001, false);
		}
		shopButtonInPause.GetComponent<UIButton>().isEnabled = false;
	}

	public void HideTurretInterface()
	{
		if (GlobalGameController.switchingWeaponSwipe)
		{
			shopPanelForSwipe.gameObject.SetActive(true);
		}
		else
		{
			shopPanelForTap.gameObject.SetActive(true);
		}
		swipeWeaponPanel.gameObject.SetActive(true);
		turretPanel.SetActive(false);
		shopButtonInPause.GetComponent<UIButton>().isEnabled = true;
	}

	private void FastChatClick(object sender, EventArgs e)
	{
		SetVisibleFactChatPanel(fastChatToggle.value);
	}

	public void SetVisibleFactChatPanel(bool _visible)
	{
		fastChatPanel.SetActive(_visible);
	}

	private void RunTurret(object sender, EventArgs e)
	{
		if (playerMoveC != null)
		{
			playerMoveC.RunTurret();
		}
		HideTurretInterface();
	}

	private void CancelTurret(object sender, EventArgs e)
	{
		if (playerMoveC != null)
		{
			playerMoveC.CancelTurret();
		}
		HideTurretInterface();
	}

	private void StartUpdatePotionButton(GameObject potionButton)
	{
		if (potionButton != null)
		{
			potionButton.gameObject.GetComponent<ButtonHandler>().Clicked += HandlePotionClicked;
			ElexirInGameButtonController component = potionButton.GetComponent<ElexirInGameButtonController>();
			string text = component.myPotion.name;
			string key = ((!Defs.isDaterRegim) ? text : GearManager.HolderQuantityForID(component.idForPriceInDaterRegim));
			if (PotionsController.sharedController.PotionIsActive(text))
			{
				UIButton component2 = potionButton.GetComponent<UIButton>();
				component.isActivePotion = true;
				component.myLabelTime.gameObject.SetActive(true);
				component.myLabelTime.enabled = true;
				component.priceLabel.SetActive(false);
				component.myLabelCount.gameObject.SetActive(true);
				component.plusSprite.SetActive(false);
				component.myLabelCount.text = Storager.getInt(key, false).ToString();
				component2.isEnabled = false;
			}
		}
	}

	public void HandleBuyGrenadeClicked(object sender, EventArgs e)
	{
		if (!Defs.isDaterRegim)
		{
			return;
		}
		string parameterValue = GearManager.AnalyticsIDForOneItemOfGear(GearManager.Like, true);
		ItemPrice priceByShopId = ItemDb.GetPriceByShopId(GearManager.OneItemIDForGear("LikeID", GearManager.CurrentNumberOfUphradesForGear("LikeID")));
		ItemPrice price = new ItemPrice(priceByShopId.Price * 1, priceByShopId.Currency);
		ShopNGUIController.TryToBuy(base.gameObject, price, delegate
		{
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.GrenadeCount++;
			}
			FlurryPluginWrapper.LogPurchaseByModes(ShopNGUIController.CategoryNames.GearCategory, GearManager.Like, 1, true);
			FlurryPluginWrapper.LogGearPurchases(GearManager.Like, 1, true);
			FlurryEvents.LogSales(GearManager.Like, "Gear");
			Dictionary<string, string> parameters2 = new Dictionary<string, string> { { "Succeeded", parameterValue } };
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Fast Purchase", parameters2);
			FlurryPluginWrapper.LogFastPurchase(parameterValue);
		}, delegate
		{
			JoystickController.leftJoystick.Reset();
			Dictionary<string, string> parameters = new Dictionary<string, string> { { "Failed", parameterValue } };
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Fast Purchase", parameters);
		});
	}

	private void ClickPotionButton(int index)
	{
		timerShowPotion = timerShowPotionMax;
		ElexirInGameButtonController myController = upButtonsInShopPanel[index].GetComponent<ElexirInGameButtonController>();
		ElexirInGameButtonController myController2 = upButtonsInShopPanelSwipeRegim[index].GetComponent<ElexirInGameButtonController>();
		UIButton myButton = upButtonsInShopPanel[index].GetComponent<UIButton>();
		UIButton myButton2 = upButtonsInShopPanelSwipeRegim[index].GetComponent<UIButton>();
		string text = myController.myPotion.name;
		string myStaragerKey = ((!Defs.isDaterRegim) ? text : GearManager.HolderQuantityForID(myController.idForPriceInDaterRegim));
		int @int = Storager.getInt(myStaragerKey, false);
		if (@int > 0)
		{
			if (text.Equals(GearManager.Turret))
			{
				ShowTurretInterface();
			}
			else
			{
				if (Defs.isDaterRegim)
				{
					Storager.setInt(myStaragerKey, Storager.getInt(myStaragerKey, false) - 1, false);
				}
				PotionsController.sharedController.ActivatePotion(text, playerMoveC, new Dictionary<string, object>());
			}
			string text2 = Storager.getInt(myStaragerKey, false).ToString();
			myController.myLabelCount.gameObject.SetActive(true);
			myController.plusSprite.SetActive(false);
			myController.myLabelCount.text = text2;
			myController.isActivePotion = true;
			myButton.isEnabled = false;
			myController.myLabelTime.enabled = true;
			myController.myLabelTime.gameObject.SetActive(true);
			myController2.myLabelCount.gameObject.SetActive(true);
			myController2.plusSprite.SetActive(false);
			myController2.myLabelCount.text = text2;
			myController2.isActivePotion = true;
			myButton2.isEnabled = false;
			myController2.myLabelTime.enabled = true;
			myController2.myLabelTime.gameObject.SetActive(true);
			return;
		}
		string parameterValue = GearManager.AnalyticsIDForOneItemOfGear(myStaragerKey ?? "Potion", true);
		ItemPrice priceByShopId = ItemDb.GetPriceByShopId(GearManager.OneItemIDForGear(myStaragerKey, GearManager.CurrentNumberOfUphradesForGear(myStaragerKey)));
		ShopNGUIController.TryToBuy(base.gameObject, priceByShopId, delegate
		{
			Storager.setInt(myStaragerKey, Storager.getInt(myStaragerKey, false) + 1, false);
			myButton.normalSprite = "game_clear";
			myButton.pressedSprite = "game_clear_n";
			myController.myLabelCount.gameObject.SetActive(true);
			myController.plusSprite.SetActive(false);
			myController.priceLabel.SetActive(false);
			myController.myLabelCount.text = Storager.getInt(myStaragerKey, false).ToString();
			myButton2.normalSprite = "game_clear";
			myButton2.pressedSprite = "game_clear_n";
			myController2.myLabelCount.gameObject.SetActive(true);
			myController2.plusSprite.SetActive(false);
			myController2.priceLabel.SetActive(false);
			myController2.myLabelCount.text = Storager.getInt(myStaragerKey, false).ToString();
			if (myStaragerKey != null)
			{
				FlurryPluginWrapper.LogPurchaseByModes(ShopNGUIController.CategoryNames.GearCategory, GearManager.HolderQuantityForID(myStaragerKey), 1, true);
				FlurryPluginWrapper.LogGearPurchases(GearManager.HolderQuantityForID(myStaragerKey), 1, true);
				FlurryEvents.LogSales(GearManager.HolderQuantityForID(myStaragerKey), "Gear");
			}
			Dictionary<string, string> parameters2 = new Dictionary<string, string> { { "Succeeded", parameterValue } };
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Fast Purchase", parameters2);
			FlurryPluginWrapper.LogFastPurchase(parameterValue);
		}, delegate
		{
			JoystickController.leftJoystick.Reset();
			Dictionary<string, string> parameters = new Dictionary<string, string> { { "Failed", parameterValue } };
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Fast Purchase", parameters);
		});
	}

	private void HandlePotionClicked(object sender, EventArgs e)
	{
		int index = 0;
		for (int i = 0; i < upButtonsInShopPanel.Length; i++)
		{
			if (upButtonsInShopPanel[i].name.Equals(((ButtonHandler)sender).gameObject.name))
			{
				index = i;
				break;
			}
		}
		ClickPotionButton(index);
	}

	private void HandleGearToogleClicked(object sender, EventArgs e)
	{
		bool value = gearToogle.GetComponent<UIToggle>().value;
		fonBig.SetActive(value);
		if (value)
		{
			timerShowPotion = timerShowPotionMax;
		}
		else
		{
			timerShowPotion = -1f;
		}
		for (int i = 0; i < upButtonsInShopPanel.Length; i++)
		{
			upButtonsInShopPanel[i].SetActive(value);
		}
	}

	private void HandlePrimaryToogleClicked(object sender, EventArgs e)
	{
		SelectWeaponFromCategory(1);
	}

	private void HandleBackupToogleClicked(object sender, EventArgs e)
	{
		SelectWeaponFromCategory(2);
	}

	private void HandleMeleeToogleClicked(object sender, EventArgs e)
	{
		SelectWeaponFromCategory(3);
	}

	private void HandleSpecialToogleClicked(object sender, EventArgs e)
	{
		SelectWeaponFromCategory(4);
	}

	private void HandleSniperToogleClicked(object sender, EventArgs e)
	{
		SelectWeaponFromCategory(5);
	}

	private void HandlePremiumToogleClicked(object sender, EventArgs e)
	{
		SelectWeaponFromCategory(6);
	}

	private void SelectWeaponFromCategory(int category, bool isUpdateSwipe = true)
	{
		for (int i = 0; i < WeaponManager.sharedManager.playerWeapons.Count; i++)
		{
			Weapon weapon = (Weapon)WeaponManager.sharedManager.playerWeapons[i];
			if (weapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor == category)
			{
				SelectWeaponFromIndex(i, isUpdateSwipe);
				break;
			}
		}
	}

	private void SelectWeaponFromIndex(int _index, bool updateSwipe = true)
	{
		bool[] array = new bool[6];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = false;
		}
		int num = 0;
		foreach (Weapon playerWeapon in WeaponManager.sharedManager.playerWeapons)
		{
			int num2 = playerWeapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1;
			array[num2] = true;
			num++;
		}
		for (int j = 0; j < weaponCategoriesButtons.Length; j++)
		{
			weaponCategoriesButtons[j].isEnabled = array[j];
			if (j == ((Weapon)WeaponManager.sharedManager.playerWeapons[_index]).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1)
			{
				weaponCategoriesButtons[j].GetComponent<UIToggle>().value = true;
			}
			else
			{
				weaponCategoriesButtons[j].GetComponent<UIToggle>().value = false;
			}
		}
		SetChangeWeapon(_index, updateSwipe);
	}

	private void SetChangeWeapon(int index, bool isUpdateSwipe)
	{
		if (isUpdateSwipe)
		{
			if (index < changeWeaponWrap.transform.childCount)
			{
				changeWeaponWrap.GetComponent<MyCenterOnChild>().springStrength = 1E+11f;
				changeWeaponWrap.GetComponent<MyCenterOnChild>().CenterOn(changeWeaponWrap.transform.GetChild(index));
				changeWeaponWrap.GetComponent<MyCenterOnChild>().springStrength = 8f;
			}
			else
			{
				Debug.LogError("InGameGUI: not weapon icon with index " + index);
			}
		}
		if (WeaponManager.sharedManager.CurrentWeaponIndex == index)
		{
			return;
		}
		WeaponManager.sharedManager.CurrentWeaponIndex = index;
		WeaponManager.sharedManager.SaveWeaponAsLastUsed(WeaponManager.sharedManager.CurrentWeaponIndex);
		if (playerMoveC != null)
		{
			if (playerMoveC.currentWeaponBeforeTurret >= 0)
			{
				playerMoveC.currentWeaponBeforeTurret = index;
				return;
			}
			playerMoveC.ChangeWeapon(index, false);
			playerMoveC.HideChangeWeaponTrainingHint();
		}
	}

	[Obfuscation(Exclude = true)]
	private void GenerateMiganie()
	{
		CoinsMessage.FireCoinsAddedEvent();
	}

	private void CheckWeaponScrollChanged()
	{
		if (!_disabled)
		{
			if (changeWeaponScroll.transform.localPosition.x > 0f)
			{
				weaponIndexInScroll = Mathf.RoundToInt((changeWeaponScroll.transform.localPosition.x - (float)(Mathf.FloorToInt(changeWeaponScroll.transform.localPosition.x / (float)widthWeaponScrollPreview / (float)changeWeaponWrap.transform.childCount) * widthWeaponScrollPreview * changeWeaponWrap.transform.childCount)) / (float)widthWeaponScrollPreview);
				weaponIndexInScroll = changeWeaponWrap.transform.childCount - weaponIndexInScroll;
			}
			else
			{
				weaponIndexInScroll = -1 * Mathf.RoundToInt((changeWeaponScroll.transform.localPosition.x - (float)(Mathf.CeilToInt(changeWeaponScroll.transform.localPosition.x / (float)widthWeaponScrollPreview / (float)changeWeaponWrap.transform.childCount) * widthWeaponScrollPreview * changeWeaponWrap.transform.childCount)) / (float)widthWeaponScrollPreview);
			}
			if (weaponIndexInScroll == changeWeaponWrap.transform.childCount)
			{
				weaponIndexInScroll = 0;
			}
			if (weaponIndexInScroll != weaponIndexInScrollOld)
			{
				SelectWeaponFromCategory(((Weapon)WeaponManager.sharedManager.playerWeapons[weaponIndexInScroll]).weaponPrefab.GetComponent<WeaponSounds>().categoryNabor, false);
			}
			weaponIndexInScrollOld = weaponIndexInScroll;
		}
	}

	public IEnumerator _DisableSwiping(float tm)
	{
		MyCenterOnChild _center = changeWeaponWrap.GetComponent<MyCenterOnChild>();
		int bef;
		if (_center == null || _center.centeredObject == null || !int.TryParse(_center.centeredObject.name.Replace("WeaponCat_", string.Empty), out bef))
		{
			yield break;
		}
		_disabled = true;
		yield return new WaitForSeconds(tm);
		_disabled = false;
		if (_center.centeredObject == null || _center.centeredObject.name.Equals("WeaponCat_" + bef))
		{
			yield break;
		}
		Transform goToCent = null;
		foreach (Transform t in _center.transform)
		{
			if (t.gameObject.name.Equals("WeaponCat_" + bef))
			{
				goToCent = t;
				break;
			}
		}
		if (goToCent != null)
		{
			_center.CenterOn(goToCent);
		}
	}

	private void Update()
	{
		CheckWeaponScrollChanged();
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.TapToSelectWeapon)
		{
			fastShopPanel.transform.localPosition = new Vector3(0f, 0f, -1f);
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.TapToThrowGrenade)
		{
			fastShopPanel.transform.localPosition = new Vector3(0f, 0f, -1f);
		}
		if (timerBlinkNoAmmo > 0f)
		{
			timerBlinkNoAmmo -= Time.deltaTime;
		}
		if (timerBlinkNoAmmo > 0f && playerMoveC != null && !playerMoveC.isMechActive)
		{
			blinkNoAmmoLabel.gameObject.SetActive(true);
			float num = timerBlinkNoAmmo % periodBlink / periodBlink;
			blinkNoAmmoLabel.color = new Color(blinkNoAmmoLabel.color.r, blinkNoAmmoLabel.color.g, blinkNoAmmoLabel.color.b, (!(num < 0.5f)) ? ((1f - num) * 2f) : (num * 2f));
		}
		if ((timerBlinkNoAmmo < 0f || (playerMoveC != null && playerMoveC.isMechActive)) && blinkNoAmmoLabel.gameObject.activeSelf)
		{
			blinkNoAmmoLabel.gameObject.SetActive(false);
		}
		if (playerMoveC != null)
		{
			int num2 = Mathf.FloorToInt(playerMoveC.CurHealth);
			if (num2 < oldCountHeath && timerBlinkNoHeath < 0f && num2 < 3)
			{
				timerBlinkNoHeath = periodBlink * 3f;
			}
			if (num2 > 2)
			{
				timerBlinkNoHeath = -1f;
			}
			oldCountHeath = num2;
			if (timerBlinkNoHeath > 0f)
			{
				timerBlinkNoHeath -= Time.deltaTime;
			}
			if (timerBlinkNoHeath > 0f && !playerMoveC.isMechActive)
			{
				if (num2 > 0)
				{
					PlayLowResourceBeepIfNotPlaying(1);
				}
				blinkNoHeathLabel.gameObject.SetActive(true);
				float num3 = timerBlinkNoHeath % periodBlink / periodBlink;
				float a = ((!(num3 < 0.5f)) ? ((1f - num3) * 2f) : (num3 * 2f));
				blinkNoHeathLabel.color = new Color(blinkNoHeathLabel.color.r, blinkNoHeathLabel.color.g, blinkNoHeathLabel.color.b, a);
				for (int i = 0; i < blinkNoHeathFrames.Length; i++)
				{
					blinkNoHeathFrames[i].gameObject.SetActive(true);
					blinkNoHeathFrames[i].color = new Color(1f, 1f, 1f, a);
				}
			}
		}
		if ((timerBlinkNoHeath < 0f || playerMoveC == null || (playerMoveC != null && playerMoveC.isMechActive)) && blinkNoHeathLabel.gameObject.activeSelf)
		{
			blinkNoHeathLabel.gameObject.SetActive(false);
			for (int j = 0; j < blinkNoHeathFrames.Length; j++)
			{
				blinkNoHeathFrames[j].gameObject.SetActive(false);
			}
		}
		for (int k = 0; k < ammoCategoriesLabels.Length; k++)
		{
			if (!(ammoCategoriesLabels[k] != null))
			{
				continue;
			}
			bool flag = false;
			if (weaponCategoriesButtons[k].isEnabled)
			{
				for (int l = 0; l < WeaponManager.sharedManager.playerWeapons.Count; l++)
				{
					Weapon weapon = (Weapon)WeaponManager.sharedManager.playerWeapons[l];
					if ((!weapon.weaponPrefab.GetComponent<WeaponSounds>().isMelee || weapon.weaponPrefab.GetComponent<WeaponSounds>().isShotMelee) && weapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor == k + 1)
					{
						ammoCategoriesLabels[k].text = ((!weapon.weaponPrefab.GetComponent<WeaponSounds>().isShotMelee) ? (weapon.currentAmmoInClip + "/" + weapon.currentAmmoInBackpack) : (weapon.currentAmmoInClip + weapon.currentAmmoInBackpack).ToString());
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				ammoCategoriesLabels[k].text = string.Empty;
			}
		}
		if (timerShowNow > 0f)
		{
			timerShowNow -= Time.deltaTime;
			if (!message_now.activeSelf)
			{
				message_now.SetActive(true);
			}
		}
		else if (message_now.activeSelf)
		{
			message_now.SetActive(false);
		}
		if (isMulti && playerMoveC == null && WeaponManager.sharedManager.myPlayer != null)
		{
			playerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
		}
		if (!isMulti && playerMoveC == null)
		{
			playerMoveC = WeaponManager.sharedManager.myPlayerMoveC;
		}
		if (isMulti && playerMoveC != null)
		{
			for (int m = 0; m < 3; m++)
			{
				float num4 = 0.3f;
				float num5 = 0.2f;
				if (m == 0)
				{
					float num6 = 1f;
					if (playerMoveC.myScoreController.maxTimerSumMessage - playerMoveC.myScoreController.timerAddScoreShow[m] < num4)
					{
						num6 = 1f + num5 * (playerMoveC.myScoreController.maxTimerSumMessage - playerMoveC.myScoreController.timerAddScoreShow[m]) / num4;
					}
					if (playerMoveC.myScoreController.maxTimerSumMessage - playerMoveC.myScoreController.timerAddScoreShow[m] - num4 < num4)
					{
						num6 = 1f + num5 * (1f - (playerMoveC.myScoreController.maxTimerSumMessage - playerMoveC.myScoreController.timerAddScoreShow[m] - num4) / num4);
					}
					messageAddScore[m].transform.localScale = new Vector3(num6, num6, num6);
				}
				if (playerMoveC.timerShow[m] > 0f)
				{
					killLabels[m].gameObject.SetActive(true);
					killLabels[m].SetChatLabelText(playerMoveC.killedSpisok[m][0], playerMoveC.killedSpisok[m][1], playerMoveC.killedSpisok[m][2], playerMoveC.killedSpisok[m][3]);
				}
				else
				{
					killLabels[m].gameObject.SetActive(false);
				}
				if (playerMoveC.myScoreController.timerAddScoreShow[m] > 0f)
				{
					if (!messageAddScore[m].gameObject.activeSelf)
					{
						messageAddScore[m].gameObject.SetActive(true);
					}
					messageAddScore[m].text = playerMoveC.myScoreController.addScoreString[m];
					messageAddScore[m].color = new Color(1f, 1f, 1f, (!(playerMoveC.myScoreController.timerAddScoreShow[m] > 1f)) ? playerMoveC.myScoreController.timerAddScoreShow[m] : 1f);
				}
				else if (messageAddScore[m].gameObject.activeSelf)
				{
					messageAddScore[m].gameObject.SetActive(false);
				}
			}
			if (isChatOn)
			{
				int num7 = 0;
				int num8 = playerMoveC.messages.Count - 1;
				while (num8 >= 0 && playerMoveC.messages.Count - num8 - 1 < 3)
				{
					if (Time.time - playerMoveC.messages[num8].time < 10f)
					{
						if ((!isInet && playerMoveC.messages[num8].IDLocal == WeaponManager.sharedManager.myPlayer.GetComponent<NetworkView>().viewID) || (isInet && playerMoveC.messages[num8].ID == WeaponManager.sharedManager.myPlayer.GetComponent<PhotonView>().viewID))
						{
							chatLabels[num7].GetComponent<UILabel>().color = new Color(0f, 1f, 0.15f, 1f);
						}
						else
						{
							if (playerMoveC.messages[num8].command == 0)
							{
								chatLabels[num7].GetComponent<UILabel>().color = new Color(1f, 1f, 0.15f, 1f);
							}
							if (playerMoveC.messages[num8].command == 1)
							{
								chatLabels[num7].GetComponent<UILabel>().color = new Color(0f, 0f, 0.9f, 1f);
							}
							if (playerMoveC.messages[num8].command == 2)
							{
								chatLabels[num7].GetComponent<UILabel>().color = new Color(1f, 0f, 0f, 1f);
							}
						}
						ChatLabel component = chatLabels[num7].GetComponent<ChatLabel>();
						component.nickLabel.text = playerMoveC.messages[num8].text;
						component.iconSprite.spriteName = playerMoveC.messages[num8].iconName;
						Transform transform = component.iconSprite.transform;
						transform.localPosition = new Vector3(component.nickLabel.width + 20, transform.localPosition.y, transform.localPosition.z);
						component.clanTexture.mainTexture = playerMoveC.messages[num8].clanLogo;
						chatLabels[num7].SetActive(true);
					}
					else
					{
						chatLabels[num7].SetActive(false);
					}
					num7++;
					num8--;
				}
				for (int n = num7; n < 3; n++)
				{
					chatLabels[num7].SetActive(false);
				}
			}
			if (timerShowScorePict > 0f)
			{
				timerShowScorePict -= Time.deltaTime;
			}
			if (isHunger && Initializer.players.Count == 2 && hungerGameController.isGo && playerMoveC.timeHingerGame > 10f)
			{
				duel.SetActive(true);
				multyKillPanel.gameObject.SetActive(false);
			}
			else
			{
				if (duel.activeSelf)
				{
					duel.SetActive(false);
				}
				if (timerShowScorePict > 0f)
				{
					multyKillSprite.spriteName = scorePictName;
					multyKillPanel.gameObject.SetActive(true);
					float num9 = 1f;
					float num10 = 0.5f;
					if (timerShowScorePict > maxTimerShowScorePict - num10)
					{
						num9 = (maxTimerShowScorePict - timerShowScorePict) / num10;
					}
					if (timerShowScorePict < num10)
					{
						num9 = timerShowScorePict / num10;
					}
					multyKillPanel.transform.localScale = new Vector3(num9, num9, num9);
				}
				else if (multyKillPanel.gameObject.activeSelf)
				{
					multyKillPanel.gameObject.SetActive(false);
				}
			}
			if (isHunger && !hungerGameController.isGo)
			{
				timerStartHungerLabel.gameObject.SetActive(true);
				int num11 = Mathf.FloorToInt(hungerGameController.goTimer);
				string text;
				if (num11 == 0)
				{
					text = "GO!";
					timerStartHungerLabel.color = new Color(0f, 1f, 0f, 1f);
				}
				else
				{
					text = string.Empty + num11;
					timerStartHungerLabel.color = new Color(1f, 0f, 0f, 1f);
				}
				timerStartHungerLabel.text = text;
			}
			else if (isHunger && hungerGameController.isGo && hungerGameController.isShowGo)
			{
				timerStartHungerLabel.gameObject.SetActive(true);
				timerStartHungerLabel.text = "GO!";
			}
			else
			{
				timerStartHungerLabel.gameObject.SetActive(false);
			}
		}
		if (playerMoveC != null)
		{
			if (playerMoveC.timerShowDown > 0f && playerMoveC.timerShowDown < playerMoveC.maxTimeSetTimerShow - 0.03f)
			{
				downBloodTexture.SetActive(true);
			}
			else
			{
				downBloodTexture.SetActive(false);
			}
			if (playerMoveC.timerShowUp > 0f && playerMoveC.timerShowUp < playerMoveC.maxTimeSetTimerShow - 0.03f)
			{
				upBloodTexture.SetActive(true);
			}
			else
			{
				upBloodTexture.SetActive(false);
			}
			if (playerMoveC.timerShowLeft > 0f && playerMoveC.timerShowLeft < playerMoveC.maxTimeSetTimerShow - 0.03f)
			{
				leftBloodTexture.SetActive(true);
			}
			else
			{
				leftBloodTexture.SetActive(false);
			}
			if (playerMoveC.timerShowRight > 0f && playerMoveC.timerShowRight < playerMoveC.maxTimeSetTimerShow - 0.03f)
			{
				rightBloodTexture.SetActive(true);
			}
			else
			{
				rightBloodTexture.SetActive(false);
			}
			if (!playerMoveC.isZooming && (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage != 0 || !TrainingController.isPressSkip))
			{
				if (!aimUp.activeSelf)
				{
					aimUp.SetActive(true);
				}
				if (!aimDown.activeSelf)
				{
					aimDown.SetActive(true);
				}
				if (!aimRight.activeSelf)
				{
					aimRight.SetActive(true);
				}
				if (!aimLeft.activeSelf)
				{
					aimLeft.SetActive(true);
				}
				if (!string.IsNullOrEmpty(WeaponManager.sharedManager.currentWeaponSounds.aimCornerPartSprite))
				{
					if (!aimUpLeft.activeSelf)
					{
						aimUpLeft.SetActive(true);
					}
					if (!aimDownLeft.activeSelf)
					{
						aimDownLeft.SetActive(true);
					}
					if (!aimDownRight.activeSelf)
					{
						aimDownRight.SetActive(true);
					}
					if (!aimUpRight.activeSelf)
					{
						aimUpRight.SetActive(true);
					}
				}
				if (!string.IsNullOrEmpty(WeaponManager.sharedManager.currentWeaponSounds.aimCenterSprite) && !aimCenter.activeSelf)
				{
					aimCenter.SetActive(true);
				}
				float num12 = 8f;
				num12 = (playerMoveC.isMechActive ? (num12 + (4f + playerMoveC.mechWeaponSounds.tekKoof * playerMoveC.mechWeaponSounds.startZone.y * 0.5f)) : (num12 + WeaponManager.sharedManager.currentWeaponSounds.tekKoof * WeaponManager.sharedManager.currentWeaponSounds.startZone.y * 0.5f));
				aimUp.transform.localPosition = new Vector3(0f, num12, 0f);
				aimUpRight.transform.localPosition = new Vector3(num12, num12, 0f);
				aimRight.transform.localPosition = new Vector3(num12, 0f, 0f);
				aimDownRight.transform.localPosition = new Vector3(num12, 0f - num12, 0f);
				aimDown.transform.localPosition = new Vector3(0f, 0f - num12, 0f);
				aimDownLeft.transform.localPosition = new Vector3(0f - num12, 0f - num12, 0f);
				aimLeft.transform.localPosition = new Vector3(0f - num12, 0f, 0f);
				aimUpLeft.transform.localPosition = new Vector3(0f - num12, num12, 0f);
			}
			else
			{
				if (aimUp.activeSelf)
				{
					aimUp.SetActive(false);
				}
				if (aimDown.activeSelf)
				{
					aimDown.SetActive(false);
				}
				if (aimRight.activeSelf)
				{
					aimRight.SetActive(false);
				}
				if (aimLeft.activeSelf)
				{
					aimLeft.SetActive(false);
				}
				if (aimCenter.activeSelf)
				{
					aimCenter.SetActive(false);
				}
				if (aimUpLeft.activeSelf)
				{
					aimUpLeft.SetActive(false);
				}
				if (aimDownLeft.activeSelf)
				{
					aimDownLeft.SetActive(false);
				}
				if (aimDownRight.activeSelf)
				{
					aimDownRight.SetActive(false);
				}
				if (aimUpRight.activeSelf)
				{
					aimUpRight.SetActive(false);
				}
			}
		}
		bool flag2 = true;
		if (SceneManager.GetActiveScene().name == Defs.TrainingSceneName)
		{
			flag2 = false;
		}
		shopButton.GetComponent<UIButton>().isEnabled = flag2 && !turretPanel.activeSelf;
		if (!isMulti && zombieCreator != null)
		{
			int num13 = GlobalGameController.EnemiesToKill - zombieCreator.NumOfDeadZombies;
			if (!Defs.IsSurvival && num13 == 0)
			{
				string text2 = ((!LevelBox.weaponsFromBosses.ContainsKey(Application.loadedLevelName)) ? LocalizationStore.Get("Key_0854") : LocalizationStore.Get("Key_0192"));
				if (zombieCreator.bossShowm)
				{
					text2 = LocalizationStore.Get("Key_0855");
				}
				enemiesLeftLabel.SetActive(perfectLabels == null || !perfectLabels.gameObject.activeInHierarchy);
				enemiesLeftLabel.GetComponent<UILabel>().text = text2;
			}
			else
			{
				enemiesLeftLabel.SetActive(false);
			}
		}
		if (playerMoveC != null && playerMoveC.isMechActive)
		{
			if (!mechWasActive)
			{
				currentHealthStep = Mathf.CeilToInt(health());
				currentArmorStep = Mathf.CeilToInt(armor());
				pastMechHealth = playerMoveC.liveMech;
				SetMechHealth();
				mechWasActive = true;
			}
			else if (!mechInAnim && pastMechHealth != playerMoveC.liveMech)
			{
				StartCoroutine(AnimateMechHealth());
			}
			pastMechHealth = playerMoveC.liveMech;
			return;
		}
		if (Defs.isDaterRegim)
		{
			for (int num14 = 0; num14 < Player_move_c.MaxPlayerGUIHealth; num14++)
			{
				hearts[num14].gameObject.SetActive(false);
			}
		}
		else
		{
			if (playerMoveC.respawnedForGUI || mechWasActive)
			{
				currentMechHealthStep = Mathf.CeilToInt(playerMoveC.liveMech);
				pastHealth = health();
				SetHealth();
			}
			else if (!healthInAnim && pastHealth != health())
			{
				StartCoroutine(AnimateHealth());
			}
			pastHealth = health();
		}
		if (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None)
		{
			if (Defs.isDaterRegim)
			{
				for (int num15 = 0; num15 < Player_move_c.MaxPlayerGUIHealth; num15++)
				{
					armorShields[num15].gameObject.SetActive(false);
				}
				pastArmor = 0f;
			}
			else
			{
				if (playerMoveC.respawnedForGUI || mechWasActive)
				{
					currentMechHealthStep = Mathf.CeilToInt(playerMoveC.liveMech);
					pastArmor = armor();
					SetArmor();
				}
				else if (!armorInAnim && pastArmor != armor())
				{
					StartCoroutine(AnimateArmor());
				}
				pastArmor = armor();
			}
		}
		else
		{
			for (int num16 = 0; num16 < Player_move_c.MaxPlayerGUIHealth; num16++)
			{
				armorShields[num16].gameObject.SetActive(false);
			}
		}
		mechWasActive = false;
		playerMoveC.respawnedForGUI = false;
	}

	private void SetMechHealth()
	{
		currentHealthStep = Mathf.FloorToInt(pastMechHealth);
		for (int i = 0; i < mechShields.Length; i++)
		{
			mechShields[i].SetIndex(Mathf.CeilToInt((pastMechHealth - (float)i) / 18f), HeartEffect.IndicatorType.Mech);
		}
	}

	private void SetHealth()
	{
		currentHealthStep = Mathf.FloorToInt(pastHealth);
		for (int i = 0; i < hearts.Length; i++)
		{
			hearts[i].SetIndex(Mathf.CeilToInt((pastHealth - (float)i) / 9f), HeartEffect.IndicatorType.Hearts);
		}
	}

	private void SetArmor()
	{
		currentArmorStep = Mathf.FloorToInt(pastArmor);
		for (int i = 0; i < armorShields.Length; i++)
		{
			armorShields[i].SetIndex(Mathf.CeilToInt((pastArmor - (float)i) / 9f), HeartEffect.IndicatorType.Armor);
		}
	}

	private IEnumerator AnimateHealth()
	{
		healthInAnim = true;
		currentHealthStep = Mathf.CeilToInt(pastHealth);
		while (currentHealthStep != Mathf.CeilToInt(health()))
		{
			int heartsStart = currentHealthStep - 9 * Mathf.FloorToInt((float)currentHealthStep / 9f);
			int currentHealth = Mathf.CeilToInt(health());
			if (currentHealth < 0)
			{
				currentHealth = 0;
			}
			bool minus = currentHealthStep > currentHealth;
			if (minus)
			{
				heartsStart--;
				if (heartsStart < 0)
				{
					heartsStart = 8;
				}
			}
			currentHealthStep += ((!minus) ? 1 : (-1));
			hearts[heartsStart].Animate((!minus) ? Mathf.CeilToInt((float)currentHealthStep / 9f) : Mathf.FloorToInt((float)currentHealthStep / 9f), HeartEffect.IndicatorType.Hearts);
			yield return new WaitForSeconds(0.05f);
		}
		healthInAnim = false;
	}

	private IEnumerator AnimateArmor()
	{
		armorInAnim = true;
		currentArmorStep = Mathf.CeilToInt(pastArmor);
		while (currentArmorStep != Mathf.CeilToInt(armor()))
		{
			int armorStart = currentArmorStep - 9 * Mathf.FloorToInt((float)currentArmorStep / 9f);
			int currentArmor = Mathf.CeilToInt(armor());
			if (currentArmor < 0)
			{
				currentArmor = 0;
			}
			bool minus = currentArmorStep > currentArmor;
			if (minus)
			{
				armorStart--;
				if (armorStart < 0)
				{
					armorStart = 8;
				}
			}
			currentArmorStep += ((!minus) ? 1 : (-1));
			armorShields[armorStart].Animate((!minus) ? Mathf.CeilToInt((float)currentArmorStep / 9f) : Mathf.FloorToInt((float)currentArmorStep / 9f), HeartEffect.IndicatorType.Armor);
			yield return new WaitForSeconds(0.05f);
		}
		armorInAnim = false;
	}

	private IEnumerator AnimateMechHealth()
	{
		mechInAnim = true;
		currentMechHealthStep = Mathf.CeilToInt(pastMechHealth);
		while (currentMechHealthStep != Mathf.CeilToInt(playerMoveC.liveMech))
		{
			int mechStart = currentMechHealthStep - 18 * Mathf.FloorToInt((float)currentMechHealthStep / 18f);
			int currentMech = Mathf.CeilToInt(playerMoveC.liveMech);
			if (currentMech < 0)
			{
				currentMech = 0;
			}
			bool minus = currentMechHealthStep > currentMech;
			if (minus)
			{
				mechStart--;
				if (mechStart < 0)
				{
					mechStart = 17;
				}
			}
			currentMechHealthStep += ((!minus) ? 1 : (-1));
			mechShields[mechStart].Animate((!minus) ? Mathf.CeilToInt((float)currentMechHealthStep / 18f) : Mathf.FloorToInt((float)currentMechHealthStep / 18f), HeartEffect.IndicatorType.Mech);
			yield return new WaitForSeconds(0.05f);
		}
		mechInAnim = false;
	}

	public void SetScopeForWeapon(string num)
	{
		scopeText.SetActive(true);
		string path = ((!Device.isWeakDevice && BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64) ? ResPath.Combine("Scopes", "Scope_" + num) : ResPath.Combine("Scopes", "Scope_" + num + "_small"));
		scopeText.GetComponent<UITexture>().mainTexture = Resources.Load<Texture>(path);
	}

	public void ResetScope()
	{
		scopeText.GetComponent<UITexture>().mainTexture = null;
		scopeText.SetActive(false);
	}

	public void HandleWeaponEquipped(int catNabor)
	{
		int num = 0;
		foreach (Weapon playerWeapon in WeaponManager.sharedManager.playerWeapons)
		{
			int num2 = playerWeapon.weaponPrefab.GetComponent<WeaponSounds>().categoryNabor - 1;
			num++;
		}
		int childCount = changeWeaponWrap.transform.childCount;
		for (int i = childCount; i < num; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(weaponPreviewPrefab);
			gameObject.name = "WeaponCat_" + i;
			gameObject.transform.parent = changeWeaponWrap.transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.GetComponent<UITexture>().width = Mathf.RoundToInt((float)widthWeaponScrollPreview * 0.8f);
			gameObject.GetComponent<UITexture>().height = Mathf.RoundToInt((float)widthWeaponScrollPreview * 0.8f);
			gameObject.GetComponent<BoxCollider>().size = new Vector3((float)widthWeaponScrollPreview * 1.3f, (float)widthWeaponScrollPreview * 1.3f, 1f);
		}
		changeWeaponWrap.SortAlphabetically();
		changeWeaponWrap.GetComponent<MyCenterOnChild>().enabled = false;
		changeWeaponWrap.GetComponent<MyCenterOnChild>().enabled = true;
		int num3 = 0;
		for (int j = 0; j < 6; j++)
		{
			Texture texture = ShopNGUIController.TextureForCat(j);
			if (!(texture != null))
			{
				continue;
			}
			weaponIcons[j].mainTexture = texture;
			foreach (Transform item in changeWeaponWrap.transform)
			{
				if (item.name.Equals("WeaponCat_" + num3))
				{
					item.GetComponent<UITexture>().mainTexture = texture;
					break;
				}
			}
			num3++;
		}
		for (int k = 0; k < WeaponManager.sharedManager.playerWeapons.Count; k++)
		{
			changeWeaponWrap.transform.GetChild(k).GetComponent<WeaponIconController>().myWeaponSounds = ((Weapon)WeaponManager.sharedManager.playerWeapons[k]).weaponPrefab.GetComponent<WeaponSounds>();
		}
		SelectWeaponFromCategory(catNabor + 1);
	}

	public void BlinkNoAmmo(int count)
	{
		if (count == 0)
		{
			StopPlayingLowResourceBeep();
		}
		timerBlinkNoAmmo = (float)count * periodBlink;
		blinkNoAmmoLabel.color = new Color(blinkNoAmmoLabel.color.r, blinkNoAmmoLabel.color.g, blinkNoAmmoLabel.color.b, 0f);
	}

	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(true);
		foreach (Transform transform in componentsInChildren)
		{
			transform.gameObject.layer = layerNumber;
		}
	}

	private void OnDestroy()
	{
		SetNGUITouchDragThreshold(40f);
		sharedInGameGUI = null;
		WeaponManager.WeaponEquipped -= HandleWeaponEquipped;
		PauseNGUIController.ChatSettUpdated -= HandleChatSettUpdated;
		PauseNGUIController.PlayerHandUpdated -= AdjustToPlayerHands;
		ControlsSettingsBase.ControlsChanged -= AdjustToPlayerHands;
		PauseNGUIController.SwitchingWeaponsUpdated -= SetSwitchingWeaponPanel;
	}

	public void SetInterfaceVisible(bool visible)
	{
		interfacePanel.GetComponent<UIPanel>().gameObject.SetActive(visible);
		joystikPanel.gameObject.SetActive(visible);
		shopPanel.gameObject.SetActive(visible);
		bloodPanel.gameObject.SetActive(visible);
	}

	private void SetNGUITouchDragThreshold(float newValue)
	{
		if (UICamera.mainCamera != null && UICamera.mainCamera.GetComponent<UICamera>() != null)
		{
			UICamera.mainCamera.GetComponent<UICamera>().touchDragThreshold = newValue;
		}
		else
		{
			Debug.LogWarning("UICamera.mainCamera is null");
		}
	}

	public void ShowControlSchemeConfigurator()
	{
	}
}
