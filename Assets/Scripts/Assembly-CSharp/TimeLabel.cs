using UnityEngine;

public class TimeLabel : MonoBehaviour
{
	private UILabel _label;

	private Vector3 targetScale = Vector3.one;

	private bool blink;

	private float startTime = 10f;

	private void Start()
	{
		base.gameObject.SetActive(Defs.isMulti);
		_label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!InGameGUI.sharedInGameGUI || !_label)
		{
			return;
		}
		_label.text = InGameGUI.sharedInGameGUI.timeLeft();
		if (!Defs.isHunger && (!Defs.isFlag || WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1 != WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2))
		{
			float num = (float)TimeGameController.sharedController.timerToEndMatch;
			if (num <= startTime)
			{
				float num2 = Mathf.Round(num) - num;
				blink = num2 > 0f;
				_label.transform.localScale = Vector3.MoveTowards(_label.transform.localScale, (!blink) ? Vector3.one : (Vector3.one * Mathf.Min(1.2f + (startTime - num) / 20f, 1.8f)), (!blink) ? (2.4f * Time.deltaTime) : (12f * Time.deltaTime));
				_label.color = ((!blink) ? Color.white : Color.red);
			}
			else
			{
				_label.color = Color.white;
				_label.transform.localScale = Vector3.one;
			}
		}
	}
}
