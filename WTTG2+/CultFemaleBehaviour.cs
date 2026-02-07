using UnityEngine;

public class CultFemaleBehaviour : MonoBehaviour
{
	public static CultFemaleBehaviour Ins;

	[SerializeField]
	private SkinnedMeshRenderer hammerMesh;

	[SerializeField]
	private Transform lookAtObject;

	[SerializeField]
	private Transform cameraBone;

	[SerializeField]
	private AudioHubObject voiceHub;

	[SerializeField]
	private AudioFileDefinition booSFX;

	[SerializeField]
	private AudioFileDefinition laugh1SFX;

	[SerializeField]
	private float heightDifference = 1f;

	public CustomEvent InValidSpawnLocationEvent = new CustomEvent(2);

	private Animator myAC;

	private CultSpawner mySpawner;

	public CustomEvent ValidSpawnLocationEvent = new CustomEvent(2);

	public Vector3 LookAtPosition => lookAtObject.position;

	public Transform CameraBone => cameraBone;

	private void Awake()
	{
		Ins = this;
		hammerMesh.enabled = false;
		myAC = GetComponent<Animator>();
		mySpawner = GetComponent<CultSpawner>();
	}

	private void Update()
	{
	}

	public void TriggerAnim(string SetAnim)
	{
		myAC.SetTrigger(SetAnim);
	}

	public void EnableHammerMesh()
	{
		hammerMesh.enabled = true;
	}

	public void AttemptSpawnBehindPlayer()
	{
		mySpawner.InMeshEvents.Event += notValidSpawnLocation;
		mySpawner.NotInMeshEvents.Event += validSpawnLocation;
		mySpawner.SpawnBehindPlayer(roamController.Ins.transform, heightDifference);
	}

	public void HammerJump()
	{
		EnableHammerMesh();
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			TriggerVoiceCommand(CULT_FEMALE_VOICE_COMMANDS.BOO);
		});
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			TriggerVoiceCommand(CULT_FEMALE_VOICE_COMMANDS.LAUGH1);
		});
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			TriggerVoiceCommand(CULT_FEMALE_VOICE_COMMANDS.LAUGH1);
		});
		TriggerAnim("triggerHammerJump");
	}

	public void StageDeskJump()
	{
		mySpawner.Spawn(new Vector3(3.103f, 39.585f, -2.343f), new Vector3(0f, 180f, 0f));
		GameManager.TimeSlinger.FireTimer(0.1f, delegate
		{
			EnableHammerMesh();
			TriggerAnim("deskJumpIdle");
		});
	}

	public void TriggerDeskJump()
	{
		TriggerAnim("deskJump");
	}

	public void TriggerVoiceCommand(CULT_FEMALE_VOICE_COMMANDS TheCommand)
	{
		switch (TheCommand)
		{
		case CULT_FEMALE_VOICE_COMMANDS.LAUGH1:
			voiceHub.PlaySound(laugh1SFX);
			break;
		case CULT_FEMALE_VOICE_COMMANDS.BOO:
			voiceHub.PlaySound(booSFX);
			break;
		case CULT_FEMALE_VOICE_COMMANDS.COME_CLOSER:
			voiceHub.PlaySound(CustomSoundLookUp.ComeCloser);
			break;
		case CULT_FEMALE_VOICE_COMMANDS.NOT_THAT_CLOSE:
			voiceHub.PlaySound(CustomSoundLookUp.NotThatClose);
			break;
		case CULT_FEMALE_VOICE_COMMANDS.DONT_MOVE:
			voiceHub.PlaySound(CustomSoundLookUp.DontMove);
			break;
		}
	}

	private void notValidSpawnLocation()
	{
		mySpawner.InMeshEvents.Event -= notValidSpawnLocation;
		mySpawner.NotInMeshEvents.Event -= validSpawnLocation;
		InValidSpawnLocationEvent.Execute();
	}

	private void validSpawnLocation()
	{
		mySpawner.InMeshEvents.Event -= notValidSpawnLocation;
		mySpawner.NotInMeshEvents.Event -= validSpawnLocation;
		ValidSpawnLocationEvent.Execute();
	}

	public void BodyJump()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
	}

	public void FloorHit()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FloorHit);
	}

	public void HammerHit()
	{
		MainCameraHook.Ins.AddHeadHit();
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.HeadHit);
	}

	public void Laugh()
	{
		voiceHub.PlaySound(laugh1SFX);
	}
}
