using UnityEngine;

public class BombMakerMainDoorJump : Jump
{
	protected override void DoStage()
	{
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.BombMakerApartmentJump, Vector3.zero, Quaternion.identity);
		GameObject.Find("SuitGEO01").GetComponent<SkinnedMeshRenderer>().material = BombMakerRecolorer.chosenMat;
		gameObject.transform.position = new Vector3(-2.282f, 39.75f, -4.09f);
		gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 200f, 0f));
		HitmanRoamJumper.Ins.TriggerMainDoorOutSideJump();
		BombMakerApartmentJump BMAJ = gameObject.GetComponent<BombMakerApartmentJump>();
		GameManager.AudioSlinger.PlaySoundWithCustomDelay(LookUp.SoundLookUp.JumpHit1, 0.3f);
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			BMAJ.ShoulderRotate();
		});
		GameManager.TimeSlinger.FireTimer(0.8f, delegate
		{
			BMAJ.gunRecoil();
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
		LookUp.Doors.MainDoor.CancelAutoClose();
	}

	public void GunFlashGameOver()
	{
		DataManager.ClearGameData();
		MainCameraHook.Ins.ClearARF();
		UIManager.TriggerHardGameOver("YOU WERE USELESS TO THE BOMB MAKER");
	}
}
