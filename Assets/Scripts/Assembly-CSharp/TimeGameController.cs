using System;
using System.Reflection;
using ExitGames.Client.Photon;
using Rilisoft;
using UnityEngine;

public class TimeGameController : MonoBehaviour
{
	public static TimeGameController sharedController;

	public double timeEndMatch;

	public double timerToEndMatch;

	public double networkTime;

	public PhotonView photonView;

	public double timeLocalServer;

	public string ipServera;

	private long pauseTime;

	private bool paused;

	public bool isEndMatch = true;

	private bool matchEnding;

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!PhotonNetwork.connected)
		{
			return;
		}
		if (pauseStatus)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				paused = true;
			}
			else
			{
				PhotonNetwork.Disconnect();
			}
			return;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			CheckPause();
		}
		PhotonNetwork.FetchServerTimestamp();
	}

	private void Awake()
	{
		if (!Defs.isMulti || Defs.isHunger)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		sharedController = this;
		if (Defs.isMulti && !Defs.isInet && Network.isServer)
		{
			InvokeRepeating("SinchServerTimeInvoke", 0.1f, 2f);
			Debug.Log("TimeGameController: Start synch server time");
		}
	}

	[Obfuscation(Exclude = true)]
	public void SinchServerTimeInvoke()
	{
		GetComponent<NetworkView>().RPC("SynchTimeServer", RPCMode.Others, (float)Network.time);
	}

	public void StartMatch()
	{
		bool flag = false;
		matchEnding = false;
		if (Defs.isCapturePoints || Defs.isFlag)
		{
			double num = Convert.ToDouble(PhotonNetwork.room.customProperties["TimeMatchEnd"]);
			if (num < -5000000.0)
			{
				flag = true;
			}
		}
		if (Defs.isInet && ((timeEndMatch < PhotonNetwork.time && !Defs.isFlag) || Initializer.players.Count == 0 || (Defs.isFlag && flag)))
		{
			Hashtable hashtable = new Hashtable();
			double num2 = PhotonNetwork.time + (double)(((!Defs.isCOOP) ? ((int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.maxKillProperty]) : 4) * 60);
			if (num2 > 4294967.0 && PhotonNetwork.time < 4294967.0)
			{
				num2 = 4294967.0;
			}
			hashtable["TimeMatchEnd"] = num2;
			hashtable[ConnectSceneNGUIController.endingProperty] = 0;
			PhotonNetwork.room.SetCustomProperties(hashtable);
			timerToEndMatch = num2 - PhotonNetwork.time;
		}
		if (!Defs.isInet && (timeEndMatch < networkTime || Initializer.players.Count == 0))
		{
			timeEndMatch = networkTime + (double)((PlayerPrefs.GetString("MaxKill", "9").Equals(string.Empty) ? 5 : int.Parse(PlayerPrefs.GetString("MaxKill", "5"))) * 60);
			GetComponent<NetworkView>().RPC("SynchTimeEnd", RPCMode.Others, (float)timeEndMatch);
		}
	}

	private void CheckPause()
	{
		paused = false;
		long currentUnixTime = Tools.CurrentUnixTime;
		if (pauseTime > currentUnixTime || pauseTime + 60 < currentUnixTime)
		{
			PhotonNetwork.Disconnect();
		}
	}

	private void Update()
	{
		if (paused && Defs.isInet && Application.platform == RuntimePlatform.IPhonePlayer)
		{
			CheckPause();
			if (!PhotonNetwork.connected)
			{
				return;
			}
		}
		ipServera = PhotonNetwork.ServerAddress;
		if (Defs.isInet && PhotonNetwork.room != null && PhotonNetwork.room.customProperties["TimeMatchEnd"] != null)
		{
			networkTime = PhotonNetwork.time;
			if (networkTime < 0.1)
			{
				return;
			}
			timeEndMatch = Convert.ToDouble(PhotonNetwork.room.customProperties["TimeMatchEnd"]);
			if (WeaponManager.sharedManager != null && WeaponManager.sharedManager.myPlayerMoveC != null && timeEndMatch > PhotonNetwork.time + 1500.0)
			{
				Initializer.Instance.goToConnect();
			}
			if (!matchEnding && timeEndMatch < PhotonNetwork.time + (double)((!PhotonNetwork.isMasterClient) ? 90 : 120) && (!PhotonNetwork.room.customProperties.ContainsKey(ConnectSceneNGUIController.endingProperty) || (int)PhotonNetwork.room.customProperties[ConnectSceneNGUIController.endingProperty] == 0))
			{
				matchEnding = true;
				Hashtable hashtable = new Hashtable();
				hashtable[ConnectSceneNGUIController.endingProperty] = 1;
				PhotonNetwork.room.SetCustomProperties(hashtable);
			}
			if (timeEndMatch > 4290000.0 && networkTime < 2000000.0)
			{
				Hashtable hashtable2 = new Hashtable();
				double num = networkTime + 60.0;
				hashtable2["TimeMatchEnd"] = num;
				PhotonNetwork.room.SetCustomProperties(hashtable2);
			}
			if (timeEndMatch > 0.0)
			{
				timerToEndMatch = timeEndMatch - networkTime;
			}
			else
			{
				timerToEndMatch = -1.0;
			}
		}
		if (!Defs.isInet)
		{
			if (Network.isServer)
			{
				networkTime = Network.time;
			}
			else
			{
				networkTime += Time.deltaTime;
			}
			timerToEndMatch = timeEndMatch - networkTime;
		}
		if (timerToEndMatch < 0.0 && !Defs.isFlag)
		{
			if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.CapturePoints)
			{
				if (CapturePointController.sharedController != null && !isEndMatch)
				{
					CapturePointController.sharedController.EndMatch();
					isEndMatch = true;
				}
			}
			else if (ConnectSceneNGUIController.regim == ConnectSceneNGUIController.RegimGame.TimeBattle)
			{
				if (!isEndMatch)
				{
					ZombiManager.sharedManager.EndMatch();
				}
			}
			else if (WeaponManager.sharedManager.myPlayer != null)
			{
				WeaponManager.sharedManager.myPlayerMoveC.WinFromTimer();
			}
			else
			{
				GlobalGameController.countKillsRed = 0;
				GlobalGameController.countKillsBlue = 0;
			}
		}
		else
		{
			isEndMatch = false;
		}
		pauseTime = Tools.CurrentUnixTime;
	}

	private void OnPlayerConnected(NetworkPlayer player)
	{
		if (Network.isServer)
		{
			GetComponent<NetworkView>().RPC("SynchTimeEnd", RPCMode.Others, (float)timeEndMatch);
			GetComponent<NetworkView>().RPC("SynchTimeServer", RPCMode.Others, (float)Network.time);
		}
	}

	[RPC]
	[PunRPC]
	private void SynchTimeEnd(float synchTime)
	{
		timeEndMatch = synchTime;
	}

	[PunRPC]
	[RPC]
	private void SynchTimeServer(float synchTime)
	{
		if (networkTime < (double)synchTime)
		{
			networkTime = synchTime;
		}
	}

	private void OnDestroy()
	{
		sharedController = null;
	}
}
