using System;
using System.Collections.Generic;
using Rilisoft;
using Rilisoft.NullExtensions;

public sealed class BuySmileBannerController : BannerWindow
{
	public static bool openedFromPromoActions;

	private IDisposable _backSubscription;

	public static string GetCurrentBuySmileContextName()
	{
		return (FriendsWindowGUI.Instance != null && FriendsWindowGUI.Instance.InterfaceEnabled) ? "Friends" : ((!(ChatViewrController.sharedController != null)) ? "Lobby" : "Sandbox");
	}

	private void OnEnable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
		}
		_backSubscription = BackSystem.Instance.Register(HandleEscape, "Buy Smiley Banner");
	}

	private void OnDisable()
	{
		if (_backSubscription != null)
		{
			_backSubscription.Dispose();
			_backSubscription = null;
		}
	}

	private void HandleEscape()
	{
		if (FriendsWindowGUI.Instance.Map((FriendsWindowGUI f) => f.InterfaceEnabled) || ChatViewrController.sharedController != null)
		{
			OnCloseClick();
		}
		else if (BannerWindowController.SharedController.Map((BannerWindowController b) => b.IsBannerShow(BannerWindowType.buySmiles)))
		{
			BannerWindowController.SharedController.HideBannerWindow();
		}
	}

	public void OnCloseClick()
	{
		openedFromPromoActions = false;
		base.gameObject.SetActive(false);
	}

	public void BuyStickersPack(StickersPackItem curStickPack)
	{
		ShopNGUIController.TryToBuy(base.transform.root.gameObject, VirtualCurrencyHelper.Price(curStickPack.KeyForBuy), delegate
		{
			Storager.setInt(curStickPack.KeyForBuy, 1, true);
			try
			{
				FlurryEvents.LogPurchaseStickers(curStickPack.KeyForBuy);
				FlurryEvents.LogSales(curStickPack.KeyForBuy, "Stickers");
				if (openedFromPromoActions)
				{
					FlurryEvents.LogSpecialOffersPanel("Efficiency", "Buy", "Stickers", curStickPack.KeyForBuy);
				}
				openedFromPromoActions = false;
			}
			catch (Exception)
			{
			}
			if (PrivateChatController.sharedController != null && PrivateChatController.sharedController.gameObject.activeInHierarchy)
			{
				PrivateChatController.sharedController.isBuySmile = true;
				if (!PrivateChatController.sharedController.isShowSmilePanel)
				{
					PrivateChatController.sharedController.showSmileButton.SetActive(true);
				}
				PrivateChatController.sharedController.buySmileButton.SetActive(false);
				OnCloseClick();
			}
			if (ChatViewrController.sharedController != null && ChatViewrController.sharedController.gameObject.activeInHierarchy)
			{
				ChatViewrController.sharedController.buySmileButton.SetActive(false);
				if (!ChatViewrController.sharedController.isShowSmilePanel)
				{
					ChatViewrController.sharedController.showSmileButton.SetActive(true);
				}
				OnCloseClick();
			}
			string currentBuySmileContextName = GetCurrentBuySmileContextName();
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "Succeeded", currentBuySmileContextName } };
			if (ExperienceController.sharedController != null)
			{
				dictionary.Add("Level", ExperienceController.sharedController.currentLevel.ToString());
			}
			if (ExpController.Instance != null)
			{
				dictionary.Add("Tier", ExpController.Instance.OurTier.ToString());
			}
			FlurryPluginWrapper.LogEventAndDublicateToConsole("Smile purchase", dictionary);
			curStickPack.OnBuy();
			ButtonBannerHUD.OnUpdateBanners();
		});
	}
}
