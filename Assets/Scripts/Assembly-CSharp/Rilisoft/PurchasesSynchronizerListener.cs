using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Rilisoft
{
	internal sealed class PurchasesSynchronizerListener : MonoBehaviour
	{
		private IDisposable _escapeSubscription;

		private void Start()
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("PurchasesSynchronizerListener.Start()");
			}
			PurchasesSynchronizer.Instance.PurchasesSavingStarted += HandlePurchasesSavingStarted;
		}

		private void OnDestroy()
		{
			PurchasesSynchronizer.Instance.PurchasesSavingStarted -= HandlePurchasesSavingStarted;
			if (_escapeSubscription != null)
			{
				_escapeSubscription.Dispose();
			}
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("PurchasesSynchronizerListener.OnDestroy()");
			}
		}

		private void HandlePurchasesSavingStarted(object sender, PurchasesSavingEventArgs e)
		{
			Debug.LogFormat("HandlePurchasesSavingStarted >: {0:F3}", Time.realtimeSinceStartup);
			try
			{
				_escapeSubscription = BackSystem.Instance.Register(HandleEscape, "PurchasesSynchronizerListener");
				string activeWithCaption = LocalizationStore.Get("Key_1974");
				ActivityIndicator.SetActiveWithCaption(activeWithCaption);
				InfoWindowController.BlockAllClick();
				StartCoroutine(WaitCompletionCoroutine(e.Future));
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				Debug.LogFormat("HandlePurchasesSavingStarted <: {0:F3}", Time.realtimeSinceStartup);
			}
		}

		private IEnumerator WaitCompletionCoroutine(Task<bool> future)
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.LogFormat("[Rilisoft] PurchasesSynchronizerListener.WaitCompletionCoroutine >: {0:F3}", Time.realtimeSinceStartup);
			}
			try
			{
				while (!future.IsCompleted)
				{
					yield return null;
				}
				InfoWindowController.HideCurrentWindow();
				ActivityIndicator.IsActiveIndicator = false;
				if (_escapeSubscription != null)
				{
					_escapeSubscription.Dispose();
				}
			}
			finally
			{
				if (Defs.IsDeveloperBuild)
				{
					Debug.LogFormat("[Rilisoft] PurchasesSynchronizerListener.WaitCompletionCoroutine <: {0:F3}", Time.realtimeSinceStartup);
				}
			}
		}

		private void HandleEscape()
		{
			if (Defs.IsDeveloperBuild)
			{
				Debug.Log("Ignoring [Escape] while syncing.");
			}
		}
	}
}
