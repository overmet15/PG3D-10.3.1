using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	[ExecuteInEditMode]
	public class MobileControlRig : MonoBehaviour
	{
		private void OnEnable()
		{
			CheckEnableControlRig();
		}

		private void CheckEnableControlRig()
		{
			EnableControlRig(true);
		}

		private void EnableControlRig(bool enabled)
		{
			foreach (Transform item in base.transform)
			{
				item.gameObject.SetActive(enabled);
			}
		}
	}
}
