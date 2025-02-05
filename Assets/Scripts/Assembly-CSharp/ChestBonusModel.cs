using Rilisoft;
using UnityEngine;

public class ChestBonusModel : MonoBehaviour
{
	public const string pathToCommonBonusItems = "Textures/Bank/StarterPack_Weapon";

	public static string GetUrlForDownloadBonusesData()
	{
		string arg = "https://127.0.0.2/pixelgun3d-config/chestBonus/";
		string empty = string.Empty;
		empty = (Defs.IsDeveloperBuild ? "chest_bonus_test.json" : ((BuildSettings.BuildTargetPlatform == RuntimePlatform.Android) ? ((Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "chest_bonus_android.json" : "chest_bonus_amazon.json") : ((BuildSettings.BuildTargetPlatform != RuntimePlatform.MetroPlayerX64) ? "chest_bonus_ios.json" : "chest_bonus_wp8.json")));
		return string.Format("{0}{1}", arg, empty);
	}
}
