using System;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames.BasicApi.Quests;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.Native.Cwrapper;
using GooglePlayGames.Native.PInvoke;
using GooglePlayGames.OurUtils;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace GooglePlayGames.Native
{
	public class NativeClient : IPlayGamesClient
	{
		private enum AuthState
		{
			Unauthenticated = 0,
			Authenticated = 1,
			SilentPending = 2
		}

		private readonly IClientImpl clientImpl;

		private readonly object GameServicesLock = new object();

		private readonly object AuthStateLock = new object();

		private readonly PlayGamesClientConfiguration mConfiguration;

		private GooglePlayGames.Native.PInvoke.GameServices mServices;

		private volatile NativeTurnBasedMultiplayerClient mTurnBasedClient;

		private volatile NativeRealtimeMultiplayerClient mRealTimeClient;

		private volatile ISavedGameClient mSavedGameClient;

		private volatile IEventsClient mEventsClient;

		private volatile IQuestsClient mQuestsClient;

		private volatile TokenClient mTokenClient;

		private volatile Action<Invitation, bool> mInvitationDelegate;

		private volatile Dictionary<string, GooglePlayGames.BasicApi.Achievement> mAchievements;

		private volatile GooglePlayGames.BasicApi.Multiplayer.Player mUser;

		private volatile List<GooglePlayGames.BasicApi.Multiplayer.Player> mFriends;

		private volatile Action<bool> mPendingAuthCallbacks;

		private volatile Action<bool> mSilentAuthCallbacks;

		private volatile AuthState mAuthState;

		private volatile uint mAuthGeneration;

		private volatile bool mSilentAuthFailed;

		private volatile bool friendsLoading;

		private string rationale;

		private int webclientWarningFreq = 100000;

		private int noWebClientIdWarningCount;

		internal NativeClient(PlayGamesClientConfiguration configuration, IClientImpl clientImpl)
		{
			PlayGamesHelperObject.CreateObject();
			mConfiguration = Misc.CheckNotNull(configuration);
			this.clientImpl = clientImpl;
			rationale = configuration.PermissionRationale;
		}

		private GooglePlayGames.Native.PInvoke.GameServices GameServices()
		{
			lock (GameServicesLock)
			{
				return mServices;
			}
		}

		public void Authenticate(Action<bool> callback, bool silent)
		{
			lock (AuthStateLock)
			{
				if (mAuthState == AuthState.Authenticated)
				{
					InvokeCallbackOnGameThread(callback, true);
					return;
				}
				if (mSilentAuthFailed && silent)
				{
					InvokeCallbackOnGameThread(callback, false);
					return;
				}
				if (callback != null)
				{
					if (silent)
					{
						mSilentAuthCallbacks = (Action<bool>)Delegate.Combine(mSilentAuthCallbacks, callback);
					}
					else
					{
						mPendingAuthCallbacks = (Action<bool>)Delegate.Combine(mPendingAuthCallbacks, callback);
					}
				}
			}
			InitializeGameServices();
			if (!silent)
			{
				GameServices().StartAuthorizationUI();
			}
		}

		private static Action<T> AsOnGameThreadCallback<T>(Action<T> callback)
		{
			if (callback == null)
			{
				return delegate
				{
				};
			}
			return delegate(T result)
			{
				InvokeCallbackOnGameThread(callback, result);
			};
		}

		private static void InvokeCallbackOnGameThread<T>(Action<T> callback, T data)
		{
			if (callback != null)
			{
				PlayGamesHelperObject.RunOnGameThread(delegate
				{
					GooglePlayGames.OurUtils.Logger.d("Invoking user callback on game thread");
					callback(data);
				});
			}
		}

		private void InitializeGameServices()
		{
			lock (GameServicesLock)
			{
				if (mServices != null)
				{
					return;
				}
				using (GameServicesBuilder gameServicesBuilder = GameServicesBuilder.Create())
				{
					using (PlatformConfiguration configRef = clientImpl.CreatePlatformConfiguration())
					{
						RegisterInvitationDelegate(mConfiguration.InvitationDelegate);
						gameServicesBuilder.SetOnAuthFinishedCallback(HandleAuthTransition);
						gameServicesBuilder.SetOnTurnBasedMatchEventCallback(delegate(GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent eventType, string matchId, NativeTurnBasedMatch match)
						{
							mTurnBasedClient.HandleMatchEvent(eventType, matchId, match);
						});
						gameServicesBuilder.SetOnMultiplayerInvitationEventCallback(HandleInvitation);
						if (mConfiguration.EnableSavedGames)
						{
							gameServicesBuilder.EnableSnapshots();
						}
						Debug.Log("Building GPG services, implicitly attempts silent auth");
						mAuthState = AuthState.SilentPending;
						mServices = gameServicesBuilder.Build(configRef);
						mEventsClient = new NativeEventClient(new GooglePlayGames.Native.PInvoke.EventManager(mServices));
						mQuestsClient = new NativeQuestClient(new GooglePlayGames.Native.PInvoke.QuestManager(mServices));
						mTurnBasedClient = new NativeTurnBasedMultiplayerClient(this, new TurnBasedManager(mServices));
						mTurnBasedClient.RegisterMatchDelegate(mConfiguration.MatchDelegate);
						mRealTimeClient = new NativeRealtimeMultiplayerClient(this, new RealtimeManager(mServices));
						if (mConfiguration.EnableSavedGames)
						{
							mSavedGameClient = new NativeSavedGameClient(new GooglePlayGames.Native.PInvoke.SnapshotManager(mServices));
						}
						else
						{
							mSavedGameClient = new UnsupportedSavedGamesClient("You must enable saved games before it can be used. See PlayGamesClientConfiguration.Builder.EnableSavedGames.");
						}
						mAuthState = AuthState.SilentPending;
						mTokenClient = clientImpl.CreateTokenClient(false);
					}
				}
			}
		}

		internal void HandleInvitation(GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent eventType, string invitationId, GooglePlayGames.Native.PInvoke.MultiplayerInvitation invitation)
		{
			Action<Invitation, bool> action = mInvitationDelegate;
			if (action == null)
			{
				GooglePlayGames.OurUtils.Logger.d(string.Concat("Received ", eventType, " for invitation ", invitationId, " but no handler was registered."));
			}
			else if (eventType == GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent.REMOVED)
			{
				GooglePlayGames.OurUtils.Logger.d("Ignoring REMOVED for invitation " + invitationId);
			}
			else
			{
				bool arg = eventType == GooglePlayGames.Native.Cwrapper.Types.MultiplayerEvent.UPDATED_FROM_APP_LAUNCH;
				action(invitation.AsInvitation(), arg);
			}
		}

		public string GetUserEmail()
		{
			if (!IsAuthenticated())
			{
				Debug.Log("Cannot get API client - not authenticated");
				return null;
			}
			if (!GameInfo.WebClientIdInitialized())
			{
				if (noWebClientIdWarningCount++ % webclientWarningFreq == 0)
				{
					Debug.LogError("Web client ID has not been set, cannot request email.");
					noWebClientIdWarningCount = noWebClientIdWarningCount / webclientWarningFreq + 1;
				}
				return null;
			}
			mTokenClient.SetRationale(rationale);
			return mTokenClient.GetEmail();
		}

		public string GetAccessToken()
		{
			if (!IsAuthenticated())
			{
				Debug.Log("Cannot get API client - not authenticated");
				return null;
			}
			if (!GameInfo.WebClientIdInitialized())
			{
				if (noWebClientIdWarningCount++ % webclientWarningFreq == 0)
				{
					Debug.LogError("Web client ID has not been set, cannot request access token.");
					noWebClientIdWarningCount = noWebClientIdWarningCount / webclientWarningFreq + 1;
				}
				return null;
			}
			mTokenClient.SetRationale(rationale);
			return mTokenClient.GetAccessToken();
		}

		public string GetIdToken()
		{
			if (!IsAuthenticated())
			{
				Debug.Log("Cannot get API client - not authenticated");
				return null;
			}
			if (!GameInfo.WebClientIdInitialized())
			{
				if (noWebClientIdWarningCount++ % webclientWarningFreq == 0)
				{
					Debug.LogError("Web client ID has not been set, cannot request id token.");
					noWebClientIdWarningCount = noWebClientIdWarningCount / webclientWarningFreq + 1;
				}
				return null;
			}
			mTokenClient.SetRationale(rationale);
			return mTokenClient.GetIdToken(string.Empty);
		}

		public bool IsAuthenticated()
		{
			lock (AuthStateLock)
			{
				return mAuthState == AuthState.Authenticated;
			}
		}

		public void LoadFriends(Action<bool> callback)
		{
			if (!IsAuthenticated())
			{
				GooglePlayGames.OurUtils.Logger.d("Cannot loadFriends when not authenticated");
				callback(false);
				return;
			}
			mServices.PlayerManager().FetchFriends(delegate(ResponseStatus status, List<GooglePlayGames.BasicApi.Multiplayer.Player> players)
			{
				if (status == ResponseStatus.Success || status == ResponseStatus.SuccessWithStale)
				{
					mFriends = players;
					callback(true);
				}
				else
				{
					mFriends = new List<GooglePlayGames.BasicApi.Multiplayer.Player>();
					GooglePlayGames.OurUtils.Logger.e(string.Concat("Got ", status, " loading friends"));
					callback(false);
				}
			});
		}

		public IUserProfile[] GetFriends()
		{
			if (mFriends == null && !friendsLoading)
			{
				GooglePlayGames.OurUtils.Logger.w("Getting friends before they are loaded!!!");
				friendsLoading = true;
				LoadFriends(delegate(bool ok)
				{
					GooglePlayGames.OurUtils.Logger.d("loading: " + ok + " mFriends = " + mFriends);
					friendsLoading = false;
				});
			}
			return (mFriends != null) ? mFriends.ToArray() : new IUserProfile[0];
		}

		private void PopulateAchievements(uint authGeneration, GooglePlayGames.Native.PInvoke.AchievementManager.FetchAllResponse response)
		{
			if (authGeneration != mAuthGeneration)
			{
				GooglePlayGames.OurUtils.Logger.d("Received achievement callback after signout occurred, ignoring");
				return;
			}
			GooglePlayGames.OurUtils.Logger.d("Populating Achievements, status = " + response.Status());
			lock (AuthStateLock)
			{
				if (response.Status() != CommonErrorStatus.ResponseStatus.VALID && response.Status() != CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
				{
					GooglePlayGames.OurUtils.Logger.e("Error retrieving achievements - check the log for more information. Failing signin.");
					Action<bool> action = mPendingAuthCallbacks;
					mPendingAuthCallbacks = null;
					if (action != null)
					{
						InvokeCallbackOnGameThread(action, false);
					}
					SignOut();
					return;
				}
				Dictionary<string, GooglePlayGames.BasicApi.Achievement> dictionary = new Dictionary<string, GooglePlayGames.BasicApi.Achievement>();
				foreach (NativeAchievement item in response)
				{
					using (item)
					{
						dictionary[item.Id()] = item.AsAchievement();
					}
				}
				GooglePlayGames.OurUtils.Logger.d("Found " + dictionary.Count + " Achievements");
				mAchievements = dictionary;
			}
			GooglePlayGames.OurUtils.Logger.d("Maybe finish for Achievements");
			MaybeFinishAuthentication();
		}

		private void MaybeFinishAuthentication()
		{
			Action<bool> action = null;
			lock (AuthStateLock)
			{
				if (mUser == null || mAchievements == null)
				{
					GooglePlayGames.OurUtils.Logger.d(string.Concat("Auth not finished. User=", mUser, " achievements=", mAchievements));
					return;
				}
				GooglePlayGames.OurUtils.Logger.d("Auth finished. Proceeding.");
				action = mPendingAuthCallbacks;
				mPendingAuthCallbacks = null;
				mAuthState = AuthState.Authenticated;
			}
			if (action != null)
			{
				GooglePlayGames.OurUtils.Logger.d("Invoking Callbacks: " + action);
				InvokeCallbackOnGameThread(action, true);
			}
		}

		private void PopulateUser(uint authGeneration, GooglePlayGames.Native.PInvoke.PlayerManager.FetchSelfResponse response)
		{
			GooglePlayGames.OurUtils.Logger.d("Populating User");
			if (authGeneration != mAuthGeneration)
			{
				GooglePlayGames.OurUtils.Logger.d("Received user callback after signout occurred, ignoring");
				return;
			}
			lock (AuthStateLock)
			{
				if (response.Status() != CommonErrorStatus.ResponseStatus.VALID && response.Status() != CommonErrorStatus.ResponseStatus.VALID_BUT_STALE)
				{
					GooglePlayGames.OurUtils.Logger.e("Error retrieving user, signing out");
					Action<bool> action = mPendingAuthCallbacks;
					mPendingAuthCallbacks = null;
					if (action != null)
					{
						InvokeCallbackOnGameThread(action, false);
					}
					SignOut();
					return;
				}
				mUser = response.Self().AsPlayer();
				mFriends = null;
			}
			GooglePlayGames.OurUtils.Logger.d("Found User: " + mUser);
			GooglePlayGames.OurUtils.Logger.d("Maybe finish for User");
			MaybeFinishAuthentication();
		}

		private void HandleAuthTransition(GooglePlayGames.Native.Cwrapper.Types.AuthOperation operation, CommonErrorStatus.AuthStatus status)
		{
			GooglePlayGames.OurUtils.Logger.d(string.Concat("Starting Auth Transition. Op: ", operation, " status: ", status));
			lock (AuthStateLock)
			{
				switch (operation)
				{
				case GooglePlayGames.Native.Cwrapper.Types.AuthOperation.SIGN_IN:
					if (status == CommonErrorStatus.AuthStatus.VALID)
					{
						if (mSilentAuthCallbacks != null)
						{
							mPendingAuthCallbacks = (Action<bool>)Delegate.Combine(mPendingAuthCallbacks, mSilentAuthCallbacks);
							mSilentAuthCallbacks = null;
						}
						uint currentAuthGeneration = mAuthGeneration;
						mServices.AchievementManager().FetchAll(delegate(GooglePlayGames.Native.PInvoke.AchievementManager.FetchAllResponse results)
						{
							PopulateAchievements(currentAuthGeneration, results);
						});
						mServices.PlayerManager().FetchSelf(delegate(GooglePlayGames.Native.PInvoke.PlayerManager.FetchSelfResponse results)
						{
							PopulateUser(currentAuthGeneration, results);
						});
					}
					else if (mAuthState == AuthState.SilentPending)
					{
						mSilentAuthFailed = true;
						mAuthState = AuthState.Unauthenticated;
						Action<bool> callback = mSilentAuthCallbacks;
						mSilentAuthCallbacks = null;
						Debug.Log("Invoking callbacks, AuthState changed from silentPending to Unauthenticated.");
						InvokeCallbackOnGameThread(callback, false);
						if (mPendingAuthCallbacks != null)
						{
							Debug.Log("there are pending auth callbacks - starting AuthUI");
							GameServices().StartAuthorizationUI();
						}
					}
					else
					{
						Debug.Log(string.Concat("AuthState == ", mAuthState, " calling auth callbacks with failure"));
						Action<bool> callback2 = mPendingAuthCallbacks;
						mPendingAuthCallbacks = null;
						InvokeCallbackOnGameThread(callback2, false);
					}
					break;
				case GooglePlayGames.Native.Cwrapper.Types.AuthOperation.SIGN_OUT:
					ToUnauthenticated();
					break;
				default:
					GooglePlayGames.OurUtils.Logger.e("Unknown AuthOperation " + operation);
					break;
				}
			}
		}

		private void ToUnauthenticated()
		{
			lock (AuthStateLock)
			{
				mUser = null;
				mFriends = null;
				mAchievements = null;
				mAuthState = AuthState.Unauthenticated;
				mTokenClient = clientImpl.CreateTokenClient(true);
				mAuthGeneration++;
			}
		}

		public void SignOut()
		{
			ToUnauthenticated();
			if (GameServices() != null)
			{
				GameServices().SignOut();
			}
		}

		public string GetUserId()
		{
			if (mUser == null)
			{
				return null;
			}
			return mUser.id;
		}

		public string GetUserDisplayName()
		{
			if (mUser == null)
			{
				return null;
			}
			return mUser.userName;
		}

		public string GetUserImageUrl()
		{
			if (mUser == null)
			{
				return null;
			}
			return mUser.AvatarURL;
		}

		public void GetPlayerStats(Action<CommonStatusCodes, PlayGamesLocalUser.PlayerStats> callback)
		{
			clientImpl.GetPlayerStats(GetApiClient(), callback);
		}

		public void LoadUsers(string[] userIds, Action<IUserProfile[]> callback)
		{
			mServices.PlayerManager().FetchList(userIds, delegate(NativePlayer[] nativeUsers)
			{
				IUserProfile[] array = new IUserProfile[nativeUsers.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = nativeUsers[i].AsPlayer();
				}
				callback(array);
			});
		}

		public GooglePlayGames.BasicApi.Achievement GetAchievement(string achId)
		{
			if (mAchievements == null || !mAchievements.ContainsKey(achId))
			{
				return null;
			}
			return mAchievements[achId];
		}

		public void LoadAchievements(Action<GooglePlayGames.BasicApi.Achievement[]> callback)
		{
			GooglePlayGames.BasicApi.Achievement[] array = new GooglePlayGames.BasicApi.Achievement[mAchievements.Count];
			mAchievements.Values.CopyTo(array, 0);
			callback(array);
		}

		public void UnlockAchievement(string achId, Action<bool> callback)
		{
			UpdateAchievement("Unlock", achId, callback, (GooglePlayGames.BasicApi.Achievement a) => a.IsUnlocked, delegate(GooglePlayGames.BasicApi.Achievement a)
			{
				a.IsUnlocked = true;
				GameServices().AchievementManager().Unlock(achId);
			});
		}

		public void RevealAchievement(string achId, Action<bool> callback)
		{
			UpdateAchievement("Reveal", achId, callback, (GooglePlayGames.BasicApi.Achievement a) => a.IsRevealed, delegate(GooglePlayGames.BasicApi.Achievement a)
			{
				a.IsRevealed = true;
				GameServices().AchievementManager().Reveal(achId);
			});
		}

		private void UpdateAchievement(string updateType, string achId, Action<bool> callback, Predicate<GooglePlayGames.BasicApi.Achievement> alreadyDone, Action<GooglePlayGames.BasicApi.Achievement> updateAchievment)
		{
			callback = AsOnGameThreadCallback(callback);
			Misc.CheckNotNull(achId);
			InitializeGameServices();
			GooglePlayGames.BasicApi.Achievement achievement = GetAchievement(achId);
			if (achievement == null)
			{
				GooglePlayGames.OurUtils.Logger.d("Could not " + updateType + ", no achievement with ID " + achId);
				callback(false);
				return;
			}
			if (alreadyDone(achievement))
			{
				GooglePlayGames.OurUtils.Logger.d("Did not need to perform " + updateType + ": on achievement " + achId);
				callback(true);
				return;
			}
			GooglePlayGames.OurUtils.Logger.d("Performing " + updateType + " on " + achId);
			updateAchievment(achievement);
			GameServices().AchievementManager().Fetch(achId, delegate(GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse rsp)
			{
				if (rsp.Status() == CommonErrorStatus.ResponseStatus.VALID)
				{
					mAchievements.Remove(achId);
					mAchievements.Add(achId, rsp.Achievement().AsAchievement());
					callback(true);
				}
				else
				{
					GooglePlayGames.OurUtils.Logger.e("Cannot refresh achievement " + achId + ": " + rsp.Status());
					callback(false);
				}
			});
		}

		public void IncrementAchievement(string achId, int steps, Action<bool> callback)
		{
			Misc.CheckNotNull(achId);
			callback = AsOnGameThreadCallback(callback);
			InitializeGameServices();
			GooglePlayGames.BasicApi.Achievement achievement = GetAchievement(achId);
			if (achievement == null)
			{
				GooglePlayGames.OurUtils.Logger.e("Could not increment, no achievement with ID " + achId);
				callback(false);
				return;
			}
			if (!achievement.IsIncremental)
			{
				GooglePlayGames.OurUtils.Logger.e("Could not increment, achievement with ID " + achId + " was not incremental");
				callback(false);
				return;
			}
			if (steps < 0)
			{
				GooglePlayGames.OurUtils.Logger.e("Attempted to increment by negative steps");
				callback(false);
				return;
			}
			GameServices().AchievementManager().Increment(achId, Convert.ToUInt32(steps));
			GameServices().AchievementManager().Fetch(achId, delegate(GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse rsp)
			{
				if (rsp.Status() == CommonErrorStatus.ResponseStatus.VALID)
				{
					mAchievements.Remove(achId);
					mAchievements.Add(achId, rsp.Achievement().AsAchievement());
					callback(true);
				}
				else
				{
					GooglePlayGames.OurUtils.Logger.e("Cannot refresh achievement " + achId + ": " + rsp.Status());
					callback(false);
				}
			});
		}

		public void SetStepsAtLeast(string achId, int steps, Action<bool> callback)
		{
			Misc.CheckNotNull(achId);
			callback = AsOnGameThreadCallback(callback);
			InitializeGameServices();
			GooglePlayGames.BasicApi.Achievement achievement = GetAchievement(achId);
			if (achievement == null)
			{
				GooglePlayGames.OurUtils.Logger.e("Could not increment, no achievement with ID " + achId);
				callback(false);
				return;
			}
			if (!achievement.IsIncremental)
			{
				GooglePlayGames.OurUtils.Logger.e("Could not increment, achievement with ID " + achId + " is not incremental");
				callback(false);
				return;
			}
			if (steps < 0)
			{
				GooglePlayGames.OurUtils.Logger.e("Attempted to increment by negative steps");
				callback(false);
				return;
			}
			GameServices().AchievementManager().SetStepsAtLeast(achId, Convert.ToUInt32(steps));
			GameServices().AchievementManager().Fetch(achId, delegate(GooglePlayGames.Native.PInvoke.AchievementManager.FetchResponse rsp)
			{
				if (rsp.Status() == CommonErrorStatus.ResponseStatus.VALID)
				{
					mAchievements.Remove(achId);
					mAchievements.Add(achId, rsp.Achievement().AsAchievement());
					callback(true);
				}
				else
				{
					GooglePlayGames.OurUtils.Logger.e("Cannot refresh achievement " + achId + ": " + rsp.Status());
					callback(false);
				}
			});
		}

		public void ShowAchievementsUI(Action<UIStatus> cb)
		{
			if (!IsAuthenticated())
			{
				return;
			}
			Action<CommonErrorStatus.UIStatus> callback = Callbacks.NoopUICallback;
			if (cb != null)
			{
				callback = delegate(CommonErrorStatus.UIStatus result)
				{
					cb((UIStatus)result);
				};
			}
			callback = AsOnGameThreadCallback(callback);
			GameServices().AchievementManager().ShowAllUI(callback);
		}

		public int LeaderboardMaxResults()
		{
			return GameServices().LeaderboardManager().LeaderboardMaxResults;
		}

		public void ShowLeaderboardUI(string leaderboardId, LeaderboardTimeSpan span, Action<UIStatus> cb)
		{
			if (!IsAuthenticated())
			{
				return;
			}
			Action<CommonErrorStatus.UIStatus> callback = Callbacks.NoopUICallback;
			if (cb != null)
			{
				callback = delegate(CommonErrorStatus.UIStatus result)
				{
					cb((UIStatus)result);
				};
			}
			callback = AsOnGameThreadCallback(callback);
			if (leaderboardId == null)
			{
				GameServices().LeaderboardManager().ShowAllUI(callback);
			}
			else
			{
				GameServices().LeaderboardManager().ShowUI(leaderboardId, span, callback);
			}
		}

		public void LoadScores(string leaderboardId, LeaderboardStart start, int rowCount, LeaderboardCollection collection, LeaderboardTimeSpan timeSpan, Action<LeaderboardScoreData> callback)
		{
			GameServices().LeaderboardManager().LoadLeaderboardData(leaderboardId, start, rowCount, collection, timeSpan, mUser.id, callback);
		}

		public void LoadMoreScores(ScorePageToken token, int rowCount, Action<LeaderboardScoreData> callback)
		{
			GameServices().LeaderboardManager().LoadScorePage(null, rowCount, token, callback);
		}

		public void SubmitScore(string leaderboardId, long score, Action<bool> callback)
		{
			callback = AsOnGameThreadCallback(callback);
			if (!IsAuthenticated())
			{
				callback(false);
			}
			InitializeGameServices();
			if (leaderboardId == null)
			{
				throw new ArgumentNullException("leaderboardId");
			}
			GameServices().LeaderboardManager().SubmitScore(leaderboardId, score, null);
			callback(true);
		}

		public void SubmitScore(string leaderboardId, long score, string metadata, Action<bool> callback)
		{
			callback = AsOnGameThreadCallback(callback);
			if (!IsAuthenticated())
			{
				callback(false);
			}
			InitializeGameServices();
			if (leaderboardId == null)
			{
				throw new ArgumentNullException("leaderboardId");
			}
			GameServices().LeaderboardManager().SubmitScore(leaderboardId, score, metadata);
			callback(true);
		}

		public IRealTimeMultiplayerClient GetRtmpClient()
		{
			if (!IsAuthenticated())
			{
				return null;
			}
			lock (GameServicesLock)
			{
				return mRealTimeClient;
			}
		}

		public ITurnBasedMultiplayerClient GetTbmpClient()
		{
			lock (GameServicesLock)
			{
				return mTurnBasedClient;
			}
		}

		public ISavedGameClient GetSavedGameClient()
		{
			lock (GameServicesLock)
			{
				return mSavedGameClient;
			}
		}

		public IEventsClient GetEventsClient()
		{
			lock (GameServicesLock)
			{
				return mEventsClient;
			}
		}

		public IQuestsClient GetQuestsClient()
		{
			lock (GameServicesLock)
			{
				return mQuestsClient;
			}
		}

		public void RegisterInvitationDelegate(InvitationReceivedDelegate invitationDelegate)
		{
			if (invitationDelegate == null)
			{
				mInvitationDelegate = null;
				return;
			}
			mInvitationDelegate = Callbacks.AsOnGameThreadCallback(delegate(Invitation invitation, bool autoAccept)
			{
				invitationDelegate(invitation, autoAccept);
			});
		}

		public string GetToken()
		{
			if (mTokenClient != null)
			{
				return mTokenClient.GetAccessToken();
			}
			return null;
		}

		public IntPtr GetApiClient()
		{
			return InternalHooks.InternalHooks_GetApiClient(mServices.AsHandle());
		}
	}
}
