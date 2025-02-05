using System;
using System.Linq;
using Rilisoft.NullExtensions;
using UnityEngine;

public class EventX3Banner : BannerWindow
{
	public GameObject amazonEventObject;

	public UILabel amazonEventCaptionLabel;

	public UILabel amazonEventTitleLabel;

	private void OnEnable()
	{
		bool isAmazonEventX3Active = PromoActionsManager.sharedManager.IsAmazonEventX3Active;
		amazonEventObject.SetActive(isAmazonEventX3Active);
		PromoActionsManager.EventAmazonX3Updated += OnAmazonEventUpdated;
		RefreshAmazonBonus();
	}

	private void OnDisable()
	{
		PromoActionsManager.EventAmazonX3Updated -= OnAmazonEventUpdated;
	}

	private void RefreshAmazonBonus()
	{
		UILabel[] componentsInChildren = amazonEventObject.GetComponentsInChildren<UILabel>();
		UILabel uILabel = amazonEventCaptionLabel ?? componentsInChildren.FirstOrDefault((UILabel l) => "CaptionLabel".Equals(l.name, StringComparison.OrdinalIgnoreCase));
		if (uILabel != null)
		{
			uILabel.text = PromoActionsManager.sharedManager.Catch((PromoActionsManager p) => p.AmazonEvent.Caption) ?? string.Empty;
		}
		UILabel o = amazonEventTitleLabel ?? componentsInChildren.FirstOrDefault((UILabel l) => "TitleLabel".Equals(l.name, StringComparison.OrdinalIgnoreCase));
		UILabel[] array = o.Map((UILabel t) => t.GetComponentsInChildren<UILabel>()) ?? new UILabel[0];
		float num = PromoActionsManager.sharedManager.Catch((PromoActionsManager p) => p.AmazonEvent.Percentage);
		string text = LocalizationStore.Get("Key_1672");
		UILabel[] array2 = array;
		foreach (UILabel uILabel2 in array2)
		{
			uILabel2.text = ("Key_1672".Equals(text, StringComparison.OrdinalIgnoreCase) ? string.Empty : string.Format(text, num));
		}
	}

	private void OnAmazonEventUpdated()
	{
		amazonEventObject.SetActive(PromoActionsManager.sharedManager.IsAmazonEventX3Active);
		RefreshAmazonBonus();
	}
}
