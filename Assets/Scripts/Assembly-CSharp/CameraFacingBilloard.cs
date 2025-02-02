using UnityEngine;

public class CameraFacingBilloard : MonoBehaviour
{
	private Transform thisTransform;

	private void Awake()
	{
		thisTransform = base.transform;
	}

	private void Update()
	{
		if (NickLabelController.currentCamera != null)
		{
			thisTransform.rotation = NickLabelController.currentCamera.transform.rotation;
		}
	}
}
