using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rilisoft;
using Rilisoft.NullExtensions;
using UnityEngine;

public sealed class LeaderboardsView : MonoBehaviour
{
	public enum State
	{
		None = 0,
		Clans = 1,
		Friends = 2,
		BestPlayers = 3,
		Default = 3
	}

	internal const string LeaderboardsTabCache = "Leaderboards.TabCache";

	public UIWrapContent clansGrid;

	public UIWrapContent friendsGrid;

	public UIWrapContent bestPlayersGrid;

	public ButtonHandler backButton;

	public UIButton clansButton;

	public UIButton friendsButton;

	public UIButton bestPlayersButton;

	public UIDragScrollView clansPanel;

	public UIDragScrollView friendsPanel;

	public UIDragScrollView bestPlayersPanel;

	public UIScrollView clansScroll;

	public UIScrollView friendsScroll;

	public UIScrollView bestPlayersScroll;

	public GameObject defaultTableHeader;

	public GameObject clansTableHeader;

	private Lazy<GameObject> _tableFooterClan;

	private Lazy<GameObject> _tableFooterPlayer;

	private readonly Lazy<UIPanel> _leaderboardsPanel;

	private bool _prepared;

	private State _currentState;

	public State CurrentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			if (_currentState == value)
			{
				return;
			}
			PlayerPrefs.SetInt("Leaderboards.TabCache", (int)value);
			if (clansButton != null)
			{
				clansButton.isEnabled = value != State.Clans;
				Transform transform = clansButton.gameObject.transform.Find("SpriteLabel");
				if (transform != null)
				{
					transform.gameObject.SetActive(value != State.Clans);
				}
				Transform transform2 = clansButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform2 != null)
				{
					transform2.gameObject.SetActive(value == State.Clans);
				}
			}
			if (friendsButton != null)
			{
				friendsButton.isEnabled = value != State.Friends;
				Transform transform3 = friendsButton.gameObject.transform.Find("SpriteLabel");
				if (transform3 != null)
				{
					transform3.gameObject.SetActive(value != State.Friends);
				}
				Transform transform4 = friendsButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform4 != null)
				{
					transform4.gameObject.SetActive(value == State.Friends);
				}
			}
			if (bestPlayersButton != null)
			{
				bestPlayersButton.isEnabled = value != State.BestPlayers;
				Transform transform5 = bestPlayersButton.gameObject.transform.Find("SpriteLabel");
				if (transform5 != null)
				{
					transform5.gameObject.SetActive(value != State.BestPlayers);
				}
				Transform transform6 = bestPlayersButton.gameObject.transform.Find("ChekmarkLabel");
				if (transform6 != null)
				{
					transform6.gameObject.SetActive(value == State.BestPlayers);
				}
			}
			if (defaultTableHeader != null)
			{
				defaultTableHeader.SetActive(value != State.Clans);
				_tableFooterPlayer.Value.Do(delegate(GameObject f)
				{
					f.SetActive(value == State.BestPlayers);
				});
			}
			if (clansTableHeader != null)
			{
				clansTableHeader.SetActive(value == State.Clans);
				string clanId = FriendsController.sharedController.Map((FriendsController c) => c.ClanID);
				_tableFooterClan.Value.Do(delegate(GameObject f)
				{
					f.SetActive(!string.IsNullOrEmpty(clanId));
				});
			}
			bestPlayersGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition3 = g.transform.localPosition;
				localPosition3.x = ((value != State.BestPlayers) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition3;
				if (!g.gameObject.activeInHierarchy)
				{
				}
			});
			friendsGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition2 = g.transform.localPosition;
				localPosition2.x = ((value != State.Friends) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition2;
				if (!g.gameObject.activeInHierarchy)
				{
				}
			});
			clansGrid.Do(delegate(UIWrapContent g)
			{
				Vector3 localPosition = g.transform.localPosition;
				localPosition.x = ((value != State.Clans) ? 9000f : 0f);
				g.gameObject.transform.localPosition = localPosition;
				if (!g.gameObject.activeInHierarchy)
				{
				}
			});
			_currentState = value;
		}
	}

	internal bool Prepared
	{
		get
		{
			return _prepared;
		}
	}

	public event EventHandler BackPressed;

	public LeaderboardsView()
	{
		_tableFooterClan = new Lazy<GameObject>(FindTableFooterClan);
		_tableFooterPlayer = new Lazy<GameObject>(FindTableFooterPlayer);
		_leaderboardsPanel = new Lazy<UIPanel>(base.GetComponent<UIPanel>);
	}

	private GameObject FindTableFooterClan()
	{
		return clansTableHeader.Map((GameObject h) => h.transform.Find("TableFooterClans")).Map((Transform t) => t.gameObject);
	}

	private GameObject FindTableFooterPlayer()
	{
		return defaultTableHeader.Map((GameObject h) => h.transform.Find("TableFooterPlayer")).Map((Transform t) => t.gameObject);
	}

	private void RefreshGrid(UIGrid grid)
	{
		grid.Reposition();
	}

	private IEnumerator SkipFrameAndExecuteCoroutine(Action a)
	{
		if (a != null)
		{
			yield return new WaitForEndOfFrame();
			a();
		}
	}

	private void HandleTabPressed(object sender, EventArgs e)
	{
		GameObject gameObject = ((ButtonHandler)sender).gameObject;
		if (clansButton != null && gameObject == clansButton.gameObject)
		{
			CurrentState = State.Clans;
		}
		else if (friendsButton != null && gameObject == friendsButton.gameObject)
		{
			CurrentState = State.Friends;
		}
		else if (bestPlayersButton != null && gameObject == bestPlayersButton.gameObject)
		{
			CurrentState = State.BestPlayers;
		}
	}

	private void RaiseBackPressed(object sender, EventArgs e)
	{
		EventHandler backPressed = this.BackPressed;
		if (backPressed != null)
		{
			backPressed(sender, e);
		}
	}

	private static IEnumerator SetGrid(UIGrid grid, IList<LeaderboardItemViewModel> value, string itemPrefabPath)
	{
		if (string.IsNullOrEmpty(itemPrefabPath))
		{
			throw new ArgumentException("itemPrefabPath");
		}
		if (!(grid != null))
		{
			yield break;
		}
		while (!grid.gameObject.activeInHierarchy)
		{
			yield return null;
		}
		IEnumerable<LeaderboardItemViewModel> enumerable2;
		if (value == null)
		{
			IEnumerable<LeaderboardItemViewModel> enumerable = new List<LeaderboardItemViewModel>();
			enumerable2 = enumerable;
		}
		else
		{
			enumerable2 = value.Where((LeaderboardItemViewModel it) => it != null);
		}
		IEnumerable<LeaderboardItemViewModel> filteredList = enumerable2;
		List<Transform> list = grid.GetChildList();
		for (int i = 0; i != list.Count; i++)
		{
			UnityEngine.Object.Destroy(list[i].gameObject);
		}
		list.Clear();
		grid.Reposition();
		foreach (LeaderboardItemViewModel item in filteredList)
		{
			GameObject o = UnityEngine.Object.Instantiate(Resources.Load(itemPrefabPath)) as GameObject;
			if (o != null)
			{
				LeaderboardItemView liv = o.GetComponent<LeaderboardItemView>();
				if (liv != null)
				{
					liv.Reset(item);
					o.transform.parent = grid.transform;
					grid.AddChild(o.transform);
					o.transform.localScale = Vector3.one;
				}
			}
		}
		grid.Reposition();
		UIScrollView scrollView = grid.transform.parent.gameObject.GetComponent<UIScrollView>();
		if (scrollView != null)
		{
			scrollView.enabled = true;
			yield return null;
			scrollView.ResetPosition();
			scrollView.UpdatePosition();
			yield return null;
			scrollView.enabled = value.Count >= 10;
		}
	}

	private IEnumerator UpdateGridsAndScrollers()
	{
		_prepared = false;
		yield return new WaitForEndOfFrame();
		IEnumerable<UIWrapContent> wraps = new UIWrapContent[3] { friendsGrid, bestPlayersGrid, clansGrid }.Where((UIWrapContent g) => g != null);
		foreach (UIWrapContent w in wraps)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		}
		yield return null;
		IEnumerable<UIScrollView> scrolls = new UIScrollView[3] { clansScroll, friendsScroll, bestPlayersScroll }.Where((UIScrollView s) => s != null);
		foreach (UIScrollView s2 in scrolls)
		{
			s2.ResetPosition();
			s2.UpdatePosition();
		}
		_prepared = true;
	}

	private void OnDestroy()
	{
		if (backButton != null)
		{
			backButton.Clicked -= RaiseBackPressed;
		}
	}

	private void OnEnable()
	{
		StartCoroutine(UpdateGridsAndScrollers());
	}

	private void OnDisable()
	{
		_prepared = false;
	}

	private void Awake()
	{
		List<UIWrapContent> list = new UIWrapContent[1] { friendsGrid }.Where((UIWrapContent g) => g != null).ToList();
		foreach (UIWrapContent item in list)
		{
			item.gameObject.SetActive(true);
			Vector3 localPosition = item.transform.localPosition;
			localPosition.x = 9000f;
			item.gameObject.transform.localPosition = localPosition;
		}
		List<UIWrapContent> list2 = new UIWrapContent[2] { bestPlayersGrid, clansGrid }.Where((UIWrapContent g) => g != null).ToList();
		foreach (UIWrapContent item2 in list2)
		{
			item2.gameObject.SetActive(true);
			Vector3 localPosition2 = item2.transform.localPosition;
			localPosition2.x = 9000f;
			item2.gameObject.transform.localPosition = localPosition2;
		}
	}

	private IEnumerator Start()
	{
		IEnumerable<UIButton> buttons = new UIButton[3] { clansButton, friendsButton, bestPlayersButton }.Where((UIButton b) => b != null);
		foreach (UIButton b2 in buttons)
		{
			ButtonHandler bh = b2.GetComponent<ButtonHandler>();
			if (bh != null)
			{
				bh.Clicked += HandleTabPressed;
			}
		}
		if (backButton != null)
		{
			backButton.Clicked += RaiseBackPressed;
		}
		IEnumerable<UIScrollView> scrollViews = new UIScrollView[3] { clansScroll, friendsScroll, bestPlayersScroll }.Where((UIScrollView s) => s != null);
		foreach (UIScrollView scrollView in scrollViews)
		{
			scrollView.ResetPosition();
		}
		yield return null;
		friendsGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		bestPlayersGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		clansGrid.Do(delegate(UIWrapContent w)
		{
			w.SortBasedOnScrollMovement();
			w.WrapContent();
		});
		yield return null;
		int stateInt = PlayerPrefs.GetInt("Leaderboards.TabCache", 3);
		State state = ((!Enum.IsDefined(typeof(State), stateInt)) ? State.BestPlayers : ((State)stateInt));
		CurrentState = ((state == State.None) ? State.BestPlayers : state);
	}
}
