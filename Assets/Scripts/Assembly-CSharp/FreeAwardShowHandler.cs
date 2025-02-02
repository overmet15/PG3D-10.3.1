using System;
using System.Collections;
using System.Collections.Generic;
using Rilisoft.NullExtensions;
using UnityEngine;

public class FreeAwardShowHandler : MonoBehaviour
{
	private enum SkipReason
	{
		None = 0,
		CameraTouchOverGui = 1,
		FriendsInterfaceEnabled = 2,
		BankInterfaceEnabled = 3,
		ShopInterfaceEnabled = 4,
		RewardedVideoInterfaceEnabled = 5,
		BannerEnabled = 6,
		MainMenuComponentEnabled = 7,
		LeaderboardEnabled = 8,
		ProfileEnabled = 9,
		NewsEnabled = 10,
		LevelUpShown = 11,
		AskNameWindow = 12
	}

	public GameObject chestModel;

	public GameObject freeAwardGuiPrefab;

	public LeaderboardsView leaderboardsView;

	public static FreeAwardShowHandler Instance;

	private Animation _animationSystem;

	private NickLabelController nickController;

	private bool clicked;

	private bool inside;

	public bool IsInteractable
	{
		get
		{
			return base.gameObject.GetComponent<Collider>() != null && base.gameObject.GetComponent<Collider>().enabled;
		}
		set
		{
			if (base.gameObject.GetComponent<Collider>() != null)
			{
				base.gameObject.GetComponent<Collider>().enabled = value;
			}
		}
	}

	private void Awake()
	{
		_animationSystem = GetComponent<Animation>();
		Instance = this;
		if (FreeAwardController.Instance == null && freeAwardGuiPrefab != null)
		{
			UnityEngine.Object @object = UnityEngine.Object.Instantiate(freeAwardGuiPrefab, Vector3.zero, Quaternion.identity);
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private NickLabelController GetNickController()
	{
		return (!(MainMenuController.sharedController != null)) ? null : MainMenuController.sharedController.persNickLabel;
	}

	private bool NeedToSkip()
	{
		SkipReason skipReason = NeedToSkipCore();
		if (Defs.IsDeveloperBuild && skipReason != 0)
		{
			Debug.Log("Skipping free award chest: " + skipReason);
		}
		return skipReason != SkipReason.None;
	}

	private SkipReason NeedToSkipCore()
	{
		if (UICamera.currentTouch.Map((UICamera.MouseOrTouch t) => t.isOverUI))
		{
			return SkipReason.CameraTouchOverGui;
		}
		if (FriendsWindowGUI.Instance.InterfaceEnabled)
		{
			return SkipReason.FriendsInterfaceEnabled;
		}
		if (BankController.Instance != null && BankController.Instance.InterfaceEnabled)
		{
			return SkipReason.BankInterfaceEnabled;
		}
		if (ShopNGUIController.sharedShop != null && ShopNGUIController.GuiActive)
		{
			return SkipReason.ShopInterfaceEnabled;
		}
		if (FreeAwardController.Instance != null && !FreeAwardController.Instance.IsInState<FreeAwardController.IdleState>())
		{
			return SkipReason.RewardedVideoInterfaceEnabled;
		}
		if (BannerWindowController.SharedController != null && BannerWindowController.SharedController.IsAnyBannerShown)
		{
			return SkipReason.BannerEnabled;
		}
		if (AskNameManager.instance != null && !AskNameManager.isComplete)
		{
			return SkipReason.AskNameWindow;
		}
		MainMenuController sharedController = MainMenuController.sharedController;
		if (sharedController != null && (sharedController.RentExpiredPoint.Map((Transform r) => r.childCount > 0) || sharedController.SettingsJoysticksPanel.activeSelf || sharedController.supportPanel.activeSelf || sharedController.settingsPanel.activeSelf || sharedController.freePanel.activeSelf || sharedController.singleModePanel.activeSelf))
		{
			return SkipReason.MainMenuComponentEnabled;
		}
		if (leaderboardsView.Map((LeaderboardsView l) => l.isActiveAndEnabled))
		{
			return SkipReason.LeaderboardEnabled;
		}
		if (FriendsController.sharedController.Map((FriendsController c) => c.ProfileInterfaceActive))
		{
			return SkipReason.ProfileEnabled;
		}
		if (NewsLobbyController.sharedController != null && NewsLobbyController.sharedController.gameObject.activeSelf)
		{
			return SkipReason.NewsEnabled;
		}
		if (ExpController.Instance != null && ExpController.Instance.IsLevelUpShown)
		{
			return SkipReason.LevelUpShown;
		}
		return SkipReason.None;
	}

	private void OnMouseDown()
	{
		clicked = true;
		inside = true;
	}

	private void OnMouseExit()
	{
		inside = false;
	}

	private void OnMouseEnter()
	{
		if (clicked)
		{
			inside = true;
		}
	}

	private void OnMouseUp()
	{
		clicked = false;
		if (!inside || NeedToSkip())
		{
			return;
		}
		inside = false;
		if (!FreeAwardController.Instance.AdvertCountLessThanLimit())
		{
			return;
		}
		List<double> list = ((!MobileAdManager.IsPayingUser()) ? PromoActionsManager.MobileAdvert.RewardedVideoDelayMinutesNonpaying : PromoActionsManager.MobileAdvert.RewardedVideoDelayMinutesPaying);
		if (list.Count == 0)
		{
			return;
		}
		DateTime date = DateTime.UtcNow.Date;
		KeyValuePair<int, DateTime> keyValuePair = FreeAwardController.Instance.LastAdvertShow(date);
		int num = Math.Max(0, keyValuePair.Key + 1);
		if (num <= list.Count)
		{
			DateTime dateTime = ((!(keyValuePair.Value < date)) ? keyValuePair.Value : date);
			TimeSpan timeSpan = TimeSpan.FromMinutes(list[num]);
			DateTime watchState = dateTime + timeSpan;
			FreeAwardController.Instance.SetWatchState(watchState);
			if (ButtonClickSound.Instance != null)
			{
				ButtonClickSound.Instance.PlayClick();
			}
		}
	}

	private void OnEnable()
	{
		StartCoroutine(ShowChestCoroutine());
	}

	private void PlayeAnimationTitle(bool isReverse, GameObject titleLabel)
	{
		UIPlayTween component = titleLabel.GetComponent<UIPlayTween>();
		component.resetOnPlay = true;
		component.tweenGroup = (isReverse ? 1 : 0);
		component.Play(true);
	}

	private void CheckShowFreeAwardTitle(bool isEnable, bool needExitAnim = false)
	{
		if (nickController == null)
		{
			nickController = GetNickController();
		}
		if (isEnable && nickController != null)
		{
			nickController.freeAwardTitle.gameObject.SetActive(true);
			PlayeAnimationTitle(false, nickController.freeAwardTitle);
		}
		else if (needExitAnim)
		{
			PlayeAnimationTitle(true, nickController.freeAwardTitle);
		}
	}

	private IEnumerator ShowChestCoroutine()
	{
		yield return null;
		PlayAnimationShowChest(false);
		chestModel.SetActive(true);
		CheckShowFreeAwardTitle(true);
	}

	private IEnumerator HideChestCoroutine()
	{
		PlayAnimationShowChest(true);
		CheckShowFreeAwardTitle(false, true);
		yield return new WaitForSeconds(_animationSystem["Animate"].length);
		base.gameObject.SetActive(false);
	}

	private void PlayAnimationShowChest(bool isReserse)
	{
		if (isReserse)
		{
			_animationSystem["Animate"].speed = -1f;
			_animationSystem["Animate"].time = _animationSystem["Animate"].length;
		}
		else
		{
			_animationSystem["Animate"].speed = 1f;
			_animationSystem["Animate"].time = 0f;
		}
		_animationSystem.Play();
	}

	public void HideChestWithAnimation()
	{
		StartCoroutine(HideChestCoroutine());
	}

	public void HideChestTitle()
	{
		if (!(nickController == null))
		{
			nickController.freeAwardTitle.gameObject.SetActive(false);
		}
	}

	public static void CheckShowChest(bool interfaceEnabled)
	{
		if (!(Instance == null) && interfaceEnabled && Instance.gameObject.activeSelf)
		{
			Instance.HideChestTitle();
			Instance.chestModel.transform.localScale = Vector3.zero;
			Instance.gameObject.SetActive(false);
		}
	}
}
