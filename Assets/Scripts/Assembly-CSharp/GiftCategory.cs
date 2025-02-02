using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GiftCategory
{
	public TypeGiftCategory typeCat;

	public int positionInScroll;

	public List<GiftInfo> listGifts = new List<GiftInfo>();

	public float sumPerAllGifts = -1f;

	public string keyTranslateInfoCommon = string.Empty;

	private readonly List<GiftInfo> listAvalibalGift = new List<GiftInfo>();

	private float sumPerAvalibalGifts = -1f;

	public int CountAvaliableGifts
	{
		get
		{
			if (listAvalibalGift == null)
			{
				return 0;
			}
			return listAvalibalGift.Count;
		}
	}

	public void CheckGifts()
	{
		GetSumPercent();
		listAvalibalGift.Clear();
		for (int i = 0; i < listGifts.Count; i++)
		{
			GiftInfo giftInfo = listGifts[i];
			if (typeCat == TypeGiftCategory.Armor)
			{
				giftInfo.IdGift = GiftController.GetIdArmorOrHat();
			}
			if (typeCat == TypeGiftCategory.Skins)
			{
				if (giftInfo.IdGift.ToLower().Equals("all"))
				{
					giftInfo.isRandomSkin = true;
				}
				if (giftInfo.isRandomSkin)
				{
					giftInfo.IdGift = SkinsController.RandomUnboughtSkinId();
				}
			}
			if (GiftController.AvailableGift(giftInfo.IdGift, typeCat))
			{
				listAvalibalGift.Add(giftInfo);
			}
		}
		GetSumAvailableGift();
	}

	private void GetSumPercent()
	{
		if (listGifts != null)
		{
			sumPerAllGifts = 0f;
			for (int i = 0; i < listGifts.Count; i++)
			{
				sumPerAllGifts += listGifts[i].percentAddInSlot;
			}
		}
	}

	private void GetSumAvailableGift()
	{
		if (listAvalibalGift != null)
		{
			sumPerAvalibalGifts = 0f;
			for (int i = 0; i < listAvalibalGift.Count; i++)
			{
				sumPerAvalibalGifts += listAvalibalGift[i].percentAddInSlot;
			}
		}
	}

	public GiftInfo GetRandomGift()
	{
		if (listAvalibalGift == null || listAvalibalGift.Count == 0)
		{
			return null;
		}
		if (sumPerAvalibalGifts < 0f)
		{
			GetSumAvailableGift();
		}
		if (sumPerAvalibalGifts < 0f)
		{
			return null;
		}
		float num = UnityEngine.Random.Range(0f, sumPerAvalibalGifts);
		float num2 = 0f;
		GiftInfo result = null;
		for (int i = 0; i < listAvalibalGift.Count; i++)
		{
			GiftInfo giftInfo = listAvalibalGift[i];
			num2 += giftInfo.percentAddInSlot;
			if (num2 > num)
			{
				result = giftInfo;
				break;
			}
		}
		return result;
	}
}
