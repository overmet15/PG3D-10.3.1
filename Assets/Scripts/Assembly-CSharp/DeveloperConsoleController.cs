using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

internal sealed class DeveloperConsoleController : MonoBehaviour
{
	public static DeveloperConsoleController instance;

	public DeveloperConsoleView view;

	public static bool isDebugGuiVisible;

	private IDisposable _escapeSubscription;

	private bool? _enemiesInCampaignDirty;

	private bool _backRequested;

	private bool _initialized;

	private bool _needsRestart;

	public void HandleInvalidateQuestConfig(UILabel label)
	{
	}

	public void HandleFacebookLoginReward(UIToggle toggle)
	{
	}

	public void HandleBackButton()
	{
		_backRequested = true;
	}

	public void HandleClearKeychainAndPlayerPrefs()
	{
	}

	public void HandleLevelMinusButton()
	{
	}

	public void HandleTipsShownButton()
	{
	}

	public void HandleLevelPlusButton()
	{
	}

	public void HandleCoinsInputSubmit(UIInput input)
	{
		if (input.isActiveAndEnabled)
		{
		}
	}

	public void HandleEnemyCountInSurvivalWaveInput(UIInput input)
	{
	}

	public void HandleEnemiesInCampaignChange()
	{
	}

	public void HandleEnemiesInCampaignInput(UIInput input)
	{
	}

	public void HandleTrainingCompleteChanged(UIToggle toggle)
	{
	}

	public void HandleStrongDeviceChanged(UIToggle toggle)
	{
	}

	public void HandleSet60FpsChanged(UIToggle toggle)
	{
	}

	public void HandleMouseControlChanged(UIToggle toggle)
	{
	}

	public void HandleSpectatorMode(UIToggle toggle)
	{
	}

	public void HandleTempGunChanged(UIToggle toggle)
	{
	}

	public void HandleIpadMiniRetinaChanged(UIToggle toggle)
	{
	}

	public void HandleIsPayingChanged(UIToggle toggle)
	{
	}

	public void HandleIsDebugGuiVisibleChanged(UIToggle toggle)
	{
	}

	public void HandleIsPixelGunLowChanged(UIToggle toggle)
	{
	}

	public void HandleForcedEventX3Changed(UIToggle toggle)
	{
	}

	public void HandleAdIdCanged(UIToggle toggle)
	{
	}

	private static void SetItemsBought(bool bought, bool onlyGuns = true)
	{
	}

	public void HandleFillGunsButton()
	{
	}

	public void HandleClearPurchasesButton()
	{
	}

	public void HandleClearProgressButton()
	{
	}

	public void HandleFillProgressButton()
	{
	}

	public void HandleClearCloud()
	{
	}

	public void HandleUnbanUs(UIButton butt)
	{
	}

	public void HandleClearX3()
	{
	}

	private void RefreshExperience()
	{
	}

	public void HandleExperienceSliderChanged()
	{
	}

	public void HandleSignInOuButton(UILabel socialUsernameLabel)
	{
	}

	public void SetMarathonTestMode(UIToggle toggle)
	{
	}

	public void SetMarathonCurrentDay(UIInput input)
	{
	}

	public void SetOffGameGUIMode(UIToggle toggle)
	{
	}

	public void ClearStarterPackData()
	{
	}

	private void Refresh()
	{
		view.isPixelGunLowCheckbox.value = Device.isPixelGunLow;
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private IEnumerator Start()
	{
		yield break;
	}

	public void ChangePremiumAccountLiveTime(UIInput input)
	{
	}

	public void ClearAllPremiumAccounts()
	{
	}

	public void ClearCurrentPremiumAccont()
	{
	}

	private void HandleGemsInputSubmit(UIInput input)
	{
		if (input.isActiveAndEnabled)
		{
		}
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		if (_escapeSubscription != null)
		{
			_escapeSubscription.Dispose();
		}
		_escapeSubscription = BackSystem.Instance.Register(HandleEscape, "DevConsole");
	}

	private void OnDisable()
	{
		if (_escapeSubscription != null)
		{
			_escapeSubscription.Dispose();
			_escapeSubscription = null;
		}
	}

	private void HandleEscape()
	{
		_backRequested = true;
	}

	public void OnChangeStarterPackLive(UIInput inputField)
	{
	}

	public void OnChangeStarterPackCooldown(UIInput inputField)
	{
	}

	public void UpdateStateActiveMemoryInfo()
	{
	}

	public void OnChangeStateMemoryInfo()
	{
	}

	public void OnChangeReviewActive()
	{
	}

	public void FillAll()
	{
	}
}
