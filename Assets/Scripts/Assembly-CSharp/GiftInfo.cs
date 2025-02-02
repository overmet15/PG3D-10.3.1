using System;
using Rilisoft;
using UnityEngine;

[Serializable]
public class GiftInfo
{
	public string IdGift;

	public SaltedInt count = new SaltedInt(12499947, 0);

	public float percentAddInSlot;

	public string keyTranslateInfo = string.Empty;

	[HideInInspector]
	public bool isRandomSkin;

	public ShopNGUIController.CategoryNames typeShopCat;

	public void UpdateType()
	{
		if (IdGift != null)
		{
			int itemCategory = ItemDb.GetItemCategory(IdGift);
			if (itemCategory != -1)
			{
				ShopNGUIController.CategoryNames categoryNames = (ShopNGUIController.CategoryNames)itemCategory;
				typeShopCat = categoryNames;
			}
		}
	}
}
