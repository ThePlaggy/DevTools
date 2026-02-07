using DG.Tweening;
using SWS;
using UnityEngine;
using UnityEngine.AI;

public class HitmanBehaviour : MonoBehaviour
{
	public static HitmanBehaviour Ins;

	[SerializeField]
	private SkinnedMeshRenderer hitmanMesh;

	[SerializeField]
	private SkinnedMeshRenderer gunMesh;

	[SerializeField]
	private MeshRenderer gunFlash;

	[SerializeField]
	private AudioHubObject voiceHub;

	[SerializeField]
	private AudioHubObject footHub;

	[SerializeField]
	private AudioHubObject gunHub;

	[SerializeField]
	private PathManagerDefinition walkFromMainDoorPath;

	[SerializeField]
	private PathManagerDefinition walkIntoMainRoomPath;

	[SerializeField]
	private PathManagerDefinition walkOutOfMainRoomPath;

	[SerializeField]
	private AudioFileDefinition gunShotSFX;

	[SerializeField]
	private AudioFileDefinition prayForYouSFX;

	[SerializeField]
	private AudioFileDefinition youFoolSFX;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	[SerializeField]
	private PatrolPointDefinition bathRoomPatrolPoint;

	private PatrolPointDefinition currentPatrolPoint;

	private bool destInProgress;

	public CustomEvent GunFlashDoneEvents = new CustomEvent(2);

	private bool hadPathPreviousFrame;

	private bool killWalking;

	private Animator myAC;

	private NavMeshAgent myNavMeshAgent;

	public CustomEvent ReachedEndPath = new CustomEvent(2);

	public CustomEvent ReachedEndPoint = new CustomEvent(2);

	private Tween turnTween;

	public CustomEvent WildCardEvents = new CustomEvent(2);

	public CustomEvent WildSelfDestructEvents = new CustomEvent(2);

	public bool FootStepSounds { get; set; }

	public bool InBathRoom { get; private set; }

	public HitmanSpawnDefinition SpawnData { get; set; }

	public splineMove SplineMove { get; private set; }

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
		SplineMove = GetComponent<splineMove>();
		myNavMeshAgent = GetComponent<NavMeshAgent>();
		hitmanMesh.enabled = false;
		gunMesh.enabled = false;
		gunFlash.enabled = false;
		myNavMeshAgent.enabled = false;
		SplineMove.PathIsCompleted += pathIsDone;
	}

	private void Update()
	{
		if (destInProgress && myNavMeshAgent.enabled)
		{
			if (myNavMeshAgent.hasPath)
			{
				myAC.SetFloat("walking", myNavMeshAgent.velocity.magnitude);
				hadPathPreviousFrame = true;
			}
			else if (hadPathPreviousFrame)
			{
				hadPathPreviousFrame = false;
				reachedEndPoint();
			}
		}
	}

	private void OnDestroy()
	{
		SplineMove.PathIsCompleted -= pathIsDone;
		GunFlashDoneEvents.Clear();
		WildCardEvents.Clear();
		ReachedEndPoint.Clear();
		ReachedEndPath.Clear();
	}

	public void Spawn(HitmanSpawnDefinition SetSpawnData = null)
	{
		if (HitmanProxyBehaviour.FromElevator)
		{
			Spawn(new Vector3(-5.14f, 39.582f, -6.266f), new Vector3(0f, 90f, 0f));
			FollowPath(4f, GameObject.Find("HitmanDoorPath1").GetComponent<PathManager>());
		}
		else
		{
			Spawn(new Vector3(1.155f, 39.582f, -6.266f), new Vector3(0f, -90f, 0f));
			FollowPath(4f, GameObject.Find("HitmanDoorPath2").GetComponent<PathManager>());
		}
	}

	public void Spawn(Vector3 SetPOS, Vector3 SetROT)
	{
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
		hitmanMesh.enabled = true;
		SkinnedMeshRenderer[] componentsInChildren = hitmanMesh.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = true;
		}
	}

	public void DeSpawn()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		hitmanMesh.enabled = false;
		SkinnedMeshRenderer[] componentsInChildren = hitmanMesh.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		gunMesh.enabled = false;
		gunFlash.enabled = false;
		FootStepSounds = false;
		GunFlashDoneEvents.Clear();
		WildCardEvents.Clear();
		ReachedEndPoint.Clear();
	}

	public void FollowPath(PathManagerDefinition SetPath)
	{
		Debug.Log("[HitmanBehaviour] FollowPath");
		killWalking = false;
		setWalkRate(0f, 1f, 0.35f);
		setWalkRate(1f, 0f, 0.4f, SetPath.PathTime - 0.15f);
		SplineMove.pathContainer = SetPath.ThePath;
		SplineMove.SetPath(SetPath.ThePath);
		SplineMove.speed = SetPath.PathTime;
		SplineMove.StartMove();
	}

	public void FollowPath(float PathTime, PathManager ThePath)
	{
		Debug.Log("[HitmanBehaviour] FollowPath NEW");
		killWalking = false;
		setWalkRate(0f, 1f, 0.35f);
		setWalkRate(1f, 0f, 0.4f, PathTime - 0.15f);
		SplineMove.pathContainer = ThePath;
		SplineMove.SetPath(ThePath);
		SplineMove.speed = PathTime;
		SplineMove.StartMove();
	}

	public void PatrolTo(PatrolPointDefinition Point)
	{
		currentPatrolPoint = Point;
		myNavMeshAgent.enabled = true;
		myNavMeshAgent.SetDestination(Point.Position);
		destInProgress = true;
	}

	public void GotoTarget(Vector3 Destination)
	{
		myNavMeshAgent.enabled = true;
		myNavMeshAgent.SetDestination(Destination);
		destInProgress = true;
	}

	public void TriggerAnim(string SetTrigger)
	{
		myAC.SetTrigger(SetTrigger);
	}

	public void ActivateGunMesh()
	{
		gunMesh.enabled = true;
	}

	public void WalkIntoMainRoom()
	{
		FollowPath(walkIntoMainRoomPath);
	}

	public void LeaveMainRoom()
	{
		WildCardEvents.Event += openMainDoorFromInside;
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 160.688f, 0f), 0.35f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				TriggerAnim("mainDoorOpenInside");
			});
	}

	public void ExitMainRoom()
	{
		FollowPath(walkOutOfMainRoomPath);
	}

	public void WalkAwayFromMainDoor()
	{
		turnTween = DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(0f, 90f, 0f), 0.75f).SetEase(Ease.Linear).SetOptions()
			.OnComplete(delegate
			{
				FollowPath(walkFromMainDoorPath);
			});
	}

	public void KillWalk()
	{
		killWalking = true;
		myAC.SetFloat("walking", 0f);
		turnTween.Kill();
		SplineMove.Stop();
	}

	public void KillPatrol()
	{
		ReachedEndPoint.Clear();
		myNavMeshAgent.enabled = false;
		destInProgress = false;
		myAC.SetFloat("walking", 0f);
	}

	public void EnterMainRoom()
	{
		WildCardEvents.Event += openMainDoorFromOutSide;
		Ins.Spawn(new Vector3(-2.309f, 39.589f, -5.935f), Vector3.zero);
		Ins.TriggerAnim("mainDoorOpenOutside");
	}

	public void EnterBathRoom()
	{
		InBathRoom = true;
		LookUp.Doors.BathroomDoor.NPCOpenDoor();
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			PatrolTo(bathRoomPatrolPoint);
			GameManager.TimeSlinger.FireTimer(16.5f, delegate
			{
				InBathRoom = false;
				LookUp.Doors.BathroomDoor.ForceDoorClose();
			});
		});
	}

	public void TriggerGunFlash()
	{
		gunHub.PlaySound(gunShotSFX);
		gunFlash.enabled = true;
		GameManager.TimeSlinger.FireTimer(0.15f, delegate
		{
			GunFlashDoneEvents.Execute();
			gunFlash.enabled = false;
		});
	}

	public void TriggerPrayForYouAni()
	{
		myAC.SetTrigger("prayForYou");
	}

	public void TriggerYouFool()
	{
		myAC.SetTrigger("youFool");
	}

	public void SayPrayForYou()
	{
		voiceHub.PlaySound(prayForYouSFX);
	}

	public void SayYouFool()
	{
		voiceHub.PlaySound(youFoolSFX);
	}

	public void TriggerWildCardEvent()
	{
		WildCardEvents.Execute();
		WildSelfDestructEvents.ExecuteAndKill();
	}

	public void TriggerFootSound()
	{
		if (FootStepSounds)
		{
			int num = Random.Range(1, footStepSFXs.Length);
			AudioFileDefinition audioFileDefinition = footStepSFXs[num];
			footHub.PlaySoundWithWildPitch(footStepSFXs[num], 0.85f, 1.1f);
			footStepSFXs[num] = footStepSFXs[0];
			footStepSFXs[0] = audioFileDefinition;
		}
	}

	private void setWalkRate(float FromValue, float ToValue, float Duration)
	{
		if (killWalking)
		{
			return;
		}
		GameManager.TweenSlinger.FireDOSTweenLiner(FromValue, ToValue, Duration, delegate(float setValue)
		{
			if (!killWalking)
			{
				myAC.SetFloat("walking", setValue);
			}
		});
	}

	private void setWalkRate(float FromValue, float ToValue, float Duration, float Delay)
	{
		GameManager.TimeSlinger.FireTimer(Delay, setWalkRate, FromValue, ToValue, Duration);
	}

	private void reachedEndPoint()
	{
		myAC.SetFloat("walking", 0f);
		destInProgress = false;
		myNavMeshAgent.enabled = false;
		if (currentPatrolPoint != null)
		{
			currentPatrolPoint.InvokeEvents();
		}
		ReachedEndPoint.Execute();
	}

	private void pathIsDone()
	{
		ReachedEndPath.Execute();
	}

	private void openMainDoorFromOutSide()
	{
		WildCardEvents.Event -= openMainDoorFromOutSide;
		WildCardEvents.Event += enterMainRoomFromOutSide;
		LookUp.Doors.MainDoor.NPCOpenDoor();
	}

	private void enterMainRoomFromOutSide()
	{
		FootStepSounds = true;
		WildCardEvents.Event -= enterMainRoomFromOutSide;
		WalkIntoMainRoom();
	}

	private void openMainDoorFromInside()
	{
		WildCardEvents.Event -= openMainDoorFromInside;
		LookUp.Doors.MainDoor.NPCOpenDoor();
		GameManager.TimeSlinger.FireTimer(1.3f, exitRoom);
	}

	private void exitRoom()
	{
		SplineMove.PathIsCompleted += deSpawn;
		ExitMainRoom();
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			FootStepSounds = false;
			LookUp.Doors.MainDoor.ForceDoorClose();
		});
	}

	private void deSpawn()
	{
		EnemyManager.HitManManager.DeSpawn();
		SplineMove.PathIsCompleted -= deSpawn;
		if (LookUp.Doors.MainDoor.SetDistanceLODs != null)
		{
			for (int i = 0; i < LookUp.Doors.MainDoor.SetDistanceLODs.Length; i++)
			{
				LookUp.Doors.MainDoor.SetDistanceLODs[i].OverwriteCulling = false;
			}
		}
		DeSpawn();
	}

	public void GunFlashBombMaker()
	{
		AudioFileDefinition audioFileDefinition = Object.Instantiate(gunShotSFX);
		audioFileDefinition.MyAudioLayer = AUDIO_LAYER.PLAYER;
		audioFileDefinition.MyAudioHub = AUDIO_HUB.PLAYER_HUB;
		GameManager.AudioSlinger.PlaySound(audioFileDefinition);
		Object.Destroy(audioFileDefinition);
	}

	public void PlayIdiot()
	{
		AudioFileDefinition audioFileDefinition = Object.Instantiate(youFoolSFX);
		audioFileDefinition.AudioClip = CustomSoundLookUp.idiot_1;
		audioFileDefinition.Volume = 1f;
		audioFileDefinition.Delay = false;
		voiceHub.PlaySound(audioFileDefinition);
	}

	public void PlayComeOut()
	{
		AudioFileDefinition audioFileDefinition = Object.Instantiate(prayForYouSFX);
		audioFileDefinition.AudioClip = CustomSoundLookUp.comeout;
		audioFileDefinition.Volume = 0.5f;
		audioFileDefinition.Delay = false;
		voiceHub.PlaySound(audioFileDefinition);
	}
}
