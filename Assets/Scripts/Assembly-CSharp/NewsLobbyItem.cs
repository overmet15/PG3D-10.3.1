using System.Collections;
using Rilisoft;
using UnityEngine;

public class NewsLobbyItem : MonoBehaviour
{
	public GameObject indicatorNew;

	public UILabel headerLabel;

	public UILabel shortDescLabel;

	public UILabel dateLabel;

	public UITexture previewPic;

	public string previewPicUrl;

	public void LoadPreview(string url)
	{
		StartCoroutine(LoadPreviewPicture(url));
	}

	private IEnumerator LoadPreviewPicture(string picLink)
	{
		if (previewPic.mainTexture != null && previewPicUrl == picLink)
		{
			yield break;
		}
		previewPic.width = 100;
		if (previewPic.mainTexture != null)
		{
			Object.Destroy(previewPic.mainTexture);
		}
		WWW loadPic = Tools.CreateWwwIfNotConnected(picLink);
		if (loadPic == null)
		{
			yield return new WaitForSeconds(60f);
			StartCoroutine(LoadPreviewPicture(picLink));
			yield break;
		}
		yield return loadPic;
		if (!string.IsNullOrEmpty(loadPic.error))
		{
			Debug.LogWarning("Download preview pic error: " + loadPic.error);
			if (loadPic.error.StartsWith("Resolving host timed out"))
			{
				yield return new WaitForSeconds(1f);
				if (Application.isEditor && FriendsController.isDebugLogWWW)
				{
					Debug.Log("Reloading timed out pic");
				}
				StartCoroutine(LoadPreviewPicture(picLink));
			}
		}
		else
		{
			previewPicUrl = picLink;
			previewPic.mainTexture = loadPic.texture;
			previewPic.mainTexture.filterMode = FilterMode.Point;
			previewPic.width = 100;
		}
	}
}
