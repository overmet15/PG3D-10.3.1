using UnityEngine;

[ExecuteInEditMode]
public class Label3d : MonoBehaviour
{
	public bool apply;

	public Color shadedColor = Color.gray;

	public float offset = -3f;

	private void Create3dText()
	{
		GameObject gameObject = Object.Instantiate(base.gameObject);
		Object.DestroyImmediate(gameObject.GetComponent<Label3d>());
		gameObject.GetComponent<UILabel>().depth = gameObject.GetComponent<UILabel>().depth - 2;
		gameObject.transform.parent = base.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		GameObject gameObject2 = Object.Instantiate(base.gameObject);
		Object.DestroyImmediate(gameObject2.GetComponent<Label3d>());
		gameObject2.transform.parent = base.transform;
		gameObject2.GetComponent<UILabel>().depth = gameObject2.GetComponent<UILabel>().depth - 1;
		gameObject2.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject2.transform.localPosition = new Vector3(0f, offset, 0f);
		gameObject2.GetComponent<UILabel>().color = shadedColor;
		base.gameObject.GetComponent<UILabel>().effectStyle = UILabel.Effect.None;
		Object.DestroyImmediate(gameObject2.transform.GetChild(0).gameObject);
		Object.DestroyImmediate(base.gameObject.GetComponent<Label3d>());
	}

	private void Update()
	{
		if (apply)
		{
			apply = false;
			Invoke("Create3dText", 0f);
		}
	}
}
