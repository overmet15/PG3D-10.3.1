using UnityEngine;

public class CountKillsCommandBlue : MonoBehaviour
{
	public static float localScaleForLabels = 1.25f;

	private UILabel _label;

	public bool isEnemyCommandLabel;

	private WeaponManager _weaponManager;

	public GameObject myBackground;

	private void Start()
	{
		_weaponManager = WeaponManager.sharedManager;
		InGameGUI sharedInGameGUI = InGameGUI.sharedInGameGUI;
		_label = GetComponent<UILabel>();
	}

	private void Update()
	{
		if (!_weaponManager || !_weaponManager.myPlayer)
		{
			return;
		}
		if (Defs.isFlag)
		{
			if (WeaponManager.sharedManager.myTable != null)
			{
				if (isEnemyCommandLabel == (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2))
				{
					_label.text = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag1.ToString();
				}
				else
				{
					_label.text = WeaponManager.sharedManager.myNetworkStartTable.scoreCommandFlag2.ToString();
				}
			}
		}
		else if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
		{
			if (isEnemyCommandLabel == (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2))
			{
				_label.text = Mathf.RoundToInt(CapturePointController.sharedController.scoreBlue).ToString();
			}
			else
			{
				_label.text = Mathf.RoundToInt(CapturePointController.sharedController.scoreRed).ToString();
			}
		}
		else if (isEnemyCommandLabel == (WeaponManager.sharedManager.myNetworkStartTable.myCommand == 2))
		{
			_label.text = _weaponManager.myPlayerMoveC.countKillsCommandBlue.ToString();
		}
		else
		{
			_label.text = _weaponManager.myPlayerMoveC.countKillsCommandRed.ToString();
		}
	}
}
