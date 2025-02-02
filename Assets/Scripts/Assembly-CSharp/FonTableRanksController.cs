using UnityEngine;

public class FonTableRanksController : MonoBehaviour
{
	public bool isTeamTable;

	public GameObject scoreHead;

	public GameObject countKillsHead;

	public GameObject likeHead;

	public int command;

	public string nameCommand;

	public UILabel totalScoreHead;

	public UILabel totalScore;

	public UISprite fon;

	public UISprite line;

	public UILabel headLabel;

	private void Start()
	{
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.FlagCapture)
		{
			float x = countKillsHead.transform.position.x;
			countKillsHead.transform.position = new Vector3(scoreHead.transform.position.x, countKillsHead.transform.position.y, countKillsHead.transform.position.z);
			scoreHead.transform.position = new Vector3(x, scoreHead.transform.position.y, scoreHead.transform.position.z);
			countKillsHead.GetComponent<UILabel>().text = LocalizationStore.Get("Key_1006");
			if (likeHead != null)
			{
				likeHead.SetActive(false);
			}
		}
		if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
		{
			scoreHead.transform.position = new Vector3(countKillsHead.transform.position.x, scoreHead.transform.position.y, scoreHead.transform.position.z);
			countKillsHead.SetActive(false);
			if (likeHead != null)
			{
				likeHead.SetActive(false);
			}
		}
		if (Defs.isDaterRegim)
		{
			scoreHead.gameObject.SetActive(false);
			countKillsHead.gameObject.SetActive(false);
			if (likeHead != null)
			{
				likeHead.SetActive(true);
			}
		}
		else if (likeHead != null)
		{
			likeHead.SetActive(false);
		}
	}

	public void SetCommand(int _command)
	{
		if (isTeamTable)
		{
			if (_command == 0)
			{
				fon.spriteName = "table_team_noteam_small";
				totalScore.color = Color.white;
				totalScoreHead.color = Color.white;
				line.color = Color.gray;
				headLabel.text = LocalizationStore.Get(nameCommand);
			}
			if (_command == 1)
			{
				fon.spriteName = "table_team_blue_small";
				Color color = new Color(0.153f, 0.416f, 0.984f);
				totalScore.color = color;
				totalScoreHead.color = color;
				line.color = new Color(0.494f, 0.788f, 1f);
				headLabel.text = LocalizationStore.Get("Key_1771");
			}
			if (_command == 2)
			{
				fon.spriteName = "table_team_red_small";
				Color red = Color.red;
				totalScore.color = red;
				totalScoreHead.color = red;
				line.color = new Color(1f, 0.494f, 0.494f);
				headLabel.text = LocalizationStore.Get("Key_1772");
			}
		}
	}

	private void Update()
	{
		int num = ((WeaponManager.sharedManager.myNetworkStartTable.myCommand > 0) ? WeaponManager.sharedManager.myNetworkStartTable.myCommand : WeaponManager.sharedManager.myNetworkStartTable.myCommandOld);
		SetCommand((num > 0) ? command : 0);
	}
}
