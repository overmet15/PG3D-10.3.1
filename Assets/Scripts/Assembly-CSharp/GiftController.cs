using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public class GiftController : MonoBehaviour
{
	private const float timerUpdateDataFromServer = 870f;

	private const string keyForSaveServerTime = "SaveServerTime";

	public const string keyCountGiftNewPlayer = "keyCountGiftNewPlayer";

	private const string keyIsGetArmorNewPlayer = "keyIsGetArmorNewPlayer";

	private const string keyIsGetSkinNewPlayer = "keyIsGetSkinNewPlayer";

	public static GiftController instance;

	public SaltedInt costBuyCanGetGift = new SaltedInt(15461355, 0);

	public bool _canGetGift;

	public float _localTimer = -1f;

	private int _oldTime = -1;

	public List<GiftNewPlayerInfo> parametrNewPlayer = new List<GiftNewPlayerInfo>();

	public List<SlotInfo> slots = new List<SlotInfo>();

	public List<GiftCategory> category = new List<GiftCategory>();

	private bool _isLoadingDataActive;

	private int _CountGiftForNewPlayer;

	private bool _activeGift;

	private bool alreadyGenerateSlot;

	private int timeForNextGift = 86400;

	public int curTimer
	{
		get
		{
			return (int)_localTimer;
		}
	}

	public static int CountGetGiftForNewPlayer
	{
		get
		{
			return Storager.getInt("keyCountGiftNewPlayer", false);
		}
		set
		{
			if (value >= 0 && value < CountGetGiftForNewPlayer)
			{
				Storager.setInt("keyCountGiftNewPlayer", value, false);
			}
		}
	}

	public static string UrlForLoadData
	{
		get
		{
			if (Defs.IsDeveloperBuild)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_test.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_ios.json";
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
			{
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_android.json";
				}
				if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
				{
					return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_amazon.json";
				}
				return string.Empty;
			}
			if (BuildSettings.BuildTargetPlatform == RuntimePlatform.MetroPlayerX64)
			{
				return "https://secure.pixelgunserver.com/pixelgun3d-config/gift/gift_pixelgun_wp8.json";
			}
			return string.Empty;
		}
	}

	public bool CanGetGift
	{
		get
		{
			return _canGetGift && ActiveGift;
		}
	}

	public bool ActiveGift
	{
		get
		{
			return _activeGift && DataIsLoaded && FriendsController.ServerTime >= 0;
		}
	}

	public bool DataIsLoaded
	{
		get
		{
			if (slots == null)
			{
				return false;
			}
			if (slots.Count == 0)
			{
				return false;
			}
			return true;
		}
	}

	private long LastTimeGetGift
	{
		get
		{
			return Storager.getInt("SaveServerTime", false);
		}
		set
		{
			int val = (int)value;
			Storager.setInt("SaveServerTime", val, false);
		}
	}

	public static event Action onChangeSlots;

	public static event Action onTimerEnded;

	public static event Action<string> onUpdateTimer;

	private void Awake()
	{
		instance = this;
		_localTimer = -1f;
		category.Clear();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Storager.hasKey("SaveServerTime"))
		{
			Storager.setInt("keyCountGiftNewPlayer", 2, false);
		}
		if (!Storager.hasKey("keyCountGiftNewPlayer"))
		{
			Storager.setInt("keyCountGiftNewPlayer", 0, false);
		}
		if (!Storager.hasKey("keyIsGetArmorNewPlayer"))
		{
			Storager.setInt("keyIsGetArmorNewPlayer", 0, false);
		}
		if (!Storager.hasKey("keyIsGetSkinNewPlayer"))
		{
			Storager.setInt("keyIsGetSkinNewPlayer", 0, false);
		}
		Storager.getInt("keyCountGiftNewPlayer", false);
		StartCoroutine(GetDataFromServerLoop());
		FriendsController.ServerTimeUpdated += OnUpdateTimeFromServer;
	}

	private void OnDestroy()
	{
		FriendsController.ServerTimeUpdated -= OnUpdateTimeFromServer;
		instance = null;
	}

	private void Update()
	{
		if (curTimer > 0)
		{
			_localTimer -= Time.deltaTime;
			if (_localTimer < 0f)
			{
				_localTimer = 0f;
			}
			_canGetGift = false;
			if (_oldTime != curTimer)
			{
				_oldTime = curTimer;
				if (GiftController.onUpdateTimer != null)
				{
					GiftController.onUpdateTimer(GetStringTimer());
				}
			}
		}
		else if (!_canGetGift && curTimer == 0)
		{
			_localTimer = -1f;
			_canGetGift = true;
			if (GiftController.onUpdateTimer != null)
			{
				GiftController.onUpdateTimer(GetStringTimer());
			}
			if (GiftController.onTimerEnded != null)
			{
				GiftController.onTimerEnded();
			}
		}
	}

	public void SetTimer(int val)
	{
		if (val > timeForNextGift)
		{
			val = timeForNextGift;
		}
		if (val == 0)
		{
			LastTimeGetGift = FriendsController.ServerTime - timeForNextGift + 1;
		}
		else
		{
			long lastTimeGetGift = FriendsController.ServerTime - (timeForNextGift - val);
			LastTimeGetGift = lastTimeGetGift;
		}
		OnUpdateTimeFromServer();
	}

	private TypeGiftCategory ParseToEnum(string typeCat)
	{
		switch (typeCat.ToLower())
		{
		case "coins":
			return TypeGiftCategory.Coins;
		case "gems":
			return TypeGiftCategory.Gems;
		case "skins":
			return TypeGiftCategory.Skins;
		case "armorandhat":
			return TypeGiftCategory.Armor;
		case "gear":
			return TypeGiftCategory.Gear;
		case "grenades":
			return TypeGiftCategory.Grenades;
		case "wear":
			return TypeGiftCategory.Wear;
		case "guns":
			return TypeGiftCategory.Guns;
		case "event_content":
			return TypeGiftCategory.Event_content;
		default:
			return TypeGiftCategory.none;
		}
	}

	public void CheckGifts()
	{
		if (_activeGift)
		{
			if (category != null && category.Count > 0)
			{
				StartCoroutine(CheckAvailableGifts());
			}
			return;
		}
		category.Clear();
		slots.Clear();
		if (GiftController.onChangeSlots != null)
		{
			GiftController.onChangeSlots();
		}
	}

	public void CreateSlots()
	{
		if (alreadyGenerateSlot || !_activeGift)
		{
			return;
		}
		alreadyGenerateSlot = true;
		List<SlotInfo> list = new List<SlotInfo>();
		for (int i = 0; i < category.Count; i++)
		{
			GiftCategory giftCategory = category[i];
			giftCategory.CheckGifts();
			if (giftCategory.CountAvaliableGifts <= 0)
			{
				continue;
			}
			SlotInfo slotInfo = new SlotInfo();
			slotInfo.category = giftCategory;
			slotInfo.gift = giftCategory.GetRandomGift();
			if (slotInfo.gift != null && !string.IsNullOrEmpty(slotInfo.gift.IdGift))
			{
				slotInfo.percentGetSlot = giftCategory.sumPerAllGifts;
				slotInfo.positionInScroll = giftCategory.positionInScroll;
				slotInfo.isActiveEvent = false;
				if (CountGetGiftForNewPlayer > 0)
				{
					SetPerGetGiftForNewPlayer(slotInfo);
				}
				list.Add(slotInfo);
			}
		}
		list.Sort(delegate(SlotInfo left, SlotInfo right)
		{
			if (left == null && right == null)
			{
				return 0;
			}
			if (left == null)
			{
				return -1;
			}
			return (right == null) ? 1 : left.positionInScroll.CompareTo(right.positionInScroll);
		});
		slots = list;
		if (GiftController.onChangeSlots != null)
		{
			GiftController.onChangeSlots();
		}
		OnUpdateTimeFromServer();
	}

	public GiftNewPlayerInfo GetInfoNewPlayer(TypeGiftCategory needCat)
	{
		return parametrNewPlayer.Find((GiftNewPlayerInfo val) => val.typeCategory == needCat);
	}

	private void SetPerGetGiftForNewPlayer(SlotInfo curSlot)
	{
		float percentGetSlot = 0f;
		int value = curSlot.gift.count.Value;
		curSlot.isActiveEvent = true;
		GiftNewPlayerInfo infoNewPlayer = GetInfoNewPlayer(curSlot.category.typeCat);
		if (infoNewPlayer != null)
		{
			value = infoNewPlayer.count.Value;
			if (curSlot.category.typeCat == TypeGiftCategory.Armor && Storager.getInt("keyIsGetArmorNewPlayer", false) == 0)
			{
				percentGetSlot = infoNewPlayer.percent;
			}
			if (curSlot.category.typeCat == TypeGiftCategory.Skins && Storager.getInt("keyIsGetSkinNewPlayer", false) == 0)
			{
				percentGetSlot = infoNewPlayer.percent;
			}
			if (curSlot.category.typeCat == TypeGiftCategory.Coins)
			{
				percentGetSlot = infoNewPlayer.percent;
			}
			if (curSlot.category.typeCat == TypeGiftCategory.Gems)
			{
				percentGetSlot = infoNewPlayer.percent;
			}
		}
		curSlot.percentGetSlot = percentGetSlot;
		curSlot.CountGift = value;
	}

	public void UpdateSlot(SlotInfo curSlot)
	{
		curSlot.category.CheckGifts();
		curSlot.gift = curSlot.category.GetRandomGift();
		if (curSlot.gift != null)
		{
			curSlot.percentGetSlot = curSlot.category.sumPerAllGifts;
			curSlot.positionInScroll = curSlot.category.positionInScroll;
		}
	}

	public void ReCreateSlots()
	{
		alreadyGenerateSlot = false;
		CheckGifts();
	}

	public SlotInfo GetRandomSlot()
	{
		return null;
	}

	private IEnumerator GetDataFromServerLoop()
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(DownloadDataFormServer());
			yield return new WaitForSeconds(870f);
		}
	}

	private IEnumerator DownloadDataFormServer()
	{
		if (_isLoadingDataActive)
		{
			yield break;
		}
		_isLoadingDataActive = true;
		string urlDataAddress = UrlForLoadData;
		WWW downloadData = null;
		int iter = 3;
		while (iter > 0)
		{
			downloadData = Tools.CreateWwwIfNotConnected(urlDataAddress);
			if (downloadData == null)
			{
				yield break;
			}
			while (!downloadData.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(downloadData.error))
			{
				yield return new WaitForSeconds(5f);
				iter--;
				continue;
			}
			break;
		}
		if (downloadData == null || !string.IsNullOrEmpty(downloadData.error))
		{
			if (Defs.IsDeveloperBuild && downloadData != null)
			{
				Debug.LogWarningFormat("Request to {0} failed: {1}", urlDataAddress, downloadData.error);
			}
			_isLoadingDataActive = false;
			yield break;
		}
		string responseText = URLs.Sanitize(downloadData);
		Dictionary<string, object> allData = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (allData == null)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogError("Bad response: " + responseText);
			}
			_isLoadingDataActive = false;
			yield break;
		}
		if (allData.ContainsKey("isActive"))
		{
			_activeGift = Convert.ToBoolean(allData["isActive"], CultureInfo.InvariantCulture);
			if (!_activeGift)
			{
				_isLoadingDataActive = false;
				OnDataLoaded();
				yield break;
			}
		}
		if (allData.ContainsKey("price"))
		{
			costBuyCanGetGift.Value = Convert.ToInt32(allData["price"], CultureInfo.InvariantCulture);
		}
		List<GiftNewPlayerInfo> infoNewPlayer = new List<GiftNewPlayerInfo>();
		if (allData.ContainsKey("newPlayerEvent"))
		{
			List<object> listAllParametrNewPlayer = allData["newPlayerEvent"] as List<object>;
			if (listAllParametrNewPlayer != null)
			{
				for (int iTG = 0; iTG < listAllParametrNewPlayer.Count; iTG++)
				{
					Dictionary<string, object> curParametr = listAllParametrNewPlayer[iTG] as Dictionary<string, object>;
					GiftNewPlayerInfo curAddInfo = new GiftNewPlayerInfo();
					if (curParametr.ContainsKey("typeCategory"))
					{
						curAddInfo.typeCategory = ParseToEnum(curParametr["typeCategory"].ToString());
						if (curParametr.ContainsKey("count"))
						{
							curAddInfo.count.Value = int.Parse(curParametr["count"].ToString());
						}
						if (curParametr.ContainsKey("percent"))
						{
							object curPercentObject = curParametr["percent"];
							curAddInfo.percent = (float)Convert.ToDouble(curPercentObject, CultureInfo.InvariantCulture);
						}
						infoNewPlayer.Add(curAddInfo);
					}
				}
			}
		}
		List<GiftCategory> newCategories = new List<GiftCategory>();
		if (allData.ContainsKey("categories"))
		{
			List<object> listCategories = allData["categories"] as List<object>;
			if (listCategories != null)
			{
				for (int iC = 0; iC < listCategories.Count; iC++)
				{
					Dictionary<string, object> infoCategory = listCategories[iC] as Dictionary<string, object>;
					if (infoCategory == null)
					{
						continue;
					}
					GiftCategory newCategory = new GiftCategory();
					if (!infoCategory.ContainsKey("typeCategory"))
					{
						continue;
					}
					newCategory.typeCat = ParseToEnum(infoCategory["typeCategory"].ToString());
					if (infoCategory.ContainsKey("posInScroll"))
					{
						newCategory.positionInScroll = int.Parse(infoCategory["posInScroll"].ToString());
					}
					if (!infoCategory.ContainsKey("gifts"))
					{
						continue;
					}
					if (infoCategory.ContainsKey("keyTransInfo"))
					{
						newCategory.keyTranslateInfoCommon = infoCategory["keyTransInfo"].ToString();
					}
					List<object> gifts = infoCategory["gifts"] as List<object>;
					List<GiftInfo> newListGifts = (newCategory.listGifts = new List<GiftInfo>());
					if (gifts != null)
					{
						for (int iG = 0; iG < gifts.Count; iG++)
						{
							Dictionary<string, object> infoGift = gifts[iG] as Dictionary<string, object>;
							if (infoGift == null)
							{
								continue;
							}
							GiftInfo newGiftInfo = new GiftInfo();
							switch (newCategory.typeCat)
							{
							case TypeGiftCategory.Coins:
								newGiftInfo.IdGift = "Coins";
								break;
							case TypeGiftCategory.Gems:
								newGiftInfo.IdGift = "Gems";
								break;
							default:
								if (infoGift.ContainsKey("idGift"))
								{
									newGiftInfo.IdGift = infoGift["idGift"].ToString();
								}
								break;
							}
							if (infoGift.ContainsKey("count"))
							{
								newGiftInfo.count.Value = int.Parse(infoGift["count"].ToString());
							}
							if (infoGift.ContainsKey("percent"))
							{
								object percentObject = infoGift["percent"];
								newGiftInfo.percentAddInSlot = (float)Convert.ToDouble(percentObject, CultureInfo.InvariantCulture);
							}
							if (infoGift.ContainsKey("keyTransInfo"))
							{
								newGiftInfo.keyTranslateInfo = infoGift["keyTransInfo"].ToString();
							}
							if (newGiftInfo.count.Value == 0)
							{
								newGiftInfo.count.Value = 1;
							}
							newListGifts.Add(newGiftInfo);
						}
					}
					if (newCategory.listGifts.Count > 0)
					{
						newCategories.Add(newCategory);
					}
				}
			}
		}
		category = newCategories;
		parametrNewPlayer = infoNewPlayer;
		OnDataLoaded();
		_isLoadingDataActive = false;
	}

	private void OnDataLoaded()
	{
		CheckGifts();
	}

	public SlotInfo GetGift()
	{
		float num = 0f;
		for (int i = 0; i < slots.Count; i++)
		{
			num += slots[i].percentGetSlot;
		}
		float num2 = UnityEngine.Random.Range(0f, num);
		float num3 = 0f;
		SlotInfo slotInfo = null;
		for (int j = 0; j < slots.Count; j++)
		{
			SlotInfo slotInfo2 = slots[j];
			num3 += slotInfo2.percentGetSlot;
			if (num2 <= num3)
			{
				slotInfo = slotInfo2;
				slotInfo.numInScroll = j;
				break;
			}
		}
		if (slotInfo != null)
		{
			CountGetGiftForNewPlayer--;
			GiveProductForSlot(slotInfo);
		}
		return slotInfo;
	}

	public void CheckAvaliableSlots()
	{
		bool flag = false;
		for (int i = 0; i < slots.Count; i++)
		{
			SlotInfo slotInfo = slots[i];
			if (slotInfo.CheckAvaliableGift())
			{
				flag = true;
			}
			if (slotInfo.gift == null)
			{
				slots.RemoveAt(i);
				i--;
			}
		}
		if (flag && GiftController.onChangeSlots != null)
		{
			GiftController.onChangeSlots();
		}
	}

	public static bool AvailableGift(string idGift, TypeGiftCategory curType)
	{
		switch (curType)
		{
		case TypeGiftCategory.Coins:
		case TypeGiftCategory.Gems:
			return true;
		case TypeGiftCategory.Grenades:
			return true;
		case TypeGiftCategory.Gear:
			return true;
		case TypeGiftCategory.Guns:
			if (Storager.getInt(idGift, true) == 0)
			{
				return true;
			}
			return false;
		case TypeGiftCategory.Wear:
			return !ItemDb.IsItemInInventory(idGift);
		case TypeGiftCategory.Armor:
			return AvaliableIdArmor(idGift);
		case TypeGiftCategory.Event_content:
			return true;
		case TypeGiftCategory.Skins:
		{
			if (string.IsNullOrEmpty(idGift))
			{
				return false;
			}
			bool isForMoneySkin = false;
			return !SkinsController.IsSkinBought(idGift, out isForMoneySkin);
		}
		default:
			return false;
		}
	}

	public void GiveProductForSlot(SlotInfo curSlot)
	{
		if (curSlot == null)
		{
			return;
		}
		switch (curSlot.category.typeCat)
		{
		case TypeGiftCategory.Coins:
			BankController.AddCoins(curSlot.CountGift, false);
			StartCoroutine(BankController.WaitForIndicationGems(false));
			break;
		case TypeGiftCategory.Gems:
			BankController.AddGems(curSlot.CountGift, false);
			StartCoroutine(BankController.WaitForIndicationGems(true));
			break;
		case TypeGiftCategory.Skins:
			Storager.setInt("keyIsGetSkinNewPlayer", 1, false);
			ShopNGUIController.ProvideShopItemOnStarterPackBoguht(ShopNGUIController.CategoryNames.SkinsCategory, curSlot.gift.IdGift, 1, false, 0, null, null, false, true, false);
			break;
		case TypeGiftCategory.Gear:
		{
			int int2 = Storager.getInt(curSlot.gift.IdGift, false);
			Storager.setInt(curSlot.gift.IdGift, int2 + curSlot.gift.count.Value, false);
			break;
		}
		case TypeGiftCategory.Grenades:
		{
			int @int = Storager.getInt(curSlot.gift.IdGift, false);
			Storager.setInt(curSlot.gift.IdGift, @int + curSlot.gift.count.Value, false);
			break;
		}
		case TypeGiftCategory.Wear:
			curSlot.gift.UpdateType();
			ShopNGUIController.ProvideShopItemOnStarterPackBoguht(curSlot.gift.typeShopCat, curSlot.gift.IdGift, 1, false, 0, null, null, true, true, false);
			if (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.wearEquipAction != null)
			{
				ShopNGUIController.sharedShop.wearEquipAction(curSlot.gift.typeShopCat, string.Empty, string.Empty);
			}
			break;
		case TypeGiftCategory.Armor:
			Storager.setInt("keyIsGetArmorNewPlayer", 1, false);
			curSlot.gift.UpdateType();
			if (curSlot.gift.typeShopCat == ShopNGUIController.CategoryNames.ArmorCategory)
			{
				ShopNGUIController.ProvideShopItemOnStarterPackBoguht(ShopNGUIController.CategoryNames.ArmorCategory, curSlot.gift.IdGift, 1, false, 0, null, null, true, true, false);
				if (ShopNGUIController.sharedShop != null && ShopNGUIController.sharedShop.wearEquipAction != null)
				{
					ShopNGUIController.sharedShop.wearEquipAction(ShopNGUIController.CategoryNames.ArmorCategory, string.Empty, string.Empty);
				}
			}
			break;
		case TypeGiftCategory.Guns:
			WeaponManager.ProvideExclusiveWeaponByTag(curSlot.gift.IdGift);
			break;
		}
	}

	private IEnumerator CheckAvailableGifts()
	{
		while (!(WeaponManager.sharedManager != null))
		{
			yield return null;
		}
		CreateSlots();
	}

	public void ReSaveLastTimeSever()
	{
		LastTimeGetGift = FriendsController.ServerTime;
		OnUpdateTimeFromServer();
	}

	public string GetStringTimer()
	{
		int num = curTimer / 3600;
		int num2 = curTimer / 60 - num * 60;
		int num3 = curTimer - num * 3600 - num2 * 60;
		string text = ((num >= 10) ? num.ToString() : ("0" + num));
		string text2 = ((num2 >= 10) ? num2.ToString() : ("0" + num2));
		string text3 = ((num3 >= 10) ? num3.ToString() : ("0" + num3));
		return text + ":" + text2 + ":" + text3;
	}

	private void OnUpdateTimeFromServer()
	{
		if (slots.Count == 0)
		{
			StartCoroutine(DownloadDataFormServer());
		}
		else
		{
			if (FriendsController.ServerTime < 0)
			{
				return;
			}
			_localTimer = -1f;
			_canGetGift = false;
			if (!Storager.hasKey("SaveServerTime"))
			{
				LastTimeGetGift = FriendsController.ServerTime - timeForNextGift + 1;
			}
			int num = (int)(FriendsController.ServerTime - LastTimeGetGift);
			if (num >= timeForNextGift)
			{
				_canGetGift = true;
				if (GiftController.onTimerEnded != null)
				{
					GiftController.onTimerEnded();
				}
			}
			else
			{
				_canGetGift = false;
				_localTimer = timeForNextGift - num;
			}
		}
	}

	public static bool AvaliableIdArmor(string needId)
	{
		if (string.IsNullOrEmpty(needId))
		{
			return false;
		}
		string text = Wear.ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames.ArmorCategory);
		if (needId == text)
		{
			return true;
		}
		return false;
	}

	public static string GetIdArmorOrHat()
	{
		string empty = string.Empty;
		return Wear.ArmorOrArmorHatAvailableForBuy(ShopNGUIController.CategoryNames.ArmorCategory);
	}

	public void TryGetData()
	{
		if (!DataIsLoaded)
		{
			StartCoroutine(DownloadDataFormServer());
		}
	}
}
