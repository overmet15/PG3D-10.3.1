using System;
using System.Collections.Generic;
using Rilisoft;
using UnityEngine;

internal sealed class CoinBonus : MonoBehaviour
{
	public GameObject player;

	public AudioClip CoinItemUpAudioClip;

	private Player_move_c test;

	public static event Action StartBlinkShop;

	public void SetPlayer()
	{
		test = GameObject.FindGameObjectWithTag("PlayerGun").GetComponent<Player_move_c>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	private void Update()
	{
		if (test == null || player == null || !(Vector3.SqrMagnitude(base.transform.position - player.transform.position) < 2.25f))
		{
			return;
		}
		try
		{
			if (TrainingController.TrainingCompleted || TrainingController.CompletedTrainingStage > TrainingController.NewTrainingCompletedStage.None)
			{
				int num = ((!(PremiumAccountController.Instance != null)) ? 1 : PremiumAccountController.Instance.RewardCoeff);
				if (!Defs.IsSurvival && !Defs.isMulti)
				{
					num = 1;
				}
				int @int = Storager.getInt("Coins", false);
				Storager.setInt("Coins", @int + 1 * num, false);
				if (Application.platform != RuntimePlatform.IPhonePlayer)
				{
					PlayerPrefs.Save();
				}
				FlurryEvents.LogCoinsGained(FlurryEvents.GetPlayingMode(), 1);
			}
			CoinsMessage.FireCoinsAddedEvent(false, 1);
			if (!test.isSurvival && TrainingController.TrainingCompleted)
			{
				string[] array = Storager.getString(Defs.LevelsWhereGetCoinS, false).Split('#');
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string item in array2)
				{
					list.Add(item);
				}
				if (!list.Contains(Application.loadedLevelName))
				{
					list.Add(Application.loadedLevelName);
					Storager.setString(Defs.LevelsWhereGetCoinS, string.Join("#", list.ToArray()), false);
				}
			}
			if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None)
			{
				TrainingController.isNextStep = TrainingState.GetTheCoin;
				if (CoinBonus.StartBlinkShop != null)
				{
					CoinBonus.StartBlinkShop();
				}
			}
		}
		finally
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
