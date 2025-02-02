using UnityEngine;
using UnityEngine.EventSystems;

public class LevelReset : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData data)
	{
		Application.LoadLevelAsync(Application.loadedLevelName);
	}

	private void Update()
	{
	}
}
