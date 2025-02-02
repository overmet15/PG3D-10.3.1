using System;
using System.Collections;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

public class GiftBannerWindow : BannerWindow
{
	public enum StepAnimation
	{
		none = 0,
		WaitForShowAward = 1,
		ShowAward = 2,
		waitForClose = 3
	}

	public const string keyTrigOpenBanner = "OpenGiftPanel";

	public const string keyTrigTapButton = "OpenGiftBtnRelease";

	public const string keyTrigShowInfoGift = "GiftInfoShow";

	public const string keyTrigCloseInfoGift = "GiftInfoClose";

	public static GiftBannerWindow instance;

	public GameObject butBuy;

	public GameObject butGift;

	public GameObject bannerObj;

	public GiftScroll scrollGift;

	public UILabel lbPriceForBuy;

	public UILabel lbTimer;

	public UISprite sprDarkFon;

	public GameObject objSound;

	public UISprite sprFonScroll;

	public GiftHUDItem panelInfoGift;

	public Animator animatorBanner;

	public GameObject objLabelTapGift;

	public float speedAnimCenter = 2f;

	public static bool blockedButton;

	private SlotInfo awardSlot;

	private float delayBeforeNextStep = 5f;

	private bool canTapOnGift = true;

	private Coroutine crtForShowAward;

	private bool needShowGift;

	[HideInInspector]
	public bool needForceShowAwardGift;

	private IDisposable _backSubscription;

	private bool needPlayStartAnim = true;

	public static bool isForceClose;

	public static bool isActiveBanner;

	public StepAnimation curStateAnimAward;

	public static event Action onClose;

	public static event Action onGetGift;

	public static event Action onHideInfoGift;

	public static event Action onOpenInfoGift;

	private void Awake()
	{
		instance = this;
		if (animatorBanner != null)
		{
			animatorBanner = GetComponent<Animator>();
		}
		BankController.Instance.BackRequested += BackFromBank;
	}

	private void OnDestroy()
	{
		BankController.Instance.BackRequested -= BackFromBank;
		instance = null;
	}

	private void OnEnable()
	{
		if (!bannerObj.activeSelf)
		{
			needPlayStartAnim = true;
		}
		if ((bool)objSound)
		{
			objSound.SetActive(Defs.isSoundFX);
		}
		GiftController.onUpdateTimer += UpdateLabelTimer;
		GiftController.onTimerEnded += OnEndTimer;
		MainMenuHeroCamera.onEndOpenGift += OnOpenBannerWindow;
		if (GiftController.instance != null)
		{
			if (GiftController.instance.ActiveGift)
			{
				CheckNeedButtonShow();
				GiftController.instance.CheckAvaliableSlots();
			}
			else
			{
				SetVisibleBanner(false);
				if (GiftBannerWindow.onClose != null)
				{
					GiftBannerWindow.onClose();
				}
			}
		}
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			if (BannerWindowController.SharedController != null && bannerObj.Map((GameObject b) => b.activeSelf))
			{
				CloseBanner();
			}
		}, "Gift (Gotcha)");
	}

	private void OnDisable()
	{
		GiftController.onUpdateTimer -= UpdateLabelTimer;
		GiftController.onTimerEnded -= OnEndTimer;
		MainMenuHeroCamera.onEndOpenGift -= OnOpenBannerWindow;
		needPlayStartAnim = true;
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void BackFromBank(object sender, EventArgs e)
	{
		if (base.IsShow)
		{
			Invoke("OnCloseBank", 0.2f);
		}
	}

	private void OnCloseBank()
	{
		needPlayStartAnim = true;
		OnOpenBannerWindow();
	}

	public void ShowShop()
	{
		if (!blockedButton)
		{
			SetVisibleBanner(false);
			MainMenuController.sharedController.ShowBankWindow();
		}
	}

	public void GetGift()
	{
		if (!blockedButton)
		{
			GetGiftCore(false);
		}
	}

	private void OnOpenBannerWindow()
	{
		if (!isForceClose && needPlayStartAnim)
		{
			SetVisibleBanner(true);
			needPlayStartAnim = false;
			scrollGift.SetCanDraggable(true);
			HideDarkFon();
			animatorBanner.SetTrigger("OpenGiftPanel");
		}
	}

	private void GetGiftCore(bool isForMoneyGift)
	{
		if ((!isForMoneyGift && !GiftController.instance.CanGetGift) || !GiftController.instance.ActiveGift)
		{
			return;
		}
		ButtonClickSound.Instance.PlayClick();
		BankController.UpdateAllIndicatorsMoney();
		BankController.canShowIndication = false;
		SlotInfo gift = GiftController.instance.GetGift();
		awardSlot = CopySlot(gift);
		if (GiftController.instance.CanGetGift)
		{
			GiftController.instance._canGetGift = false;
			GiftController.instance._localTimer = -1f;
			GiftController.instance.ReSaveLastTimeSever();
		}
		if (awardSlot != null)
		{
			if (awardSlot.gift != null)
			{
				FlurryEvents.LogDailyGift(awardSlot.gift.IdGift, awardSlot.CountGift, isForMoneyGift);
			}
			else
			{
				Debug.LogError("GetGiftCore: awardSlot.gift = null");
			}
		}
		else
		{
			Debug.LogError("GetGiftCore: awardSlot = null");
		}
		blockedButton = true;
		scrollGift.SetCanDraggable(false);
		scrollGift.AnimScrollGift(awardSlot.numInScroll);
		animatorBanner.SetTrigger("OpenGiftBtnRelease");
		GiftScroll.canReCreateSlots = false;
		GiftController.instance.ReCreateSlots();
		CheckNeedButtonShow();
		ShowDarkFon();
		StartShowAwardGift();
		if (GiftBannerWindow.onGetGift != null)
		{
			GiftBannerWindow.onGetGift();
		}
	}

	private SlotInfo CopySlot(SlotInfo curSlot)
	{
		SlotInfo slotInfo = new SlotInfo();
		slotInfo.gift = new GiftInfo();
		slotInfo.gift.IdGift = curSlot.gift.IdGift;
		slotInfo.gift.count.Value = curSlot.gift.count.Value;
		slotInfo.gift.keyTranslateInfo = curSlot.gift.keyTranslateInfo;
		slotInfo.gift.typeShopCat = curSlot.gift.typeShopCat;
		slotInfo.CountGift = curSlot.CountGift;
		slotInfo.numInScroll = curSlot.numInScroll;
		slotInfo.category = curSlot.category;
		slotInfo.isActiveEvent = curSlot.isActiveEvent;
		return slotInfo;
	}

	public void BuyCanGetGift()
	{
		if (!blockedButton)
		{
			bool buySuccess = false;
			ItemPrice price = new ItemPrice(GiftController.instance.costBuyCanGetGift.Value, "GemsCurrency");
			ShopNGUIController.TryToBuy(base.transform.root.gameObject, price, delegate
			{
				buySuccess = true;
				GetGiftCore(true);
			});
			if (!buySuccess)
			{
				SetVisibleBanner(false);
			}
			CheckNeedButtonShow();
		}
	}

	public void StartShowAwardGift()
	{
		if (awardSlot != null)
		{
			canTapOnGift = false;
			curStateAnimAward = StepAnimation.WaitForShowAward;
			AnimationGift.instance.StartAnimForGetGift();
			CancelInvoke("StartNextStep");
			Invoke("StartNextStep", delayBeforeNextStep);
		}
		else
		{
			CloseInfoGift();
		}
	}

	public void OnClickGift()
	{
		if (canTapOnGift)
		{
			StartNextStep();
		}
	}

	private void StartNextStep()
	{
		switch (curStateAnimAward)
		{
		case StepAnimation.WaitForShowAward:
			CancelInvoke("StartNextStep");
			curStateAnimAward = StepAnimation.ShowAward;
			StartNextStep();
			break;
		case StepAnimation.ShowAward:
			crtForShowAward = StartCoroutine(OnAnimOpenGift());
			break;
		case StepAnimation.waitForClose:
			CloseInfoGift();
			break;
		}
	}

	private IEnumerator OnAnimOpenGift()
	{
		CancelInvoke("StartNextStep");
		HideDarkFon();
		AnimationGift.instance.StopAnimForGetGift();
		if (GiftBannerWindow.onOpenInfoGift != null)
		{
			GiftBannerWindow.onOpenInfoGift();
		}
		panelInfoGift.SetInfoButton(awardSlot);
		awardSlot = null;
		yield return new WaitForSeconds(1f);
		BankController.canShowIndication = true;
		animatorBanner.SetTrigger("GiftInfoShow");
		yield return new WaitForSeconds(1.5f);
		curStateAnimAward = StepAnimation.waitForClose;
		canTapOnGift = true;
		Invoke("StartNextStep", delayBeforeNextStep);
	}

	public void CloseInfoGift(bool isForce = false)
	{
		canTapOnGift = true;
		CancelInvoke("StartNextStep");
		SpringPanel component = scrollGift.GetComponent<SpringPanel>();
		if ((bool)component)
		{
			UnityEngine.Object.Destroy(component);
		}
		if (crtForShowAward != null)
		{
			StopCoroutine(crtForShowAward);
		}
		animatorBanner.SetTrigger("GiftInfoClose");
		crtForShowAward = null;
		curStateAnimAward = StepAnimation.none;
		GiftScroll.canReCreateSlots = true;
		scrollGift.SetCanDraggable(true);
		HideDarkFon();
		Invoke("UnlockedBut", 1.5f);
		if (scrollGift != null)
		{
			scrollGift.UpdateListButton();
		}
		if (GiftBannerWindow.onHideInfoGift != null)
		{
			GiftBannerWindow.onHideInfoGift();
		}
		StartCoroutine(WaitAndSort());
		if (!isForce && FriendsController.ServerTime < 0)
		{
			AnimationGift.instance.CheckVisibleGift();
			ForceCloseAll();
		}
	}

	private IEnumerator WaitAndSort()
	{
		yield return null;
		scrollGift.transform.parent.localScale = Vector3.one;
		scrollGift.transform.localScale = Vector3.one;
		scrollGift.Sort();
		yield return null;
		while (scrollGift.transform.parent.localScale.Equals(Vector3.one))
		{
			yield return null;
		}
		scrollGift.Sort();
	}

	private void UnlockedBut()
	{
		blockedButton = false;
	}

	public void CloseBanner()
	{
		if (!blockedButton && (bool)BannerWindowController.SharedController && bannerObj != null && bannerObj.activeSelf)
		{
			ButtonClickSound.Instance.PlayClick();
			SetVisibleBanner(false);
			if (GiftBannerWindow.onClose != null)
			{
				GiftBannerWindow.onClose();
			}
			isActiveBanner = false;
		}
	}

	public void CloseBannerEndAnimtion()
	{
		BannerWindowController.SharedController.HideBannerWindow();
		SetVisibleBanner(true);
		if (MainMenuController.sharedController != null)
		{
			MainMenuController.sharedController.ShowSavePanel();
		}
	}

	public void SetVisibleBanner(bool val)
	{
		if (bannerObj != null)
		{
			bannerObj.SetActive(val);
		}
	}

	public string GetNameSpriteForSlot(SlotInfo curSlot)
	{
		TypeGiftCategory typeCat = curSlot.category.typeCat;
		if (typeCat == TypeGiftCategory.Armor)
		{
			return "shop_icons_armor";
		}
		return string.Empty;
	}

	public Texture GetTextureForSlot(SlotInfo curSlot)
	{
		switch (curSlot.category.typeCat)
		{
		case TypeGiftCategory.Coins:
			return Resources.Load<Texture>("OfferIcons/Marathon/bonus_coins");
		case TypeGiftCategory.Gems:
			return Resources.Load<Texture>("OfferIcons/Marathon/bonus_gems");
		case TypeGiftCategory.Skins:
			return null;
		case TypeGiftCategory.Gear:
		{
			string text3 = string.Empty;
			if (curSlot.gift.IdGift.Equals("MusicBox"))
			{
				text3 = "Dater_bonus_turret";
			}
			if (curSlot.gift.IdGift.Equals("Wings"))
			{
				text3 = "Dater_bonus_jetpack";
			}
			if (curSlot.gift.IdGift.Equals("Bear"))
			{
				text3 = "Dater_bonus_mech";
			}
			if (curSlot.gift.IdGift.Equals("BigHeadPotion"))
			{
				text3 = "Dater_bonus_potion";
			}
			return Resources.Load<Texture>("OfferIcons/Marathon/" + text3);
		}
		case TypeGiftCategory.Grenades:
		{
			string text2 = string.Empty;
			switch (curSlot.gift.IdGift)
			{
			case "GrenadeID":
				text2 = "Marathon/bonus_grenade";
				break;
			case "LikeID":
				text2 = "LikeID";
				break;
			}
			return Resources.Load<Texture>("OfferIcons/" + text2);
		}
		case TypeGiftCategory.Guns:
		case TypeGiftCategory.Armor:
		case TypeGiftCategory.Wear:
		{
			curSlot.gift.UpdateType();
			string text = PromoActionsGUIController.IconNameForKey(curSlot.gift.IdGift, (int)curSlot.gift.typeShopCat);
			return Resources.Load<Texture>("OfferIcons/" + text);
		}
		default:
			return null;
		}
	}

	private void CheckNeedButtonShow()
	{
		if ((bool)butBuy)
		{
			butBuy.SetActive(false);
		}
		if ((bool)butGift)
		{
			butGift.SetActive(false);
		}
		if ((bool)lbTimer)
		{
			lbTimer.gameObject.SetActive(false);
		}
		if ((bool)objLabelTapGift)
		{
			objLabelTapGift.SetActive(false);
		}
		if (GiftController.instance.CanGetGift)
		{
			if ((bool)objLabelTapGift)
			{
				objLabelTapGift.SetActive(true);
			}
			if ((bool)butGift)
			{
				butGift.SetActive(true);
			}
			return;
		}
		if ((bool)butBuy)
		{
			butBuy.SetActive(true);
		}
		if ((bool)lbTimer)
		{
			lbTimer.gameObject.SetActive(true);
		}
		if (lbPriceForBuy != null)
		{
			lbPriceForBuy.text = GiftController.instance.costBuyCanGetGift.Value.ToString();
		}
	}

	public void ShowDarkFon()
	{
	}

	public void HideDarkFon()
	{
	}

	public void AnimFonShow(bool val)
	{
		if (sprDarkFon != null)
		{
			if (val)
			{
				TweenAlpha.Begin(sprDarkFon.gameObject, 1f, 0.4f);
			}
			else
			{
				TweenAlpha.Begin(sprDarkFon.gameObject, 0.1f, 0f);
			}
		}
	}

	private void UpdateLabelTimer(string curTime)
	{
		if (lbTimer != null)
		{
			lbTimer.text = curTime;
		}
	}

	private void OnEndTimer()
	{
		CheckNeedButtonShow();
	}

	private void OnApplicationPause(bool pausing)
	{
		if (!pausing)
		{
			ForceCloseAll();
		}
	}

	public void ForceCloseAll()
	{
		if (!(instance == null) && curStateAnimAward == StepAnimation.none && isActiveBanner)
		{
			isActiveBanner = false;
			isForceClose = true;
			needPlayStartAnim = true;
			blockedButton = false;
			BankController.canShowIndication = true;
			canTapOnGift = true;
			CloseInfoGift(true);
			HideDarkFon();
			SetVisibleBanner(false);
			MainMenuController.canRotationLobbyPlayer = true;
			if (GiftBannerWindow.onClose != null)
			{
				GiftBannerWindow.onClose();
			}
			if (AnimationGift.instance != null)
			{
				AnimationGift.instance.ResetAnimation();
			}
			curStateAnimAward = StepAnimation.none;
		}
	}

	public void UpdateSizeScroll()
	{
		int num = scrollGift.listButton.Count;
		if (num > 8)
		{
			num = 8;
		}
		num--;
		int num2 = num * scrollGift.wrapScript.itemSize;
		UIPanel panel = scrollGift.scView.panel;
		sprFonScroll.SetDimensions(num2 + 30, (int)sprFonScroll.localSize.y);
		panel.baseClipRegion = new Vector4(panel.baseClipRegion.x, panel.baseClipRegion.y, num2, panel.baseClipRegion.w);
	}

	[ContextMenu("simulate pause")]
	private void SimPause()
	{
		ForceCloseAll();
	}
}
