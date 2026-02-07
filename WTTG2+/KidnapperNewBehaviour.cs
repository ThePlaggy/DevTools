using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(deskController))]
public class KidnapperNewBehaviour : MonoBehaviour
{
	public static KidnapperNewBehaviour Ins;

	[SerializeField]
	private Animator myAnimator;

	[SerializeField]
	private AudioHubObject myAHO;

	[SerializeField]
	private Light myHelperLight;

	private bool dofRemoved;

	private void Awake()
	{
		Ins = this;
	}

	private void OnDestroy()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= ExecPCAnimation;
		Ins = null;
		LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(ExecOutsideAnimation);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(DoorScrub);
		if (dofRemoved)
		{
			if (GameObject.Find("NoDOF") != null)
			{
				GameObject.Find("NoDOF").GetComponent<PostProcessVolume>().isGlobal = false;
			}
			else
			{
				Debug.Log("Tried to destroy kidnapper PP Vol, but it's null");
			}
		}
	}

	public void Spawn(Vector3 POS, Vector3 ROT)
	{
		base.transform.position = POS;
		base.transform.rotation = Quaternion.Euler(ROT);
	}

	private void ExecPCAnimation()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= ExecPCAnimation;
		float intensity = myHelperLight.intensity;
		myHelperLight.intensity = 0f;
		myHelperLight.enabled = true;
		myHelperLight.DOIntensity(intensity, 0.5f);
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		BombMakerDeskJumper.Ins.myDeskController.LockRecovery();
		BombMakerDeskJumper.Ins.myDeskController.SetMasterLock(setLock: true);
		BombMakerDeskJumper.Ins.KidnapperRotator();
		myAnimator.SetTrigger("pcJump");
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.kidnapperjump);
		myAHO.PlaySoundCustomDelay(CustomSoundLookUp.kidnapperline, 1f);
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.punch, 2.62f);
		GameManager.TimeSlinger.FireTimer(3.02f, delegate
		{
			MainCameraHook.Ins.AddHeadHit(3f);
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.HeadHit);
			GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.EarRing, 0.1f);
			BombMakerDeskJumper.Ins.KidnapperDeRotator();
			GameManager.TimeSlinger.FireTimer(0.4f, delegate
			{
				DataManager.ClearGameData();
				MainCameraHook.Ins.ClearARF();
				UIManager.TriggerGameOver("YOU HAVE BEEN KIDNAPPED");
			});
		});
	}

	public void AddJump()
	{
		Debug.Log("[Kidnapper] Added PC jump");
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event += ExecPCAnimation;
		TrackerManager.NotifyUserBeingTracked(-1f);
	}

	public void AddOutsideJump()
	{
		Debug.Log("[Kidnapper] Added Outside jump");
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(ExecOutsideAnimation);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.AddListener(DoorScrub);
	}

	private void DoorScrub()
	{
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(DoorScrub);
		LookUp.Doors.MainDoor.CancelAutoClose();
	}

	public void StageInsideJumpscare()
	{
		myAnimator.SetTrigger("doorJumpInsideStage");
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
	}

	public void UnstageInsideJumpscare()
	{
		Debug.Log("[Kidnapper] Unstaged Outside jump");
		myAnimator.SetTrigger("doorJumpInsideUnstage");
		base.transform.position = new Vector3(6.244856f, -5.422453f, -9.668027f);
		LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(ExecOutsideAnimation);
		LookUp.Doors.MainDoor.DoorWasOpenedEvent.RemoveListener(DoorScrub);
		CamHookBehaviour.Interruptions = false;
		CamHookBehaviour.SwitchCameraStatus(enabled: true);
	}

	private void ExecOutsideAnimation()
	{
		LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(ExecOutsideAnimation);
		dofRemoved = true;
		DataManager.LockSave = true;
		PauseManager.LockPause();
		myHelperLight.enabled = true;
		myHelperLight.DOIntensity(0.45f, 0.5f);
		if (StateManager.PlayerLocation == PLAYER_LOCATION.HALL_WAY8)
		{
			HitmanRoamJumper.Ins.TriggerMainDoorOutSideJump();
			GameObject.Find("NoDOF").GetComponent<PostProcessVolume>().isGlobal = true;
			base.transform.DOMoveX(-2.256f, 1.25f).OnComplete(delegate
			{
				myAHO.PlaySound(CustomSoundLookUp.kidnapperline);
			});
		}
		else
		{
			base.transform.position = new Vector3(-3.186f, 39.78f, -5.5f);
			HitmanRoamJumper.Ins.TriggerMainDoorOpenJump();
			GameManager.TimeSlinger.FireTimer(0.6f, delegate
			{
				roamController.Ins.transform.DOMoveZ(-4.8114f, 0.6f);
			});
			base.transform.DOMoveX(-2.186f, 1.25f).OnComplete(delegate
			{
				myAHO.PlaySound(CustomSoundLookUp.kidnapperline);
			});
		}
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.kidnapperjump, 0.5f);
		GameManager.TimeSlinger.FireTimer(0.6f, delegate
		{
			myAnimator.SetTrigger("doorJumpInside");
		});
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(CustomSoundLookUp.punch, 4.5f);
		GameManager.TimeSlinger.FireTimer(4.7f, PunchPlayer);
	}

	private void PunchPlayer()
	{
		DataManager.ClearGameData();
		MainCameraHook.Ins.AddHeadHit(5f);
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.HeadHit, 0.2f);
		MainCameraHook.Ins.ClearARF();
		UIManager.TriggerGameOver("YOU HAVE BEEN KIDNAPPED");
	}
}
