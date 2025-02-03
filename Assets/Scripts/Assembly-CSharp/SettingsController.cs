using System;
using System.Collections;
using GooglePlayGames;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class SettingsController : MonoBehaviour
{
	public const int SensitivityLowerBound = 6;

	public const int SensitivityUpperBound = 19;

	public MainMenuHeroCamera rotateCamera;

	public UIButton backButton;

	public UIButton controlsButton;

	public UIButton syncButton;

	public GameObject controlsSettings;

	public GameObject tapPanel;

	public GameObject swipePanel;

	public GameObject mainPanel;

	public UISlider sensitivitySlider;

	public UILabel versionLabel;

	public SettingsToggleButtons chatToggleButtons;

	public SettingsToggleButtons musicToggleButtons;

	public SettingsToggleButtons soundToggleButtons;

	public SettingsToggleButtons invertCameraToggleButtons;

	public SettingsToggleButtons recToggleButtons;

	public SettingsToggleButtons pressureToucheToggleButtons;

	public SettingsToggleButtons leftHandedToggleButtons;

	public SettingsToggleButtons switchingWeaponsToggleButtons;

	public Texture googlePlayServicesTexture;

	private IDisposable _backSubscription;

	private bool _backRequested;

	private float _cachedSensitivity;

	public static event Action ControlsClicked;

	public static void SwitchChatSetting(bool on, Action additional = null)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Chat] button clicked: " + on);
		}
		bool isChatOn = Defs.IsChatOn;
		if (isChatOn != on)
		{
			Defs.IsChatOn = on;
			if (additional != null)
			{
				additional();
			}
		}
	}

	public static void ChangeLeftHandedRightHanded(bool isChecked, Action handler = null)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Left Handed] button clicked: " + isChecked);
		}
		if (GlobalGameController.LeftHanded == isChecked)
		{
			return;
		}
		GlobalGameController.LeftHanded = isChecked;
		PlayerPrefs.SetInt(Defs.LeftHandedSN, isChecked ? 1 : 0);
		PlayerPrefs.Save();
		if (handler != null)
		{
			handler();
		}
		if (SettingsController.ControlsClicked != null)
		{
			SettingsController.ControlsClicked();
		}
		if (!isChecked)
		{
			FlurryPluginWrapper.LogEvent("Left-handed Layout Enabled");
			if (Debug.isDebugBuild)
			{
				Debug.Log("Left-handed Layout Enabled");
			}
		}
	}

	public static void ChangeSwitchingWeaponHanded(bool isChecked, Action handler = null)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Switching Weapon button clicked: " + isChecked);
		}
		if (GlobalGameController.switchingWeaponSwipe == isChecked)
		{
			GlobalGameController.switchingWeaponSwipe = !isChecked;
			PlayerPrefs.SetInt(Defs.SwitchingWeaponsSwipeRegimSN, GlobalGameController.switchingWeaponSwipe ? 1 : 0);
			PlayerPrefs.Save();
			if (handler != null)
			{
				handler();
			}
		}
	}

	private void SetSyncLabelText()
	{
		UILabel uILabel = null;
		Transform transform = syncButton.transform.Find("Label");
		if (transform != null)
		{
			uILabel = transform.gameObject.GetComponent<UILabel>();
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (uILabel != null)
			{
				uILabel.text = LocalizationStore.Get("Key_0080");
			}
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && uILabel != null)
		{
			uILabel.text = LocalizationStore.Get("Key_0935");
		}
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(delegate
		{
			HandleBackFromSettings(this, EventArgs.Empty);
		}, "Settings");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void Start()
	{
		LocalizationStore.AddEventCallAfterLocalize(HandleLocalizationChanged);
		string text = typeof(SettingsController).Assembly.GetName().Version.ToString();
		if (versionLabel != null)
		{
			versionLabel.text = text;
		}
		else
		{
			Debug.LogWarning("versionLabel == null");
		}
		if (backButton != null)
		{
			ButtonHandler component = backButton.GetComponent<ButtonHandler>();
			component.Clicked += HandleBackFromSettings;
		}
		if (controlsButton != null)
		{
			ButtonHandler component2 = controlsButton.GetComponent<ButtonHandler>();
			component2.Clicked += HandleControlsClicked;
		}
		if (syncButton != null)
		{
			ButtonHandler component3 = syncButton.GetComponent<ButtonHandler>();
			SetSyncLabelText();
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				syncButton.gameObject.SetActive(true);
				component3.Clicked += HandleRestoreClicked;
			}
			else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				syncButton.gameObject.SetActive(true);
				component3.Clicked += HandleSyncClicked;
			}
			else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				bool active = false;
				syncButton.gameObject.SetActive(active);
				component3.Clicked += HandleSyncClicked;
			}
		}
		if (sensitivitySlider != null)
		{
			float sensitivity = Defs.Sensitivity;
			float num = Mathf.Clamp(sensitivity, 6f, 19f);
			float num2 = num - 6f;
			sensitivitySlider.value = num2 / 13f;
			_cachedSensitivity = num;
		}
		else
		{
			Debug.LogWarning("sensitivitySlider == null");
		}
		musicToggleButtons.IsChecked = Defs.isSoundMusic;
		musicToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Music] button clicked: " + e.IsChecked);
			}
			GameObject gameObject = GameObject.FindGameObjectWithTag("MenuBackgroundMusic");
			MenuBackgroundMusic menuBackgroundMusic = ((!(gameObject != null)) ? null : gameObject.GetComponent<MenuBackgroundMusic>());
			if (Defs.isSoundMusic != e.IsChecked)
			{
				Defs.isSoundMusic = e.IsChecked;
				PlayerPrefsX.SetBool(PlayerPrefsX.SoundMusicSetting, Defs.isSoundMusic);
				PlayerPrefs.Save();
				if (menuBackgroundMusic != null)
				{
					if (e.IsChecked)
					{
						menuBackgroundMusic.Play();
					}
					else
					{
						menuBackgroundMusic.Stop();
					}
				}
				else
				{
					Debug.LogWarning("menuBackgroundMusic == null");
				}
			}
		};
		soundToggleButtons.IsChecked = Defs.isSoundFX;
		soundToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Sound] button clicked: " + e.IsChecked);
			}
			if (Defs.isSoundFX != e.IsChecked)
			{
				Defs.isSoundFX = e.IsChecked;
				PlayerPrefsX.SetBool(PlayerPrefsX.SoundFXSetting, Defs.isSoundFX);
				PlayerPrefs.Save();
			}
		};
		chatToggleButtons.IsChecked = Defs.IsChatOn;
		chatToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			SwitchChatSetting(e.IsChecked);
		};
		invertCameraToggleButtons.IsChecked = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
		invertCameraToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Invert Camera] button clicked: " + e.IsChecked);
			}
			bool flag = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
			if (flag != e.IsChecked)
			{
				PlayerPrefs.SetInt(Defs.InvertCamSN, Convert.ToInt32(e.IsChecked));
				PlayerPrefs.Save();
			}
		};
		if (leftHandedToggleButtons != null)
		{
			leftHandedToggleButtons.IsChecked = GlobalGameController.LeftHanded;
			leftHandedToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				ChangeLeftHandedRightHanded(e.IsChecked);
			};
		}
		if (switchingWeaponsToggleButtons != null)
		{
			switchingWeaponsToggleButtons.IsChecked = !GlobalGameController.switchingWeaponSwipe;
			switchingWeaponsToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				ChangeSwitchingWeaponHanded(e.IsChecked);
			};
		}
		if (Input.touchPressureSupported || Application.isEditor)
		{
			pressureToucheToggleButtons.gameObject.SetActive(true);
			recToggleButtons.gameObject.SetActive(false);
			pressureToucheToggleButtons.IsChecked = Defs.isUse3DTouch;
			pressureToucheToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
			{
				if (Application.isEditor)
				{
					Debug.Log("3D touche button clicked: " + e.IsChecked);
				}
				Defs.isUse3DTouch = e.IsChecked;
			};
			return;
		}
		pressureToucheToggleButtons.gameObject.SetActive(false);
		recToggleButtons.gameObject.SetActive(PauseNGUIController.RecButtonsAvailable());
		recToggleButtons.IsChecked = GlobalGameController.ShowRec;
		recToggleButtons.Clicked += delegate(object sender, ToggleButtonEventArgs e)
		{
			if (Application.isEditor)
			{
				Debug.Log("[Rec. Buttons] button clicked: " + e.IsChecked);
			}
			if (GlobalGameController.ShowRec != e.IsChecked)
			{
				GlobalGameController.ShowRec = e.IsChecked;
				PlayerPrefs.SetInt(Defs.ShowRecSN, e.IsChecked ? 1 : 0);
				PlayerPrefs.Save();
			}
		};
	}

	private void Update()
	{
		if (_backRequested)
		{
			_backRequested = false;
			mainPanel.SetActive(true);
			base.gameObject.SetActive(false);
			rotateCamera.OnMainMenuCloseOptions();
			return;
		}
		float num = sensitivitySlider.value * 13f;
		float num2 = Mathf.Clamp(num + 6f, 6f, 19f);
		if (_cachedSensitivity != num2)
		{
			if (Application.isEditor)
			{
				Debug.Log("New sensitivity: " + num2);
			}
			Defs.Sensitivity = num2;
			_cachedSensitivity = num2;
		}
	}

	private void HandleBackFromSettings(object sender, EventArgs e)
	{
		_backRequested = true;
	}

	private void HandleControlsClicked(object sender, EventArgs e)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Controls] button clicked.");
		}
		controlsSettings.SetActive(true);
		tapPanel.SetActive(!GlobalGameController.switchingWeaponSwipe);
		swipePanel.SetActive(false);
		swipePanel.transform.parent.gameObject.SetActive(!GlobalGameController.switchingWeaponSwipe);
		base.gameObject.SetActive(false);
		if (SettingsController.ControlsClicked != null)
		{
			SettingsController.ControlsClicked();
		}
	}

	private void HandleRestoreClicked(object sender, EventArgs e)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Restore] button clicked.");
		}
		if (ExperienceController.sharedController != null)
		{
			ExperienceController.sharedController.Refresh();
		}
		if (ExpController.Instance != null)
		{
			ExpController.Instance.Refresh();
		}
	}

	private void HandleSyncClicked(object sender, EventArgs e)
	{
		if (Application.isEditor)
		{
			Debug.Log("[Sync] button clicked.");
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
			{
				PurchasesSynchronizer.Instance.SynchronizeAmazonPurchases();
				if (WeaponManager.sharedManager != null)
				{
					WeaponManager.sharedManager.Reset();
				}
				ProgressSynchronizer.Instance.SynchronizeAmazonProgress();
				StarterPackController.Get.RestoreStarterPackForAmazon();
				SetSyncLabelText();
			}
			else
			{
				if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return;
				}
				UIButton syncButton = (sender as MonoBehaviour).Map((MonoBehaviour o) => o.GetComponent<UIButton>());
				if (syncButton != null)
				{
					syncButton.isEnabled = false;
				}
				Action afterAuth = delegate
				{
					Action<bool> callback = delegate(bool succeeded)
					{
						try
						{
							Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.Callback({0}) >: {1:F3}", succeeded, Time.realtimeSinceStartup);
							if (succeeded && WeaponManager.sharedManager != null)
							{
								WeaponManager.sharedManager.Reset();
							}
							StoreKitEventListener.purchaseInProcess = false;
							Debug.LogFormat("[Rilisoft] PurchasesSynchronizer.HasItemsToBeSaved: {0}", PurchasesSynchronizer.Instance.HasItemsToBeSaved);
							if (PurchasesSynchronizer.Instance.HasItemsToBeSaved)
							{
								int num = 0;
								foreach (string item in PurchasesSynchronizer.Instance.ItemsToBeSaved)
								{
									if (item.StartsWith("currentLevel"))
									{
										string[] array = item.Split(new string[1] { "currentLevel" }, StringSplitOptions.RemoveEmptyEntries);
										if (array.Length > 0)
										{
											string value = array[array.Length - 1];
											if (!string.IsNullOrEmpty(value))
											{
												int num2 = Convert.ToInt32(value);
												if (num2 > num)
												{
													num = num2;
												}
											}
										}
									}
								}
								if (Defs.IsDeveloperBuild)
								{
									Debug.LogFormat("[Rilisoft] Incoming level: {0}", num);
								}
								if (num > 0)
								{
									if (ShopNGUIController.GuiActive)
									{
										Debug.LogWarning("Skipping saving to storager while in Shop.");
										return;
									}
									if (!StringComparer.Ordinal.Equals(SceneManager.GetActiveScene().name, Defs.MainMenuScene))
									{
										Debug.LogWarning("Skipping saving to storager while not Main Menu.");
										return;
									}
									Storager.setInt(Defs.TrainingCompleted_4_4_Sett, 1, false);
									if (HintController.instance != null)
									{
										HintController.instance.ShowNext();
									}
									string text = LocalizationStore.Get("Key_1977");
									Debug.LogFormat("[Rilisoft] > StartCoroutine(SaveItemsToStorager): {1} {0:F3}", Time.realtimeSinceStartup, text);
									InfoWindowController.ShowRestorePanel(delegate
									{
										CoroutineRunner.Instance.StartCoroutine(MainMenuController.SaveItemsToStorager(delegate
										{
											Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.InnerCallback >: {0:F3}", Time.realtimeSinceStartup);
											PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
											if (WeaponManager.sharedManager != null)
											{
												StartCoroutine(WeaponManager.sharedManager.ResetCoroutine());
											}
											if (ExperienceController.sharedController != null)
											{
												ExperienceController.sharedController.Refresh();
											}
											if (ExpController.Instance != null)
											{
												ExpController.Instance.Refresh();
											}
											Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.InnerCallback <: {0:F3}", Time.realtimeSinceStartup);
										}));
									});
									Debug.LogFormat("[Rilisoft] < StartCoroutine(SaveItemsToStorager): {1} {0:F3}", Time.realtimeSinceStartup, text);
								}
							}
							PlayerPrefs.DeleteKey("PendingGooglePlayGamesSync");
							Debug.LogFormat("[Rilisoft] SettingsController.PurchasesSynchronizer.Callback({0}) <: {1:F3}", succeeded, Time.realtimeSinceStartup);
						}
						finally
						{
							if (syncButton != null)
							{
								syncButton.isEnabled = true;
							}
						}
					};
					if (Application.isEditor)
					{
						Debug.Log("Simulating sync...");
						IEnumerator routine = PurchasesSynchronizer.Instance.SimulateSynchronization(callback);
						CoroutineRunner.Instance.StartCoroutine(routine);
					}
					else
					{
						if (!PurchasesSynchronizer.Instance.SynchronizeIfAuthenticated(callback))
						{
							syncButton.Do(delegate(UIButton s)
							{
								s.isEnabled = true;
							});
						}
						ProgressSynchronizer.Instance.SynchronizeIfAuthenticated(delegate
						{
						});
						GoogleIAB.queryInventory(StoreKitEventListener.starterPackIds);
					}
					SetSyncLabelText();
				};
				StoreKitEventListener.purchaseInProcess = true;
				StartCoroutine(RestoreProgressIndicator(5f));
				if (PlayGamesPlatform.Instance.IsAuthenticated())
				{
					string message = string.Format("Already authenticated: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
					Debug.Log(message);
					afterAuth();
					return;
				}
				if (!Application.isEditor)
				{
					try
					{
						PlayGamesPlatform.Instance.Authenticate(Social.localUser, delegate(bool succeeded)
						{
							if (succeeded)
							{
								string message2 = string.Format("Authentication succeeded: {0}, {1}, {2}", Social.localUser.id, Social.localUser.userName, Social.localUser.state);
								Debug.Log(message2);
								afterAuth();
							}
							else
							{
								Debug.LogWarning("Authentication failed.");
								StoreKitEventListener.purchaseInProcess = false;
								if (syncButton != null)
								{
									syncButton.isEnabled = true;
								}
							}
						});
						return;
					}
					catch (InvalidOperationException exception)
					{
						Debug.LogWarning("SettingsController: Exception occured while authenticating with Google Play Games. See next exception message for details.");
						Debug.LogException(exception);
						if (syncButton != null)
						{
							syncButton.isEnabled = true;
						}
						return;
					}
				}
				afterAuth();
			}
		}
		else if (BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64)
		{
		}
	}

	private IEnumerator RestoreProgressIndicator(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		StoreKitEventListener.purchaseInProcess = false;
	}

	private void OnDestroy()
	{
		LocalizationStore.DelEventCallAfterLocalize(HandleLocalizationChanged);
	}

	private void HandleLocalizationChanged()
	{
		SetSyncLabelText();
	}
}
