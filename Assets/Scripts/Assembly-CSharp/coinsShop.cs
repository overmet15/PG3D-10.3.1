using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Rilisoft;
using UnityEngine;
using com.amazon.device.iap.cpt;

internal sealed class coinsShop : MonoBehaviour
{
	public static coinsShop thisScript;

	public static bool showPlashkuPriExit = false;

	public Action onReturnAction;

	private bool productPurchased;

	private float _timeWhenPurchShown;

	private List<string> currenciesBought = new List<string>();

	private bool productsReceived;

	public Action onResumeFronNGUI;

	private bool itemBought;

	private static readonly HashSet<string> _loggedPackages = new HashSet<string>();

	private static DateTime? _etcFileTimestamp;

	private Action _drawInnerInterface;

	public string notEnoughCurrency { get; set; }

	public bool ProductPurchasedRecently
	{
		get
		{
			return productPurchased;
		}
	}

	public static bool IsStoreAvailable
	{
		get
		{
			return !IsWideLayoutAvailable && !IsNoConnection;
		}
	}

	public static bool IsWideLayoutAvailable
	{
		get
		{
			return CheckAndroidHostsTampering() || CheckLuckyPatcherInstalled() || FlurryPluginWrapper.IsLoggingFlurryAnalyticsSupported() || HasTamperedProducts;
		}
	}

	internal static bool HasTamperedProducts { private get; set; }

	public static bool IsBillingSupported
	{
		get
		{
			if (!Application.isEditor)
			{
				return StoreKitEventListener.billingSupported;
			}
			return true;
		}
	}

	public static bool IsNoConnection
	{
		get
		{
			if (thisScript != null)
			{
				return (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon) ? (!thisScript.productsReceived) : (!thisScript.productsReceived || !IsBillingSupported);
			}
			return true;
		}
	}

	private void HandleQueryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		if (!skus.Any((GoogleSkuInfo s) => s.productId == "skinsmaker"))
		{
			string[] value = skus.Select((GoogleSkuInfo sku) => sku.productId).ToArray();
			string arg = string.Join(", ", value);
			string message = string.Format("Google: Query inventory succeeded;\tPurchases count: {0}, Skus: [{1}]", purchases.Count, arg);
			Debug.Log(message);
			productsReceived = true;
		}
	}

	private void HandleItemDataRequestFinishedEvent(GetProductDataResponse response)
	{
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning("Amazon GetProductDataResponse (CoinsShop): " + response.Status);
			return;
		}
		Debug.Log("Amazon GetProductDataResponse (CoinsShop): " + response.ToJson());
		productsReceived = true;
	}

	private void OnEnable()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.AddPurchaseResponseListener(HandlePurchaseSuccessfulEvent);
		}
		else
		{
			GoogleIABManager.purchaseSucceededEvent += HandlePurchaseSucceededEvent;
		}
		if (Application.loadedLevelName != "Loading")
		{
			ActivityIndicator.IsActiveIndicator = false;
		}
		itemBought = false;
		currenciesBought.Clear();
	}

	private void OnDisable()
	{
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.RemovePurchaseResponseListener(HandlePurchaseSuccessfulEvent);
		}
		else
		{
			GoogleIABManager.purchaseSucceededEvent -= HandlePurchaseSucceededEvent;
		}
		ActivityIndicator.IsActiveIndicator = false;
		itemBought = false;
		currenciesBought.Clear();
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - _timeWhenPurchShown >= 1.25f)
		{
			productPurchased = false;
		}
	}

	private void OnDestroy()
	{
		thisScript = null;
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.RemoveGetProductDataResponseListener(HandleItemDataRequestFinishedEvent);
		}
		else
		{
			GoogleIABManager.queryInventorySucceededEvent -= HandleQueryInventorySucceededEvent;
		}
	}

	private void HandlePurchaseSuccessfullCore()
	{
		try
		{
			if (itemBought)
			{
				itemBought = false;
				productPurchased = true;
				_timeWhenPurchShown = Time.realtimeSinceStartup;
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	private void HandlePurchaseSucceededEvent(GooglePurchase purchase)
	{
		HandlePurchaseSuccessfullCore();
	}

	private void HandlePurchaseSuccessfulEvent(PurchaseResponse response)
	{
		string message = "Amazon PurchaseResponse (CoinsShop): " + response.Status;
		if (!"SUCCESSFUL".Equals(response.Status, StringComparison.OrdinalIgnoreCase))
		{
			Debug.LogWarning(message);
			return;
		}
		Debug.Log(message);
		HandlePurchaseSuccessfullCore();
	}

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		notEnoughCurrency = null;
		if (Application.isEditor)
		{
			productsReceived = true;
		}
		thisScript = this;
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AmazonIapV2Impl.Instance.AddGetProductDataResponseListener(HandleItemDataRequestFinishedEvent);
		}
		else
		{
			GoogleIABManager.queryInventorySucceededEvent += HandleQueryInventorySucceededEvent;
		}
		RefreshProductsIfNeed();
	}

	public static void TryToFireCurrenciesAddEvent(string currency)
	{
		try
		{
			CoinsMessage.FireCoinsAddedEvent(currency == "GemsCurrency");
		}
		catch (Exception ex)
		{
			Debug.LogError("coinsShop.TryToFireCurrenciesAddEvent: FireCoinsAddedEvent( currency == Defs.Gems ): " + ex);
		}
	}

	public void HandlePurchaseButton(int i, string currency = "Coins")
	{
		ButtonClickSound.Instance.PlayClick();
		if ((currency.Equals("Coins") && (i >= StoreKitEventListener.coinIds.Length || i >= VirtualCurrencyHelper.coinInappsQuantity.Length)) || (currency.Equals("GemsCurrency") && (i >= StoreKitEventListener.gemsIds.Length || i >= VirtualCurrencyHelper.gemsInappsQuantity.Length)))
		{
			Debug.LogWarning("Index of purchase is out of range: " + i);
			return;
		}
		currenciesBought.Add(currency);
		itemBought = true;
		StoreKitEventListener.purchaseInProcess = true;
		string text;
		if ("Coins".Equals(currency))
		{
			text = StoreKitEventListener.coinIds[i];
		}
		else
		{
			if (!"GemsCurrency".Equals(currency))
			{
				Debug.LogError("Unknown currency: " + currency);
				return;
			}
			text = StoreKitEventListener.gemsIds[i];
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			SkuInput skuInput = new SkuInput();
			skuInput.Sku = text;
			SkuInput skuInput2 = skuInput;
			Debug.Log("Amazon Purchase (HandlePurchaseButton): " + skuInput2.ToJson());
			AmazonIapV2Impl.Instance.Purchase(skuInput2);
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
		}
		else
		{
			_etcFileTimestamp = GetHostsTimestamp();
			FlurryPluginWrapper.LogEventToAppsFlyer("af_initiated_checkout", new Dictionary<string, string> { { "af_content_id", text } });
			GoogleIAB.purchaseProduct(text);
			MobileAdManager.Instance.SuppressShowOnReturnFromPause = true;
		}
	}

	public static void showCoinsShop()
	{
		thisScript.enabled = true;
		coinsPlashka.hideButtonCoins = true;
		coinsPlashka.showPlashka();
	}

	public static void hideCoinsShop()
	{
		if (thisScript != null)
		{
			thisScript.enabled = false;
			thisScript.notEnoughCurrency = null;
			Resources.UnloadUnusedAssets();
		}
	}

	public static void ExitFromShop(bool performOnExitActs)
	{
		hideCoinsShop();
		if (showPlashkuPriExit)
		{
			coinsPlashka.hidePlashka();
		}
		coinsPlashka.hideButtonCoins = false;
		if (performOnExitActs)
		{
			if (thisScript.onReturnAction != null && thisScript.notEnoughCurrency != null && thisScript.currenciesBought.Contains(thisScript.notEnoughCurrency))
			{
				thisScript.currenciesBought.Clear();
				thisScript.onReturnAction();
			}
			else
			{
				thisScript.onReturnAction = null;
			}
			if (thisScript.onResumeFronNGUI != null)
			{
				thisScript.onResumeFronNGUI();
				thisScript.onResumeFronNGUI = null;
				coinsPlashka.hidePlashka();
			}
		}
	}

	internal static bool CheckAndroidHostsTampering()
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return false;
		}
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
		{
			if (!File.Exists("/etc/hosts"))
			{
				return false;
			}
			try
			{
				string[] source = File.ReadAllLines("/etc/hosts");
				IEnumerable<string> source2 = source.Where((string l) => l.TrimStart().StartsWith("127."));
				return source2.Any((string l) => l.Contains("android.clients.google.com") || l.Contains("mtalk.google.com "));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				return false;
			}
		}
		return false;
	}

	internal static bool CheckLuckyPatcherInstalled()
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return false;
		}
		string[] source = new string[3] { "Y29tLmRpbW9udmlkZW8ubHVja3lwYXRjaGVy", "Y29tLmNoZWxwdXMubGFja3lwYXRjaA==", "Y29tLmZvcnBkYS5scA==" };
		IEnumerable<string> source2 = from bytes in source.Select(Convert.FromBase64String)
			where bytes != null
			select Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		return source2.Any(PackageExists);
	}

	private static bool PackageExists(string packageName)
	{
		if (packageName == null)
		{
			throw new ArgumentNullException("packageName");
		}
		if (Application.isEditor)
		{
			return false;
		}
		try
		{
			AndroidJavaObject currentActivity = AndroidSystem.Instance.CurrentActivity;
			if (currentActivity == null)
			{
				Debug.LogWarning("activity == null");
				return false;
			}
			AndroidJavaObject androidJavaObject = currentActivity.Call<AndroidJavaObject>("getPackageManager", new object[0]);
			if (androidJavaObject == null)
			{
				Debug.LogWarning("manager == null");
				return false;
			}
			AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageInfo", new object[2] { packageName, 0 });
			if (androidJavaObject2 == null)
			{
				Debug.LogWarning("packageInfo == null");
				return false;
			}
			return true;
		}
		catch (Exception arg)
		{
			if (_loggedPackages.Contains(packageName))
			{
				return false;
			}
			string message = string.Format("Error while retrieving Android package info:    {0}", arg);
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogWarning(message);
				_loggedPackages.Add(packageName);
			}
		}
		return false;
	}

	private static DateTime? GetHostsTimestamp()
	{
		try
		{
			Debug.Log("Trying to get /ets/hosts timestamp...");
			FileInfo fileInfo = new FileInfo("/etc/hosts");
			DateTime lastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
			Debug.Log("/ets/hosts timestamp: " + lastWriteTimeUtc.ToString("s"));
			return lastWriteTimeUtc;
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	internal static bool CheckHostsTimestamp()
	{
		if (_etcFileTimestamp.HasValue)
		{
			DateTime? hostsTimestamp = GetHostsTimestamp();
			if (hostsTimestamp.HasValue && _etcFileTimestamp.Value != hostsTimestamp.Value)
			{
				Debug.LogError(string.Format("Timestamp check failed: {0:s} expcted, but actual value is {1:s}.", _etcFileTimestamp.Value, hostsTimestamp.Value));
				return false;
			}
		}
		return true;
	}

	public void RefreshProductsIfNeed(bool force = false)
	{
		if (!productsReceived || force)
		{
			StoreKitEventListener.RefreshProducts();
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			RefreshProductsIfNeed();
		}
	}
}
