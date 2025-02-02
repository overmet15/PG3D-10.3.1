using System;
using UnityEngine;

public class PauseTapReceiver : MonoBehaviour
{
	public static event Action PauseClicked;

	private void OnClick()
	{
		if (ButtonClickSound.Instance != null)
		{
			ButtonClickSound.Instance.PlayClick();
		}
		if ((!Application.loadedLevelName.Equals("Training") || TrainingController.CompletedTrainingStage != TrainingController.NewTrainingCompletedStage.ShootingRangeCompleted) && PauseTapReceiver.PauseClicked != null)
		{
			PauseTapReceiver.PauseClicked();
		}
	}
}
