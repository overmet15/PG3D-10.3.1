using System.Collections;
using Rilisoft;
using UnityEngine;

public class ReviewHUDWindow : MonoBehaviour
{
	private static ReviewHUDWindow _instance;

	[Header("Добавить все звезды в список в их порядке активации при нажатии")]
	public StarReview[] arrStarByOrder;

	[Header("Окна")]
	public GameObject objWindowRating;

	public GameObject objWindowGoToStore;

	public GameObject objWindowEnterMsg;

	public GameObject objThanks;

	[Header("Другое")]
	public UIInput inputMsg;

	public UIButton btnSendMsg;

	public UILabel lbTitle5Stars;

	public static bool isShow;

	private bool _NeedShowThanks;

	public int countStarForReview;

	private bool isInputMsgForReview;

	public static ReviewHUDWindow Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = InfoWindowController.Instance.gameObject;
				_instance = gameObject.GetComponentInChildren<ReviewHUDWindow>();
				return _instance;
			}
			return _instance;
		}
	}

	public string TitleTextTranslate
	{
		get
		{
			return string.Empty;
		}
	}

	public bool NeedShowThanks
	{
		get
		{
			return _NeedShowThanks;
		}
		set
		{
			_NeedShowThanks = value;
		}
	}

	private void Awake()
	{
		_instance = this;
		if (arrStarByOrder != null)
		{
			for (int i = 0; i < arrStarByOrder.Length; i++)
			{
				arrStarByOrder[i].numOrderStar = i;
				arrStarByOrder[i].lbNumStar.text = (i + 1).ToString();
			}
		}
	}

	private void OnEnable()
	{
		OnShowThanks();
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	public void ShowWindowRating()
	{
		ReviewController.CheckActiveReview();
		if (ReviewController.IsNeedActive)
		{
			OnShowWidowRating();
		}
	}

	public void SelectStar(StarReview curStar)
	{
		if (curStar != null)
		{
			countStarForReview = curStar.numOrderStar + 1;
		}
		if (arrStarByOrder == null)
		{
			return;
		}
		for (int i = 0; i < arrStarByOrder.Length; i++)
		{
			if (curStar != null && i <= curStar.numOrderStar)
			{
				arrStarByOrder[i].SetActiveStar(true);
			}
			else
			{
				arrStarByOrder[i].SetActiveStar(false);
			}
		}
	}

	public void OnChangeMsgReview()
	{
		UpdateStateBtnSendMsg(true);
	}

	public void OnClickStarRating()
	{
		if (countStarForReview > 0 && countStarForReview <= 4)
		{
			OnShowWindowEnterMessage();
		}
		else
		{
			SendMsgReview();
		}
	}

	public void OnSendMsgWithRating()
	{
		SendMsgReview();
	}

	private void SendMsgReview(bool isClickSend = true)
	{
		OnCloseAllWindow();
		if (countStarForReview > 0)
		{
			string msgReview = string.Empty;
			if (isInputMsgForReview)
			{
				msgReview = inputMsg.value;
			}
			if (countStarForReview == 5)
			{
				FlurryEvents.LogRateUsFake(true, 5);
			}
			else
			{
				FlurryEvents.LogRateUsFake(true, countStarForReview, isInputMsgForReview);
			}
			ReviewController.SendReview(countStarForReview, msgReview);
			if (isClickSend)
			{
				NeedShowThanks = true;
				OnShowThanks();
			}
		}
	}

	public void OnClickClose()
	{
		if (countStarForReview == 0)
		{
			FlurryEvents.LogRateUsFake(false);
		}
		else
		{
			FlurryEvents.LogRateUsFake(true, countStarForReview);
		}
		SendMsgReview(false);
		OnCloseAllWindow();
	}

	private void OnCloseAllWindow()
	{
		if ((bool)objWindowRating)
		{
			objWindowRating.SetActive(false);
		}
		if ((bool)objWindowEnterMsg)
		{
			objWindowEnterMsg.SetActive(false);
		}
		if ((bool)objWindowGoToStore)
		{
			objWindowGoToStore.SetActive(false);
		}
		if ((bool)objThanks)
		{
			objThanks.SetActive(false);
		}
	}

	private void OnShowWindowEnterMessage()
	{
		UpdateStateBtnSendMsg(false);
		if ((bool)objWindowRating)
		{
			objWindowRating.SetActive(false);
		}
		if ((bool)objWindowEnterMsg)
		{
			objWindowEnterMsg.SetActive(true);
		}
	}

	private void OnShowWidowRating()
	{
		isShow = true;
		countStarForReview = 0;
		SelectStar(null);
		ReviewController.IsSendReview = true;
		ReviewController.IsNeedActive = false;
		if ((bool)lbTitle5Stars)
		{
			lbTitle5Stars.text = TitleTextTranslate;
		}
		if ((bool)objWindowRating)
		{
			objWindowRating.SetActive(true);
		}
		if ((bool)objWindowEnterMsg)
		{
			objWindowEnterMsg.SetActive(false);
		}
		if ((bool)objWindowGoToStore)
		{
			objWindowGoToStore.SetActive(false);
		}
	}

	private void OnShowWindowGoToStore()
	{
		if ((bool)objWindowRating)
		{
			objWindowRating.SetActive(false);
		}
		if ((bool)objWindowEnterMsg)
		{
			objWindowGoToStore.SetActive(true);
		}
	}

	private void OnShowThanks()
	{
		if (NeedShowThanks)
		{
			StartCoroutine(Crt_OnShowThanks());
		}
	}

	private IEnumerator Crt_OnShowThanks()
	{
		yield return new WaitForEndOfFrame();
		if (NeedShowThanks)
		{
			NeedShowThanks = false;
			if (objThanks != null)
			{
				objThanks.SetActive(true);
			}
			yield return new WaitForSeconds(3f);
			OnCloseThanks();
		}
	}

	public void OnCloseThanks()
	{
		if (objThanks != null)
		{
			objThanks.SetActive(false);
		}
		BannerWindowController.firstScreen = false;
		isShow = false;
	}

	private void UpdateStateBtnSendMsg(bool val)
	{
		isInputMsgForReview = val;
		if (isInputMsgForReview)
		{
			btnSendMsg.enabled = true;
			btnSendMsg.state = UIButtonColor.State.Normal;
		}
		else
		{
			btnSendMsg.enabled = false;
			btnSendMsg.state = UIButtonColor.State.Disabled;
		}
	}

	[ContextMenu("Find all stars")]
	private void FindStars()
	{
		arrStarByOrder = GetComponentsInChildren<StarReview>(true);
	}

	[ContextMenu("Show window")]
	public void TestShow()
	{
		ReviewController.IsNeedActive = true;
		ShowWindowRating();
	}
}
