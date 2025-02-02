using UnityEngine;

public class ABTestInfoInConsol : MonoBehaviour
{
	private void Start()
	{
		UILabel component = GetComponent<UILabel>();
		component.text = ((Defs.ABTestDeviceNumber <= 0) ? "Не участвует в тесте" : ((Defs.ABTestDeviceNumber != 1) ? "Устройство B" : "Устройство А"));
	}
}
