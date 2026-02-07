using UnityEngine;

public class HitmanLobbyComputerJump : Jump
{
	public static bool ATApartment;

	protected override void DoStage()
	{
		HitmanBehaviour.Ins.GunFlashDoneEvents.Event += EnemyManager.HitManManager.GunFlashGameOver;
		HitmanBehaviour.Ins.ActivateGunMesh();
		HitmanBehaviour.Ins.TriggerAnim("hallDoorJumpIdle");
		if (ATApartment)
		{
			HitmanBehaviour.Ins.Spawn(new Vector3(1.96f, 39.35f, -3.6482f), new Vector3(0f, 90f, 0f));
			BombMakerDeskJumper.Ins.StageLobbyLucas();
		}
		else
		{
			HitmanBehaviour.Ins.Spawn(new Vector3(3.63f, 0f, -21.47f), new Vector3(0.2f, 176.9f, 0f));
		}
	}

	protected override void DoExecute()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.JumpHit3);
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			LucasPlusManager.Ins.PlayAudio(LucasPlusManager.LucasPlusAudioType.idiot);
		});
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			HitmanBehaviour.Ins.TriggerAnim("hallDoorJump");
		});
	}
}
