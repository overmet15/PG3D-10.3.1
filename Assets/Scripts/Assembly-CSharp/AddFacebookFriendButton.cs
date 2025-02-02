using System.Collections.Generic;
using UnityEngine;

internal sealed class AddFacebookFriendButton : MonoBehaviour
{
	private void OnClick()
	{
		FriendPreview component = base.transform.parent.GetComponent<FriendPreview>();
		ButtonClickSound.Instance.PlayClick();
		string id = component.id;
		if (id != null)
		{
			if (component.ClanInvite)
			{
				FriendsController.SendPlayerInviteToClan(id);
				FriendsController.sharedController.clanSentInvitesLocal.Add(id);
			}
			else
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("Added Friends", "Find Friends: Facebook");
				dictionary.Add("Deleted Friends", "Add");
				dictionary.Add("Search Friends", "Add");
				Dictionary<string, string> socialEventParameters = dictionary;
				FriendsController.sharedController.SendInvitation(id, socialEventParameters);
			}
		}
		if (!component.ClanInvite)
		{
			component.DisableButtons();
		}
	}
}
