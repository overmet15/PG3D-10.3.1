using System;
using System.Threading;
using Com.Google.Android.Gms.Common.Api;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace GooglePlayGames.Android
{
	internal class AndroidTokenClient : TokenClient
	{
		private const string TokenFragmentClass = "com.google.games.bridge.TokenFragment";

		private const string FetchTokenSignature = "(Landroid/app/Activity;Ljava/lang/String;ZZZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;";

		private const string FetchTokenMethod = "fetchToken";

		private bool fetchingEmail;

		private bool fetchingAccessToken;

		private bool fetchingIdToken;

		private string accountName;

		private string accessToken;

		private string idToken;

		private string idTokenScope;

		private string rationale;

		public static AndroidJavaObject GetActivity()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}

		public void SetRationale(string rationale)
		{
			this.rationale = rationale;
		}

		public AndroidJavaObject GetApiClient(bool getServerAuthCode = false, string serverClientID = null)
		{
			Debug.Log("Calling GetApiClient....");
			using (AndroidJavaObject androidJavaObject = GetActivity())
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.android.gms.plus.Plus"))
				{
					using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("com.google.android.gms.common.api.GoogleApiClient$Builder", androidJavaObject))
					{
						androidJavaObject2.Call<AndroidJavaObject>("addApi", new object[1] { androidJavaClass.GetStatic<AndroidJavaObject>("API") });
						androidJavaObject2.Call<AndroidJavaObject>("addScope", new object[1] { androidJavaClass.GetStatic<AndroidJavaObject>("SCOPE_PLUS_LOGIN") });
						if (getServerAuthCode)
						{
							androidJavaObject2.Call<AndroidJavaObject>("requestServerAuthCode", new object[2] { serverClientID, androidJavaObject2 });
						}
						AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("build", new object[0]);
						androidJavaObject3.Call("connect");
						int num = 100;
						while (!androidJavaObject3.Call<bool>("isConnected", new object[0]) && num-- != 0)
						{
							Thread.Sleep(100);
						}
						Debug.Log("Done GetApiClient is " + androidJavaObject3);
						return androidJavaObject3;
					}
				}
			}
		}

		internal void Fetch(string scope, string rationale, bool fetchEmail, bool fetchAccessToken, bool fetchIdToken, Action<bool> doneCallback)
		{
			PlayGamesHelperObject.RunOnGameThread(delegate
			{
				FetchToken(scope, rationale, fetchEmail, fetchAccessToken, fetchIdToken, delegate(int rc, string access, string id, string email)
				{
					if (rc != 0)
					{
						GooglePlayGames.OurUtils.Logger.w("Non-success returned from fetch: " + rc);
						doneCallback(false);
					}
					if (fetchAccessToken)
					{
						GooglePlayGames.OurUtils.Logger.d("a = " + access);
					}
					if (fetchEmail)
					{
						GooglePlayGames.OurUtils.Logger.d("email = " + email);
					}
					if (fetchIdToken)
					{
						GooglePlayGames.OurUtils.Logger.d("idt = " + id);
					}
					if (fetchAccessToken && !string.IsNullOrEmpty(access))
					{
						accessToken = access;
					}
					if (fetchIdToken && !string.IsNullOrEmpty(id))
					{
						idToken = id;
					}
					if (fetchEmail && !string.IsNullOrEmpty(email))
					{
						accountName = email;
					}
					doneCallback(true);
				});
			});
		}

		internal static void FetchToken(string scope, string rationale, bool fetchEmail, bool fetchAccessToken, bool fetchIdToken, Action<int, string, string, string> callback)
		{
			object[] args = new object[6];
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.google.games.bridge.TokenFragment"))
				{
					using (AndroidJavaObject androidJavaObject = GetActivity())
					{
						IntPtr staticMethodID = AndroidJNI.GetStaticMethodID(androidJavaClass.GetRawClass(), "fetchToken", "(Landroid/app/Activity;Ljava/lang/String;ZZZLjava/lang/String;)Lcom/google/android/gms/common/api/PendingResult;");
						array[0].l = androidJavaObject.GetRawObject();
						array[1].l = AndroidJNI.NewStringUTF(rationale);
						array[2].z = fetchEmail;
						array[3].z = fetchAccessToken;
						array[4].z = fetchIdToken;
						array[5].l = AndroidJNI.NewStringUTF(scope);
						IntPtr ptr = AndroidJNI.CallStaticObjectMethod(androidJavaClass.GetRawClass(), staticMethodID, array);
						PendingResult<TokenResult> pendingResult = new PendingResult<TokenResult>(ptr);
						pendingResult.setResultCallback(new TokenResultCallback(callback));
					}
				}
			}
			catch (Exception ex)
			{
				GooglePlayGames.OurUtils.Logger.e("Exception launching token request: " + ex.Message);
				GooglePlayGames.OurUtils.Logger.e(ex.ToString());
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		private string GetAccountName()
		{
			if (string.IsNullOrEmpty(accountName) && !fetchingEmail)
			{
				fetchingEmail = true;
				Fetch(idTokenScope, rationale, true, false, false, delegate
				{
					fetchingEmail = false;
				});
			}
			return accountName;
		}

		public string GetEmail()
		{
			return GetAccountName();
		}

		public string GetAuthorizationCode(string serverClientID)
		{
			throw new NotImplementedException();
		}

		public string GetAccessToken()
		{
			if (string.IsNullOrEmpty(accessToken) && !fetchingAccessToken)
			{
				fetchingAccessToken = true;
				Fetch(idTokenScope, rationale, false, true, false, delegate
				{
					fetchingAccessToken = false;
				});
			}
			return accessToken;
		}

		public string GetIdToken(string serverClientID)
		{
			string text = "audience:server:client_id:" + serverClientID;
			if ((string.IsNullOrEmpty(idToken) || text != idTokenScope) && !fetchingIdToken)
			{
				fetchingIdToken = true;
				idTokenScope = text;
				Fetch(idTokenScope, rationale, false, false, true, delegate
				{
					fetchingIdToken = false;
				});
			}
			return idToken;
		}
	}
}
