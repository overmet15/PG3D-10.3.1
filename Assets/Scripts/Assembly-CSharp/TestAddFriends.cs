using System.Collections.Generic;
using UnityEngine;

internal sealed class TestAddFriends : MonoBehaviour
{
	private void OnClick()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Added Friends", "Test");
		dictionary.Add("Deleted Friends", "Add");
		Dictionary<string, string> socialEventParameters = dictionary;
		FriendsController.sharedController.SendInvitation("123", socialEventParameters);
	}
}
