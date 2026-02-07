using DG.Tweening;
using UnityEngine;

public class DollMakerBehaviour : MonoBehaviour
{
	public static DollMakerBehaviour Ins;

	[SerializeField]
	private Vector3 doorSpawnPOS = Vector3.zero;

	[SerializeField]
	private Vector3 doorSpawnROT = Vector3.zero;

	[SerializeField]
	private Transform helperBone;

	[SerializeField]
	private AudioHubObject footHub;

	[SerializeField]
	private AudioHubObject voiceHub;

	[SerializeField]
	private AudioSource hardVoiceSource;

	[SerializeField]
	private AudioClip theTalkClip;

	[SerializeField]
	private AudioFileDefinition disapointMeClip;

	[SerializeField]
	private SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[0];

	[SerializeField]
	private SkinnedMeshRenderer knife;

	[SerializeField]
	private MeshRenderer manikin;

	[SerializeField]
	private AudioFileDefinition[] footStepSFXs = new AudioFileDefinition[0];

	private bool fidgetActive;

	private float fidgetTimeStamp;

	private float fidgetTimeWindow;

	private bool inMesh;

	private bool inMeshCheckActive;

	private float inMeshCheckTimeStamp;

	public CustomEvent InMeshEvents = new CustomEvent(2);

	private Animator myAC;

	public CustomEvent NotInMeshEvents = new CustomEvent(2);

	public Transform ManikinTransform => manikin.transform;

	public Transform HelperBone => helperBone;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = false;
		}
		manikin.enabled = false;
		knife.enabled = false;
	}

	private void Update()
	{
		if (fidgetActive && Time.time - fidgetTimeStamp >= fidgetTimeWindow)
		{
			fidgetActive = false;
			int num = Random.Range(0, 10);
			if (num < 5)
			{
				TriggerAnim("fidget1");
			}
			else
			{
				TriggerAnim("fidget2");
			}
			fidgetTimeWindow = Random.Range(7f, 16f);
			fidgetTimeStamp = Time.time;
			fidgetActive = true;
		}
		if (inMeshCheckActive && Time.time - inMeshCheckTimeStamp >= 0.05f)
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
		inMesh = true;
	}

	private void OnTriggerStay(Collider other)
	{
		inMesh = true;
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
		Vector3 position = TargetTransform.position - TargetTransform.forward * 0.85f;
		position.y -= YOffSet;
		base.transform.position = position;
		base.transform.rotation = TargetTransform.rotation;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
	}

	public void SpawnBehindDesk()
	{
		TriggerAnim("triggerDeskJumpIdle");
		base.transform.position = new Vector3(2.013f, 39.5846f, -3.731f);
		base.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		knife.enabled = true;
	}

	public void TriggerAnim(string SetTrigger)
	{
		myAC.SetTrigger(SetTrigger);
	}

	public void StageDoorSpawn()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		manikin.enabled = true;
		TriggerAnim("triggerDollGrabIdle");
		base.transform.position = doorSpawnPOS;
		base.transform.rotation = Quaternion.Euler(doorSpawnROT);
		manikin.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
	}

	public void StageSpeech()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit5);
		TriggerAnim("triggerSpeechIdle");
		knife.enabled = true;
		Vector3 position = roamController.Ins.transform.position - roamController.Ins.transform.forward * 1f;
		position.y = doorSpawnPOS.y;
		base.transform.position = position;
		base.transform.rotation = roamController.Ins.transform.rotation;
	}

	public void TriggerSpeech()
	{
		TriggerAnim("triggerSpeechStart");
	}

	public void TriggerUniJump()
	{
		TriggerAnim("triggerUniJump");
	}

	public void TriggerDeskJump()
	{
		TriggerAnim("triggerDeskJump");
	}

	public void DeSpawn()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = false;
		}
		knife.enabled = false;
		manikin.enabled = false;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		manikin.transform.localPosition = Vector3.zero;
		manikin.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public void TriggerFootSound()
	{
		int num = Random.Range(1, footStepSFXs.Length);
		AudioFileDefinition audioFileDefinition = footStepSFXs[num];
		footHub.PlaySoundWithWildPitch(footStepSFXs[num], 0.9f, 1f);
		footStepSFXs[num] = footStepSFXs[0];
		footStepSFXs[0] = audioFileDefinition;
	}

	public void TriggerPower()
	{
		EnemyManager.DollMakerManager.DoorPowerTrip();
	}

	public void TriggerTheTalk()
	{
		hardVoiceSource.clip = theTalkClip;
		hardVoiceSource.Play();
		GameManager.TimeSlinger.FireTimer(58f, endSpeech);
		fidgetTimeWindow = Random.Range(7f, 16f);
		fidgetTimeStamp = Time.time;
		fidgetActive = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => hardVoiceSource.transform.localPosition, delegate(Vector3 x)
		{
			hardVoiceSource.transform.localPosition = x;
		}, new Vector3(0.2836f, -0.2156f, 0.328f), 2f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => hardVoiceSource.panStereo, delegate(float x)
		{
			hardVoiceSource.panStereo = x;
		}, 1f, 2.4f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void TriggerDisapointMe()
	{
		voiceHub.PlaySound(disapointMeClip);
	}

	public void TriggerStab()
	{
		MainCameraHook.Ins.AddHeadHit(0f);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KnifeStab2);
	}

	public void TriggerSlash()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KnifeStab1);
		MainCameraHook.Ins.FadeDoubleVis(2f, 3f);
	}

	public void FloorHit()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FloorHit);
	}

	private void endSpeech()
	{
		TriggerAnim("triggerEndSpeech");
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.HeadHit, 0.75f);
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			DollMakerRoamJumper.Ins.ClearSpeechJump();
		});
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			DeSpawn();
			LookUp.Doors.Door8.ForceOpenDoor();
		});
		GameManager.TimeSlinger.FireTimer(4.5f, delegate
		{
			TriggerAnim("triggerClear");
			EnemyManager.DollMakerManager.ClearWarningTrigger();
		});
	}
}
