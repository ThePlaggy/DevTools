using UnityEngine;
using UnityEngine.AI;

public class SwatManBehaviour : MonoBehaviour
{
	[SerializeField]
	private float lookAtPlayerSpeed = 1f;

	[SerializeField]
	private SkinnedMeshRenderer mySkinMesh;

	[SerializeField]
	private SkinnedMeshRenderer myGunMesh;

	[SerializeField]
	private GameObject flashBangObject;

	[SerializeField]
	private Light gunLight;

	[SerializeField]
	private Transform cameraHolder;

	[SerializeField]
	private Transform lookAtObject;

	[SerializeField]
	private AudioHubObject myAudioHub;

	[SerializeField]
	private AudioHubObject footAudioHub;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	[SerializeField]
	private AudioFileDefinition getDownSFX;

	[SerializeField]
	private AudioFileDefinition policeDeptSFX;

	[SerializeField]
	private AudioFileDefinition stayDownSFX;

	[SerializeField]
	private AudioFileDefinition gotYouNowSFX;

	[SerializeField]
	private AudioFileDefinition goGoGoSFX;

	[SerializeField]
	private AudioFileDefinition clearSFX;

	[SerializeField]
	private FlashBangBehaviour flashBangBehaviour;

	[SerializeField]
	private Vector3 flashBangDirection;

	[SerializeField]
	private float flashBangForce = 5f;

	private Camera cameraIControl;

	private bool cameraLookAtMeActive;

	private bool delayFootStepActive;

	private float delayFootStepActiveTimeStamp;

	private bool destInProgress;

	private bool footStepActive;

	private int footStepCount;

	private float footStepDelay;

	private float footStepTimeStamp;

	private bool hadPathPreviousFrame;

	private float initalDelay;

	private bool lookAtPlayerActive;

	private Animator myAC;

	private AgentLinkMover myAgentLinkMover;

	private CapsuleCollider myCapCollider;

	private bool playerLookAtMeActive;

	public CustomEvent ReachedEndPoint = new CustomEvent(2);

	public NavMeshAgent NavMeshAgent { get; private set; }

	public MeshRenderer FlashBangMesh { get; private set; }

	public Light GunLight => gunLight;

	private void Update()
	{
		if (delayFootStepActive && Time.time - delayFootStepActiveTimeStamp >= initalDelay)
		{
			delayFootStepActive = false;
			footStepTimeStamp = Time.time;
			footStepActive = true;
		}
		if (footStepActive && Time.time - footStepTimeStamp >= footStepDelay)
		{
			footStepTimeStamp = Time.time;
			playFootStepSound();
			footStepCount--;
			if (footStepCount <= 0)
			{
				footStepActive = false;
			}
		}
		if (lookAtPlayerActive)
		{
			Vector3 forward = cameraIControl.transform.position - base.transform.position;
			forward.y = 0f;
			Quaternion b = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, b, lookAtPlayerSpeed * Time.deltaTime);
		}
		if (playerLookAtMeActive)
		{
			PoliceRoamJumper.Ins.TriggerConstantLookAt(lookAtObject.position);
			PoliceRoamJumper.Ins.TriggerCameraConstantLookAt(lookAtObject.position);
		}
		if (cameraLookAtMeActive)
		{
			PoliceRoamJumper.Ins.TriggerCameraConstantLookAt(lookAtObject.position);
		}
		if (destInProgress && NavMeshAgent.enabled)
		{
			if (NavMeshAgent.hasPath)
			{
				hadPathPreviousFrame = true;
			}
			else if (hadPathPreviousFrame)
			{
				hadPathPreviousFrame = false;
				ReachedEndPoint.Execute();
			}
		}
	}

	private void OnDrawGizmos()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		NavMeshAgent.enabled = false;
		myCapCollider.enabled = false;
		TakeOverCamera();
		cameraIControl.transform.localPosition = Vector3.zero;
		TriggerAnim("tackle");
		cameraLookAtMeActive = true;
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
	}

	public void Build()
	{
		myAC = GetComponent<Animator>();
		NavMeshAgent = GetComponent<NavMeshAgent>();
		myCapCollider = GetComponent<CapsuleCollider>();
		FlashBangMesh = flashBangObject.GetComponent<MeshRenderer>();
		myAgentLinkMover = GetComponent<AgentLinkMover>();
		NavMeshAgent.enabled = false;
		myCapCollider.enabled = false;
		mySkinMesh.enabled = false;
		myGunMesh.enabled = false;
		gunLight.enabled = false;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		CameraManager.Get(CAMERA_ID.MAIN, out cameraIControl);
	}

	public void SpawnMe(Vector3 SetPOS, Vector3 SetROT)
	{
		mySkinMesh.enabled = true;
		myGunMesh.enabled = true;
		gunLight.enabled = true;
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
	}

	public void SpawnMe(Vector3 SetPOS, Vector3 SetROT, string SetAnim)
	{
		TriggerAnim(SetAnim);
		SpawnMe(SetPOS, SetROT);
	}

	public void TriggerAnim(string SetTrigger)
	{
		myAC.SetTrigger(SetTrigger);
	}

	public void TriggerVoiceCommand(SWAT_VOICE_COMMANDS SetCommand, float DelayAmount = 0f)
	{
		switch (SetCommand)
		{
		case SWAT_VOICE_COMMANDS.GET_DOWN:
			myAudioHub.PlaySoundCustomDelay(getDownSFX, DelayAmount);
			break;
		case SWAT_VOICE_COMMANDS.POLICE_DEPT:
			myAudioHub.PlaySoundCustomDelay(policeDeptSFX, DelayAmount);
			break;
		case SWAT_VOICE_COMMANDS.STAY_DOWN:
			myAudioHub.PlaySoundCustomDelay(stayDownSFX, DelayAmount);
			break;
		case SWAT_VOICE_COMMANDS.GOT_YOU:
			myAudioHub.PlaySoundCustomDelay(gotYouNowSFX, DelayAmount);
			break;
		case SWAT_VOICE_COMMANDS.GO_GO:
			myAudioHub.PlaySoundCustomDelay(goGoGoSFX, DelayAmount);
			break;
		case SWAT_VOICE_COMMANDS.CLEAR:
			myAudioHub.PlaySoundCustomDelay(clearSFX, DelayAmount);
			break;
		}
	}

	public void TriggerFootSteps(float InitalDelayAmount, int FootStepCount, float FootStepDelay)
	{
		footStepCount = FootStepCount;
		footStepDelay = FootStepDelay;
		initalDelay = InitalDelayAmount;
		delayFootStepActiveTimeStamp = Time.time;
		delayFootStepActive = true;
	}

	public void TriggerFootSteps(int FootStepCount, float FootStepDelay)
	{
		footStepCount = FootStepCount;
		footStepDelay = FootStepDelay;
		footStepTimeStamp = Time.time;
		footStepActive = true;
	}

	public void TakeOverCamera()
	{
		cameraIControl.transform.SetParent(cameraHolder);
	}

	public void GoToTarget(Vector3 SetPOS)
	{
		NavMeshAgent.enabled = true;
		NavMeshAgent.SetDestination(SetPOS);
	}

	public void ChargePlayer()
	{
		myAgentLinkMover.CrossSpeed = 1f;
		myCapCollider.enabled = true;
		NavMeshAgent.acceleration = 12f;
		NavMeshAgent.speed = 4f;
		NavMeshAgent.angularSpeed = 240f;
		NavMeshAgent.autoTraverseOffMeshLink = false;
		NavMeshAgent.enabled = true;
		playerLookAtMeActive = true;
		NavMeshAgent.SetDestination(roamController.Ins.transform.position);
	}

	public void OnFootDown()
	{
		playFootStepSound();
	}

	public void BeginWalkCycle(Vector3 TargetDestination, float Speed)
	{
		NavMeshAgent.acceleration = Speed * 0.5f;
		NavMeshAgent.speed = Speed;
		BeginWalkCycle(TargetDestination);
	}

	public void BeginWalkCycle(Vector3 TargetDestination)
	{
		ReachedEndPoint.Event += EndWalkCycle;
		TriggerAnim("walk");
		GoToTarget(TargetDestination);
		destInProgress = true;
	}

	public void EndWalkCycle()
	{
		ReachedEndPoint.Event -= EndWalkCycle;
		destInProgress = false;
		TriggerAnim("crouchIdle1");
		lookAtPlayerActive = true;
	}

	public void TossFlashBang()
	{
		flashBangObject.transform.SetParent(null);
		flashBangObject.GetComponent<Rigidbody>().isKinematic = false;
		flashBangObject.GetComponent<Rigidbody>().AddForce(flashBangDirection * flashBangForce, ForceMode.VelocityChange);
		flashBangBehaviour.Thrown();
	}

	public void TriggerBodyHit()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FloorHit);
	}

	public void TriggerGotYou()
	{
		TriggerVoiceCommand(SWAT_VOICE_COMMANDS.GOT_YOU);
	}

	private void playFootStepSound()
	{
		int num = Random.Range(1, footStepSFXs.Length);
		AudioFileDefinition audioFileDefinition = footStepSFXs[num];
		footAudioHub.PlaySoundWithWildPitch(footStepSFXs[num], 0.5f, 1.25f);
		footStepSFXs[num] = footStepSFXs[0];
		footStepSFXs[0] = audioFileDefinition;
	}
}
