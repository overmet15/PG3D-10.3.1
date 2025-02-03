using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLock : MonoBehaviour
{
#if UNITY_STANDALONE
	[RuntimeInitializeOnLoadMethod]
	static void OnGameStart()
	{
		DontDestroyOnLoad(new GameObject("MouseLock", typeof(MouseLock)));
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
			else Cursor.lockState = CursorLockMode.Locked;

        }
	}
#endif
}
