using UnityEngine;

public class ExpHUD : MonoBehaviour
{
	public UILabel lbCurLev;

	public UILabel lbExp;

	public UITexture txExp;

	private void OnEnable()
	{
		ExpController.Instance.experienceView.VisibleHUD = false;
		UpdateHUD();
	}

	private void OnDisable()
	{
		ExpController.Instance.experienceView.VisibleHUD = true;
	}

	public void UpdateHUD()
	{
		lbCurLev.text = ExperienceController.sharedController.currentLevel.ToString();
		lbExp.text = ExpController.ExpToString();
		if (ExperienceController.sharedController.currentLevel == ExperienceController.maxLevel)
		{
			txExp.fillAmount = 1f;
		}
		else
		{
			txExp.fillAmount = ExpController.progressExpInPer();
		}
	}
}
