using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Rilisoft;
using Rilisoft.MiniJson;
using UnityEngine;

public sealed class PromoActionsManager : MonoBehaviour
{
	public class AdvertInfo
	{
		public bool enabled;

		public string imageUrl;

		public string adUrl;

		public string message;

		public bool showAlways;

		public bool btnClose;

		public int minLevel;

		public int maxLevel;
	}

	public class MobileAdvertInfo
	{
		internal const int DefaultAwardCoins = 1;

		private int _awardCoinsPayning = 1;

		private int _awardCoinsNonpayning = 1;

		private List<string> _admobImageAdUnitIds = new List<string>();

		private List<string> _admobVideoAdUnitIds = new List<string>();

		private List<int> _adProviders = new List<int>();

		private double _daysOfBeingPayingUser = double.MaxValue;

		private List<int> _interstitialProviders = new List<int>();

		private List<List<string>> _admobImageIdGroups = new List<List<string>>();

		private List<List<string>> _admobVideoIdGroups = new List<List<string>>();

		private List<double> _rewardedVideoDelayMinutesNonpaying = new List<double>();

		private List<double> _rewardedVideoDelayMinutesPaying = new List<double>();

		[Obsolete]
		public bool ImageEnabled { get; set; }

		public bool VideoEnabled { get; set; }

		public bool VideoShowPaying { get; set; }

		public bool VideoShowNonpaying { get; set; }

		public int CountVideoShowNonpaying
		{
			get
			{
				return _rewardedVideoDelayMinutesNonpaying.Count;
			}
		}

		public int CountVideoShowPaying
		{
			get
			{
				return _rewardedVideoDelayMinutesPaying.Count;
			}
		}

		public int TimeoutBetweenShowInterstitial { get; set; }

		public int TimeoutWaitVideo { get; set; }

		public int CountSessionNewPlayer { get; set; }

		public int CountRoundReplaceProviders { get; set; }

		public int TimeoutSkipVideoPaying { get; set; }

		public int TimeoutSkipVideoNonpaying { get; set; }

		public double ConnectSceneDelaySeconds { get; set; }

		public double DaysOfBeingPayingUser
		{
			get
			{
				return _daysOfBeingPayingUser;
			}
			set
			{
				_daysOfBeingPayingUser = Math.Max(0.0, value);
			}
		}

		public string AdmobVideoAdUnitId { get; set; }

		public List<string> AdmobImageAdUnitIds
		{
			get
			{
				return _admobImageAdUnitIds;
			}
			set
			{
				_admobImageAdUnitIds = value ?? new List<string>();
			}
		}

		public List<string> AdmobVideoAdUnitIds
		{
			get
			{
				return _admobVideoAdUnitIds;
			}
			set
			{
				_admobVideoAdUnitIds = value ?? new List<string>();
			}
		}

		public List<List<string>> AdmobImageIdGroups
		{
			get
			{
				return _admobImageIdGroups;
			}
			set
			{
				_admobImageIdGroups = value ?? new List<List<string>>();
			}
		}

		public List<List<string>> AdmobVideoIdGroups
		{
			get
			{
				return _admobVideoIdGroups;
			}
			set
			{
				_admobVideoIdGroups = value ?? new List<List<string>>();
			}
		}

		public int AdProvider { get; set; }

		public int AwardCoinsPaying
		{
			get
			{
				return _awardCoinsPayning;
			}
			set
			{
				_awardCoinsPayning = Math.Max(1, value);
			}
		}

		public int AwardCoinsNonpaying
		{
			get
			{
				return _awardCoinsNonpayning;
			}
			set
			{
				_awardCoinsNonpayning = Math.Max(1, value);
			}
		}

		public List<int> AdProviders
		{
			get
			{
				return _adProviders;
			}
			set
			{
				_adProviders = value ?? new List<int>();
			}
		}

		public List<int> InterstitialProviders
		{
			get
			{
				return _interstitialProviders;
			}
			set
			{
				_interstitialProviders = value ?? new List<int>();
			}
		}

		public List<double> RewardedVideoDelayMinutesNonpaying
		{
			get
			{
				return _rewardedVideoDelayMinutesNonpaying;
			}
			set
			{
				_rewardedVideoDelayMinutesNonpaying = value ?? new List<double>();
			}
		}

		public List<double> RewardedVideoDelayMinutesPaying
		{
			get
			{
				return _rewardedVideoDelayMinutesPaying;
			}
			set
			{
				_rewardedVideoDelayMinutesPaying = value ?? new List<double>();
			}
		}

		public bool ShowInterstitialAfterMatchLoser { get; set; }

		public bool ShowInterstitialAfterMatchWinner { get; set; }

		public float TimeoutWaitingInterstitialAfterMatchSeconds { get; set; }

		public double MinMatchDurationMinutes { get; set; }
	}

	public class ReplaceAdmobPerelivInfo
	{
		public bool enabled;

		public List<string> imageUrls = new List<string>();

		public List<string> adUrls = new List<string>();

		public int ShowEveryTimes { get; set; }

		public int ShowTimesTotal { get; set; }

		public bool ShowToPaying { get; set; }

		public bool ShowToNew { get; set; }

		public int MinLevel { get; set; }

		public int MaxLevel { get; set; }
	}

	internal sealed class AmazonEventInfo
	{
		public DateTime StartTime { get; set; }

		public DateTime EndTime
		{
			get
			{
				return StartTime + TimeSpan.FromSeconds(DurationSeconds);
			}
		}

		public float DurationSeconds { get; set; }

		public List<int> Timezones { get; set; }

		public float Percentage { get; set; }

		public string Caption { get; set; }

		public AmazonEventInfo()
		{
			StartTime = DateTime.MaxValue;
			Timezones = new List<int>();
			Caption = string.Empty;
		}
	}

	public delegate void OnDayOfValorEnableDelegate(bool enable);

	private const float OffersUpdateTimeout = 900f;

	private const float EventX3UpdateTimeout = 930f;

	private const float AdvertInfoTimeout = 960f;

	public const long NewbieEventX3Duration = 259200L;

	public const long NewbieEventX3Timeout = 259200L;

	private const float BestBuyInfoTimeout = 1020f;

	public const int ShownCountDaysOfValor = 1;

	private const float DayOfValorInfoTimeout = 1050f;

	public static PromoActionsManager sharedManager;

	public static bool ActionsAvailable = true;

	public Dictionary<string, List<int>> discounts = new Dictionary<string, List<int>>();

	public List<string> topSellers = new List<string>();

	public List<string> news = new List<string>();

	private float startTime;

	private string promoActionAddress = URLs.PromoActionsTest;

	private float _eventX3GetInfoStartTime;

	private float _eventX3LastCheckTime;

	private long _newbieEventX3StartTime;

	private long _newbieEventX3StartTimeAdditional;

	private long _eventX3StartTime;

	private long _eventX3Duration;

	private bool _eventX3Active;

	private long _eventX3AmazonEventStartTime;

	private long _eventX3AmazonEventEndTime;

	private List<long> _eventX3AmazonEventValidTimeZone = new List<long>();

	private bool _eventX3AmazonEventActive;

	private float _advertGetInfoStartTime;

	private static AdvertInfo _paidAdvert = new AdvertInfo();

	private static AdvertInfo _freeAdvert = new AdvertInfo();

	private static ReplaceAdmobPerelivInfo _replaceAdmobPereliv = new ReplaceAdmobPerelivInfo();

	private static MobileAdvertInfo _mobileAdvert = new MobileAdvertInfo();

	public static float startupTime = 0f;

	private bool _isGetEventX3InfoRunning;

	private AmazonEventInfo _amazonEventInfo;

	public static bool x3InfoDownloadaedOnceDuringCurrentRun = false;

	private bool _isGetAdvertInfoRunning;

	private string _previousResponseText;

	private List<string> _bestBuyIds = new List<string>();

	private bool _isGetBestBuyInfoRunning;

	private float _bestBuyGetInfoStartTime;

	private long _dayOfValorStartTime;

	private long _dayOfValorEndTime;

	private long _dayOfValorMultiplyerForExp;

	private long _dayOfValorMultiplyerForMoney;

	private bool _isGetDayOfValorInfoRunning;

	private float _dayOfValorGetInfoStartTime;

	private static TimeSpan TimeToShowDaysOfValor = TimeSpan.FromHours(12.0);

	private TimeSpan _timeToEndDayOfValor;

	public bool IsEventX3Active
	{
		get
		{
			return Defs.ABTestDeviceNumber != 2 && _eventX3Active;
		}
	}

	public bool IsAmazonEventX3Active
	{
		get
		{
			if (_amazonEventInfo == null)
			{
				return false;
			}
			if (_amazonEventInfo.DurationSeconds <= float.Epsilon)
			{
				return false;
			}
			if (!CheckTimezone(_amazonEventInfo.Timezones))
			{
				return false;
			}
			DateTime utcNow = DateTime.UtcNow;
			return _amazonEventInfo.StartTime <= utcNow && utcNow <= _amazonEventInfo.EndTime;
		}
	}

	public long EventX3RemainedTime
	{
		get
		{
			if (IsEventX3Active)
			{
				return _eventX3StartTime + _eventX3Duration - CurrentUnixTime;
			}
			return 0L;
		}
	}

	public static AdvertInfo Advert
	{
		get
		{
			return (!FlurryPluginWrapper.IsPayingUser()) ? _freeAdvert : _paidAdvert;
		}
	}

	public static ReplaceAdmobPerelivInfo ReplaceAdmobPereliv
	{
		get
		{
			return _replaceAdmobPereliv;
		}
	}

	public static MobileAdvertInfo MobileAdvert
	{
		get
		{
			return _mobileAdvert;
		}
	}

	internal AmazonEventInfo AmazonEvent
	{
		get
		{
			return _amazonEventInfo;
		}
	}

	public bool IsNewbieEventX3Active
	{
		get
		{
			if (_newbieEventX3StartTime == 0L)
			{
				return false;
			}
			long currentUnixTime = CurrentUnixTime;
			long num = _newbieEventX3StartTime + 259200 + 259200;
			if (currentUnixTime >= num)
			{
				ResetNewbieX3StartTime();
				return false;
			}
			return _newbieEventX3StartTime < currentUnixTime && currentUnixTime < _newbieEventX3StartTime + 259200;
		}
	}

	private bool IsX3StartTimeAfterNewbieX3TimeoutEndTime
	{
		get
		{
			if (_newbieEventX3StartTimeAdditional == 0L)
			{
				return true;
			}
			long num = _newbieEventX3StartTimeAdditional + 259200 + 259200;
			return _eventX3StartTime >= num;
		}
	}

	public static long CurrentUnixTime
	{
		get
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return (long)(DateTime.UtcNow - dateTime).TotalSeconds;
		}
	}

	public bool IsDayOfValorEventActive { get; private set; }

	public int DayOfValorMultiplyerForExp
	{
		get
		{
			if (!IsDayOfValorEventActive)
			{
				return 1;
			}
			return (int)_dayOfValorMultiplyerForExp;
		}
	}

	public int DayOfValorMultiplyerForMoney
	{
		get
		{
			if (!IsDayOfValorEventActive)
			{
				return 1;
			}
			return (int)_dayOfValorMultiplyerForMoney;
		}
	}

	public static event Action ActionsUUpdated;

	public static event Action EventX3Updated;

	public static event Action EventAmazonX3Updated;

	public static event Action BestBuyStateUpdate;

	public static event OnDayOfValorEnableDelegate OnDayOfValorEnable;

	private void Awake()
	{
		startupTime = Time.realtimeSinceStartup;
		promoActionAddress = URLs.PromoActions;
	}

	private void Start()
	{
		sharedManager = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		StartCoroutine(GetActionsLoop());
		StartCoroutine(GetEventX3InfoLoop());
		StartCoroutine(GetAdvertInfoLoop());
		StartCoroutine(GetBestBuyInfoLoop());
		StartCoroutine(GetDayOfValorInfoLoop());
	}

	private void OnDestroy()
	{
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup - _eventX3LastCheckTime >= 1f)
		{
			CheckEventX3Active();
			if (Time.frameCount % 120 == 0)
			{
				RefreshAmazonEvent();
			}
			CheckDayOfValorActive();
			_eventX3LastCheckTime = Time.realtimeSinceStartup;
		}
	}

	private IEnumerator OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			yield return null;
			yield return null;
			yield return null;
			StartCoroutine(GetActions());
			StartCoroutine(GetEventX3Info());
			StartCoroutine(GetAmazonEventCoroutine());
			StartCoroutine(GetAdvertInfo());
			StartCoroutine(DownloadBestBuyInfo());
			StartCoroutine(DownloadDayOfValorInfo());
		}
	}

	private IEnumerator GetActionsLoop()
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage < TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted)
		{
			yield return null;
		}
		while (true)
		{
			StartCoroutine(GetActions());
			while (Time.realtimeSinceStartup - startTime < 900f)
			{
				yield return null;
			}
		}
	}

	private IEnumerator GetEventX3InfoLoop()
	{
		UpdateNewbieEventX3Info();
		while (true)
		{
			yield return StartCoroutine(GetEventX3Info());
			yield return StartCoroutine(GetAmazonEventCoroutine());
			while (Time.realtimeSinceStartup - _eventX3GetInfoStartTime < 930f)
			{
				yield return null;
			}
		}
	}

	private IEnumerator GetAdvertInfoLoop()
	{
		while (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage <= TrainingController.NewTrainingCompletedStage.None)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(GetAdvertInfo());
			while (Time.realtimeSinceStartup - _advertGetInfoStartTime < 960f)
			{
				yield return null;
			}
		}
	}

	private IEnumerator GetAmazonEventCoroutine()
	{
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android || Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
		{
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.AmazonEvent);
		if (response == null)
		{
			yield break;
		}
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (!string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarning("Amazon event response error: " + response.error);
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Amazon event response is empty");
			yield break;
		}
		Dictionary<string, object> amazonEvent = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (amazonEvent == null)
		{
			Debug.LogWarning("Amazon event bad response: " + responseText);
		}
		else
		{
			if (!IsNeedCheckAmazonEventX3())
			{
				yield break;
			}
			_amazonEventInfo = new AmazonEventInfo();
			object startTimeObj;
			if (amazonEvent.TryGetValue("startTime", out startTimeObj))
			{
				try
				{
					_amazonEventInfo.StartTime = Convert.ToDateTime(startTimeObj, CultureInfo.InvariantCulture);
				}
				catch (Exception ex6)
				{
					Exception ex3 = ex6;
					Debug.LogException(ex3);
				}
			}
			object durationSecondsObj;
			if (amazonEvent.TryGetValue("durationSeconds", out durationSecondsObj))
			{
				try
				{
					_amazonEventInfo.DurationSeconds = Convert.ToSingle(durationSecondsObj);
				}
				catch (Exception ex7)
				{
					Exception ex5 = ex7;
					Debug.LogException(ex5);
				}
			}
			object timezonesObj;
			if (amazonEvent.TryGetValue("timezones", out timezonesObj))
			{
				List<object> timezonesRaw = (timezonesObj as List<object>) ?? new List<object>();
				try
				{
					_amazonEventInfo.Timezones = timezonesRaw.ConvertAll(Convert.ToInt32);
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
				}
			}
			object percentageObj;
			if (amazonEvent.TryGetValue("percentage", out percentageObj))
			{
				try
				{
					_amazonEventInfo.Percentage = Convert.ToSingle(percentageObj);
				}
				catch (Exception ex8)
				{
					Exception ex2 = ex8;
					Debug.LogException(ex2);
				}
			}
			object captionObj;
			if (amazonEvent.TryGetValue("caption", out captionObj))
			{
				try
				{
					_amazonEventInfo.Caption = Convert.ToString(captionObj, CultureInfo.InvariantCulture) ?? string.Empty;
				}
				catch (Exception ex9)
				{
					Exception ex = ex9;
					Debug.LogException(ex);
				}
			}
			RefreshAmazonEvent();
		}
	}

	private IEnumerator GetEventX3Info()
	{
		if (_isGetEventX3InfoRunning)
		{
			yield break;
		}
		_eventX3GetInfoStartTime = Time.realtimeSinceStartup;
		_isGetEventX3InfoRunning = true;
		if (string.IsNullOrEmpty(URLs.EventX3))
		{
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.EventX3);
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (response == null || !string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarningFormat("EventX3 response error: {0}", (response == null) ? "null" : response.error);
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("EventX3 response is empty");
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		object eventX3InfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> eventX3Info = eventX3InfoObj as Dictionary<string, object>;
		if (eventX3Info == null || !eventX3Info.ContainsKey("start") || !eventX3Info.ContainsKey("duration"))
		{
			Debug.LogWarning("EventX3 response is bad");
			_isGetEventX3InfoRunning = false;
			yield break;
		}
		long startTime = (long)eventX3Info["start"];
		long duration = (long)eventX3Info["duration"];
		Debug.Log(string.Format("EventX3: ({0}, {1})", startTime, duration));
		_eventX3StartTime = startTime;
		_eventX3Duration = duration;
		CheckEventX3Active();
		x3InfoDownloadaedOnceDuringCurrentRun = true;
		_isGetEventX3InfoRunning = false;
	}

	private bool IsNeedCheckAmazonEventX3()
	{
		if (Defs.IsDeveloperBuild)
		{
			return true;
		}
		if (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android)
		{
			return false;
		}
		if (Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon)
		{
			return false;
		}
		return true;
	}

	private static bool CheckTimezone(List<int> timezones)
	{
		if (timezones == null)
		{
			return false;
		}
		return timezones.Any(DateTimeOffset.Now.Offset.Hours.Equals);
	}

	private bool CheckAvailabelTimeZoneForAmazonEvent()
	{
		if (!_eventX3Active)
		{
			return false;
		}
		if (_eventX3AmazonEventValidTimeZone == null || _eventX3AmazonEventValidTimeZone.Count == 0)
		{
			return false;
		}
		TimeSpan offset = DateTimeOffset.Now.Offset;
		for (int i = 0; i < _eventX3AmazonEventValidTimeZone.Count; i++)
		{
			long num = _eventX3AmazonEventValidTimeZone[i];
			if (num == offset.Hours)
			{
				return true;
			}
		}
		return false;
	}

	private void ParseAmazonEventData(Dictionary<string, object> jsonData)
	{
		if (jsonData.ContainsKey("startAmazonEventTime"))
		{
			_eventX3AmazonEventStartTime = (long)jsonData["startAmazonEventTime"];
		}
		if (jsonData.ContainsKey("endAmazonEventTime"))
		{
			_eventX3AmazonEventEndTime = (long)jsonData["endAmazonEventTime"];
		}
		if (jsonData.ContainsKey("timeZonesForEventAmazon"))
		{
			List<object> list = jsonData["timeZonesForEventAmazon"] as List<object>;
			for (int i = 0; i < list.Count; i++)
			{
				_eventX3AmazonEventValidTimeZone.Add((long)list[i]);
			}
		}
	}

	private void RefreshAmazonEvent()
	{
		if (PromoActionsManager.EventAmazonX3Updated != null)
		{
			PromoActionsManager.EventAmazonX3Updated();
		}
	}

	[Obsolete]
	private void CheckAmazonEventX3Active()
	{
		if (!_eventX3Active || !CheckAvailabelTimeZoneForAmazonEvent())
		{
			_eventX3AmazonEventActive = false;
			return;
		}
		bool eventX3AmazonEventActive = _eventX3AmazonEventActive;
		if (_eventX3AmazonEventStartTime != 0L && _eventX3AmazonEventEndTime != 0L)
		{
			long currentUnixTime = CurrentUnixTime;
			_eventX3AmazonEventActive = _eventX3StartTime < currentUnixTime && currentUnixTime < _eventX3AmazonEventEndTime;
		}
		else
		{
			_eventX3AmazonEventStartTime = 0L;
			_eventX3AmazonEventEndTime = 0L;
			_eventX3AmazonEventActive = false;
		}
		if (_eventX3AmazonEventActive != eventX3AmazonEventActive && PromoActionsManager.EventAmazonX3Updated != null)
		{
			PromoActionsManager.EventAmazonX3Updated();
		}
	}

	public void ForceCheckEventX3Active()
	{
		CheckEventX3Active();
	}

	private void CheckEventX3Active()
	{
		bool eventX3Active = _eventX3Active;
		if (IsNewbieEventX3Active)
		{
			_eventX3StartTime = _newbieEventX3StartTime;
			_eventX3Duration = 259200L;
			_eventX3Active = true;
		}
		else if (_eventX3StartTime != 0L && _eventX3Duration != 0L && IsX3StartTimeAfterNewbieX3TimeoutEndTime)
		{
			long currentUnixTime = CurrentUnixTime;
			_eventX3Active = _eventX3StartTime < currentUnixTime && currentUnixTime < _eventX3StartTime + _eventX3Duration;
		}
		else
		{
			_eventX3StartTime = 0L;
			_eventX3Duration = 0L;
			_eventX3Active = false;
		}
		if (_eventX3Active != eventX3Active)
		{
			if (_eventX3Active)
			{
				PlayerPrefs.SetInt(Defs.EventX3WindowShownCount, 1);
				PlayerPrefs.Save();
			}
			if (PromoActionsManager.EventX3Updated != null)
			{
				PromoActionsManager.EventX3Updated();
			}
		}
	}

	private void ResetNewbieX3StartTime()
	{
		if (_newbieEventX3StartTime != 0L)
		{
			Storager.setString(Defs.NewbieEventX3StartTime, 0L.ToString(), false);
			_newbieEventX3StartTime = 0L;
		}
	}

	public static long GetUnixTimeFromStorage(string storageId)
	{
		long result = 0L;
		if (Storager.hasKey(storageId))
		{
			string @string = Storager.getString(storageId, false);
			long.TryParse(@string, out result);
		}
		return result;
	}

	public void UpdateNewbieEventX3Info()
	{
		_newbieEventX3StartTime = GetUnixTimeFromStorage(Defs.NewbieEventX3StartTime);
		_newbieEventX3StartTimeAdditional = GetUnixTimeFromStorage(Defs.NewbieEventX3StartTimeAdditional);
	}

	private long GetNewbieEventX3LastLoggedTime()
	{
		if (_newbieEventX3StartTime != 0L)
		{
			return GetUnixTimeFromStorage(Defs.NewbieEventX3LastLoggedTime);
		}
		return 0L;
	}

	private IEnumerator GetAdvertInfo()
	{
		if (_isGetAdvertInfoRunning)
		{
			yield break;
		}
		_advertGetInfoStartTime = Time.realtimeSinceStartup;
		_isGetAdvertInfoRunning = true;
		_paidAdvert.enabled = false;
		_freeAdvert.enabled = false;
		if (string.IsNullOrEmpty(URLs.Advert))
		{
			_isGetAdvertInfoRunning = false;
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.Advert);
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (response == null || !string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarningFormat("Advert response error: {0}", (response == null) ? "null" : response.error);
			_isGetAdvertInfoRunning = false;
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Advert response is empty");
			_isGetAdvertInfoRunning = false;
			yield break;
		}
		object advertInfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> advertInfo = advertInfoObj as Dictionary<string, object>;
		if (advertInfoObj == null)
		{
			_isGetAdvertInfoRunning = false;
			yield break;
		}
		if (advertInfo.ContainsKey("paid"))
		{
			ParseAdvertInfo(advertInfo["paid"], _paidAdvert);
		}
		if (advertInfo.ContainsKey("free"))
		{
			ParseAdvertInfo(advertInfo["free"], _freeAdvert);
		}
		if (advertInfo.ContainsKey("mobileAdvert_list"))
		{
			List<object> mobileAdvertList = advertInfo["mobileAdvert_list"] as List<object>;
			if (mobileAdvertList != null)
			{
				string myDevice = SystemInfo.deviceModel;
				Debug.Log("--------" + myDevice + "--------");
				Dictionary<string, object> customSettings = null;
				Dictionary<string, object> allSettings = null;
				for (int i = 0; i < mobileAdvertList.Count; i++)
				{
					Dictionary<string, object> currentMobileAdvert = (Dictionary<string, object>)mobileAdvertList[i];
					object devices;
					if (currentMobileAdvert.TryGetValue("devices", out devices))
					{
						List<object> ids = devices as List<object>;
						List<string> devicesList = ((ids == null) ? new List<string>() : ids.OfType<string>().ToList());
						if (devicesList.Contains("ALL"))
						{
							allSettings = currentMobileAdvert;
						}
						else if (devicesList.Contains(myDevice))
						{
							customSettings = currentMobileAdvert;
						}
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “devices” field.");
					}
				}
				Dictionary<string, object> mobileAdvert = null;
				mobileAdvert = ((customSettings == null) ? allSettings : customSettings);
				if (mobileAdvert != null)
				{
					_mobileAdvert = new MobileAdvertInfo();
					object imageEnabled;
					if (mobileAdvert.TryGetValue("imageEnabled", out imageEnabled))
					{
						_mobileAdvert.ImageEnabled = (long)imageEnabled == 1;
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “imageEnabled” field.");
					}
					object daysOfBeingPayingUser;
					if (mobileAdvert.TryGetValue("daysOfBeingPayingUser", out daysOfBeingPayingUser))
					{
						try
						{
							_mobileAdvert.DaysOfBeingPayingUser = Convert.ToDouble(daysOfBeingPayingUser, CultureInfo.InvariantCulture);
						}
						catch (Exception ex11)
						{
							Exception ex4 = ex11;
							Debug.LogException(ex4);
						}
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “daysOfBeingPayingUser” field.");
					}
					object videoEnabled;
					if (mobileAdvert.TryGetValue("videoEnabled", out videoEnabled))
					{
						_mobileAdvert.VideoEnabled = (long)videoEnabled == 1;
					}
					object videoShowPaying;
					if (mobileAdvert.TryGetValue("videoShowPaying", out videoShowPaying))
					{
						_mobileAdvert.VideoShowPaying = Convert.ToBoolean(videoShowPaying);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “videoShowPaying” field.");
					}
					object videoShowNonpaying;
					if (mobileAdvert.TryGetValue("videoShowNonpaying", out videoShowNonpaying))
					{
						_mobileAdvert.VideoShowNonpaying = Convert.ToBoolean(videoShowNonpaying);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “videoShowNonpaying” field.");
					}
					object timeoutWaitVideo;
					if (mobileAdvert.TryGetValue("timeoutWaitVideo", out timeoutWaitVideo))
					{
						_mobileAdvert.TimeoutWaitVideo = Convert.ToInt32(timeoutWaitVideo);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “timeoutWaitVideo” field.");
					}
					object countSessionNewPlayer;
					if (mobileAdvert.TryGetValue("countSessionNewPlayer", out countSessionNewPlayer))
					{
						_mobileAdvert.CountSessionNewPlayer = Convert.ToInt32(countSessionNewPlayer);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “countSessionNewPlayer” field.");
					}
					object countRoundReplaceProviders;
					if (mobileAdvert.TryGetValue("countRoundReplaceProviders", out countRoundReplaceProviders))
					{
						_mobileAdvert.CountRoundReplaceProviders = Convert.ToInt32(countRoundReplaceProviders);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “countRoundReplaceProviders” field.");
					}
					object timeoutSkipVideoNonpaying;
					if (mobileAdvert.TryGetValue("timeoutSkipVideoNonpaying", out timeoutSkipVideoNonpaying))
					{
						_mobileAdvert.TimeoutSkipVideoNonpaying = Convert.ToInt32(timeoutSkipVideoNonpaying);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “timeoutSkipVideoNonpaying” field.");
					}
					object timeoutSkipVideoPaying;
					if (mobileAdvert.TryGetValue("timeoutSkipVideoPaying", out timeoutSkipVideoPaying))
					{
						_mobileAdvert.TimeoutSkipVideoPaying = Convert.ToInt32(timeoutSkipVideoPaying);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “timeoutSkipVideoPaying” field.");
					}
					object timeoutBetweenShowInterstitial;
					if (mobileAdvert.TryGetValue("timeoutBetweenShowInterstitial", out timeoutBetweenShowInterstitial))
					{
						_mobileAdvert.TimeoutBetweenShowInterstitial = Convert.ToInt32(timeoutBetweenShowInterstitial);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “timeoutBetweenShowInterstitial” field.");
					}
					object admobVideoAdUnitId;
					if (mobileAdvert.TryGetValue("admobVideoAdUnitId", out admobVideoAdUnitId))
					{
						_mobileAdvert.AdmobVideoAdUnitId = Convert.ToString(admobVideoAdUnitId) ?? string.Empty;
					}
					object admobImageAdUnitIds;
					if (mobileAdvert.TryGetValue("admobImageAdUnitIds-10.0.4", out admobImageAdUnitIds))
					{
						List<object> ids3 = admobImageAdUnitIds as List<object>;
						List<string> list4 = ((ids3 == null) ? new List<string>() : ids3.OfType<string>().ToList());
						_mobileAdvert.AdmobImageAdUnitIds = list4;
						object admobImageIdGroups;
						if (mobileAdvert.TryGetValue("admobImageIdGroups-10.0.4", out admobImageIdGroups))
						{
							IEnumerable<string> listCopy2 = new List<string>(list4);
							List<object> groups2 = admobImageIdGroups as List<object>;
							if (groups2 != null)
							{
								foreach (object group2 in groups2)
								{
									int count2 = Convert.ToInt32(group2);
									_mobileAdvert.AdmobImageIdGroups.Add(listCopy2.Take(count2).ToList());
									listCopy2 = listCopy2.Skip(count2);
								}
							}
						}
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “admobImageAdUnitIds” field.");
					}
					object admobVideoAdUnitIds;
					if (mobileAdvert.TryGetValue("admobVideoAdUnitIds-10.0.4", out admobVideoAdUnitIds))
					{
						List<object> ids2 = admobVideoAdUnitIds as List<object>;
						List<string> list3 = ((ids2 == null) ? new List<string>() : ids2.OfType<string>().ToList());
						_mobileAdvert.AdmobVideoAdUnitIds = list3;
						object admobVideoIdGroups;
						if (mobileAdvert.TryGetValue("admobVideoIdGroups-10.0.4", out admobVideoIdGroups))
						{
							IEnumerable<string> listCopy = new List<string>(list3);
							List<object> groups = admobVideoIdGroups as List<object>;
							if (groups != null)
							{
								foreach (object group in groups)
								{
									int count = Convert.ToInt32(group);
									_mobileAdvert.AdmobVideoIdGroups.Add(listCopy.Take(count).ToList());
									listCopy = listCopy.Skip(count);
								}
							}
						}
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “admobVideoAdUnitIds” field.");
					}
					object awardCoinsPayng;
					if (mobileAdvert.TryGetValue("awardCoinsPaying", out awardCoinsPayng))
					{
						_mobileAdvert.AwardCoinsPaying = Convert.ToInt32(awardCoinsPayng);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “awardCoinsPaying” field. Default value is " + 1);
					}
					object awardCoinsNonpaying;
					if (mobileAdvert.TryGetValue("awardCoinsNonpaying", out awardCoinsNonpaying))
					{
						_mobileAdvert.AwardCoinsNonpaying = Convert.ToInt32(awardCoinsNonpaying);
					}
					else
					{
						Debug.Log("mobileAdvert doesn't contain “awardCoinsNonpaying” field. Default value is " + 1);
					}
					object adProvider;
					if (mobileAdvert.TryGetValue("adProvider", out adProvider))
					{
						_mobileAdvert.AdProvider = Convert.ToInt32(adProvider);
						if (_mobileAdvert.AdProvider != 4)
						{
						}
					}
					object adProviders;
					if (mobileAdvert.TryGetValue("adProviders-10.0.4", out adProviders))
					{
						try
						{
							List<object> providers2 = adProviders as List<object>;
							List<int> list2 = ((providers2 == null) ? new List<int>() : providers2.Select(Convert.ToInt32).ToList());
							_mobileAdvert.AdProviders = list2;
						}
						catch (Exception ex12)
						{
							Exception ex10 = ex12;
							Debug.LogException(ex10);
						}
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “adProviders” field.");
					}
					object interstitialProviders;
					if (mobileAdvert.TryGetValue("interstitialProviders-10.0.4", out interstitialProviders))
					{
						try
						{
							List<object> providers = interstitialProviders as List<object>;
							List<int> list = ((providers == null) ? new List<int>() : providers.Select(Convert.ToInt32).ToList());
							_mobileAdvert.InterstitialProviders = list;
						}
						catch (Exception ex13)
						{
							Exception ex9 = ex13;
							Debug.LogException(ex9);
						}
					}
					else
					{
						Debug.LogWarning("mobileAdvert doesn't contain “interstitialProviders” field.");
					}
					object connectSceneDelaySeconds;
					if (mobileAdvert.TryGetValue("connectSceneDelaySeconds", out connectSceneDelaySeconds))
					{
						try
						{
							_mobileAdvert.ConnectSceneDelaySeconds = Math.Max(0.0, Convert.ToDouble(connectSceneDelaySeconds));
						}
						catch (Exception ex14)
						{
							Exception ex8 = ex14;
							Debug.LogException(ex8);
						}
					}
					object rewardedVideoDelayMinutesNonpaying;
					if (mobileAdvert.TryGetValue("rewardedVideoDelayMinutesNonpaying", out rewardedVideoDelayMinutesNonpaying))
					{
						try
						{
							List<object> delayObjects2 = rewardedVideoDelayMinutesNonpaying as List<object>;
							List<double> delays2 = ((delayObjects2 == null) ? new List<double>() : delayObjects2.Select(Convert.ToDouble).ToList());
							_mobileAdvert.RewardedVideoDelayMinutesNonpaying = delays2;
						}
						catch (Exception ex15)
						{
							Exception ex7 = ex15;
							Debug.LogException(ex7);
						}
					}
					object rewardedVideoDelayMinutesPaying;
					if (mobileAdvert.TryGetValue("rewardedVideoDelayMinutesPaying", out rewardedVideoDelayMinutesPaying))
					{
						try
						{
							List<object> delayObjects = rewardedVideoDelayMinutesPaying as List<object>;
							List<double> delays = ((delayObjects == null) ? new List<double>() : delayObjects.Select(Convert.ToDouble).ToList());
							_mobileAdvert.RewardedVideoDelayMinutesPaying = delays;
						}
						catch (Exception ex16)
						{
							Exception ex6 = ex16;
							Debug.LogException(ex6);
						}
					}
					object showInterstitialAfterMatchLoserObject;
					if (mobileAdvert.TryGetValue("showInterstitialAfterMatchLoser", out showInterstitialAfterMatchLoserObject))
					{
						try
						{
							int showInterstitialAfterMatchLoser = Convert.ToInt32(showInterstitialAfterMatchLoserObject);
							_mobileAdvert.ShowInterstitialAfterMatchLoser = showInterstitialAfterMatchLoser > 0;
						}
						catch (Exception ex17)
						{
							Exception ex5 = ex17;
							Debug.LogException(ex5);
						}
					}
					object showInterstitialAfterMatchWinnerObject;
					if (mobileAdvert.TryGetValue("showInterstitialAfterMatchWinner", out showInterstitialAfterMatchWinnerObject))
					{
						try
						{
							int showInterstitialAfterMatchWinner = Convert.ToInt32(showInterstitialAfterMatchWinnerObject);
							_mobileAdvert.ShowInterstitialAfterMatchWinner = showInterstitialAfterMatchWinner > 0;
						}
						catch (Exception ex18)
						{
							Exception ex3 = ex18;
							Debug.LogException(ex3);
						}
					}
					object timeoutWaitingInterstitialAfterMatchSecondsObject;
					if (mobileAdvert.TryGetValue("timeoutWaitingInterstitialAfterMatchSeconds", out timeoutWaitingInterstitialAfterMatchSecondsObject))
					{
						try
						{
							float timeoutWaitingInterstitialAfterMatchSeconds = Convert.ToSingle(timeoutWaitingInterstitialAfterMatchSecondsObject);
							_mobileAdvert.TimeoutWaitingInterstitialAfterMatchSeconds = Math.Max(0f, timeoutWaitingInterstitialAfterMatchSeconds);
						}
						catch (Exception ex19)
						{
							Exception ex2 = ex19;
							Debug.LogException(ex2);
						}
					}
					object minMatchDurationForAftermatchInterstitial;
					if (mobileAdvert.TryGetValue("minMatchDurationMinutes", out minMatchDurationForAftermatchInterstitial))
					{
						try
						{
							double duration = Convert.ToDouble(minMatchDurationForAftermatchInterstitial);
							_mobileAdvert.MinMatchDurationMinutes = Math.Max(0.0, duration);
						}
						catch (Exception ex20)
						{
							Exception ex = ex20;
							Debug.LogException(ex);
						}
					}
				}
				else
				{
					Debug.LogWarning("mobileAdvert == null");
				}
			}
		}
		else
		{
			Debug.Log("Advert response doesn't contain “mobileAdvert” property.");
		}
		if (advertInfo.ContainsKey("replace_admob_pereliv_10_2_0"))
		{
			Dictionary<string, object> replaceAdmob = advertInfo["replace_admob_pereliv_10_2_0"] as Dictionary<string, object>;
			ParseReplaceAdmobPereliv(replaceAdmob, _replaceAdmobPereliv);
		}
		else
		{
			Debug.Log("Advert response doesn't contain “replace_admob_pereliv_10_2_0” property.");
		}
		_isGetAdvertInfoRunning = false;
	}

	private static void ParseReplaceAdmobPereliv(Dictionary<string, object> replaceAdmob, ReplaceAdmobPerelivInfo replaceAdmobObj)
	{
		if (replaceAdmob != null)
		{
			try
			{
				replaceAdmobObj.imageUrls = (replaceAdmob["imageUrls"] as List<object>).OfType<string>().ToList();
				replaceAdmobObj.adUrls = (replaceAdmob["adUrls"] as List<object>).OfType<string>().ToList();
				replaceAdmobObj.enabled = (long)replaceAdmob["enabled"] == 1;
				replaceAdmobObj.ShowEveryTimes = Mathf.Max((int)(long)replaceAdmob["showEveryTimes"], 1);
				replaceAdmobObj.ShowTimesTotal = Mathf.Max((int)(long)replaceAdmob["showTimesTotal"], 0);
				replaceAdmobObj.ShowToPaying = (long)replaceAdmob["showToPaying"] == 1;
				replaceAdmobObj.ShowToNew = (long)replaceAdmob["showToNew"] == 1;
				try
				{
					replaceAdmobObj.MinLevel = (int)(long)replaceAdmob["minLevel"];
				}
				catch
				{
					replaceAdmobObj.MinLevel = -1;
				}
				try
				{
					replaceAdmobObj.MaxLevel = (int)(long)replaceAdmob["maxLevel"];
					return;
				}
				catch
				{
					replaceAdmobObj.MaxLevel = -1;
					return;
				}
			}
			catch
			{
				Debug.LogWarning("replace_admob_pereliv_10_2_0 exception whiel parsing");
				return;
			}
		}
		Debug.LogWarning("replaceAdmob == null");
	}

	private void ParseAdvertInfo(object advertInfoObj, AdvertInfo advertInfo)
	{
		Dictionary<string, object> dictionary = advertInfoObj as Dictionary<string, object>;
		if (dictionary != null)
		{
			advertInfo.imageUrl = (string)dictionary["imageUrl"];
			advertInfo.adUrl = (string)dictionary["adUrl"];
			advertInfo.message = (string)dictionary["text"];
			advertInfo.showAlways = (long)dictionary["showAlways"] == 1;
			advertInfo.btnClose = (long)dictionary["btnClose"] == 1;
			advertInfo.minLevel = (int)(long)dictionary["minLevel"];
			advertInfo.maxLevel = (int)(long)dictionary["maxLevel"];
			advertInfo.enabled = (long)dictionary["enabled"] == 1;
		}
	}

	private void ClearAll()
	{
		discounts.Clear();
		topSellers.Clear();
		news.Clear();
	}

	public IEnumerator GetActions()
	{
		startTime = Time.realtimeSinceStartup;
		WWW download = Tools.CreateWwwIfNotConnected(promoActionAddress);
		if (download == null)
		{
			yield break;
		}
		yield return download;
		string response = URLs.Sanitize(download);
		if (string.IsNullOrEmpty(download.error) && !string.IsNullOrEmpty(response) && Debug.isDebugBuild)
		{
			Debug.Log("GetActions response:    " + response);
		}
		if (!string.IsNullOrEmpty(download.error))
		{
			if (Debug.isDebugBuild)
			{
				Debug.LogWarning("GetActions error:    " + download.error);
			}
			ClearAll();
			ActionsAvailable = false;
			yield break;
		}
		string responseText = response;
		if (_previousResponseText != null && responseText != null && responseText == _previousResponseText)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("GetActions:    responseText == _previousResponseText");
			}
			yield break;
		}
		_previousResponseText = responseText;
		ActionsAvailable = true;
		ClearAll();
		Dictionary<string, object> actions = Json.Deserialize(responseText) as Dictionary<string, object>;
		if (actions == null)
		{
			if (Debug.isDebugBuild || Application.isEditor)
			{
				Debug.LogWarning(" GetActions actions = null");
			}
			yield break;
		}
		object discountsObj;
		if (actions.TryGetValue("discounts_up", out discountsObj) && discountsObj is List<object>)
		{
			foreach (List<object> discount in discountsObj as List<object>)
			{
				if (discount.Count >= 2)
				{
					List<int> vals = new List<int> { Convert.ToInt32((long)discount[1]) };
					discounts.Add(discount[0] as string, vals);
				}
			}
		}
		object newsObj;
		if (actions.TryGetValue("news_up", out newsObj) && newsObj is List<object>)
		{
			foreach (string tg2 in newsObj as List<object>)
			{
				news.Add(tg2);
			}
		}
		object topSellersObj;
		if (actions.TryGetValue("topSellers_up", out topSellersObj) && topSellersObj is List<object>)
		{
			foreach (string tg in topSellersObj as List<object>)
			{
				topSellers.Add(tg);
			}
		}
		if (PromoActionsManager.ActionsUUpdated != null)
		{
			PromoActionsManager.ActionsUUpdated();
		}
	}

	private IEnumerator DownloadBestBuyInfo()
	{
		if (_isGetBestBuyInfoRunning)
		{
			yield break;
		}
		_bestBuyGetInfoStartTime = Time.realtimeSinceStartup;
		_isGetBestBuyInfoRunning = true;
		if (string.IsNullOrEmpty(URLs.BestBuy))
		{
			_isGetBestBuyInfoRunning = false;
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.BestBuy);
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (response == null || !string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarningFormat("Best buy response error: {0}", (response == null) ? "null" : response.error);
			_bestBuyIds.Clear();
			_isGetBestBuyInfoRunning = false;
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Best buy response is empty");
			_bestBuyIds.Clear();
			_isGetBestBuyInfoRunning = false;
			yield break;
		}
		object bestBuyInfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> bestBuyInfo = bestBuyInfoObj as Dictionary<string, object>;
		if (bestBuyInfo == null || !bestBuyInfo.ContainsKey("bestBuy"))
		{
			Debug.LogWarning("Best buy response is bad");
			_bestBuyIds.Clear();
			_isGetBestBuyInfoRunning = false;
			yield break;
		}
		List<object> bestBuyItemObjects = bestBuyInfo["bestBuy"] as List<object>;
		if (bestBuyItemObjects != null)
		{
			_bestBuyIds.Clear();
			for (int i = 0; i < bestBuyItemObjects.Count; i++)
			{
				string bestBuyId = (string)bestBuyItemObjects[i];
				_bestBuyIds.Add(bestBuyId);
			}
		}
		if (PromoActionsManager.BestBuyStateUpdate != null)
		{
			PromoActionsManager.BestBuyStateUpdate();
		}
		_isGetBestBuyInfoRunning = false;
	}

	public bool IsBankItemBestBuy(PurchaseEventArgs purchaseInfo)
	{
		if (_bestBuyIds.Count == 0 || purchaseInfo == null)
		{
			return false;
		}
		string arg = ((!(purchaseInfo.Currency == "GemsCurrency")) ? "coins" : "gems");
		string item = string.Format("{0}_{1}", arg, purchaseInfo.Index + 1);
		return _bestBuyIds.Contains(item);
	}

	private IEnumerator GetBestBuyInfoLoop()
	{
		while (true)
		{
			yield return StartCoroutine(DownloadBestBuyInfo());
			while (Time.realtimeSinceStartup - _bestBuyGetInfoStartTime < 1020f)
			{
				yield return null;
			}
		}
	}

	private void ClearDataDayOfValor()
	{
		_dayOfValorStartTime = 0L;
		_dayOfValorEndTime = 0L;
		_dayOfValorMultiplyerForExp = 1L;
		_dayOfValorMultiplyerForMoney = 1L;
	}

	private IEnumerator DownloadDayOfValorInfo()
	{
		if (_isGetDayOfValorInfoRunning)
		{
			yield break;
		}
		_dayOfValorGetInfoStartTime = Time.realtimeSinceStartup;
		_isGetDayOfValorInfoRunning = true;
		if (string.IsNullOrEmpty(URLs.DayOfValor))
		{
			_isGetDayOfValorInfoRunning = false;
			yield break;
		}
		WWW response = Tools.CreateWwwIfNotConnected(URLs.DayOfValor);
		yield return response;
		string responseText = URLs.Sanitize(response);
		if (response == null || !string.IsNullOrEmpty(response.error))
		{
			Debug.LogWarningFormat("Day of valor response error: {0}", (response == null) ? "null" : response.error);
			_isGetDayOfValorInfoRunning = false;
			ClearDataDayOfValor();
			yield break;
		}
		if (string.IsNullOrEmpty(responseText))
		{
			Debug.LogWarning("Best buy response is empty");
			_isGetDayOfValorInfoRunning = false;
			ClearDataDayOfValor();
			yield break;
		}
		object dayOfValorInfoObj = Json.Deserialize(responseText);
		Dictionary<string, object> dayOfValorInfo = dayOfValorInfoObj as Dictionary<string, object>;
		if (dayOfValorInfo == null)
		{
			Debug.LogWarning("Day of valor response is bad");
			_isGetDayOfValorInfoRunning = false;
			ClearDataDayOfValor();
			yield break;
		}
		ClearDataDayOfValor();
		if (dayOfValorInfo.ContainsKey("startTime"))
		{
			_dayOfValorStartTime = (long)dayOfValorInfo["startTime"];
		}
		if (dayOfValorInfo.ContainsKey("endTime"))
		{
			_dayOfValorEndTime = (long)dayOfValorInfo["endTime"];
		}
		if (dayOfValorInfo.ContainsKey("multiplyerForExp"))
		{
			_dayOfValorMultiplyerForExp = (long)dayOfValorInfo["multiplyerForExp"];
		}
		if (dayOfValorInfo.ContainsKey("multiplyerForMoney"))
		{
			_dayOfValorMultiplyerForMoney = (long)dayOfValorInfo["multiplyerForMoney"];
		}
		_isGetDayOfValorInfoRunning = false;
	}

	private IEnumerator GetDayOfValorInfoLoop()
	{
		while (!TrainingController.TrainingCompleted)
		{
			yield return null;
		}
		while (true)
		{
			yield return StartCoroutine(DownloadDayOfValorInfo());
			while (Time.realtimeSinceStartup - _dayOfValorGetInfoStartTime < 1050f)
			{
				yield return null;
			}
		}
	}

	private void CheckDayOfValorActive()
	{
		bool isDayOfValorEventActive = IsDayOfValorEventActive;
		if (_dayOfValorStartTime != 0L && _dayOfValorEndTime != 0L && ExpController.LobbyLevel >= 1)
		{
			long currentUnixTime = CurrentUnixTime;
			IsDayOfValorEventActive = _dayOfValorStartTime < currentUnixTime && currentUnixTime < _dayOfValorEndTime;
			_timeToEndDayOfValor = TimeSpan.FromSeconds(_dayOfValorEndTime - currentUnixTime);
		}
		else
		{
			ClearDataDayOfValor();
			IsDayOfValorEventActive = false;
		}
		if (IsDayOfValorEventActive != isDayOfValorEventActive && PromoActionsManager.OnDayOfValorEnable != null)
		{
			PromoActionsManager.OnDayOfValorEnable(IsDayOfValorEventActive);
		}
	}

	public static void UpdateDaysOfValorShownCondition()
	{
		string @string = PlayerPrefs.GetString("LastTimeShowDaysOfValor", string.Empty);
		if (!string.IsNullOrEmpty(@string))
		{
			DateTime result = default(DateTime);
			if (DateTime.TryParse(@string, out result) && DateTime.UtcNow - result >= TimeToShowDaysOfValor)
			{
				PlayerPrefs.SetInt("DaysOfValorShownCount", 1);
			}
		}
	}

	public string GetTimeToEndDaysOfValor()
	{
		if (!IsDayOfValorEventActive)
		{
			return string.Empty;
		}
		if (_timeToEndDayOfValor.Days > 0)
		{
			return string.Format("{0} days {1:00}:{2:00}:{3:00}", _timeToEndDayOfValor.Days, _timeToEndDayOfValor.Hours, _timeToEndDayOfValor.Minutes, _timeToEndDayOfValor.Seconds);
		}
		return string.Format("{0:00}:{1:00}:{2:00}", _timeToEndDayOfValor.Hours, _timeToEndDayOfValor.Minutes, _timeToEndDayOfValor.Seconds);
	}
}
