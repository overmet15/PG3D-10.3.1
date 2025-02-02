using UnityEngine;

public class HeartEffect : MonoBehaviour
{
	public enum IndicatorType
	{
		Hearts = 0,
		Armor = 1,
		Mech = 2
	}

	private UISprite mySprite;

	private bool activeEffect;

	private bool activeHeart;

	private Vector3 targetScale;

	private string spriteName;

	public int heartIndex;

	private readonly string[] mechShieldsSpriteName = new string[6] { "mech_armor1", "mech_armor2", "mech_armor3", "mech_armor4", "mech_armor5", "mech_armor6" };

	private readonly string[] armSpriteName = new string[7] { "wood_armor", "armor", "gold_armor", "crystal_armor", "red_armor", "adamant_armor", "adamant_armor" };

	public void Animate(int index, IndicatorType type)
	{
		heartIndex = index;
		if (heartIndex < 1)
		{
			heartIndex = 0;
			ShowHide(false);
			return;
		}
		if (heartIndex == 1)
		{
			ShowHide(true);
		}
		switch (type)
		{
		case IndicatorType.Hearts:
			spriteName = "heart" + heartIndex;
			break;
		case IndicatorType.Armor:
			spriteName = armSpriteName[heartIndex - 1];
			break;
		case IndicatorType.Mech:
			spriteName = mechShieldsSpriteName[heartIndex - 1];
			break;
		}
		ChangeSpriteEffect(spriteName);
	}

	public void SetIndex(int index, IndicatorType type)
	{
		heartIndex = index;
		activeEffect = false;
		targetScale = Vector3.one;
		base.transform.localScale = Vector3.one;
		if (heartIndex < 1)
		{
			heartIndex = 0;
			base.gameObject.SetActive(false);
			activeHeart = false;
			switch (type)
			{
			case IndicatorType.Hearts:
				mySprite.spriteName = "heart1";
				break;
			case IndicatorType.Armor:
				mySprite.spriteName = armSpriteName[0];
				break;
			case IndicatorType.Mech:
				mySprite.spriteName = mechShieldsSpriteName[0];
				break;
			}
		}
		else
		{
			activeHeart = true;
			base.gameObject.SetActive(true);
			switch (type)
			{
			case IndicatorType.Hearts:
				mySprite.spriteName = "heart" + heartIndex;
				break;
			case IndicatorType.Armor:
				mySprite.spriteName = armSpriteName[heartIndex - 1];
				break;
			case IndicatorType.Mech:
				mySprite.spriteName = mechShieldsSpriteName[heartIndex - 1];
				break;
			}
		}
	}

	private void ShowHide(bool show)
	{
		activeHeart = show;
		activeEffect = true;
		if (show)
		{
			base.gameObject.SetActive(true);
			targetScale = Vector3.one;
		}
		else
		{
			targetScale = Vector3.one * 0.001f;
		}
	}

	private void ChangeSpriteEffect(string newSprite)
	{
		spriteName = newSprite;
		activeEffect = true;
		activeHeart = true;
		targetScale = Vector3.one * 1.7f;
		base.gameObject.SetActive(true);
	}

	private void Awake()
	{
		mySprite = GetComponent<UISprite>();
	}

	private void Update()
	{
		if (!activeEffect)
		{
			return;
		}
		base.transform.localScale = Vector3.MoveTowards(base.transform.localScale, targetScale, 7f * Time.deltaTime);
		if (base.transform.localScale == targetScale)
		{
			if (!activeHeart || base.transform.localScale == Vector3.one)
			{
				activeEffect = false;
				base.gameObject.SetActive(activeHeart);
			}
			else
			{
				mySprite.spriteName = spriteName;
				targetScale = Vector3.one;
			}
		}
	}
}
