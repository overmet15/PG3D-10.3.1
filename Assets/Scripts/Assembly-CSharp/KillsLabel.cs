using UnityEngine;

public class KillsLabel : MonoBehaviour
{
	private UILabel _label;

	private InGameGUI _inGameGUI;

	private void Start()
	{
		base.gameObject.SetActive((Defs.isMulti && ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.Deathmatch) || Defs.isDaterRegim);
		_label = GetComponent<UILabel>();
		_inGameGUI = InGameGUI.sharedInGameGUI;
	}

	private void Update()
	{
		if ((bool)_inGameGUI && (bool)_label)
		{
			if (Defs.isDaterRegim)
			{
				_label.text = GlobalGameController.CountKills.ToString();
			}
			else if (_inGameGUI != null)
			{
				_label.text = _inGameGUI.killsToMaxKills();
			}
		}
	}
}
