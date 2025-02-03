using System.Collections;
using GooglePlayGames;
using Rilisoft;
using UnityEngine;

internal sealed class FirstPersonControlSharp : MonoBehaviour
{
	private const string newbieJumperAchievement = "NewbieJumperAchievement";

	private const int maxJumpCount = 10;

	private const string keyNinja = "NinjaJumpsCount";

	public Transform cameraPivot;

	public float forwardSpeed = 4f;

	public float backwardSpeed = 1f;

	public float sidestepSpeed = 1f;

	public float jumpSpeed = 4.5f;

	public float inAirMultiplier = 0.25f;

	public Vector2 rotationSpeed = new Vector2(2f, 1f);

	public float tiltPositiveYAxis = 0.6f;

	public float tiltNegativeYAxis = 0.4f;

	public float tiltXAxisMinimum = 0.1f;

	public string myIp;

	public GameObject playerGameObject;

	public int typeAnim;

	private Transform thisTransform;

	public GameObject camPlayer;

	private CharacterController character;

	private Vector3 cameraVelocity;

	private Vector3 velocity;

	private bool canJump = true;

	public bool isMine;

	private Rect fireZone;

	private Rect jumpZone;

	private bool jump;

	private float timeUpdateAnim;

	public AudioClip jumpClip;

	private Player_move_c _moveC;

	public float gravityMultiplier = 1f;

	private Vector3 mousePosOld = Vector3.zero;

	private bool _invert;

	public bool ninjaJumpUsed = true;

	private HungerGameController hungerGameController;

	private bool isHunger;

	private bool isInet;

	private bool isMulti;

	private SkinName mySkinName;

	private int oldJumpCount;

	private int oldNinjaJumpsCount;

	private Vector3 _movement;

	private Vector2 _cameraMouseDelta;

	private Vector3 rinkMovement;

	private bool steptRink;

	private bool secondJumpEnabled = true;

	private void Awake()
	{
		isHunger = Defs.isHunger;
		isInet = Defs.isInet;
		isMulti = Defs.isMulti;
	}

	private void Start()
	{
		mySkinName = GetComponent<SkinName>();
		if (!isInet)
		{
			isMine = GetComponent<NetworkView>().isMine;
		}
		else
		{
			isMine = PhotonView.Get(this).isMine;
		}
		if (isHunger)
		{
			hungerGameController = HungerGameController.Instance;
		}
		if (!isMulti || isMine)
		{
			HandleInvertCamUpdated();
			PauseNGUIController.InvertCamUpdated += HandleInvertCamUpdated;
			oldJumpCount = PlayerPrefs.GetInt("NewbieJumperAchievement", 0);
			oldNinjaJumpsCount = (Storager.hasKey("NinjaJumpsCount") ? Storager.getInt("NinjaJumpsCount", false) : 0);
		}
		thisTransform = GetComponent<Transform>();
		character = GetComponent<CharacterController>();
		_moveC = playerGameObject.GetComponent<Player_move_c>();
	}

	private void HandleInvertCamUpdated()
	{
		_invert = PlayerPrefs.GetInt(Defs.InvertCamSN, 0) == 1;
	}

	private void OnEndGame()
	{
		if (!isMulti || isMine)
		{
			if ((bool)JoystickController.leftJoystick)
			{
				JoystickController.leftJoystick.transform.parent.gameObject.SetActive(false);
			}
			if ((bool)JoystickController.rightJoystick)
			{
				JoystickController.rightJoystick.gameObject.SetActive(false);
			}
		}
		base.enabled = false;
	}

	[RPC]
	[PunRPC]
	private void setIp(string _ip)
	{
		myIp = _ip;
	}

	private Vector2 updateKeyboardControls()
	{
		int num = 0;
		int num2 = 0;
		if (Input.GetKey("w"))
		{
			num = 1;
		}
		if (Input.GetKey("s"))
		{
			num = -1;
		}
		if (Input.GetKey("a"))
		{
			num2 = -1;
		}
		if (Input.GetKey("d"))
		{
			num2 = 1;
		}
		return new Vector2(num2, num);
	}

	private void Jump()
	{
		if (!TrainingController.TrainingCompleted)
		{
			HintController.instance.HideHintByName("press_jump");
		}
		jump = true;
		canJump = false;
		if (!Defs.isJetpackEnabled)
		{
			mySkinName.sendAnimJump();
		}
		if (!TrainingController.TrainingCompleted || (BuildSettings.BuildTargetPlatform != RuntimePlatform.Android && BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer) || !Social.localUser.authenticated)
		{
			return;
		}
		int num = oldJumpCount + 1;
		if (oldJumpCount >= 10)
		{
			return;
		}
		PlayerPrefs.SetInt("NewbieJumperAchievement", num);
		oldJumpCount = num;
		if (num != 10)
		{
			return;
		}
		float newProgress = 100f;
		string text = ((BuildSettings.BuildTargetPlatform != RuntimePlatform.IPhonePlayer && Defs.AndroidEdition != Defs.RuntimeAndroidEdition.Amazon) ? "CgkIr8rGkPIJEAIQAQ" : "Jumper_id");
		if (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.Amazon)
		{
			AGSAchievementsClient.UpdateAchievementProgress(text, newProgress);
			return;
		}
		Social.ReportProgress(text, newProgress, delegate(bool success)
		{
			string text2 = string.Format("Newbie Jumper achievement progress {0:0.0}%: {1}", newProgress, success);
		});
    }

    Vector3 KeyboardInput()
    {
        Vector3 result = new Vector3();

        if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) result.x = 1;
        else if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) result.x = -1;

        if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) result.z = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) result.z = -1;

        return result;
    }

    private void Update()
	{
		if ((isMulti && !isMine) || mySkinName.playerMoveC.isKilled || JoystickController.leftJoystick == null || JoystickController.rightJoystick == null)
		{
			return;
		}
		if (mySkinName.playerMoveC.isRocketJump && character.isGrounded)
		{
			mySkinName.playerMoveC.isRocketJump = false;
		}
#if UNITY_STANDALONE
		_movement = thisTransform.TransformDirection(KeyboardInput());
#else
		_movement = thisTransform.TransformDirection(new Vector3(JoystickController.leftJoystick.value.x, 0f, JoystickController.leftJoystick.value.y));
#endif
        if ((!isHunger || !hungerGameController.isGo) && isHunger)
		{
			_movement = Vector3.zero;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining < TrainingState.TapToMove)
		{
			_movement = Vector3.zero;
		}
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.TapToMove && _movement != Vector3.zero)
		{
			TrainingController.isNextStep = TrainingState.TapToMove;
		}
		_movement.y = 0f;
		_movement.Normalize();
#if UNITY_STANDALONE
		Vector2 vector = new Vector2(Mathf.Abs(KeyboardInput().x), Mathf.Abs(KeyboardInput().z));
#else
		Vector2 vector = new Vector2(Mathf.Abs(JoystickController.leftJoystick.value.x), Mathf.Abs(JoystickController.leftJoystick.value.y));
#endif
		if (JoystickController.leftTouchPad.isShooting && JoystickController.leftTouchPad.isActiveFireButton)
		{
			vector = new Vector2(0f, 0f);
		}
		if (vector.y > vector.x)
		{
			if (JoystickController.leftJoystick.value.y > 0f)
			{
				_movement *= forwardSpeed * EffectsController.SpeedModifier(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1) * vector.y;
			}
			else
			{
				_movement *= backwardSpeed * EffectsController.SpeedModifier(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1) * vector.y;
			}
		}
		else
		{
			_movement *= sidestepSpeed * EffectsController.SpeedModifier(WeaponManager.sharedManager.currentWeaponSounds.categoryNabor - 1) * vector.x * (float)((!character.isGrounded) ? 1 : 1);
		}
		if (character.isGrounded)
		{
			if (EffectsController.NinjaJumpEnabled)
			{
				ninjaJumpUsed = false;
			}
			canJump = true;
			jump = false;
			TouchPadController rightJoystick = JoystickController.rightJoystick;
#if UNITY_STANDALONE
            bool isJumpPressed = (TrainingController.TrainingCompleted ||TrainingController.stepTraining >= TrainingState.GetTheGun)
				&& Input.GetKeyDown(KeyCode.Space);
#else
            bool isJumpPressed = rightJoystick.jumpPressed || JoystickController.leftTouchPad.isJumpPressed;
#endif

            if (canJump && isJumpPressed)
			{
				if (!Defs.isJetpackEnabled)
				{
					rightJoystick.jumpPressed = false;
				}
				Jump();
			}
			if (jump)
			{
				secondJumpEnabled = false;
				StartCoroutine(EnableSecondJump());
				velocity = Vector3.zero;
				velocity.y = jumpSpeed * EffectsController.JumpModifier;
			}
		}
		else
		{
			if (jump && mySkinName.interpolateScript.myAnim == 0 && !Defs.isJetpackEnabled)
			{
				mySkinName.sendAnimJump();
			}
			TouchPadController rightJoystick2 = JoystickController.rightJoystick;
			if (rightJoystick2.jumpPressed && ((EffectsController.NinjaJumpEnabled && !ninjaJumpUsed) || Defs.isJetpackEnabled))
			{
				if (!Defs.isJetpackEnabled)
				{
					RegisterNinjAchievment();
				}
				ninjaJumpUsed = true;
				if (!Defs.isJetpackEnabled)
				{
					rightJoystick2.jumpPressed = false;
				}
				canJump = false;
				if (!Defs.isJetpackEnabled)
				{
					mySkinName.sendAnimJump();
				}
				velocity.y = 1.1f * (jumpSpeed * EffectsController.JumpModifier);
			}
			velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
			if (JoystickController.rightJoystick.jumpPressed && !Defs.isJetpackEnabled)
			{
				JoystickController.rightJoystick.jumpPressed = false;
			}
		}
		_movement += velocity;
		_movement += Physics.gravity * gravityMultiplier;
		_movement *= Time.deltaTime;
		timeUpdateAnim -= Time.deltaTime;
		if (timeUpdateAnim < 0f)
		{
			if (character.isGrounded)
			{
				timeUpdateAnim = 0.5f;
				if (new Vector2(_movement.x, _movement.z).sqrMagnitude > 0f)
				{
					_moveC.WalkAnimation();
				}
				else
				{
					_moveC.IdleAnimation();
				}
			}
			else
			{
				_moveC.WalkAnimation();
			}
		}
		Update2();
	}

	private void Update2()
	{
		if (!character.enabled)
		{
			return;
		}
		if (!mySkinName.onRink)
		{
			if (mySkinName.onConveyor)
			{
				_movement += mySkinName.conveyorDirection * Time.deltaTime;
			}
			character.Move(_movement);
			_movement = Vector3.zero;
			steptRink = false;
		}
		else
		{
			if (!steptRink)
			{
				rinkMovement = _movement;
				steptRink = true;
			}
			rinkMovement = Vector3.MoveTowards(rinkMovement, _movement, 0.068f * Time.deltaTime);
			rinkMovement.y = _movement.y;
			character.Move(rinkMovement);
		}
		if (character.isGrounded)
		{
			velocity = Vector3.zero;
		}
		else
		{
			if (mySkinName.onRink)
			{
				rinkMovement = _movement;
			}
			mySkinName.onConveyor = false;
		}
		Vector2 delta = GrabCameraInputDelta();
		//if (Device.isPixelGunLow && Defs.isTouchControlSmoothDump)
		//{
			MoveCamera(delta);
		//}
		if (Defs.isMulti && CameraSceneController.sharedController.killCamController.enabled)
		{
			CameraSceneController.sharedController.killCamController.UpdateMouseX();
		}

#if UNITY_STANDALONE
        if (TrainingController.TrainingCompleted || TrainingController.stepTraining >= TrainingState.GetTheGun) UpdatePC();
#endif
	}

	void UpdatePC()
	{
        if (Input.GetMouseButton(0)) _moveC.ShotPressed();
		if (Input.GetKeyDown(KeyCode.R)) _moveC.ReloadPressed();

		if (!TrainingController.TrainingCompleted) return;

        if (Input.GetMouseButtonDown(1)) _moveC.ZoomPress();
        if (Input.GetKeyDown(KeyCode.Tab)) _moveC.RanksPressed();
		if (Input.GetKeyUp(KeyCode.Tab)) _moveC.BackRanksPressed();
    }

	public void MoveCamera(Vector2 delta)
	{
		if (!TrainingController.TrainingCompleted && TrainingController.CompletedTrainingStage == TrainingController.NewTrainingCompletedStage.None && TrainingController.stepTraining == TrainingState.SwipeToRotate && delta != Vector2.zero)
		{
			TrainingController.isNextStep = TrainingState.SwipeToRotate;
		}
		float sensitivity = Defs.Sensitivity;
		float num = 1f;
		if (_moveC != null)
		{
			num *= ((!_moveC.isZooming) ? 1f : 0.2f);
		}
		if (JoystickController.rightJoystick != null)
		{
			JoystickController.rightJoystick.ApplyDeltaTo(delta, thisTransform, cameraPivot.transform, sensitivity * num, _invert);
		}
	}

	private Vector2 GrabCameraInputDelta()
    {
        Vector2 result = Vector2.zero;
#if UNITY_STANDALONE
		result = new Vector2(Input.GetAxisRaw("Mouse X") * 2.5f, Input.GetAxisRaw("Mouse Y") * 2.5f);
		return result;
#else
        TouchPadController rightJoystick = JoystickController.rightJoystick;
		if (rightJoystick != null)
		{
			result = rightJoystick.GrabDeltaPosition();
		}
		return result;
#endif
	}

	private void RegisterNinjAchievment()
	{
		if (!Social.localUser.authenticated)
		{
			return;
		}
		int num = oldNinjaJumpsCount + 1;
		if (oldNinjaJumpsCount < 50)
		{
			Storager.setInt("NinjaJumpsCount", num, false);
		}
		oldNinjaJumpsCount = num;
		if (Storager.hasKey("ParkourNinjaAchievementCompleted") || num < 50)
		{
			return;
		}
		if (BuildSettings.BuildTargetPlatform == RuntimePlatform.Android && (Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GoogleLite || Defs.AndroidEdition == Defs.RuntimeAndroidEdition.GooglePro))
		{
			PlayGamesPlatform.Instance.IncrementAchievement("CgkIr8rGkPIJEAIQAw", 1, delegate(bool success)
			{
				Debug.Log("Achievement Parkour Ninja incremented: " + success);
			});
		}
		Storager.setInt("ParkourNinjaAchievementCompleted", 1, false);
	}

	private IEnumerator EnableSecondJump()
	{
		yield return new WaitForSeconds(0.25f);
		secondJumpEnabled = true;
	}

	private void OnDestroy()
	{
		if (!isMulti || isMine)
		{
			PauseNGUIController.InvertCamUpdated -= HandleInvertCamUpdated;
		}
	}
}
