using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

[RequireComponent(typeof(CharacterController))]
public class moveableController : mouseableController
{
	public bool DisableRun;

	public bool DisableDuck;

	public Vector3 DuckCameraPOS;

	public Vector3 DefaultCameraPOS;

	public Vector3 DefaultCameraROT;

	public float DuckSpeed = 3f;

	public float WalkSpeed = 5f;

	public float RunSpeed = 10f;

	[Range(0f, 1f)]
	public float RunStepLenghten;

	[Range(0f, 1f)]
	public float WalkStepLengten;

	public float DuckStepLenghten = 3f;

	public float GravityMultiplier = 2f;

	public float StickToGroundForce = 10f;

	public bool UseHeadBob;

	public float StepInterval;

	public CurveControlledBob HeadBob = new CurveControlledBob();

	[SerializeField]
	public LayerMask hitLayers;

	public AUDIO_HUB MyFootAudioHub;

	public AudioFileDefinition[] WalkFootStepSFXS;

	public AudioFileDefinition[] RunFootStepSFXS;

	public AudioFileDefinition[] DuckFootStepSFXS;

	private Vector2 currentInput;

	private Vector3 currentMoveDir;

	private Vector3 duckCameraDiff;

	private RaycastHit hitInfo;

	protected bool MoveableControllerInit;

	protected CharacterController MyCharcterController;

	private float nextStep;

	private float speed;

	private float stepCycle;

	private Vector3 theMove;

	public CustomEvent startedRunning = new CustomEvent(5);

	public CustomEvent startedDucking = new CustomEvent(5);

	protected new void Awake()
	{
		base.Awake();
	}

	protected new void Start()
	{
		base.Start();
		Debug.Log("CONTROLLER: " + base.gameObject.name);
	}

	protected new void Update()
	{
		base.Update();
		if (TarotManager.CurSpeed == playerSpeedMode.WEAK)
		{
			RunSpeed = 3f;
		}
		else if (TarotManager.CurSpeed == playerSpeedMode.QUICK)
		{
			RunSpeed = 10f;
		}
		else
		{
			RunSpeed = 5f;
		}
	}

	private void FixedUpdate()
	{
		if (!MoveableControllerInit || lockControl)
		{
			return;
		}
		getInput(out speed);
		theMove = base.transform.forward * currentInput.y + base.transform.right * (currentInput.x * 1f);
		Physics.SphereCast(base.transform.position, MyCharcterController.radius, Vector3.down, out hitInfo, MyCharcterController.height / 2f, hitLayers.value);
		theMove = Vector3.ProjectOnPlane(theMove, hitInfo.normal).normalized;
		currentMoveDir.x = theMove.x * speed;
		currentMoveDir.z = theMove.z * speed;
		if (MyCharcterController.isGrounded)
		{
			currentMoveDir.y = 0f - StickToGroundForce;
		}
		else
		{
			currentMoveDir += Physics.gravity * GravityMultiplier * Time.fixedDeltaTime;
		}
		MyCharcterController.Move(currentMoveDir * Time.fixedDeltaTime);
		UpdateCameraPOS(speed);
		processStep(speed);
		if (!(hitInfo.collider != null))
		{
			return;
		}
		SurfaceTypeObject component = hitInfo.collider.GetComponent<SurfaceTypeObject>();
		if (stepCycle > nextStep)
		{
			nextStep = stepCycle + StepInterval;
			if (component != null && component.HasCustomFootStepSFXS)
			{
				playSurfaceFootStepAudio(component);
			}
			else
			{
				playFootStepAudio();
			}
		}
	}

	protected new void OnDestroy()
	{
		base.OnDestroy();
	}

	protected new void Init()
	{
		base.Init();
		MyCharcterController = GetComponent<CharacterController>();
		duckCameraDiff = DuckCameraPOS - DefaultCameraPOS;
		currentInput = Vector2.zero;
		currentMoveDir = Vector3.zero;
		stepCycle = 0f;
		nextStep = stepCycle / 2f;
		if (cameraIsSet)
		{
			HeadBob.Setup(MyCamera, StepInterval);
			MoveableControllerInit = true;
		}
	}

	private void getInput(out float setSpeed)
	{
		float num = 0f;
		float num2 = 0f;
		float axis = CrossPlatformInputManager.GetAxis("Horizontal");
		float axis2 = CrossPlatformInputManager.GetAxis("Vertical");
		if (!DisableRun)
		{
			num = CrossPlatformInputManager.GetAxis("Run");
		}
		if (!DisableDuck)
		{
			num2 = CrossPlatformInputManager.GetAxis("Duck");
		}
		currentInput = new Vector2(axis, axis2);
		if (currentInput.sqrMagnitude > 1f)
		{
			currentInput.Normalize();
		}
		if (num2 > 0f)
		{
			MyState = GAME_CONTROLLER_STATE.DUCKING;
			startedDucking.Execute();
			setSpeed = DuckSpeed;
			return;
		}
		setSpeed = Mathf.Max(Mathf.Abs(axis2), Mathf.Abs(axis));
		setSpeed = ((axis2 <= 0f || num <= 0f) ? (WalkSpeed * setSpeed) : (RunSpeed * Mathf.Abs(axis2)));
		if (setSpeed > WalkSpeed)
		{
			MyState = GAME_CONTROLLER_STATE.RUNING;
			startedRunning.Execute();
		}
		else if (setSpeed > 0f)
		{
			MyState = GAME_CONTROLLER_STATE.WALKING;
		}
		else
		{
			MyState = GAME_CONTROLLER_STATE.IDLE;
		}
	}

	private void UpdateCameraPOS(float setSpeed)
	{
		if (UseHeadBob)
		{
			switch (MyState)
			{
			case GAME_CONTROLLER_STATE.RUNING:
			{
				Vector3 localPosition3;
				if (MyCharcterController.velocity.magnitude > 0f && MyCharcterController.isGrounded)
				{
					localPosition3 = HeadBob.DoHeadBob(MyCharcterController.velocity.magnitude + setSpeed * RunStepLenghten, UseRun: true);
				}
				else
				{
					localPosition3 = MyCamera.transform.localPosition;
					localPosition3.y = DefaultCameraPOS.y;
				}
				MyCamera.transform.localPosition = localPosition3;
				break;
			}
			default:
			{
				Vector3 localPosition2;
				if (MyCharcterController.velocity.magnitude > 0f && MyCharcterController.isGrounded)
				{
					localPosition2 = HeadBob.DoHeadBob(MyCharcterController.velocity.magnitude + setSpeed * WalkStepLengten, UseRun: false);
				}
				else
				{
					localPosition2 = MyCamera.transform.localPosition;
					localPosition2.y = DefaultCameraPOS.y;
				}
				MyCamera.transform.localPosition = localPosition2;
				break;
			}
			case GAME_CONTROLLER_STATE.DUCKING:
			{
				float axis = CrossPlatformInputManager.GetAxis("Duck");
				Vector3 localPosition = default(Vector3);
				localPosition.x = DefaultCameraPOS.x + duckCameraDiff.x * axis;
				localPosition.y = DefaultCameraPOS.y + duckCameraDiff.y * axis;
				localPosition.z = DefaultCameraPOS.z + duckCameraDiff.z * axis;
				MyCamera.transform.localPosition = localPosition;
				break;
			}
			}
		}
		else if (MyState != GAME_CONTROLLER_STATE.DUCKING)
		{
			MyCamera.transform.localPosition = DefaultCameraPOS;
		}
		else
		{
			float axis2 = CrossPlatformInputManager.GetAxis("Duck");
			Vector3 localPosition4 = default(Vector3);
			localPosition4.x = DefaultCameraPOS.x + duckCameraDiff.x * axis2;
			localPosition4.y = DefaultCameraPOS.y + duckCameraDiff.y * axis2;
			localPosition4.z = DefaultCameraPOS.z + duckCameraDiff.z * axis2;
			MyCamera.transform.localPosition = localPosition4;
		}
	}

	private void processStep(float setSpeed)
	{
		if (MyCharcterController.velocity.sqrMagnitude > 0f && (currentInput.x != 0f || currentInput.y != 0f))
		{
			switch (MyState)
			{
			case GAME_CONTROLLER_STATE.DUCKING:
				stepCycle += (MyCharcterController.velocity.magnitude + setSpeed * DuckStepLenghten) * Time.fixedDeltaTime;
				break;
			case GAME_CONTROLLER_STATE.RUNING:
				stepCycle += (MyCharcterController.velocity.magnitude + setSpeed * RunStepLenghten) * Time.fixedDeltaTime;
				break;
			case GAME_CONTROLLER_STATE.WALKING:
				stepCycle += (MyCharcterController.velocity.magnitude + setSpeed * WalkStepLengten) * Time.fixedDeltaTime;
				break;
			}
		}
	}

	private void playFootStepAudio()
	{
		if (!MyCharcterController.isGrounded)
		{
			return;
		}
		switch (MyState)
		{
		case GAME_CONTROLLER_STATE.DUCKING:
		{
			int num3 = Random.Range(1, DuckFootStepSFXS.Length);
			if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER) && GetType() == typeof(roamController))
			{
				EXESoundPopper.PopSound(1337);
			}
			GameManager.AudioSlinger.PlaySoundWithWildPitch(DuckFootStepSFXS[num3], 0.85f, 1.1f);
			AudioFileDefinition audioFileDefinition3 = DuckFootStepSFXS[num3];
			DuckFootStepSFXS[num3] = DuckFootStepSFXS[0];
			DuckFootStepSFXS[0] = audioFileDefinition3;
			break;
		}
		case GAME_CONTROLLER_STATE.RUNING:
		{
			int num2 = Random.Range(1, RunFootStepSFXS.Length);
			if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER) && GetType() == typeof(roamController))
			{
				EXESoundPopper.PopSound(1337);
			}
			GameManager.AudioSlinger.PlaySoundWithWildPitch(RunFootStepSFXS[num2], 0.85f, 1.1f);
			AudioFileDefinition audioFileDefinition2 = RunFootStepSFXS[num2];
			RunFootStepSFXS[num2] = RunFootStepSFXS[0];
			RunFootStepSFXS[0] = audioFileDefinition2;
			break;
		}
		case GAME_CONTROLLER_STATE.WALKING:
		{
			int num = Random.Range(1, WalkFootStepSFXS.Length);
			if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER) && GetType() == typeof(roamController))
			{
				EXESoundPopper.PopSound(1337);
			}
			GameManager.AudioSlinger.PlaySoundWithWildPitch(WalkFootStepSFXS[num], 0.85f, 1.1f);
			AudioFileDefinition audioFileDefinition = WalkFootStepSFXS[num];
			WalkFootStepSFXS[num] = WalkFootStepSFXS[0];
			WalkFootStepSFXS[0] = audioFileDefinition;
			break;
		}
		}
	}

	private void playSurfaceFootStepAudio(SurfaceTypeObject STO)
	{
		if (MyCharcterController.isGrounded)
		{
			switch (MyState)
			{
			case GAME_CONTROLLER_STATE.DUCKING:
			{
				int index3 = Random.Range(1, STO.MyFootStepSFXS.DuckFootStepSFXS.Count);
				GameManager.AudioSlinger.PlaySoundWithWildPitch(STO.MyFootStepSFXS.DuckFootStepSFXS[index3], 0.85f, 1.1f);
				AudioFileDefinition value3 = STO.MyFootStepSFXS.DuckFootStepSFXS[index3];
				STO.MyFootStepSFXS.DuckFootStepSFXS[index3] = STO.MyFootStepSFXS.DuckFootStepSFXS[0];
				STO.MyFootStepSFXS.DuckFootStepSFXS[0] = value3;
				break;
			}
			case GAME_CONTROLLER_STATE.RUNING:
			{
				int index2 = Random.Range(1, STO.MyFootStepSFXS.RunFootStepSFXS.Count);
				GameManager.AudioSlinger.PlaySoundWithWildPitch(STO.MyFootStepSFXS.RunFootStepSFXS[index2], 0.85f, 1.1f);
				AudioFileDefinition value2 = STO.MyFootStepSFXS.RunFootStepSFXS[index2];
				STO.MyFootStepSFXS.RunFootStepSFXS[index2] = STO.MyFootStepSFXS.RunFootStepSFXS[0];
				STO.MyFootStepSFXS.RunFootStepSFXS[0] = value2;
				break;
			}
			case GAME_CONTROLLER_STATE.WALKING:
			{
				int index = Random.Range(1, STO.MyFootStepSFXS.WalkFootStepSFXS.Count);
				GameManager.AudioSlinger.PlaySoundWithWildPitch(STO.MyFootStepSFXS.WalkFootStepSFXS[index], 0.85f, 1.1f);
				AudioFileDefinition value = STO.MyFootStepSFXS.WalkFootStepSFXS[index];
				STO.MyFootStepSFXS.WalkFootStepSFXS[index] = STO.MyFootStepSFXS.WalkFootStepSFXS[0];
				STO.MyFootStepSFXS.WalkFootStepSFXS[0] = value;
				break;
			}
			}
		}
	}
}
