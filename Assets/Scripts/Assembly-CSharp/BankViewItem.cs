using System;
using UnityEngine;

public class BankViewItem : MonoBehaviour, IComparable<BankViewItem>
{
	public UILabel countLabel;

	public UILabel countX3Label;

	public UITexture icon;

	public UILabel priceLabel;

	public UISprite discountSprite;

	public UILabel discountPercentsLabel;

	public UIButton btnBuy;

	[NonSerialized]
	public PurchaseEventArgs purchaseInfo;

	public UISprite bestBuy;

	public ChestBonusButtonView bonusButtonView;

	private Animator _bestBuyAnimator;

	private Animator _discountAnimator;

	private static bool PaymentOccursInLastTwoWeeks()
	{
		string @string = PlayerPrefs.GetString("Last Payment Time", string.Empty);
		DateTime result;
		if (!string.IsNullOrEmpty(@string) && DateTime.TryParse(@string, out result))
		{
			TimeSpan timeSpan = DateTime.UtcNow - result;
			return timeSpan <= TimeSpan.FromDays(14.0);
		}
		return false;
	}

	public int CompareTo(BankViewItem other)
	{
		int value = ((other != null) ? other.purchaseInfo.Count : 0);
		return (!PaymentOccursInLastTwoWeeks()) ? purchaseInfo.Count.CompareTo(value) : value.CompareTo(purchaseInfo.Count);
	}

	private void Awake()
	{
		_bestBuyAnimator = ((!(bestBuy == null)) ? bestBuy.GetComponent<Animator>() : null);
		_discountAnimator = ((!(discountSprite == null)) ? discountSprite.GetComponent<Animator>() : null);
		bonusButtonView.Initialize();
		bonusButtonView.UpdateState(purchaseInfo);
		PromoActionsManager.BestBuyStateUpdate += UpdateViewBestBuy;
	}

	private void UpdateAnimationEventSprite(bool isEventActive)
	{
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		if (sharedManager != null && sharedManager.IsEventX3Active)
		{
			return;
		}
		bool flag = discountSprite != null && discountSprite.gameObject.activeSelf;
		if (flag && _discountAnimator != null)
		{
			if (isEventActive)
			{
				_discountAnimator.Play("DiscountAnimation");
			}
			else
			{
				_discountAnimator.Play("Idle");
			}
		}
		if (isEventActive && _bestBuyAnimator != null)
		{
			if (flag)
			{
				_bestBuyAnimator.Play("BestBuyAnimation");
			}
			else
			{
				_bestBuyAnimator.Play("Idle");
			}
		}
	}

	public void UpdateViewBestBuy()
	{
		PromoActionsManager sharedManager = PromoActionsManager.sharedManager;
		bool flag = !(sharedManager == null) && sharedManager.IsBankItemBestBuy(purchaseInfo);
		bestBuy.gameObject.SetActive(flag);
		UpdateAnimationEventSprite(flag);
	}

	private void OnEnable()
	{
		UpdateViewBestBuy();
	}

	private void OnDestroy()
	{
		bonusButtonView.Deinitialize();
		PromoActionsManager.BestBuyStateUpdate -= UpdateViewBestBuy;
	}
}
