using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using FyberPlugin;
using Rilisoft;
using UnityEngine;

internal sealed class FyberFacade
{
	private readonly LinkedList<Task<Ad>> _requests = new LinkedList<Task<Ad>>();

	private static readonly Lazy<FyberFacade> _instance = new Lazy<FyberFacade>(() => new FyberFacade());

	public static FyberFacade Instance
	{
		get
		{
			return _instance.Value;
		}
	}

	public LinkedList<Task<Ad>> Requests
	{
		get
		{
			return _requests;
		}
	}

	private FyberFacade()
	{
	}

	public Task<Ad> RequestImageInterstitial(string callerName = null)
	{
		if (callerName == null)
		{
			callerName = string.Empty;
		}
		TaskCompletionSource<Ad> promise = new TaskCompletionSource<Ad>();
		return RequestImageInterstitialCore(promise, callerName);
	}

	private Task<Ad> RequestImageInterstitialCore(TaskCompletionSource<Ad> promise, string callerName)
	{
		Action<Ad> onAdAvailable = null;
		Action<AdFormat> onAdNotAvailable = null;
		Action<RequestError> onRequestFail = null;
		onAdAvailable = delegate(Ad ad)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("RequestImageInterstitialCore > AdAvailable: {{ format: {0}, placementId: '{1}' }}", ad.AdFormat, ad.PlacementId);
			}
			promise.SetResult(ad);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		onAdNotAvailable = delegate(AdFormat adFormat)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("RequestImageInterstitialCore > AdNotAvailable: {{ format: {0} }}", adFormat);
			}
			AdNotAwailableException exception2 = new AdNotAwailableException("Ad not available: " + adFormat);
			promise.SetException(exception2);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		onRequestFail = delegate(RequestError requestError)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("RequestImageInterstitialCore > RequestFail: {{ requestError: {0} }}", requestError.Description);
			}
			AdRequestException exception = new AdRequestException(requestError.Description);
			promise.SetException(exception);
			FyberCallback.AdAvailable -= onAdAvailable;
			FyberCallback.AdNotAvailable -= onAdNotAvailable;
			FyberCallback.RequestFail -= onRequestFail;
		};
		FyberCallback.AdAvailable += onAdAvailable;
		FyberCallback.AdNotAvailable += onAdNotAvailable;
		FyberCallback.RequestFail += onRequestFail;
		RequestInterstitialAds(callerName);
		if (Application.isEditor)
		{
			promise.SetException(new NotSupportedException("Ads are not supported in Editor."));
		}
		return promise.Task;
	}

	public Task<AdResult> ShowInterstitial(Dictionary<string, string> parameters, string callerName = null)
	{
		if (parameters == null)
		{
			parameters = new Dictionary<string, string>();
		}
		if (callerName == null)
		{
			callerName = string.Empty;
		}
		if (Defs.IsDeveloperBuild)
		{
			Debug.LogFormat("[Rilisoft] ShowInterstitial('{0}')", callerName);
		}
		if (Requests.Count == 0)
		{
			Debug.LogWarning("[Rilisoft]No active requests.");
			TaskCompletionSource<AdResult> taskCompletionSource = new TaskCompletionSource<AdResult>();
			taskCompletionSource.SetException(new InvalidOperationException("No active requests."));
			return taskCompletionSource.Task;
		}
		Debug.LogWarning("[Rilisoft] Active requests count: " + Requests.Count);
		LinkedListNode<Task<Ad>> requestNode = null;
		for (LinkedListNode<Task<Ad>> linkedListNode = Requests.Last; linkedListNode != null; linkedListNode = linkedListNode.Previous)
		{
			if (!linkedListNode.Value.IsFaulted)
			{
				if (linkedListNode.Value.IsCompleted)
				{
					requestNode = linkedListNode;
					break;
				}
				if (requestNode == null)
				{
					requestNode = linkedListNode;
				}
			}
		}
		if (requestNode == null)
		{
			string text = "All requests are faulted: " + Requests.Count;
			Debug.LogWarning("[Rilisoft]" + text);
			TaskCompletionSource<AdResult> taskCompletionSource2 = new TaskCompletionSource<AdResult>();
			taskCompletionSource2.SetException(new InvalidOperationException(text));
			return taskCompletionSource2.Task;
		}
		TaskCompletionSource<AdResult> showPromise = new TaskCompletionSource<AdResult>();
		Action<Task<Ad>> action = delegate(Task<Ad> requestFuture)
		{
			if (requestFuture.IsFaulted)
			{
				string text2 = "Ad request failed: " + requestFuture.Exception.InnerException.Message;
				Debug.LogWarningFormat("[Rilisoft] {0}", text2);
				showPromise.SetException(new AdRequestException(text2, requestFuture.Exception.InnerException));
			}
			else
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("[Rilisoft] Ad request succeeded: {{ adFormat: {0}, placementId: '{1}' }}", requestFuture.Result.AdFormat, requestFuture.Result.PlacementId);
				}
				Action<AdResult> adFinished = null;
				adFinished = delegate(AdResult adResult)
				{
					Lazy<string> lazy = new Lazy<string>(() => string.Format("[Rilisoft] Ad show finished: {{ format: {0}, status: {1}, message: '{2}' }}", adResult.AdFormat, adResult.Status, adResult.Message));
					if (adResult.Status == AdStatus.Error)
					{
						Debug.LogWarning(lazy.Value);
					}
					else if (Defs.IsDeveloperBuild)
					{
						Debug.Log(lazy.Value);
					}
					FyberCallback.AdFinished -= adFinished;
					showPromise.SetResult(adResult);
					if (adResult.Status == AdStatus.OK)
					{
						parameters["Fyber - Interstitial"] = "Impression: " + adResult.Message;
						FlurryPluginWrapper.LogEventAndDublicateToConsole("Ads Show Stats - Total", parameters);
					}
				};
				FyberCallback.AdFinished += adFinished;
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("Start showing ad: {{ format: {0}, placementId: '{1}' }}", requestFuture.Result.AdFormat, requestFuture.Result.PlacementId);
				}
				requestFuture.Result.Start();
				Requests.Remove(requestNode);
			}
		};
		if (requestNode.Value.IsCompleted)
		{
			if (Defs.IsDeveloperBuild)
			{
				action(requestNode.Value);
			}
		}
		else
		{
			requestNode.Value.ContinueWith(action);
		}
		return showPromise.Task;
	}

	public void SetUserPaying(string payingBin)
	{
		if (string.IsNullOrEmpty(payingBin))
		{
			payingBin = "0";
		}
		SetUserPayingCore(payingBin);
	}

	public void UpdateUserPaying()
	{
		string userPayingCore = Storager.getInt("PayingUser", true).ToString(CultureInfo.InvariantCulture);
		SetUserPayingCore(userPayingCore);
	}

	private void SetUserPayingCore(string payingBin)
	{
		User.PutCustomValue("pg3d_paying", payingBin);
	}

	private static void RequestInterstitialAds(string callerName)
	{
		InterstitialRequester.Create().Request();
		if (Defs.IsDeveloperBuild)
		{
			string message = string.Format("[Rilisoft] RequestInterstitialAds('{0}')", callerName);
			Debug.Log(message);
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Fyber - Interstitial", "Request");
		Dictionary<string, string> parameters = dictionary;
		FlurryPluginWrapper.LogEventAndDublicateToConsole("Ads Show Stats - Total", parameters);
	}
}
