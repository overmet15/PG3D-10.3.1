using System;
using UnityEngine;

public class MyUIInput : UIInput
{
	[NonSerialized]
	public float heightKeyboard;

	public Action onKeyboardInter;

	public Action onKeyboardCancel;

	public Action onKeyboardVisible;

	public Action onKeyboardHide;

	private bool isKeyboardVisible;

	private float timerlog = 0.3f;

	private void Awake()
	{
		hideInput = false;
	}

	protected override void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			OnSelectEvent();
		}
		else if (!hideInput)
		{
			OnDeselectEvent();
		}
	}

	public void DeselectInput()
	{
		OnDeselectEventCustom();
	}

	protected void OnDeselectEventCustom()
	{
		if (mDoInit)
		{
			Init();
		}
		if (UIInput.mKeyboard != null)
		{
			UIInput.mKeyboard.active = false;
			UIInput.mKeyboard = null;
		}
		if (label != null)
		{
			mValue = base.value;
			if (string.IsNullOrEmpty(mValue))
			{
				label.text = mDefaultText;
				label.color = mDefaultColor;
			}
			else
			{
				label.text = mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			label.alignment = mAlignment;
		}
		base.isSelected = false;
		UIInput.selection = null;
		UpdateLabel();
	}

	private new void Update()
	{
		if (Application.isEditor)
		{
			if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown("enter")) && onKeyboardInter != null)
			{
				onKeyboardInter();
			}
			if (Input.GetKeyDown(KeyCode.KeypadPlus) && onKeyboardVisible != null)
			{
				onKeyboardVisible();
			}
			if (Input.GetKeyDown(KeyCode.KeypadMinus))
			{
				if (onKeyboardHide != null)
				{
					onKeyboardHide();
				}
				DeselectInput();
			}
		}
		base.Update();
	}

	public float GetKeyboardHeight()
	{
		return heightKeyboard;
	}

	private void SetKeyboardHeight()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView", new object[0]);
			using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.graphics.Rect"))
			{
				androidJavaObject.Call("getWindowVisibleDisplayFrame", androidJavaObject2);
				heightKeyboard = Screen.height - androidJavaObject2.Call<int>("height", new object[0]);
			}
		}
	}

	private void OnDestroy()
	{
		base.OnSelect(false);
		DeselectInput();
	}

	private void OnDisable()
	{
		base.OnSelect(false);
		DeselectInput();
		base.Cleanup();
	}
}
