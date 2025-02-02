using System;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using UnityEngine;

public sealed class VirtualCurrencyHelper
{
	public static readonly int[] coinInappsQuantity;

	public static readonly int[] gemsInappsQuantity;

	public static readonly int[] coinPriceIds;

	public static readonly int[] gemsPriceIds;

	public static readonly int[] starterPackFakePrice;

	private static Dictionary<string, SaltedInt> prices;

	private static System.Random _prng;

	private static WeakReference _referencePricesInUsd;

	internal static Dictionary<string, decimal> ReferencePricesInUsd
	{
		get
		{
			if (_referencePricesInUsd != null && _referencePricesInUsd.IsAlive)
			{
				return (Dictionary<string, decimal>)_referencePricesInUsd.Target;
			}
			Dictionary<string, decimal> dictionary = InitializeReferencePrices();
			_referencePricesInUsd = new WeakReference(dictionary, false);
			return dictionary;
		}
	}

	static VirtualCurrencyHelper()
	{
		coinInappsQuantity = new int[7] { 15, 45, 80, 165, 335, 865, 2000 };
		gemsInappsQuantity = new int[7] { 9, 27, 48, 100, 200, 517, 1200 };
		coinPriceIds = new int[7] { 1, 3, 5, 10, 20, 50, 100 };
		gemsPriceIds = new int[7] { 1, 3, 5, 10, 20, 50, 100 };
		starterPackFakePrice = new int[8] { 6, 5, 4, 3, 2, 1, 1, 1 };
		prices = new Dictionary<string, SaltedInt>();
		_prng = new System.Random(4103);
		AddPrice(PremiumAccountController.AccountType.OneDay.ToString(), 5);
		AddPrice(PremiumAccountController.AccountType.ThreeDay.ToString(), 10);
		AddPrice(PremiumAccountController.AccountType.SevenDays.ToString(), 20);
		AddPrice(PremiumAccountController.AccountType.Month.ToString(), 60);
		AddPrice("crystalsword", 185);
		AddPrice("Fullhealth", 15);
		AddPrice("bigammopack", 15);
		AddPrice("MinerWeapon", 35);
		AddPrice(StoreKitEventListener.elixirID, 15);
		AddPrice(StoreKitEventListener.chief, 25);
		AddPrice(StoreKitEventListener.nanosoldier, 25);
		AddPrice(StoreKitEventListener.endmanskin, 25);
		AddPrice(StoreKitEventListener.spaceengineer, 25);
		AddPrice(StoreKitEventListener.steelman, 25);
		AddPrice(StoreKitEventListener.CaptainSkin, 25);
		AddPrice(StoreKitEventListener.HawkSkin, 25);
		AddPrice(StoreKitEventListener.TunderGodSkin, 25);
		AddPrice(StoreKitEventListener.GreenGuySkin, 25);
		AddPrice(StoreKitEventListener.GordonSkin, 25);
		AddPrice(StoreKitEventListener.armor, 10);
		AddPrice(StoreKitEventListener.armor2, 15);
		AddPrice(StoreKitEventListener.armor3, 20);
		AddPrice(StoreKitEventListener.magicGirl, 25);
		AddPrice(StoreKitEventListener.braveGirl, 25);
		AddPrice(StoreKitEventListener.glamDoll, 25);
		AddPrice(StoreKitEventListener.kittyGirl, 25);
		AddPrice(StoreKitEventListener.famosBoy, 25);
		AddPrice(StoreKitEventListener.skin810_1, 25);
		AddPrice(StoreKitEventListener.skin810_2, 25);
		AddPrice(StoreKitEventListener.skin810_3, 25);
		AddPrice(StoreKitEventListener.skin810_4, 25);
		AddPrice(StoreKitEventListener.skin810_5, 25);
		AddPrice(StoreKitEventListener.skin810_6, 25);
		AddPrice(StoreKitEventListener.skin931_1, 25);
		AddPrice(StoreKitEventListener.skin931_2, 25);
		for (int i = 0; i < StoreKitEventListener.Skins_11_040915.Length; i++)
		{
			AddPrice(StoreKitEventListener.Skins_11_040915[i], 25);
		}
		AddPrice("super_socialman", 25);
		AddPrice(StoreKitEventListener.skin_tiger, 25);
		AddPrice(StoreKitEventListener.skin_pitbull, 25);
		AddPrice(StoreKitEventListener.skin_santa, 25);
		AddPrice(StoreKitEventListener.skin_elf_new_year, 25);
		AddPrice(StoreKitEventListener.skin_girl_new_year, 25);
		AddPrice(StoreKitEventListener.skin_cookie_new_year, 25);
		AddPrice(StoreKitEventListener.skin_snowman_new_year, 25);
		AddPrice(StoreKitEventListener.skin_jetti_hnight, 25);
		AddPrice(StoreKitEventListener.skin_startrooper, 25);
		AddPrice(StoreKitEventListener.easter_skin1, 25);
		AddPrice(StoreKitEventListener.easter_skin2, 25);
		AddPrice("CustomSkinID", Defs.skinsMakerPrice);
		AddPrice("cape_Archimage", 60);
		AddPrice("cape_BloodyDemon", 60);
		AddPrice("cape_RoyalKnight", 60);
		AddPrice("cape_SkeletonLord", 60);
		AddPrice("cape_EliteCrafter", 60);
		AddPrice("cape_Custom", 75);
		AddPrice("HitmanCape_Up1", 30);
		AddPrice("BerserkCape_Up1", 30);
		AddPrice("DemolitionCape_Up1", 30);
		AddPrice("SniperCape_Up1", 30);
		AddPrice("StormTrooperCape_Up1", 30);
		AddPrice("HitmanCape_Up2", 45);
		AddPrice("BerserkCape_Up2", 45);
		AddPrice("DemolitionCape_Up2", 45);
		AddPrice("SniperCape_Up2", 45);
		AddPrice("StormTrooperCape_Up2", 45);
		AddPrice("cape_Engineer", 60);
		AddPrice("cape_Engineer_Up1", 30);
		AddPrice("cape_Engineer_Up2", 45);
		AddPrice("hat_DiamondHelmet", 65);
		AddPrice("hat_Adamant_3", 3);
		AddPrice("hat_Headphones", 50);
		AddPrice("hat_ManiacMask", 65);
		AddPrice("hat_KingsCrown", 150);
		AddPrice("hat_SeriousManHat", 50);
		AddPrice("hat_Samurai", 95);
		AddPrice("hat_AlmazHelmet", 95);
		AddPrice("hat_ArmyHelmet", 95);
		AddPrice("hat_SteelHelmet", 95);
		AddPrice("hat_GoldHelmet", 95);
		AddPrice("hat_Army_1", 70);
		AddPrice("hat_Army_2", 70);
		AddPrice("hat_Army_3", 70);
		AddPrice("hat_Army_4", 70);
		AddPrice("hat_Steel_1", 85);
		AddPrice("hat_Steel_2", 85);
		AddPrice("hat_Steel_3", 85);
		AddPrice("hat_Steel_4", 85);
		AddPrice("hat_Royal_1", 100);
		AddPrice("hat_Royal_2", 100);
		AddPrice("hat_Royal_3", 100);
		AddPrice("hat_Royal_4", 100);
		AddPrice("hat_Almaz_1", 120);
		AddPrice("hat_Almaz_2", 120);
		AddPrice("hat_Almaz_3", 120);
		AddPrice("hat_Almaz_4", 120);
		AddPrice(PotionsController.HastePotion, 2);
		AddPrice(PotionsController.MightPotion, 2);
		AddPrice(PotionsController.RegenerationPotion, 5);
		AddPrice(GearManager.UpgradeIDForGear("InvisibilityPotion", 1), 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 2, 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 3, 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 4, 1);
		AddPrice("InvisibilityPotion" + GearManager.UpgradeSuffix + 5, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 1, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 2, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 3, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 4, 1);
		AddPrice("GrenadeID" + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 1, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 2, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 3, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 4, 1);
		AddPrice(GearManager.Turret + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 1, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 2, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 3, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 4, 1);
		AddPrice(GearManager.Mech + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 1, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 2, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 3, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 4, 1);
		AddPrice(GearManager.Jetpack + GearManager.UpgradeSuffix + 5, 1);
		AddPrice(GearManager.Wings + GearManager.OneItemSuffix + 0, 3);
		AddPrice(GearManager.Bear + GearManager.OneItemSuffix + 0, 2);
		AddPrice(GearManager.BigHeadPotion + GearManager.OneItemSuffix + 0, 1);
		AddPrice(GearManager.MusicBox + GearManager.OneItemSuffix + 0, 5);
		AddPrice(GearManager.Like + GearManager.OneItemSuffix + 0, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 0, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 1, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 2, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 3, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 4, 3);
		AddPrice("InvisibilityPotion" + GearManager.OneItemSuffix + 5, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 0, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 1, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 2, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 3, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 4, 3);
		AddPrice("GrenadeID" + GearManager.OneItemSuffix + 5, 3);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 0, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 1, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 2, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 3, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 4, 5);
		AddPrice(GearManager.Turret + GearManager.OneItemSuffix + 5, 5);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 0, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 1, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 2, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 3, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 4, 7);
		AddPrice(GearManager.Mech + GearManager.OneItemSuffix + 5, 7);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 0, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 1, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 2, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 3, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 4, 4);
		AddPrice(GearManager.Jetpack + GearManager.OneItemSuffix + 5, 4);
		AddPrice("boots_red", 50);
		AddPrice("boots_gray", 50);
		AddPrice("boots_blue", 50);
		AddPrice("boots_green", 50);
		AddPrice("boots_black", 50);
		AddPrice("boots_tabi", 120);
		AddPrice("HitmanBoots_Up1", 25);
		AddPrice("StormTrooperBoots_Up1", 25);
		AddPrice("SniperBoots_Up1", 25);
		AddPrice("DemolitionBoots_Up1", 25);
		AddPrice("BerserkBoots_Up1", 25);
		AddPrice("HitmanBoots_Up2", 40);
		AddPrice("StormTrooperBoots_Up2", 40);
		AddPrice("SniperBoots_Up2", 40);
		AddPrice("DemolitionBoots_Up2", 40);
		AddPrice("BerserkBoots_Up2", 40);
		AddPrice("mask_sniper", 40);
		AddPrice("mask_sniper_up1", 15);
		AddPrice("mask_sniper_up2", 30);
		AddPrice("mask_demolition", 40);
		AddPrice("mask_demolition_up1", 15);
		AddPrice("mask_demolition_up2", 30);
		AddPrice("mask_hitman_1", 40);
		AddPrice("mask_hitman_1_up1", 15);
		AddPrice("mask_hitman_1_up2", 30);
		AddPrice("mask_berserk", 40);
		AddPrice("mask_berserk_up1", 15);
		AddPrice("mask_berserk_up2", 30);
		AddPrice("mask_trooper", 40);
		AddPrice("mask_trooper_up1", 15);
		AddPrice("mask_trooper_up2", 30);
		AddPrice("mask_engineer", 40);
		AddPrice("mask_engineer_up1", 15);
		AddPrice("mask_engineer_up2", 30);
		AddPrice("EngineerBoots", 50);
		AddPrice("EngineerBoots_Up1", 25);
		AddPrice("EngineerBoots_Up2", 40);
		AddPrice("Armor_Army_1", 70);
		AddPrice("Armor_Army_2", 70);
		AddPrice("Armor_Army_3", 70);
		AddPrice("Armor_Steel_1", 85);
		AddPrice("Armor_Steel_2", 85);
		AddPrice("Armor_Steel_3", 85);
		AddPrice("Armor_Royal_1", 100);
		AddPrice("Armor_Royal_2", 100);
		AddPrice("Armor_Royal_3", 100);
		AddPrice("Armor_Almaz_1", 120);
		AddPrice("Armor_Almaz_2", 120);
		AddPrice("Armor_Almaz_3", 120);
		AddPrice("Armor_Army_4", 120);
		AddPrice("Armor_Steel_4", 120);
		AddPrice("Armor_Royal_4", 135);
		AddPrice("Armor_Almaz_4", 120);
		AddPrice("Armor_Adamant_3", 3);
		AddPrice("Armor_Rubin_1", 135);
		AddPrice("Armor_Rubin_2", 135);
		AddPrice("Armor_Rubin_3", 135);
		AddPrice("hat_Rubin_1", 135);
		AddPrice("hat_Rubin_2", 135);
		AddPrice("hat_Rubin_3", 135);
		AddPrice("Armor_Adamant_Const_1", 170);
		AddPrice("Armor_Adamant_Const_2", 170);
		AddPrice("Armor_Adamant_Const_3", 170);
		AddPrice("hat_Adamant_Const_1", 170);
		AddPrice("hat_Adamant_Const_2", 170);
		AddPrice("hat_Adamant_Const_3", 170);
		AddPrice(StickersController.KeyForBuyPack(TypePackSticker.classic), 20);
		AddPrice(StickersController.KeyForBuyPack(TypePackSticker.christmas), 30);
		AddPrice(StickersController.KeyForBuyPack(TypePackSticker.easter), 40);
		AddPrice(Defs.BuyGiftKey, 5);
		for (int j = 0; j < 11; j++)
		{
			AddPrice("newskin_" + j, 25);
		}
		for (int k = 11; k < 19; k++)
		{
			AddPrice("newskin_" + k, 25);
		}
		_prng = null;
	}

	public static ItemPrice Price(string key)
	{
		if (key == null || !prices.ContainsKey(key))
		{
			return null;
		}
		int value = prices[key].Value;
		string currency = "Coins";
		string text = GearManager.HolderQuantityForID(key);
		bool flag = false;
		flag = text != null && (GearManager.Gear.Contains(text) || GearManager.DaterGear.Contains(text)) && !key.Contains(GearManager.UpgradeSuffix) && !text.Equals(GearManager.Grenade);
		switch (key)
		{
		case "cape_Archimage":
		case "cape_BloodyDemon":
		case "cape_RoyalKnight":
		case "cape_SkeletonLord":
		case "cape_EliteCrafter":
		case "HitmanCape_Up1":
		case "BerserkCape_Up1":
		case "DemolitionCape_Up1":
		case "SniperCape_Up1":
		case "StormTrooperCape_Up1":
		case "HitmanCape_Up2":
		case "BerserkCape_Up2":
		case "DemolitionCape_Up2":
		case "SniperCape_Up2":
		case "StormTrooperCape_Up2":
		case "cape_Engineer":
		case "cape_Engineer_Up1":
		case "cape_Engineer_Up2":
			flag = true;
			break;
		}
		switch (key)
		{
		case "boots_red":
		case "boots_gray":
		case "boots_blue":
		case "boots_green":
		case "boots_black":
		case "HitmanBoots_Up1":
		case "StormTrooperBoots_Up1":
		case "SniperBoots_Up1":
		case "DemolitionBoots_Up1":
		case "BerserkBoots_Up1":
		case "HitmanBoots_Up2":
		case "StormTrooperBoots_Up2":
		case "SniperBoots_Up2":
		case "DemolitionBoots_Up2":
		case "BerserkBoots_Up2":
		case "EngineerBoots":
		case "EngineerBoots_Up1":
		case "EngineerBoots_Up2":
			flag = true;
			break;
		}
		IEnumerable<string> source = Wear.wear[ShopNGUIController.CategoryNames.MaskCategory].SelectMany((List<string> l) => l);
		if (key != "hat_ManiacMask" && source.Contains(key))
		{
			flag = true;
		}
		if (key == StickersController.KeyForBuyPack(TypePackSticker.classic))
		{
			flag = true;
		}
		if (key == StickersController.KeyForBuyPack(TypePackSticker.christmas))
		{
			flag = true;
		}
		if (key == StickersController.KeyForBuyPack(TypePackSticker.easter))
		{
			flag = true;
		}
		if (key == Defs.BuyGiftKey)
		{
			flag = true;
		}
		if (TempItemsController.PriceCoefs.ContainsKey(key))
		{
			flag = true;
		}
		if (key != null && (key.Equals(PremiumAccountController.AccountType.OneDay.ToString()) || key.Equals(PremiumAccountController.AccountType.ThreeDay.ToString()) || key.Equals(PremiumAccountController.AccountType.SevenDays.ToString()) || key.Equals(PremiumAccountController.AccountType.Month.ToString())))
		{
			flag = true;
		}
		if (flag)
		{
			currency = "GemsCurrency";
		}
		return new ItemPrice(value, currency);
	}

	public static int GetVirtualCurrencyAmount(string currency, int inappIndex)
	{
		if (string.IsNullOrEmpty(currency))
		{
			throw new ArgumentException("Empty currency.", "currency");
		}
		HashSet<string> hashSet = new HashSet<string>();
		hashSet.Add("Coins");
		hashSet.Add("GemsCurrency");
		HashSet<string> hashSet2 = hashSet;
		if (!hashSet2.Contains(currency))
		{
			throw new ArgumentException("Invalid currency: " + currency, "currency");
		}
		return ((!PromoActionsManager.sharedManager.IsEventX3Active && Defs.ABTestDeviceNumber != 2) ? 1 : 3) * ((!currency.Equals("Coins")) ? gemsInappsQuantity[inappIndex] : coinInappsQuantity[inappIndex]);
	}

	public static int GetCoinInappsQuantity(int inappIndex)
	{
		if (PromoActionsManager.sharedManager == null)
		{
			Debug.LogError("GetCoinInappsQuantity: PromoActionsManager.sharedManager == null when calculating");
		}
		if (PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.IsEventX3Active)
		{
			return 3 * coinInappsQuantity[inappIndex];
		}
		return ((Defs.ABTestDeviceNumber != 2) ? 1 : 3) * coinInappsQuantity[inappIndex];
	}

	public static int GetGemsInappsQuantity(int inappIndex)
	{
		if (PromoActionsManager.sharedManager == null)
		{
			Debug.LogError("GetGemsInappsQuantity: PromoActionsManager.sharedManager == null when calculating");
		}
		if (PromoActionsManager.sharedManager != null && PromoActionsManager.sharedManager.IsEventX3Active)
		{
			return 3 * gemsInappsQuantity[inappIndex];
		}
		return ((Defs.ABTestDeviceNumber != 2) ? 1 : 3) * gemsInappsQuantity[inappIndex];
	}

	private static void AddPrice(string key, int value)
	{
		prices.Add(key, new SaltedInt(_prng.Next(), value));
	}

	private static Dictionary<string, decimal> InitializeReferencePrices()
	{
		Dictionary<string, decimal> dictionary = new Dictionary<string, decimal>();
		dictionary.Add("coin1", 0.99m);
		dictionary.Add("coin7", 2.99m);
		dictionary.Add("coin2", 4.99m);
		dictionary.Add("coin4", 19.99m);
		dictionary.Add("coin5", 49.99m);
		dictionary.Add("coin8", 99.99m);
		dictionary.Add("gem1", 0.99m);
		dictionary.Add("gem2", 2.99m);
		dictionary.Add("gem3", 4.99m);
		dictionary.Add("gem4", 9.99m);
		dictionary.Add("gem5", 19.99m);
		dictionary.Add("gem6", 49.99m);
		dictionary.Add("gem7", 99.99m);
		dictionary.Add("starterpack8", 0.99m);
		dictionary.Add("starterpack7", 0.99m);
		dictionary.Add("starterpack5", 1.99m);
		dictionary.Add("starterpack3", 3.99m);
		dictionary.Add("starterpack1", 5.99m);
		Dictionary<string, decimal> dictionary2 = dictionary;
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android)
		{
			dictionary2.Add("coin3.", 9.99m);
			dictionary2.Add("starterpack6", 0.99m);
			dictionary2.Add("starterpack4", 2.99m);
			dictionary2.Add("starterpack2", 4.99m);
		}
		else if (BuildSettings.BuildTargetPlatform == RuntimePlatform.IPhonePlayer)
		{
			dictionary2.Add("coin3", 9.99m);
			dictionary2.Add("starterpack10", 2.99m);
			dictionary2.Add("starterpack9", 4.99m);
		}
		return dictionary2;
	}
}
