using System.Collections.Generic;
using UnityEngine;

internal sealed class AddFrendsButtonInTableRangs : MonoBehaviour
{
	public int ID;

	private void OnPress(bool isDown)
	{
		if (!isDown)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Added Friends", "AddFrendsButtonInTableRangs");
			dictionary.Add("Deleted Friends", "Add");
			Dictionary<string, string> socialEventParameters = dictionary;
			FriendsController.sharedController.SendInvitation(ID.ToString(), socialEventParameters);
			if (!FriendsController.sharedController.notShowAddIds.Contains(ID.ToString()))
			{
				FriendsController.sharedController.notShowAddIds.Add(ID.ToString());
			}
			base.gameObject.SetActive(false);
		}
	}
}
