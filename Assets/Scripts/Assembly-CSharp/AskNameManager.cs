using System;
using System.Collections;
using Rilisoft;
using UnityEngine;

public class AskNameManager : MonoBehaviour
{
	public const string keyNameAlreadySet = "keyNameAlreadySet";

	public static AskNameManager instance;

	public GameObject objWindow;

	public GameObject objPanelSetName;

	public GameObject objPanelEnterName;

	public UILabel lbPlayerName;

	public UIInput inputPlayerName;

	public UIButton btnSetName;

	public GameObject objLbWarning;

	private int _NameAlreadySet = -1;

	private string curChooseName = string.Empty;

	private bool isAutoName;

	public static bool isComplete;

	public static bool isShow;

	private bool CanShowWindow
	{
		get
		{
			if (!NameAlreadySet && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.ShopCompleted)
			{
				if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
				{
					return true;
				}
				if (MainMenuController.sharedController.SyncFuture.IsCompleted)
				{
					if (Defs.IsDeveloperBuild)
					{
						Debug.Log("- End Wait Synch");
					}
					return true;
				}
			}
			return false;
		}
	}

	private bool NameAlreadySet
	{
		get
		{
			if (_NameAlreadySet == -1)
			{
				_NameAlreadySet = Load.LoadInt("keyNameAlreadySet");
			}
			return _NameAlreadySet == 1;
		}
		set
		{
			_NameAlreadySet = (value ? 1 : 0);
			Save.SaveInt("keyNameAlreadySet", _NameAlreadySet);
		}
	}

	private bool CanSetName
	{
		get
		{
			string value = curChooseName.Trim();
			if (!string.IsNullOrEmpty(value))
			{
				return true;
			}
			return false;
		}
	}

	public static event Action onComplete;

	private void Awake()
	{
		instance = this;
		isComplete = false;
		isShow = false;
		objWindow.SetActive(false);
		objPanelSetName.SetActive(false);
		objPanelEnterName.SetActive(false);
		objLbWarning.SetActive(false);
	}

	private void OnEnable()
	{
		MainMenuController.onLoadMenu += ShowWindow;
	}

	private void OnDisable()
	{
		MainMenuController.onLoadMenu -= ShowWindow;
	}

	private void OnDestory()
	{
		instance = null;
	}

	public void ShowWindow()
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("+ Start wait enter name window");
		}
		StopCoroutine("WaitAndShowWindow");
		StartCoroutine("WaitAndShowWindow");
	}

	private IEnumerator WaitAndShowWindow()
	{
		if (AskIsCompleted())
		{
			yield break;
		}
		while (!CanShowWindow)
		{
			if (AskIsCompleted())
			{
				yield break;
			}
			yield return null;
			yield return null;
		}
		OnShowWindowSetName();
	}

	private bool AskIsCompleted()
	{
		bool flag = NameAlreadySet || TrainingController.TrainingCompleted;
		if (flag)
		{
			isComplete = true;
			if (AskNameManager.onComplete != null)
			{
				AskNameManager.onComplete();
			}
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("- Enter name completed");
			}
		}
		return flag;
	}

	private void OnShowWindowSetName()
	{
		if (Defs.IsDeveloperBuild)
		{
			Debug.Log("+ OnShowWindowSetName");
		}
		isShow = true;
		curChooseName = GetNameForAsk();
		lbPlayerName.text = curChooseName;
		inputPlayerName.value = curChooseName;
		CheckActiveBtnSetName();
		objPanelSetName.SetActive(true);
		objWindow.SetActive(true);
		isAutoName = true;
	}

	public void OnShowWindowEnterName()
	{
		objPanelEnterName.SetActive(true);
		objPanelSetName.SetActive(false);
	}

	private string GetNameForAsk()
	{
		return ProfileController.GetPlayerNameOrDefault();
	}

	private void CheckActiveBtnSetName()
	{
		BoxCollider component = btnSetName.GetComponent<BoxCollider>();
		objLbWarning.SetActive(false);
		if (CanSetName)
		{
			component.enabled = true;
			btnSetName.SetState(UIButtonColor.State.Normal, true);
		}
		else
		{
			objLbWarning.SetActive(true);
			component.enabled = false;
			btnSetName.SetState(UIButtonColor.State.Disabled, true);
		}
	}

	public void OnStartEnterName()
	{
		if (isAutoName)
		{
			curChooseName = string.Empty;
			inputPlayerName.value = curChooseName;
			CheckActiveBtnSetName();
			isAutoName = false;
		}
	}

	public void OnChangeName()
	{
		curChooseName = inputPlayerName.value;
		CheckActiveBtnSetName();
	}

	public void SaveChooseName()
	{
		if (ProfileController.Instance != null)
		{
			ProfileController.Instance.SaveNamePlayer(curChooseName);
		}
		if (MainMenuController.sharedController != null && MainMenuController.sharedController.persNickLabel != null)
		{
			MainMenuController.sharedController.persNickLabel.UpdateNickInLobby();
			MainMenuController.sharedController.persNickLabel.UpdateInfo();
		}
		NameAlreadySet = true;
		OnCloseAllWindow();
	}

	private void OnCloseAllWindow()
	{
		objWindow.SetActive(false);
		isComplete = true;
		if (AskNameManager.onComplete != null)
		{
			AskNameManager.onComplete();
		}
		isShow = false;
	}

	[ContextMenu("Show Window")]
	public void TestShow()
	{
		isComplete = false;
		OnShowWindowSetName();
	}
}
