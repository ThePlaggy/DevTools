using UnityEngine;

public class LucasPlusManager : MonoBehaviour
{
	public enum LucasPlusAudioType
	{
		idiot,
		comeout
	}

	public static LucasPlusManager Ins;

	public bool playedComeOut;

	private AudioSource audioSource;

	private GameObject lucasPlusDebugMesh;

	private void Awake()
	{
		Ins = this;
	}

	private void Start()
	{
		Debug.Log("[Lucas+] Lucas+ Active");
		audioSource = base.gameObject.AddComponent<AudioSource>();
	}

	private void OnDestroy()
	{
		Ins = null;
		LookUp.Doors.BalconyDoor.DoorOpenEvent.RemoveListener(TriggerBalconyJump);
	}

	public void TriggerBalconyJump()
	{
		GameManager.TimeSlinger.KillTimer(EnemyManager.HitManManager.LockPickTimer);
		HitmanBehaviour.Ins.KillWalk();
		HitmanBehaviour.Ins.DeSpawn();
		InstantiateLucasDebugMesh(new Vector3(-5.0073f, 39.2191f, 2.5891f), Quaternion.identity);
		lucasPlusDebugMesh.GetComponent<Animator>().runtimeAnimatorController = CustomObjectLookUp.BalconyJumpAnimator;
		HitmanRoamJumper.Ins.myRoamController.LockFromDoorRecovry();
		HitmanRoamJumper.Ins.myRoamController.SetMasterLock(setLock: true);
		DataManager.LockSave = true;
		PauseManager.LockPause();
		GameManager.InteractionManager.LockInteraction();
		LookUp.Doors.BalconyDoor.CancelAutoClose();
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			Debug.Log("[Lucas+] Balcony Jump");
			MainCameraHook.Ins.TriggerHitManJump();
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit3);
		});
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			audioSource.clip = CustomSoundLookUp.idiot_1;
			audioSource.volume = 1f;
			audioSource.Play();
		});
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			DataManager.ClearGameData();
			MainCameraHook.Ins.ClearARF();
			UIManager.TriggerHardGameOver("ASSASSINATED");
			HitmanRoamJumper.Ins.ClearPPVol();
		});
		GameManager.TimeSlinger.FireTimer(2.8f, delegate
		{
			HitmanBehaviour.Ins.GunFlashBombMaker();
		});
	}

	public void PlayAudio(LucasPlusAudioType audioType)
	{
		switch (audioType)
		{
		case LucasPlusAudioType.idiot:
			HitmanBehaviour.Ins.PlayIdiot();
			break;
		case LucasPlusAudioType.comeout:
			HitmanBehaviour.Ins.PlayComeOut();
			break;
		}
	}

	public void InstantiateLucasDebugMesh(Vector3 location, Quaternion rotation)
	{
		lucasPlusDebugMesh = Object.Instantiate(CustomObjectLookUp.HitmanTest, location, rotation);
		lucasPlusDebugMesh.GetComponent<Animator>().runtimeAnimatorController = null;
	}

	public void StageBalconyDoorJump()
	{
		Debug.Log("[Lucas+] Stagging Balcony Jump....");
		LookUp.Doors.BalconyDoor.DoorOpenEvent.AddListener(TriggerBalconyJump);
	}

	public void PlayComeOut()
	{
		if (!playedComeOut)
		{
			playedComeOut = true;
			PlayAudio(LucasPlusAudioType.comeout);
		}
	}
}
