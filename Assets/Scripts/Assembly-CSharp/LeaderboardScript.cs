using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Rilisoft;
using Rilisoft.MiniJson;
using Rilisoft.NullExtensions;
using UnityEngine;

internal sealed class LeaderboardScript : MonoBehaviour
{
	private enum GridState
	{
		Empty = 0,
		FillingWithCache = 1,
		Cache = 2,
		FillingWithResponse = 3,
		Response = 4
	}

	private const int VisibleItemMaxCount = 15;

	private const string LeaderboardsResponseCache = "Leaderboards.ResponseCache";

	private const string LeaderboardsResponseCacheTimestamp = "Leaderboards.ResponseCacheTimestamp";

	public GameObject topFriendsGrid;

	public GameObject topPlayersGrid;

	public GameObject topClansGrid;

	public GameObject tableFooterIndividuals;

	public GameObject tableFooterClans;

	public UILabel expirationLabel;

	public UITexture loadingTexture;

	private float _expirationTimeSeconds;

	private float _expirationNextUpateTimeSeconds;

	private bool _fillLock;

	private readonly List<LeaderboardItemViewModel> _clansList = new List<LeaderboardItemViewModel>(101);

	private readonly List<LeaderboardItemViewModel> _friendsList = new List<LeaderboardItemViewModel>(101);

	private readonly List<LeaderboardItemViewModel> _playersList = new List<LeaderboardItemViewModel>(101);

	public ButtonHandler backButton;

	private readonly Lazy<UIPanel> _leaderboardsPanel;

	private readonly Lazy<LeaderboardsView> _leaderboardsView;

	private readonly Lazy<MainMenuController> _mainMenuController;

	private TaskCompletionSource<bool> _returnPromise = new TaskCompletionSource<bool>();

	private bool _profileIsOpened;

	private static TaskCompletionSource<string> _currentRequestPromise;

	private GridState _state;

	private Task<string> CurrentRequest
	{
		get
		{
			return _currentRequestPromise.Map((TaskCompletionSource<string> p) => p.Task);
		}
	}

	public static event EventHandler<ClickedEventArgs> PlayerClicked;

	public LeaderboardScript()
	{
		_leaderboardsView = new Lazy<LeaderboardsView>(base.GetComponentInChildren<LeaderboardsView>);
		_leaderboardsPanel = new Lazy<UIPanel>(() => _leaderboardsView.Value.Map((LeaderboardsView l) => l.GetComponent<UIPanel>()));
		_mainMenuController = new Lazy<MainMenuController>(() => MainMenuController.sharedController);
	}

	private void UpdateLocs()
	{
		if (tableFooterIndividuals != null)
		{
			tableFooterIndividuals.transform.Find("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
			{
				n.text = LocalizationStore.Get("Key_0053");
			});
		}
		if (tableFooterClans != null)
		{
			tableFooterClans.transform.Find("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
			{
				n.text = LocalizationStore.Get("Key_0053");
			});
		}
	}

	private IEnumerator FillGrids(string response, string playerId, GridState state)
	{
		while (_fillLock)
		{
			yield return null;
		}
		_fillLock = true;
		try
		{
			if (string.IsNullOrEmpty(playerId))
			{
				throw new ArgumentException("Player id should not be empty", "playerId");
			}
			Dictionary<string, object> d = Json.Deserialize(response ?? string.Empty) as Dictionary<string, object>;
			if (d == null)
			{
				Debug.LogWarning("Leaderboards response is ill-formed.");
				d = new Dictionary<string, object>();
			}
			else if (d.Count == 0)
			{
				Debug.LogWarning("Leaderboards response contains no elements.");
			}
			object expirationTimespanSecondsObject;
			if (d.TryGetValue("leaderboards_generate_next_time", out expirationTimespanSecondsObject))
			{
				try
				{
					float expirationTimespanSeconds = Convert.ToSingle(expirationTimespanSecondsObject);
					_expirationTimeSeconds = Time.realtimeSinceStartup + expirationTimespanSeconds;
					if (state == GridState.FillingWithCache)
					{
						string cacheTimestampString = PlayerPrefs.GetString("Leaderboards.ResponseCacheTimestamp", string.Empty);
						DateTime cacheTimestamp;
						if (!string.IsNullOrEmpty(cacheTimestampString) && DateTime.TryParse(cacheTimestampString, out cacheTimestamp))
						{
							float timespanSinceLastCache = (float)(DateTime.UtcNow - cacheTimestamp).TotalSeconds;
							timespanSinceLastCache = Math.Max(0f, timespanSinceLastCache);
							_expirationTimeSeconds = Math.Max(0f, _expirationTimeSeconds - timespanSinceLastCache);
						}
					}
					GridState state2 = default(GridState);
					expirationLabel.Do(delegate(UILabel l)
					{
						l.color = ((state2 != GridState.FillingWithCache) ? Color.white : Color.grey);
					});
				}
				catch (Exception ex2)
				{
					Exception ex = ex2;
					Debug.LogWarning(ex);
				}
			}
			LeaderboardItemViewModel me = new LeaderboardItemViewModel
			{
				Id = playerId,
				Nickname = ProfileController.GetPlayerNameOrDefault(),
				Rank = ExperienceController.sharedController.currentLevel,
				WinCount = PlayerPrefs.GetInt("TotalWinsForLeaderboards", 0),
				Highlight = true,
				ClanName = FriendsController.sharedController.Map((FriendsController s) => s.clanName, string.Empty),
				ClanLogo = FriendsController.sharedController.Map((FriendsController s) => s.clanLogo, string.Empty)
			};
			object meObject;
			if (d.TryGetValue("me", out meObject))
			{
				Dictionary<string, object> meDictionary = meObject as Dictionary<string, object>;
				object myWinCount;
				if (meDictionary.TryGetValue("wins", out myWinCount))
				{
					me.WinCount = Convert.ToInt32(myWinCount);
					PlayerPrefs.SetInt("TotalWinsForLeaderboards", me.WinCount);
				}
			}
			List<LeaderboardItemViewModel> rawFriendsList = LeaderboardsController.ParseLeaderboardEntries(playerId, "friends", d);
			HashSet<string> friendIds = new HashSet<string>(FriendsController.sharedController.friends);
			if (FriendsController.sharedController != null)
			{
				for (int i = rawFriendsList.Count - 1; i >= 0; i--)
				{
					LeaderboardItemViewModel item = rawFriendsList[i];
					Dictionary<string, object> info;
					if (friendIds.Contains(item.Id) && FriendsController.sharedController.friendsInfo.TryGetValue(item.Id, out info))
					{
						try
						{
							Dictionary<string, object> playerDict = info["player"] as Dictionary<string, object>;
							item.Nickname = Convert.ToString(playerDict["nick"]);
							item.Rank = Convert.ToInt32(playerDict["rank"]);
							object clanName;
							if (playerDict.TryGetValue("clan_name", out clanName))
							{
								item.ClanName = Convert.ToString(clanName);
							}
							object clanLogo;
							if (playerDict.TryGetValue("clan_logo", out clanLogo))
							{
								item.ClanLogo = Convert.ToString(clanLogo);
							}
						}
						catch (KeyNotFoundException)
						{
							Debug.LogError("Failed to process cached friend: " + item.Id);
						}
					}
					else
					{
						rawFriendsList.RemoveAt(i);
					}
				}
			}
			rawFriendsList.Add(me);
			yield return StartCoroutine(FillFriendsGrid(list: GroupAndOrder(rawFriendsList), gridGo: topFriendsGrid.gameObject, state: state));
			List<LeaderboardItemViewModel> rawTopPlayersList = LeaderboardsController.ParseLeaderboardEntries(playerId, "best_players", d);
			List<LeaderboardItemViewModel> orderedTopPlayersList = GroupAndOrder(rawTopPlayersList);
			AddCacheInProfileInfo(rawTopPlayersList);
			Coroutine fillPlayersCoroutine = StartCoroutine(FillPlayersGrid(topPlayersGrid.gameObject, orderedTopPlayersList, state));
			if (tableFooterIndividuals != null)
			{
				LeaderboardItemViewModel meInTop = rawTopPlayersList.FirstOrDefault((LeaderboardItemViewModel p) => p.Id == me.Id);
				tableFooterClans.SetActive(meInTop == null);
				if (meInTop == null)
				{
					tableFooterIndividuals.transform.Find("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = LocalizationStore.Get("Key_0053");
					});
					tableFooterIndividuals.transform.Find("LabelNick").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = me.Nickname;
					});
					tableFooterIndividuals.transform.Find("LabelLevel").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
					{
						n.text = me.Rank.ToString(CultureInfo.InvariantCulture);
					});
					tableFooterIndividuals.transform.Find("LabelWins").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel w)
					{
						w.text = me.WinCount.ToString(CultureInfo.InvariantCulture);
					});
					UILabel clanLabel2 = tableFooterIndividuals.transform.Find("LabelClan").Map((Transform t) => t.gameObject.GetComponent<UILabel>());
					clanLabel2.Do(delegate(UILabel cl)
					{
						cl.text = me.ClanName;
					});
					clanLabel2.Map((UILabel cl) => cl.GetComponentsInChildren<UITexture>(true).FirstOrDefault()).Do(delegate(UITexture s)
					{
						SetClanLogo(s, me.ClanLogo);
					});
				}
			}
			List<LeaderboardItemViewModel> rawClansList = LeaderboardsController.ParseLeaderboardEntries(playerId, "top_clans", d);
			Coroutine fillClansCoroutine = StartCoroutine(FillClansGrid(list: GroupAndOrder(rawClansList), gridGo: topClansGrid, state: state));
			if (tableFooterClans != null)
			{
				string clanId = FriendsController.sharedController.Map((FriendsController s) => s.ClanID);
				if (string.IsNullOrEmpty(clanId))
				{
					tableFooterClans.SetActive(false);
				}
				else
				{
					LeaderboardItemViewModel myClanInTop = rawClansList.FirstOrDefault((LeaderboardItemViewModel c) => c.Id == clanId);
					tableFooterClans.SetActive(myClanInTop == null);
					if (myClanInTop == null)
					{
						tableFooterClans.transform.Find("LabelPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
						{
							n.text = LocalizationStore.Get("Key_0053");
						});
						tableFooterClans.transform.Find("LabelMembers").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel n)
						{
							n.text = string.Empty;
						});
						tableFooterClans.transform.Find("LabelWins").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel w)
						{
							w.text = string.Empty;
						});
						UILabel clanLabel = tableFooterClans.transform.Find("LabelName").Map((Transform t) => t.gameObject.GetComponent<UILabel>());
						clanLabel.Do(delegate(UILabel cl)
						{
							cl.text = FriendsController.sharedController.Map((FriendsController s) => s.clanName, string.Empty);
						});
						clanLabel.Map((UILabel cl) => cl.GetComponentsInChildren<UITexture>(true).FirstOrDefault()).Do(delegate(UITexture t)
						{
							SetClanLogo(t, FriendsController.sharedController.Map((FriendsController s) => s.clanLogo, string.Empty));
						});
					}
				}
			}
			yield return fillPlayersCoroutine;
			yield return fillClansCoroutine;
		}
		finally
		{
			_fillLock = false;
		}
	}

	private void AddCacheInProfileInfo(List<LeaderboardItemViewModel> _list)
	{
		foreach (LeaderboardItemViewModel item in _list)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("nick", item.Nickname);
			dictionary.Add("rank", item.Rank);
			dictionary.Add("clan_name", item.ClanName);
			dictionary.Add("clan_logo", item.ClanLogo);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("player", dictionary);
			if (!FriendsController.sharedController.profileInfo.ContainsKey(item.Id))
			{
				FriendsController.sharedController.profileInfo.Add(item.Id, dictionary2);
			}
			else
			{
				FriendsController.sharedController.profileInfo[item.Id] = dictionary2;
			}
		}
	}

	private IEnumerator FillClansGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillClanItem(gridGo2, _clansList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_clansList.Clear();
		_clansList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_clansList.Count > 0);
		int bound = Math.Min(15, _clansList.Count);
		for (int i = 0; i != bound; i++)
		{
			LeaderboardItemViewModel item = _clansList[i];
			GameObject newItem;
			if (i < childCount)
			{
				newItem = gridGo.transform.GetChild(i).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = i.ToString(CultureInfo.InvariantCulture);
			}
			FillClanItem(gridGo, _clansList, state, i, newItem);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
		}
	}

	private IEnumerator FillPlayersGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillIndividualItem(gridGo2, _playersList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_playersList.Clear();
		_playersList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_playersList.Count > 0);
		int bound = Math.Min(15, _playersList.Count);
		for (int i = 0; i != bound; i++)
		{
			LeaderboardItemViewModel item = _playersList[i];
			GameObject newItem;
			if (i < childCount)
			{
				newItem = gridGo.transform.GetChild(i).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = i.ToString(CultureInfo.InvariantCulture);
			}
			FillIndividualItem(gridGo, _playersList, state, i, newItem);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
		}
	}

	private IEnumerator FillFriendsGrid(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		UIWrapContent wrap = gridGo.GetComponent<UIWrapContent>();
		if (wrap == null)
		{
			throw new InvalidOperationException("Game object does not contain UIWrapContent component.");
		}
		wrap.minIndex = Math.Min(-list.Count + 1, wrap.maxIndex);
		wrap.onInitializeItem = null;
		GameObject gridGo2 = default(GameObject);
		GridState state2 = default(GridState);
		wrap.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(wrap.onInitializeItem, (UIWrapContent.OnInitializeItem)delegate(GameObject go, int wrapIndex, int realIndex)
		{
			int index = -realIndex;
			FillIndividualItem(gridGo2, _friendsList, state2, index, go);
		});
		int childCount = gridGo.transform.childCount;
		if (childCount == 0)
		{
			Debug.LogError("No children in grid.");
			yield break;
		}
		Transform itemPrototype = gridGo.transform.GetChild(childCount - 1);
		if (itemPrototype == null)
		{
			Debug.LogError("Cannot find prototype for item.");
			yield break;
		}
		_friendsList.Clear();
		_friendsList.AddRange(list);
		GameObject itemPrototypeGo = itemPrototype.gameObject;
		itemPrototypeGo.SetActive(_friendsList.Count > 0);
		int bound = Math.Min(15, _friendsList.Count);
		for (int j = 0; j != bound; j++)
		{
			LeaderboardItemViewModel item2 = _friendsList[j];
			GameObject newItem;
			if (j < childCount)
			{
				newItem = gridGo.transform.GetChild(j).gameObject;
			}
			else
			{
				newItem = NGUITools.AddChild(gridGo, itemPrototypeGo);
				newItem.name = j.ToString(CultureInfo.InvariantCulture);
			}
			FillIndividualItem(gridGo, _friendsList, state, j, newItem);
		}
		int newChildCount = gridGo.transform.childCount;
		List<Transform> oddItemsToRemove = new List<Transform>(Math.Max(0, newChildCount - bound));
		for (int i = list.Count; i < newChildCount; i++)
		{
			oddItemsToRemove.Add(gridGo.transform.GetChild(i));
		}
		foreach (Transform item in oddItemsToRemove)
		{
			NGUITools.Destroy(item);
		}
		yield return new WaitForEndOfFrame();
		wrap.SortBasedOnScrollMovement();
		wrap.WrapContent();
		UIScrollView scrollView = gridGo.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
		}
	}

	internal void RefreshMyLeaderboardEntries()
	{
		foreach (LeaderboardItemViewModel friends in _friendsList)
		{
			if (friends != null && friends.Id == FriendsController.sharedController.id)
			{
				friends.Nickname = ProfileController.GetPlayerNameOrDefault();
				friends.ClanName = FriendsController.sharedController.clanName ?? string.Empty;
				break;
			}
		}
		UILabel uILabel = tableFooterIndividuals.transform.Find("LabelNick").Map((Transform t) => t.gameObject.GetComponent<UILabel>());
		if (uILabel != null)
		{
			uILabel.text = ProfileController.GetPlayerNameOrDefault();
		}
		UILabel uILabel2 = tableFooterIndividuals.transform.Find("LabelClan").Map((Transform t) => t.gameObject.GetComponent<UILabel>());
		if (uILabel2 != null)
		{
			uILabel2.text = FriendsController.sharedController.clanName ?? string.Empty;
		}
	}

	private void FillIndividualItem(GameObject grid, List<LeaderboardItemViewModel> list, GridState state, int index, GameObject newItem)
	{
		if (index >= list.Count)
		{
			return;
		}
		if (index < 0)
		{
			string message = string.Format("Unexpected index {0} in list of {1} leaderboard items.", index, list.Count);
			Debug.LogError(message);
			return;
		}
		LeaderboardItemViewModel item = list[index];
		LeaderboardItemView component = newItem.GetComponent<LeaderboardItemView>();
		if (component != null)
		{
			component.NewReset(item);
		}
		component.Clicked += delegate(object sender, ClickedEventArgs e)
		{
			LeaderboardScript.PlayerClicked.Do(delegate(EventHandler<ClickedEventArgs> handler)
			{
				handler(this, e);
			});
			if (Application.isEditor && Defs.IsDeveloperBuild)
			{
				Debug.Log(string.Format("Clicked: {0}", e.Id));
			}
		};
		UILabel[] componentsInChildren = newItem.GetComponentsInChildren<UILabel>(true);
		Transform[] array = new Transform[3]
		{
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsFirstPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsSecondPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsThirdPlace")).Map((UILabel l) => l.transform)
		};
		for (int p2 = 0; p2 != array.Length; p2++)
		{
			array[p2].Do(delegate(Transform l)
			{
				l.gameObject.SetActive(p2 + 1 == item.Place && item.WinCount > 0);
			});
		}
		newItem.transform.Find("LabelsPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel p)
		{
			p.text = ((item.Place <= 3) ? string.Empty : item.Place.ToString(CultureInfo.InvariantCulture));
		});
	}

	private void FillClanItem(GameObject gridGo, List<LeaderboardItemViewModel> list, GridState state, int index, GameObject newItem)
	{
		if (index >= list.Count)
		{
			return;
		}
		LeaderboardItemViewModel item = list[index];
		LeaderboardItemView component = newItem.GetComponent<LeaderboardItemView>();
		if (component != null)
		{
			component.NewReset(item);
		}
		UILabel[] componentsInChildren = newItem.GetComponentsInChildren<UILabel>(true);
		Transform[] array = new Transform[3]
		{
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsFirstPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsSecondPlace")).Map((UILabel l) => l.transform),
			componentsInChildren.FirstOrDefault((UILabel l) => l.gameObject.name.Equals("LabelsThirdPlace")).Map((UILabel l) => l.transform)
		};
		for (int p2 = 0; p2 != array.Length; p2++)
		{
			array[p2].Do(delegate(Transform l)
			{
				l.gameObject.SetActive(p2 + 1 == item.Place);
			});
		}
		newItem.transform.Find("LabelsPlace").Map((Transform t) => t.gameObject.GetComponent<UILabel>()).Do(delegate(UILabel p)
		{
			p.text = ((item.Place <= 3) ? string.Empty : item.Place.ToString(CultureInfo.InvariantCulture));
		});
	}

	internal static void SetClanLogo(UITexture s, Texture2D clanLogoTexture)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		Texture mainTexture = s.mainTexture;
		s.mainTexture = ((!(clanLogoTexture != null)) ? null : UnityEngine.Object.Instantiate(clanLogoTexture));
		mainTexture.Do(UnityEngine.Object.Destroy);
	}

	internal static void SetClanLogo(UITexture s, string clanLogo)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		Texture mainTexture = s.mainTexture;
		if (string.IsNullOrEmpty(clanLogo))
		{
			s.mainTexture = null;
		}
		else
		{
			s.mainTexture = LeaderboardItemViewModel.CreateLogoFromBase64String(clanLogo);
		}
		mainTexture.Do(UnityEngine.Object.Destroy);
	}

	private static List<LeaderboardItemViewModel> GroupAndOrder(List<LeaderboardItemViewModel> items)
	{
		List<LeaderboardItemViewModel> list = new List<LeaderboardItemViewModel>();
		IOrderedEnumerable<IGrouping<int, LeaderboardItemViewModel>> orderedEnumerable = from vm in items
			group vm by vm.WinCount into g
			orderby g.Key descending
			select g;
		int num = 1;
		foreach (IGrouping<int, LeaderboardItemViewModel> item in orderedEnumerable)
		{
			IOrderedEnumerable<LeaderboardItemViewModel> orderedEnumerable2 = item.OrderByDescending((LeaderboardItemViewModel vm) => vm.Rank);
			foreach (LeaderboardItemViewModel item2 in orderedEnumerable2)
			{
				item2.Place = num;
				list.Add(item2);
			}
			num += item.Count();
		}
		return list;
	}

	internal static void RequestLeaderboards(string playerId)
	{
		if (string.IsNullOrEmpty(playerId))
		{
			throw new ArgumentException("Player id should not be empty", "playerId");
		}
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("Friends controller is null.");
			return;
		}
		if (_currentRequestPromise != null)
		{
			_currentRequestPromise.TrySetCanceled();
		}
		_currentRequestPromise = new TaskCompletionSource<string>();
		FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, _currentRequestPromise));
	}

	private void Awake()
	{
		if (backButton != null)
		{
			backButton.Clicked += ReturnBack;
		}
		if (loadingTexture != null)
		{
			loadingTexture.gameObject.SetActive(true);
			ActivityIndicator.IsActiveIndicator = true;
		}
		LeaderboardScript.PlayerClicked = (EventHandler<ClickedEventArgs>)Delegate.Combine(LeaderboardScript.PlayerClicked, new EventHandler<ClickedEventArgs>(HandlePlayerClicked));
		LocalizationStore.AddEventCallAfterLocalize(UpdateLocs);
	}

	private void HandlePlayerClicked(object sender, ClickedEventArgs e)
	{
		if (_leaderboardsPanel.Value == null)
		{
			Debug.LogError("Leaderboards panel not found.");
			return;
		}
		_leaderboardsPanel.Value.alpha = float.Epsilon;
		_leaderboardsPanel.Value.gameObject.SetActive(false);
		Action<bool> onCloseEvent = delegate
		{
			_leaderboardsPanel.Value.gameObject.SetActive(true);
			topFriendsGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			topPlayersGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			topClansGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			topFriendsGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			topPlayersGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			topClansGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			_leaderboardsPanel.Value.alpha = 1f;
			_profileIsOpened = false;
		};
		_profileIsOpened = true;
		FriendsController.ShowProfile(e.Id, ProfileWindowType.other, onCloseEvent);
	}

	private void OnDestroy()
	{
		if (_currentRequestPromise != null)
		{
			_currentRequestPromise.TrySetCanceled();
		}
		_currentRequestPromise = null;
		LeaderboardScript.PlayerClicked = null;
		FriendsController.DisposeProfile();
		_mainMenuController.Value.Do(delegate(MainMenuController m)
		{
			m.BackPressed -= ReturnBack;
		});
		LocalizationStore.DelEventCallAfterLocalize(UpdateLocs);
	}

	private IEnumerator Start()
	{
		if (FriendsController.sharedController == null)
		{
			Debug.LogError("Friends controller is null.");
			yield break;
		}
		string playerId = FriendsController.sharedController.id;
		if (string.IsNullOrEmpty(playerId))
		{
			Debug.LogError("Player id should not be null.");
			yield break;
		}
		if (_currentRequestPromise == null)
		{
			_currentRequestPromise = new TaskCompletionSource<string>();
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, _currentRequestPromise));
		}
		if (!CurrentRequest.IsCompleted)
		{
			string response2 = PlayerPrefs.GetString("Leaderboards.ResponseCache", string.Empty);
			if (string.IsNullOrEmpty(response2))
			{
				yield return StartCoroutine(FillGrids(string.Empty, playerId, _state));
			}
			else
			{
				_state = GridState.FillingWithCache;
				yield return StartCoroutine(FillGrids(response2, playerId, _state));
				_state = GridState.Cache;
			}
			if (loadingTexture != null)
			{
				loadingTexture.gameObject.SetActive(false);
				ActivityIndicator.IsActiveIndicator = false;
			}
		}
		while (!CurrentRequest.IsCompleted)
		{
			yield return null;
		}
		if (CurrentRequest.IsCanceled)
		{
			Debug.LogWarning("Request is cancelled.");
		}
		else if (CurrentRequest.IsFaulted)
		{
			Debug.LogException(CurrentRequest.Exception);
		}
		else
		{
			string response = CurrentRequest.Result;
			_state = GridState.FillingWithResponse;
			yield return StartCoroutine(FillGrids(response, playerId, _state));
			_state = GridState.Response;
		}
		if (loadingTexture != null)
		{
			loadingTexture.gameObject.SetActive(false);
			ActivityIndicator.IsActiveIndicator = false;
		}
	}

	private static string FormatExpirationLabel(float expirationTimespanSeconds)
	{
		if (expirationTimespanSeconds < 0f)
		{
			throw new ArgumentOutOfRangeException("expirationTimespanSeconds");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(expirationTimespanSeconds);
		try
		{
			return string.Format(LocalizationStore.Get("Key_1478"), Convert.ToInt32(Math.Floor(timeSpan.TotalDays)), timeSpan.Hours, timeSpan.Minutes);
		}
		catch
		{
			if (timeSpan.TotalHours < 1.0)
			{
				return string.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
			}
			if (timeSpan.TotalDays < 1.0)
			{
				return string.Format("{0}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			return string.Format("{0}d {1}:{2:00}:{3:00}", Convert.ToInt32(Math.Floor(timeSpan.TotalDays)), timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
	}

	private void Update()
	{
		if (!(Time.realtimeSinceStartup > _expirationNextUpateTimeSeconds))
		{
			return;
		}
		_expirationNextUpateTimeSeconds = Time.realtimeSinceStartup + 5f;
		if (!(expirationLabel != null))
		{
			return;
		}
		if (_state == GridState.Empty)
		{
			expirationLabel.text = LocalizationStore.Key_0348;
			return;
		}
		float num = _expirationTimeSeconds - Time.realtimeSinceStartup;
		if (num <= 0f)
		{
			expirationLabel.text = string.Empty;
		}
		else
		{
			expirationLabel.text = FormatExpirationLabel(num);
		}
	}

	private static IEnumerator LoadLeaderboardsCoroutine(string playerId, TaskCompletionSource<string> requestPromise)
	{
		if (!TrainingController.TrainingCompleted)
		{
			yield break;
		}
		if (requestPromise == null)
		{
			throw new ArgumentNullException("requestPromise");
		}
		if (requestPromise.Task.IsCanceled)
		{
			yield break;
		}
		if (string.IsNullOrEmpty(playerId))
		{
			throw new ArgumentException("Player id should not be null.", "playerId");
		}
		if (FriendsController.sharedController == null)
		{
			throw new InvalidOperationException("Friends controller should not be null.");
		}
		if (string.IsNullOrEmpty(FriendsController.sharedController.id))
		{
			Debug.LogWarning("Current player id is empty.");
			requestPromise.TrySetException(new InvalidOperationException("Current player id is empty."));
			yield break;
		}
		string leaderboardsResponseCacheTimestampString = PlayerPrefs.GetString("Leaderboards.ResponseCacheTimestamp", string.Empty);
		DateTime leaderboardsResponseCacheTimestamp;
		if (DateTime.TryParse(leaderboardsResponseCacheTimestampString, out leaderboardsResponseCacheTimestamp))
		{
			DateTime timeOfNnextRequest = leaderboardsResponseCacheTimestamp + TimeSpan.FromSeconds(Defs.pauseUpdateLeaderboard);
			float secondsTillNextRequest = (float)(timeOfNnextRequest - DateTime.UtcNow).TotalSeconds;
			if (secondsTillNextRequest > 3600f)
			{
				secondsTillNextRequest = 0f;
			}
			yield return new WaitForSeconds(secondsTillNextRequest);
		}
		if (requestPromise.Task.IsCanceled)
		{
			yield break;
		}
		WWWForm form = new WWWForm();
		form.AddField("action", "get_leaderboards_wins");
		form.AddField("app_version", string.Format("{0}:{1}", ProtocolListGetter.CurrentPlatform, GlobalGameController.AppVersion));
		form.AddField("id", playerId);
		form.AddField("uniq_id", FriendsController.sharedController.id);
		form.AddField("auth", FriendsController.Hash("get_leaderboards_wins"));
		WWW request = Tools.CreateWwwIfNotConnected(FriendsController.actionAddress, form, string.Empty);
		if (request == null)
		{
			requestPromise.TrySetException(new InvalidOperationException("Request forbidden while connected."));
			TaskCompletionSource<string> newRequestPromise = (_currentRequestPromise = new TaskCompletionSource<string>());
			yield return new WaitForSeconds(Defs.timeUpdateLeaderboardIfNullResponce);
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, newRequestPromise));
			yield break;
		}
		while (!request.isDone)
		{
			yield return null;
		}
		if (!string.IsNullOrEmpty(request.error))
		{
			requestPromise.TrySetException(new InvalidOperationException(request.error));
			Debug.LogError(request.error);
			TaskCompletionSource<string> newRequestPromise2 = (_currentRequestPromise = new TaskCompletionSource<string>());
			yield return new WaitForSeconds(Defs.timeUpdateLeaderboardIfNullResponce);
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, newRequestPromise2));
			yield break;
		}
		string responseText = URLs.Sanitize(request);
		if (string.IsNullOrEmpty(responseText))
		{
			requestPromise.TrySetException(new InvalidOperationException("Leaderboars response is empty."));
			Debug.LogWarning("Leaderboars response is empty.");
			TaskCompletionSource<string> newRequestPromise3 = (_currentRequestPromise = new TaskCompletionSource<string>());
			yield return new WaitForSeconds(Defs.timeUpdateLeaderboardIfNullResponce);
			FriendsController.sharedController.StartCoroutine(LoadLeaderboardsCoroutine(playerId, newRequestPromise3));
		}
		else
		{
			requestPromise.TrySetResult(responseText);
			PlayerPrefs.SetString("Leaderboards.ResponseCache", responseText);
			PlayerPrefs.SetString("Leaderboards.ResponseCacheTimestamp", DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture));
		}
	}

	public Task GetReturnFuture()
	{
		if (_returnPromise.Task.IsCompleted)
		{
			_returnPromise = new TaskCompletionSource<bool>();
		}
		_mainMenuController.Value.Do(delegate(MainMenuController m)
		{
			m.BackPressed -= ReturnBack;
		});
		_mainMenuController.Value.Do(delegate(MainMenuController m)
		{
			m.BackPressed += ReturnBack;
		});
		return _returnPromise.Task;
	}

	private void ReturnBack(object sender, EventArgs e)
	{
		if (!_profileIsOpened)
		{
			topFriendsGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			topPlayersGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			topClansGrid.Map((GameObject go) => go.GetComponent<UIWrapContent>()).Do(delegate(UIWrapContent w)
			{
				w.SortBasedOnScrollMovement();
				w.WrapContent();
			});
			topFriendsGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			topPlayersGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			topClansGrid.Map((GameObject go) => go.GetComponentInParent<UIScrollView>()).Do(delegate(UIScrollView s)
			{
				s.ResetPosition();
				s.UpdatePosition();
			});
			_returnPromise.TrySetResult(true);
			_mainMenuController.Value.Do(delegate(MainMenuController m)
			{
				m.BackPressed -= ReturnBack;
			});
		}
	}
}
