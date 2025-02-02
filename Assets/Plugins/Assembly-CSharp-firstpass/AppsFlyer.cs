using System;
using System.Collections.Generic;
using UnityEngine;

public class AppsFlyer : MonoBehaviour
{
	private static AndroidJavaClass cls_AppsFlyer = new AndroidJavaClass("com.appsflyer.AppsFlyerLib");

	private static AndroidJavaClass cls_AppsFlyerHelper = new AndroidJavaClass("com.appsflyer.AppsFlyerUnityHelper");

	public static void trackEvent(string eventName, string eventValue)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyer.CallStatic("sendTrackingWithEvent", androidJavaObject, eventName, eventValue);
			}
		}
	}

	public static void setCurrencyCode(string currencyCode)
	{
		cls_AppsFlyer.CallStatic("setCurrencyCode", currencyCode);
	}

	public static void setCustomerUserID(string customerUserID)
	{
		cls_AppsFlyer.CallStatic("setAppUserId", customerUserID);
	}

	public static void loadConversionData(string callbackObject, string callbackMethod, string callbackFailedMethod)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyerHelper.CallStatic("createConversionDataListener", androidJavaObject, callbackObject, callbackMethod, callbackFailedMethod);
			}
		}
	}

	public static void setCollectIMEI(bool shouldCollect)
	{
		cls_AppsFlyer.CallStatic("setCollectIMEI", shouldCollect);
	}

	public static void setCollectAndroidID(bool shouldCollect)
	{
		MonoBehaviour.print("AF.cs setCollectAndroidID");
		cls_AppsFlyer.CallStatic("setCollectAndroidID", shouldCollect);
	}

	public static void setAppsFlyerKey(string key)
	{
		cls_AppsFlyer.CallStatic("setAppsFlyerKey", key);
	}

	public static void trackAppLaunch()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyer.CallStatic("sendTracking", androidJavaObject);
			}
		}
	}

	public static void setAppID(string appleAppId)
	{
	}

	public static void createValidateInAppListener(string aObject, string callbackMethod, string callbackFailedMethod)
	{
		MonoBehaviour.print("AF.cs createValidateInAppListener called");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				cls_AppsFlyerHelper.CallStatic("createValidateInAppListener", androidJavaObject, aObject, callbackMethod, callbackFailedMethod);
			}
		}
	}

	public static void validateReceipt(string publicKey, string purchaseData, string signature)
	{
		MonoBehaviour.print("AF.cs validateReceipt pk = " + publicKey + " data = " + purchaseData + "sig = " + signature);
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				MonoBehaviour.print("inside cls_activity");
				cls_AppsFlyer.CallStatic("validateAndTrackInAppPurchase", androidJavaObject, publicKey, purchaseData, signature);
			}
		}
	}

	public static void trackRichEvent(string eventName, Dictionary<string, string> eventValues)
	{
		using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap"))
		{
			IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			object[] array = new object[2];
			foreach (KeyValuePair<string, string> eventValue in eventValues)
			{
				using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", eventValue.Key))
				{
					using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", eventValue.Value))
					{
						array[0] = androidJavaObject2;
						array[1] = androidJavaObject3;
						AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
					}
				}
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject4 = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					cls_AppsFlyer.CallStatic("trackEvent", androidJavaObject4, eventName, androidJavaObject);
				}
			}
		}
	}

	public static void setIsDebug(bool isDebug)
	{
	}

	public static void setIsSandbox(bool isSandbox)
	{
	}

	public static void getConversionData()
	{
	}

	public static void handleOpenUrl(string url, string sourceApplication, string annotation)
	{
	}

	public static string getAppsFlyerId()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				return cls_AppsFlyer.CallStatic<string>("getAppsFlyerUID", new object[1] { androidJavaObject });
			}
		}
	}
}
