using UnityEngine;

public class HintTween : MonoBehaviour
{
	private void Start()
	{
		base.transform.localScale = Vector3.one * 0.3f;
	}

	private void Update()
	{
		if (base.transform.localScale.sqrMagnitude != Vector3.one.sqrMagnitude)
		{
			base.transform.localScale = Vector3.Lerp(base.transform.localScale, new Vector3(1f, 1f, 1f), 3f * Time.deltaTime);
		}
	}
}
