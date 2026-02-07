using UnityEngine;

public class TannerMainDoorJump : Jump
{
	protected override void DoStage()
	{
		TannerBehaviour.Ins.SwitchToNormal();
		TannerBehaviour.Ins.Spawn(new Vector3(-2.28f, 39.586f, -5.455f), Vector3.zero);
		TannerBehaviour.Ins.TriggerAnim("StageClosetJump");
		GameManager.TimeSlinger.FireTimer(0.4f, delegate
		{
			roamController.Ins.KillOutOfWay();
			TannerBehaviour.Ins.TriggerRushPlayerJump();
		});
	}

	protected override void DoExecute()
	{
		LookUp.Doors.MainDoor.CancelAutoClose();
	}
}
