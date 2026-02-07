using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class BreatherBehaviour : MonoBehaviour
{
	public static BreatherBehaviour Ins;

	[SerializeField]
	private SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[0];

	[SerializeField]
	private SkinnedMeshRenderer knife;

	[SerializeField]
	private Transform helperBone;

	[SerializeField]
	private AudioHubObject voiceAudioHub;

	[SerializeField]
	private AudioHubObject footAudioHub;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	[SerializeField]
	private AudioFileDefinition[] cementFootStepsSFXs = new AudioFileDefinition[0];

	[SerializeField]
	private AudioFileDefinition laugh1;

	[SerializeField]
	private AudioFileDefinition hi;

	[SerializeField]
	private AudioFileDefinition breathing;

	[SerializeField]
	private AudioFileDefinition peekABoo;

	private bool autoUpdateDest;

	private bool destInProgress;

	private bool hadPathPreviousFrame;

	private bool inMesh;

	private bool inMeshCheckActive;

	private float inMeshCheckTimeStamp;

	public CustomEvent InMeshEvents = new CustomEvent(2);

	private Animator myAC;

	private NavMeshAgent myNavMeshAgent;

	public CustomEvent NotInMeshEvents = new CustomEvent(2);

	public CustomEvent ReachedEndPoint = new CustomEvent(2);

	public Transform HelperBone => helperBone;

	public CapsuleCollider CapsuleCollider { get; private set; }

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
		myNavMeshAgent = GetComponent<NavMeshAgent>();
		CapsuleCollider = GetComponent<CapsuleCollider>();
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = false;
		}
		knife.enabled = false;
	}

	private void Update()
	{
		if (autoUpdateDest)
		{
			myNavMeshAgent.SetDestination(roamController.Ins.transform.position);
		}
		if (inMeshCheckActive && Time.time - inMeshCheckTimeStamp >= 0.1f)
		{
			inMeshCheckActive = false;
			if (inMesh)
			{
				InMeshEvents.Execute();
			}
			else
			{
				NotInMeshEvents.Execute();
			}
			inMesh = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (destInProgress && other.tag == "Player")
		{
			destInProgress = false;
			autoUpdateDest = false;
			myNavMeshAgent.enabled = false;
			ReachedEndPoint.Execute();
		}
		inMesh = true;
	}

	private void OnTriggerStay(Collider other)
	{
		inMesh = true;
	}

	public void TriggerAnim(string SetAnim)
	{
		myAC.SetTrigger(SetAnim);
	}

	public void SoftSpawn()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
	}

	public void DeSpawn()
	{
		voiceAudioHub.KillSound(breathing.AudioClip);
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = false;
		}
		knife.enabled = false;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	public void HardDeSpawn()
	{
		voiceAudioHub.KillSound(breathing.AudioClip);
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = false;
		}
		knife.enabled = false;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		myNavMeshAgent.enabled = false;
		BreatherPatrolBehaviour.Ins.KillPatrol();
		TriggerAnim("triggerIdle");
	}

	public void AttemptSpawnBehindPlayer(Transform TargetTransform, float YOffSet = 0f)
	{
		Vector3 position = TargetTransform.position - TargetTransform.forward * 0.85f;
		position.y -= YOffSet;
		base.transform.position = position;
		base.transform.rotation = TargetTransform.rotation;
		inMeshCheckTimeStamp = Time.time;
		inMeshCheckActive = true;
	}

	public void SpawnBehindPlayer(Transform TargetTransform, float YOffSet = 0f)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
		Vector3 position = TargetTransform.position - TargetTransform.forward * 0.85f;
		position.y -= YOffSet;
		base.transform.position = position;
		base.transform.rotation = TargetTransform.rotation;
	}

	public void TriggerExitRush()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
		base.transform.position = new Vector3(-0.623f, 0f, 202.565f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
		ReachedEndPoint.Event += triggerExitJump;
		chargePlayer();
	}

	public void TriggerPickUpRush()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
		base.transform.position = new Vector3(21.62f, 0f, 199.889f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
		ReachedEndPoint.Event += triggerExitJump;
		chargePlayer();
	}

	public void TriggerDumpsterJump()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
		base.transform.position = new Vector3(15.003f, 0f, 200.83f);
		base.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		TriggerAnim("triggerDumpsterJump");
	}

	public void TriggerWalkToDoor()
	{
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 108.791f, 0f), 0.5f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				voiceAudioHub.PlaySound(breathing);
				TriggerAnim("triggerWalkToDoor");
			});
	}

	public void TriggerWalkAwayFromDoor()
	{
		TriggerAnim("triggerWalkAwayFromDoor");
	}

	public void TriggerDoorJump()
	{
		voiceAudioHub.KillSound(breathing.AudioClip);
		TriggerAnim("triggerDoorJump");
		voiceAudioHub.PlaySoundCustomDelay(laugh1, 0.3f);
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.HOLDTHEDOOR);
		});
	}

	public void TriggerPeekABooJump()
	{
		TriggerAnim("triggerPeekABooJump");
	}

	public void TriggerVoice(BREATHER_VOICE_COMMANDS TheCommand)
	{
		switch (TheCommand)
		{
		case BREATHER_VOICE_COMMANDS.PEEKABOO:
			voiceAudioHub.PlaySound(peekABoo);
			break;
		case BREATHER_VOICE_COMMANDS.HI:
			voiceAudioHub.PlaySound(hi);
			break;
		case BREATHER_VOICE_COMMANDS.LAUGH1:
			voiceAudioHub.PlaySound(laugh1);
			break;
		}
	}

	private void chargePlayer()
	{
		TriggerAnim("triggerRush");
		myNavMeshAgent.enabled = true;
		myNavMeshAgent.acceleration = 12f;
		myNavMeshAgent.speed = 7.5f;
		myNavMeshAgent.angularSpeed = 300f;
		autoUpdateDest = true;
		destInProgress = true;
	}

	private void triggerExitJump()
	{
		ReachedEndPoint.Event -= triggerExitJump;
		MainCameraHook.Ins.AddHeadHit(0f);
		BreatherRoamJumper.Ins.TriggerExitRushJump();
		TriggerAnim("triggerRushJump");
	}

	private void playFootStepSound()
	{
		int num = Random.Range(1, footStepSFXs.Length);
		AudioFileDefinition audioFileDefinition = footStepSFXs[num];
		footAudioHub.PlaySound(footStepSFXs[num]);
		footStepSFXs[num] = footStepSFXs[0];
		footStepSFXs[0] = audioFileDefinition;
	}

	private void playCementFootStepSound()
	{
		int num = Random.Range(1, cementFootStepsSFXs.Length);
		AudioFileDefinition audioFileDefinition = cementFootStepsSFXs[num];
		footAudioHub.PlaySound(cementFootStepsSFXs[num]);
		cementFootStepsSFXs[num] = cementFootStepsSFXs[0];
		cementFootStepsSFXs[0] = audioFileDefinition;
	}

	public void SayHi()
	{
		voiceAudioHub.PlaySound(hi);
	}

	public void SayPeekABoo()
	{
		TriggerVoice(BREATHER_VOICE_COMMANDS.PEEKABOO);
	}

	public void BodyJump()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
	}

	public void FloorHit()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FloorHit);
	}

	public void KnifeStab()
	{
		MainCameraHook.Ins.AddHeadHit(0.5f);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KnifeStab3);
	}

	public void QuickKnifeStab()
	{
		MainCameraHook.Ins.AddHeadHit(0f);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KnifeStab2);
	}

	public void KnifeSlash()
	{
		MainCameraHook.Ins.AddHeadHit(0.5f);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KnifeStab1);
	}

	public void TriggerGameOver()
	{
		UIManager.TriggerGameOver("YOU WERE KILLED");
		MainCameraHook.Ins.ClearARF(1f);
	}
}
