using System;
using Rilisoft;
using UnityEngine;

[Serializable]
public class SlotInfo
{
	public GiftInfo gift;

	public int positionInScroll;

	public float percentGetSlot;

	[HideInInspector]
	public GiftCategory category;

	[HideInInspector]
	public bool isActiveEvent;

	private SaltedInt _countGift = new SaltedInt(15645675, 0);

	[HideInInspector]
	public int numInScroll;

	public int CountGift
	{
		get
		{
			if (isActiveEvent)
			{
				return _countGift.Value;
			}
			return gift.count.Value;
		}
		set
		{
			_countGift.Value = value;
		}
	}

	public bool CheckAvaliableGift()
	{
		if (GiftController.instance != null && (category == null || !GiftController.AvailableGift(gift.IdGift, category.typeCat)))
		{
			GiftController.instance.UpdateSlot(this);
			return true;
		}
		return false;
	}
}
