using System;
using DG.Tweening;
using UnityEngine;

public class FemaleNoirBehavior : MonoBehaviour
{
	public static FemaleNoirBehavior Ins;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private GameObject hammer;

	[SerializeField]
	private Transform cameraBone;

	[SerializeField]
	private NewNoirTrigger minigameTrigger;

	[SerializeField]
	private NewNoirTrigger turnAroundTrigger;

	[SerializeField]
	private NewNoirTrigger tooCloseTrigger;

	private bool behindJump;

	private bool checkPlayerRotation;

	private bool canSafelyRotate;

	private bool dontMove;

	private float dontMoveRotation;

	private float dontMovePosX;

	private float dontMovePosZ;

	public bool alleywaySpawn;

	private Vector2 desiredRot;

	public AudioHubObject myAho;

	private void Awake()
	{
		Ins = this;
		myAho = base.gameObject.AddComponent<AudioHubObject>();
	}

	public void PlayVoiceCommand(FEMALE_NOIR_VOICE_COMMAND command)
	{
		if (!(myAho == null))
		{
			switch (command)
			{
			case FEMALE_NOIR_VOICE_COMMAND.COME_CLOSER:
				myAho.PlaySound(CustomSoundLookUp.ComeCloser);
				break;
			case FEMALE_NOIR_VOICE_COMMAND.NOT_THAT_CLOSE:
				myAho.PlaySound(CustomSoundLookUp.NotThatClose);
				break;
			case FEMALE_NOIR_VOICE_COMMAND.DONT_MOVE:
				myAho.PlaySound(CustomSoundLookUp.DontMove);
				break;
			case FEMALE_NOIR_VOICE_COMMAND.BOO:
				myAho.PlaySound(CustomSoundLookUp.Boo);
				break;
			case FEMALE_NOIR_VOICE_COMMAND.LAUGH:
				myAho.PlaySound(CustomSoundLookUp.NoirLaugh);
				break;
			}
		}
	}

	public void SpawnOnAlleywayEntry()
	{
		EnemyStateManager.AddEnemyState(ENEMY_STATE.NEWNOIR);
		alleywaySpawn = true;
		base.transform.position = new Vector3(25f, 0f, 199.8f);
		base.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
		minigameTrigger.playerEnterEvent.Event += BeginMinigame;
		desiredRot = new Vector2(250f, 290f);
	}

	public void Spawn8thFloor()
	{
		base.transform.position = new Vector3(-3.173f, 39.579f, -6.236f);
		base.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
		minigameTrigger.playerEnterEvent.Event += BeginMinigame;
		desiredRot = new Vector2(70f, 110f);
	}

	public void Stage8thFloor()
	{
		EnemyStateManager.AddEnemyState(ENEMY_STATE.NEWNOIR);
		alleywaySpawn = false;
	}

	private void BeginMinigame()
	{
		minigameTrigger.playerEnterEvent.Event -= BeginMinigame;
		minigameTrigger.playerLeftEvent.Event += TriggerMaleNoirKill;
		tooCloseTrigger.playerEnterEvent.Event += TriggerTooCloseKill;
		turnAroundTrigger.playerEnterEvent.Event += TriggerHeadTilt;
		roamController.Ins.startedRunning.Event += TriggerMaleNoirKill;
		roamController.Ins.startedDucking.Event += TriggerMaleNoirKill;
		PlayVoiceCommand(FEMALE_NOIR_VOICE_COMMAND.COME_CLOSER);
		checkPlayerRotation = true;
	}

	private void TriggerMaleNoirKill()
	{
		if (!behindJump)
		{
			minigameTrigger.playerLeftEvent.Event -= TriggerMaleNoirKill;
			roamController.Ins.startedRunning.Event -= TriggerMaleNoirKill;
			roamController.Ins.startedDucking.Event -= TriggerMaleNoirKill;
			Debug.Log("[Female Noir] Triggering Male Noir Kill");
			MaleNoirBehavior.Ins.StageBehindJump();
		}
	}

	private void TriggerTooCloseKill()
	{
		if (!behindJump)
		{
			tooCloseTrigger.playerEnterEvent.Event -= TriggerTooCloseKill;
			PlayVoiceCommand(FEMALE_NOIR_VOICE_COMMAND.NOT_THAT_CLOSE);
			GameManager.TimeSlinger.FireTimer(1.25f, delegate
			{
				hammer.SetActive(value: true);
			});
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit4);
			PauseManager.LockPause();
			roamController.Ins.SetMasterLock(setLock: true);
			FlashLightBehaviour.Ins.LockOut();
			GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
			CameraManager.Get(roamController.Ins.CameraIControl, out var MyCamera);
			GameManager.TimeSlinger.FireTimer(0.5f, delegate
			{
				Transform transform = MyCamera.transform;
				transform.SetParent(cameraBone);
				transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.1f).SetEase(Ease.Linear);
				transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.1f).SetEase(Ease.Linear);
			});
			GameManager.TimeSlinger.FireTimer(0.76f, delegate
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
			});
			GameManager.TimeSlinger.FireTimer(2.06f, delegate
			{
				HammerHit();
			});
			GameManager.TimeSlinger.FireTimer(3.33f, delegate
			{
				HammerHit();
			});
			GameManager.TimeSlinger.FireTimer(4f, delegate
			{
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FloorHit);
			});
			GameManager.TimeSlinger.FireTimer(5.75f, delegate
			{
				MainCameraHook.Ins.ClearARF(0.45f);
			});
			GameManager.TimeSlinger.FireTimer(6f, delegate
			{
				UIManager.TriggerGameOver("KILLED");
			});
			animator.SetBool("TooClose", value: true);
		}
	}

	public void StageBehindJump(bool bypassCheck = false)
	{
		Debug.Log("[Female Noir] Staged Behind Jump");
		animator.SetBool("StageBehindJump", value: true);
		behindJump = true;
		if (Physics.Raycast(roamController.Ins.transform.position, roamController.Ins.transform.forward * -1f, 1f) && !bypassCheck)
		{
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				StageBehindJump();
			});
		}
		else
		{
			TriggerBehindJump();
		}
	}

	public void TriggerBehindJump()
	{
		hammer.SetActive(value: true);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit4);
		PlayVoiceCommand(FEMALE_NOIR_VOICE_COMMAND.BOO);
		GameObject.Find("SM_NoirFemale.mo").GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
		animator.SetBool("BehindJump", value: true);
		roamController.Ins.LoseControl();
		PauseManager.LockPause();
		FlashLightBehaviour.Ins.LockOut();
		GameManager.BehaviourManager.CrossHairBehaviour.HideCrossHairGroup();
		base.transform.position = roamController.Ins.transform.position + roamController.Ins.transform.forward * -1f - new Vector3(0f, 0.95f, 0f);
		Vector3 eulerAngles = Quaternion.LookRotation(roamController.Ins.transform.position - base.transform.position).eulerAngles;
		eulerAngles.x = 0f;
		eulerAngles.z = 0f;
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		CameraManager.Get(roamController.Ins.CameraIControl, out var ReturnCamera);
		Transform transform = ReturnCamera.transform;
		transform.SetParent(cameraBone);
		transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.2f).SetEase(Ease.Linear);
		transform.DOLocalRotate(new Vector3(0f, 180f, 0f), 0.2f).SetEase(Ease.Linear);
		GameManager.TimeSlinger.FireTimer(5.75f, delegate
		{
			MainCameraHook.Ins.ClearARF(0.45f);
		});
		GameManager.TimeSlinger.FireTimer(6f, delegate
		{
			UIManager.TriggerGameOver("KILLED");
		});
		GameManager.TimeSlinger.FireTimer(1.5f, HammerHit);
		GameManager.TimeSlinger.FireTimer(2.4f, HammerHit);
		GameManager.TimeSlinger.FireTimer(3.2f, delegate
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.BodyHit);
		});
		GameManager.TimeSlinger.FireTimer(3.93f, FinalHit);
	}

	private void TriggerHeadTilt()
	{
		turnAroundTrigger.playerEnterEvent.Event -= TriggerHeadTilt;
		canSafelyRotate = true;
		checkPlayerRotation = true;
		animator.SetTrigger("HeadTilt");
	}

	public void HammerHit()
	{
		MainCameraHook.Ins.AddHeadHit();
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.HeadHit);
	}

	public void FinalHit()
	{
		MainCameraHook.Ins.AddHeadHit(2f);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.HeadHit);
	}

	private void Update()
	{
		if (dontMove)
		{
			bool flag = (double)Math.Abs(roamController.Ins.transform.position.x - dontMovePosX) > 0.2;
			bool flag2 = Math.Abs(roamController.Ins.transform.rotation.eulerAngles.y - dontMoveRotation) > 2f;
			bool flag3 = (double)Math.Abs(roamController.Ins.transform.position.z - dontMovePosZ) > 0.2;
			bool flag4 = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E);
			if (flag || flag2 || flag3 || flag4)
			{
				dontMove = false;
				Debug.Log("[Female Noir] Dont Move Failed!");
				StageBehindJump(bypassCheck: true);
			}
		}
		else if (checkPlayerRotation)
		{
			Debug.Log(roamController.Ins.transform.rotation.eulerAngles);
			if (roamController.Ins.transform.rotation.eulerAngles.y > desiredRot.x && roamController.Ins.transform.rotation.eulerAngles.y < desiredRot.y)
			{
				PlayerRotated();
			}
		}
	}

	public static int GetPowerOffChances()
	{
		return EnemyManager.CultManager.keyDiscoveryCount * 5 + 30;
	}

	private void PlayerRotated()
	{
		checkPlayerRotation = false;
		if (!canSafelyRotate)
		{
			StageBehindJump(bypassCheck: true);
			return;
		}
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			if (roamController.Ins.transform.rotation.eulerAngles.y > desiredRot.x && roamController.Ins.transform.rotation.eulerAngles.y < desiredRot.y)
			{
				dontMoveRotation = roamController.Ins.transform.rotation.eulerAngles.y;
				dontMovePosX = roamController.Ins.transform.position.x;
				dontMovePosZ = roamController.Ins.transform.position.z;
				dontMove = true;
				AudioSource source = base.gameObject.AddComponent<AudioSource>();
				source.clip = CustomSoundLookUp.DontMove.AudioClip;
				source.Play();
				source.spatialBlend = 0f;
				source.dopplerLevel = 0f;
				GameManager.TimeSlinger.FireTimer(10f, delegate
				{
					minigameTrigger.playerLeftEvent.Event -= TriggerMaleNoirKill;
					roamController.Ins.startedRunning.Event -= TriggerMaleNoirKill;
					roamController.Ins.startedDucking.Event -= TriggerMaleNoirKill;
					dontMove = false;
					base.transform.position = Vector3.zero;
					EnemyStateManager.RemoveEnemyState(ENEMY_STATE.NEWNOIR);
					UnityEngine.Object.Destroy(source);
				});
			}
			else
			{
				StageBehindJump(bypassCheck: true);
			}
		});
	}
}
