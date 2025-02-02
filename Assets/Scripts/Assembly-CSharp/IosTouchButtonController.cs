using UnityEngine;

public class IosTouchButtonController : MonoBehaviour
{
	private UIToggle touchButton;

	private void Start()
	{
		if (!Input.touchPressureSupported && !Application.isEditor)
		{
			Object.Destroy(base.gameObject);
		}
		touchButton = GetComponent<UIToggle>();
		touchButton.value = Defs.isUse3DTouch;
	}

	private void OnClick()
	{
		Defs.isUse3DTouch = touchButton.value;
	}
}
