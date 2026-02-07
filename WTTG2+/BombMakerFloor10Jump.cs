using UnityEngine;

public class BombMakerFloor10Jump : Jump
{
	protected override void DoStage()
	{
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.BombMakerHallwayJump, Vector3.zero, Quaternion.identity);
		GameObject.Find("SuitGEO01").GetComponent<SkinnedMeshRenderer>().material = BombMakerRecolorer.chosenMat;
		gameObject.transform.position = new Vector3(25.95f, 51.08f, -6.31f);
		gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, -90f, 0f));
		HitmanRoamJumper.Ins.TriggerHallWayDoorJump();
		BombMakerBehindJump BMBH = gameObject.GetComponent<BombMakerBehindJump>();
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit1, 0.3f);
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			BMBH.ElbowRot();
		});
		GameManager.TimeSlinger.FireTimer(0.8f, delegate
		{
			BMBH.gunRecoil();
			HitmanBehaviour.Ins.GunFlashBombMaker();
		});
		GameManager.TimeSlinger.FireTimer(1.2f, GunFlashGameOver);
		GameManager.TimeSlinger.FireTimer(1.25f, delegate
		{
			Object.Destroy(gameObject);
		});
	}

	protected override void DoExecute()
	{
		LookUp.Doors.Door10.CancelAutoClose();
	}

	public void GunFlashGameOver()
	{
		DataManager.ClearGameData();
		MainCameraHook.Ins.ClearARF();
		UIManager.TriggerHardGameOver("YOU WERE USELESS TO THE BOMB MAKER");
	}
}
