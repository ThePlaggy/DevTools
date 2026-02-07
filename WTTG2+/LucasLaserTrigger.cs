using DG.Tweening;
using UnityEngine;

public class LucasLaserTrigger : MonoBehaviour
{
	public static int stood;

	public static bool frozen;

	public static int jumpmode;

	private void OnTriggerEnter(Collider other)
	{
		if (jumpmode == 1)
		{
			Debug.Log("[LucasLaser] Laser hit player, early jump");
			jumpmode++;
			LucasLaserManager.Ins.CancelTweener();
			LucasLaserManager.ExplodeGameOver();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (StateManager.PlayerState == PLAYER_STATE.ROAMING && other.gameObject.name == "roamController" && !frozen)
		{
			stood++;
			if (stood >= 100)
			{
				LucasLaserManager.Ins.CancelTweener();
				PauseManager.LockPause();
				float y = LucasLaserManager.Ins.myLaser.transform.eulerAngles.y;
				float z = LucasLaserManager.Ins.myLaser.transform.eulerAngles.z;
				LucasLaserManager.Ins.myLaser.transform.DORotate(new Vector3(359.8181f, y, z), 0.35f, RotateMode.FastBeyond360);
				GameManager.TimeSlinger.FireTimer(0.25f, LucasLaserManager.ExplodeGameOver);
			}
		}
	}
}
