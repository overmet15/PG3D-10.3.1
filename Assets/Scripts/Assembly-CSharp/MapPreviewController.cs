using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MapPreviewController : MonoBehaviour
{
	public UILabel NameMapLbl;

	public UILabel SizeMapNameLbl;

	public UILabel popularityLabel;

	public GameObject premium;

	public GameObject milee;

	public GameObject dater;

	public int mapID;

	public UITexture mapPreviewTexture;

	private string[] masRatingStr;

	private MyCenterOnChild centerChild;

	private void Start()
	{
		masRatingStr = new string[5]
		{
			LocalizationStore.Key_0545,
			LocalizationStore.Key_0546,
			LocalizationStore.Key_0547,
			LocalizationStore.Key_0548,
			LocalizationStore.Key_0549
		};
		StartCoroutine(SetPopularity());
		centerChild = ConnectSceneNGUIController.sharedController.grid.GetComponent<MyCenterOnChild>();
	}

	private IEnumerator SetPopularity()
	{
		Dictionary<string, string> _mapsPoplarityInCurrentRegim;
		while (true)
		{
			if (FriendsController.mapPopularityDictionary.Count > 0)
			{
				_mapsPoplarityInCurrentRegim = null;
				try
				{
					_mapsPoplarityInCurrentRegim = FriendsController.mapPopularityDictionary[((int)ConnectSceneNGUIController.regim).ToString()];
				}
				catch (KeyNotFoundException)
				{
				}
				if (_mapsPoplarityInCurrentRegim != null)
				{
					break;
				}
				yield return StartCoroutine(MyWaitForSeconds(2f));
			}
			else
			{
				yield return StartCoroutine(MyWaitForSeconds(2f));
			}
		}
		int rating = 0;
		if (_mapsPoplarityInCurrentRegim.ContainsKey(mapID.ToString()))
		{
			int _countPlayersOnMap = int.Parse(_mapsPoplarityInCurrentRegim[mapID.ToString()]);
			if ((float)_countPlayersOnMap > 1f && _countPlayersOnMap < 8)
			{
				rating = 1;
			}
			if ((float)_countPlayersOnMap >= 8f && _countPlayersOnMap < 15)
			{
				rating = 2;
			}
			if ((float)_countPlayersOnMap >= 15f && _countPlayersOnMap < 35)
			{
				rating = 3;
			}
			if ((float)_countPlayersOnMap >= 35f)
			{
				rating = 4;
			}
		}
		popularityLabel.text = masRatingStr[rating];
		popularityLabel.gameObject.SetActive(true);
	}

	public IEnumerator MyWaitForSeconds(float tm)
	{
		float startTime = Time.realtimeSinceStartup;
		do
		{
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < tm);
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		ConnectSceneNGUIController.sharedController.StopFingerAnim();
		if (centerChild.centeredObject != base.transform.gameObject)
		{
			centerChild.CenterOn(base.transform);
		}
		else if (!ConnectSceneNGUIController.sharedController.createPanel.activeSelf)
		{
			ConnectSceneNGUIController.sharedController.HandleGoBtnClicked(null, EventArgs.Empty);
		}
	}
}
