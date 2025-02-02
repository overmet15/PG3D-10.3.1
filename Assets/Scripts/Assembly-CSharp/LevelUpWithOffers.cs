using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Rilisoft;
using UnityEngine;

public class LevelUpWithOffers : MonoBehaviour
{
	public struct ItemDesc
	{
		public string tag;

		public ShopNGUIController.CategoryNames category;
	}

	public RewardWindowBase shareScript;

	public UILabel[] rewardGemsPriceLabel;

	public UILabel[] currentRankLabel;

	public UILabel[] rewardPriceLabel;

	public UILabel[] healthLabel;

	public List<UILabel> youReachedLabels;

	public NewAvailableItemInShop[] items;

	public bool isTierLevelUp;

	private IEnumerator UpdatePanelsAndAnchors()
	{
		yield return new WaitForEndOfFrame();
		Player_move_c.PerformActionRecurs(base.transform.parent.parent.parent.gameObject, delegate(Transform t)
		{
			UIPanel component2 = t.GetComponent<UIPanel>();
			if (component2 != null)
			{
				component2.Refresh();
			}
		});
		Player_move_c.PerformActionRecurs(base.transform.parent.parent.parent.gameObject, delegate(Transform t)
		{
			UIRect component = t.GetComponent<UIRect>();
			if (component != null)
			{
				component.UpdateAnchors();
			}
		});
	}

	[ContextMenu("Update")]
	public void OnEnable()
	{
		StartCoroutine(UpdatePanelsAndAnchors());
	}

	private void OnDisable()
	{
		ShowIndicationMoney();
	}

	private void OnDestroy()
	{
		ShowIndicationMoney();
	}

	private void ShowIndicationMoney()
	{
		BankController.canShowIndication = true;
		BankController.UpdateAllIndicatorsMoney();
	}

	public void SetCurrentRank(string currentRank)
	{
		for (int i = 0; i < currentRankLabel.Length; i++)
		{
			currentRankLabel[i].text = LocalizationStore.Get("Key_0226").ToUpper() + " " + currentRank + "!";
		}
		string text = string.Empty;
		switch (ProfileController.CurOrderCup)
		{
		case 0:
			text = ScriptLocalization.Get("Key_1938");
			break;
		case 1:
			text = ScriptLocalization.Get("Key_1939");
			break;
		case 2:
			text = ScriptLocalization.Get("Key_1940");
			break;
		case 3:
			text = ScriptLocalization.Get("Key_1941");
			break;
		case 4:
			text = ScriptLocalization.Get("Key_1942");
			break;
		case 5:
			text = ScriptLocalization.Get("Key_1943");
			break;
		}
		foreach (UILabel youReachedLabel in youReachedLabels)
		{
			youReachedLabel.text = text;
		}
	}

	public void SetRewardPrice(string rewardPrice)
	{
		for (int i = 0; i < rewardPriceLabel.Length; i++)
		{
			rewardPriceLabel[i].text = rewardPrice;
		}
	}

	public void SetGemsRewardPrice(string gemsReward)
	{
		for (int i = 0; i < rewardGemsPriceLabel.Length; i++)
		{
			rewardGemsPriceLabel[i].text = gemsReward;
		}
	}

	public void SetAddHealthCount(string count)
	{
		if (healthLabel != null)
		{
			for (int i = 0; i < healthLabel.Length; i++)
			{
				healthLabel[i].text = count;
			}
		}
	}

	public void SetItems(List<string> itemTags)
	{
		if (items == null || items.Length == 0)
		{
			return;
		}
		for (int i = 0; i < items.Length; i++)
		{
			items[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < itemTags.Count; j++)
		{
			items[j].gameObject.SetActive(true);
			string text = itemTags[j];
			int itemCategory = ItemDb.GetItemCategory(text);
			items[j]._tag = text;
			items[j].category = (ShopNGUIController.CategoryNames)itemCategory;
			items[j].itemImage.mainTexture = ItemDb.GetItemIcon(text, (ShopNGUIController.CategoryNames)itemCategory, !isTierLevelUp);
			foreach (UILabel item in items[j].itemName)
			{
				item.text = ItemDb.GetItemName(text, (ShopNGUIController.CategoryNames)itemCategory);
			}
			items[j].GetComponent<UIButton>().isEnabled = !Defs.isHunger || text == null || ItemDb.GetByTag(text) == null;
		}
	}
}
