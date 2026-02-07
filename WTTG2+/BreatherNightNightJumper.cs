using UnityEngine;

public class BreatherNightNightJumper : MonoBehaviour
{
	private void OnDestroy()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerJump;
	}

	public void AddComputerJump()
	{
		CamHookBehaviour.Interruptions = true;
		CamHookBehaviour.SwitchCameraStatus(enabled: false);
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event += TriggerJump;
	}

	public void TriggerJump()
	{
		HitmanComputerJumper.Ins.myComputerController.LeaveEvents.Event -= TriggerJump;
		DataManager.LockSave = true;
		PauseManager.LockPause();
		ComputerChairObject.Ins.Hide();
		GameManager.InteractionManager.LockInteraction();
		BombMakerDeskJumper.Ins.myDeskController.LockRecovery();
		BombMakerDeskJumper.Ins.myDeskController.SetMasterLock(setLock: true);
		BombMakerDeskJumper.Ins.BreatherRotator();
		BreatherNightNightTrigger.me.ActivateJumpscare();
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit7);
		GameManager.TimeSlinger.FireTimer(1.05f, BreatherNightNightTrigger.me.PlayBreatherSound, CustomSoundLookUp.breather_nightnight);
		GameManager.TimeSlinger.FireTimer(1.6f, DOMoveBreatherfix);
		GameManager.TimeSlinger.FireTimer(4f, BreatherBehaviour.Ins.KnifeSlash);
		GameManager.TimeSlinger.FireTimer(4.2f, BreatherBehaviour.Ins.TriggerGameOver);
	}

	private void DOMoveBreatherfix()
	{
		BreatherNightNightTrigger.me.MoveBreatherDown();
	}
}
