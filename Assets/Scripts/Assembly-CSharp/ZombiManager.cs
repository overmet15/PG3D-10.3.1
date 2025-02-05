using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ZombiManager : MonoBehaviour
{
	public static ZombiManager sharedManager;

	public double timeGame;

	public float nextTimeSynch;

	public float nextAddZombi;

	public List<string> zombiePrefabs = new List<string>();

	private List<string[]> _enemies = new List<string[]>();

	private GameObject[] _enemyCreationZones;

	public bool startGame;

	public double maxTimeGame = 240.0;

	public PhotonView photonView;

	public bool isPizzaMap;

	private void Awake()
	{
		try
		{
			string[] array = null;
			isPizzaMap = Application.loadedLevelName.Equals("Pizza") || Application.loadedLevelName.Equals("Paradise_Night");
			array = ((!isPizzaMap) ? new string[11]
			{
				"1", "79", "14", "2", "3", "9", "11", "57", "10", "16",
				"56"
			} : new string[9] { "85", "86", "88", "84", "87", "82", "81", "80", "83" });
			string[] array2 = array;
			foreach (string text in array2)
			{
				string item = "Enemies/Enemy" + text + "_go";
				zombiePrefabs.Add(item);
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Cooperative mode failure.");
			Debug.LogException(exception);
			throw;
		}
	}

	private void Start()
	{
		if (!Defs.isMulti || !Defs.isCOOP)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		sharedManager = this;
		try
		{
			nextAddZombi = 5f;
			_enemyCreationZones = GameObject.FindGameObjectsWithTag("EnemyCreationZone");
			photonView = PhotonView.Get(this);
		}
		catch (Exception exception)
		{
			Debug.LogError("Cooperative mode failure.");
			Debug.LogException(exception);
			throw;
		}
	}

	[RPC]
	[PunRPC]
	private void synchTime(float _time)
	{
	}

	public void EndMatch()
	{
		if (!photonView.isMine || TimeGameController.sharedController.isEndMatch)
		{
			return;
		}
		TimeGameController.sharedController.isEndMatch = true;
		startGame = false;
		timeGame = 0.0;
		GameObject[] array = GameObject.FindGameObjectsWithTag("NetworkTable");
		float num = -100f;
		string text = string.Empty;
		int num2 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			if ((float)array[i].GetComponent<NetworkStartTable>().score > num)
			{
				num = array[i].GetComponent<NetworkStartTable>().score;
				text = array[i].GetComponent<NetworkStartTable>().NamePlayer;
				num2 = i;
			}
		}
		photonView.RPC("win", PhotonTargets.All, text);
		photonView.RPC("WinID", PhotonTargets.All, array[num2].GetComponent<PhotonView>().ownerId);
	}

	private void Update()
	{
		try
		{
			int count = Initializer.players.Count;
			if (!startGame && count > 0)
			{
				startGame = true;
				timeGame = 0.0;
				nextTimeSynch = 0f;
				nextAddZombi = 0f;
			}
			if (startGame && count == 0)
			{
				startGame = false;
				timeGame = 0.0;
				nextTimeSynch = 0f;
				nextAddZombi = 0f;
			}
			if (!startGame)
			{
				return;
			}
			timeGame = maxTimeGame - TimeGameController.sharedController.timerToEndMatch;
			if (timeGame > (double)nextAddZombi && photonView.isMine && Initializer.enemiesObj.Count < 15)
			{
				float num = 4f;
				if (timeGame > maxTimeGame * 0.4000000059604645)
				{
					num = 3f;
				}
				if (timeGame > maxTimeGame * 0.800000011920929)
				{
					num = 2f;
				}
				nextAddZombi += num;
				addZombi();
			}
		}
		catch (Exception exception)
		{
			Debug.LogError("Cooperative mode failure.");
			Debug.LogException(exception);
			throw;
		}
	}

	[PunRPC]
	[RPC]
	private void win(string _winer)
	{
		if (WeaponManager.sharedManager.myPlayerMoveC != null)
		{
			WeaponManager.sharedManager.myNetworkStartTable.win(_winer);
		}
	}

	private void addZombi()
	{
		GameObject gameObject = _enemyCreationZones[UnityEngine.Random.Range(0, _enemyCreationZones.Length)];
		BoxCollider component = gameObject.GetComponent<BoxCollider>();
		Vector2 vector = new Vector2(component.size.x * gameObject.transform.localScale.x, component.size.z * gameObject.transform.localScale.z);
		Rect rect = new Rect(gameObject.transform.position.x - vector.x / 2f, gameObject.transform.position.z - vector.y / 2f, vector.x, vector.y);
		Vector3 position = new Vector3(rect.x + UnityEngine.Random.Range(0f, rect.width), gameObject.transform.position.y, rect.y + UnityEngine.Random.Range(0f, rect.height));
		int index = 0;
		double num = timeGame / maxTimeGame * 100.0;
		if (isPizzaMap)
		{
			if (num < 15.0)
			{
				index = UnityEngine.Random.Range(0, 4);
			}
			if (num >= 15.0 && num < 30.0)
			{
				index = UnityEngine.Random.Range(0, 5);
			}
			if (num >= 30.0 && num < 45.0)
			{
				index = UnityEngine.Random.Range(1, 6);
			}
			if (num >= 45.0 && num < 60.0)
			{
				index = UnityEngine.Random.Range(2, 7);
			}
			if (num >= 60.0 && num < 75.0)
			{
				index = UnityEngine.Random.Range(3, 9);
			}
			if (num >= 75.0)
			{
				index = UnityEngine.Random.Range(4, 9);
			}
		}
		else
		{
			if (num < 15.0)
			{
				index = UnityEngine.Random.Range(0, 3);
			}
			if (num >= 15.0 && num < 30.0)
			{
				index = UnityEngine.Random.Range(0, 5);
			}
			if (num >= 30.0 && num < 45.0)
			{
				index = UnityEngine.Random.Range(0, 6);
			}
			if (num >= 45.0 && num < 60.0)
			{
				index = UnityEngine.Random.Range(3, 8);
			}
			if (num >= 60.0 && num < 75.0)
			{
				index = UnityEngine.Random.Range(5, 9);
			}
			if (num >= 75.0)
			{
				index = UnityEngine.Random.Range(5, 11);
			}
		}
		PhotonNetwork.InstantiateSceneObject(zombiePrefabs[index], position, Quaternion.identity, 0, null);
	}

	[PunRPC]
	[RPC]
	private void WinID(int winID)
	{
		WeaponManager weaponManager = WeaponManager.sharedManager;
		if (weaponManager.myTable != null && weaponManager.myTable.GetComponent<PhotonView>().ownerId == winID)
		{
			int val = Storager.getInt(Defs.RatingCOOP, false) + 1;
			Storager.setInt(Defs.RatingCOOP, val, false);
			val = Storager.getInt("Rating", false) + 1;
			Storager.setInt("Rating", val, false);
			if (FriendsController.sharedController != null)
			{
				FriendsController.sharedController.TryIncrementWinCountTimestamp();
			}
			FriendsController.sharedController.wins.Value = val;
			FriendsController.sharedController.SendOurData();
		}
	}

	private void OnDestroy()
	{
		sharedManager = null;
	}
}
