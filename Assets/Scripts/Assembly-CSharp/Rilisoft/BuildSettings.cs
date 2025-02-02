using UnityEngine;

namespace Rilisoft
{
	public static class BuildSettings
	{
		public static RuntimePlatform BuildTargetPlatform
		{
			get
			{
				return Application.platform;
			}
		}
	}
}
