using UnityEngine;

public class SpawnToLobbyTrigger : MonoBehaviour
{
	public Vector3 LobbyPOS;

	public Vector3 LobbyROT;

	private bool lockedOut;

	private void OnTriggerEnter(Collider other)
	{
		triggerSpawn();
	}

	public void LockOut()
	{
		lockedOut = true;
	}

	private void triggerSpawn()
	{
		if (lockedOut)
		{
			return;
		}
		EnemyManager.BreatherManager.PlayerLeftDeadDrop();
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).SpawnMeTo(LobbyPOS, LobbyROT, 1f);
		if (EnemyStateManager.HasEnemyState(ENEMY_STATE.NEWNOIR) && FemaleNoirBehavior.Ins.alleywaySpawn)
		{
			if (EnvironmentManager.PowerState == POWER_STATE.OFF)
			{
				EnvironmentManager.PowerBehaviour.PowerOnNewNoir();
			}
			PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.NEWNOIR);
			FemaleNoirBehavior.Ins.transform.position = Vector3.zero;
			MaleNoirBehavior.Ins.Stage8thFloorHallwayJump();
		}
		if (EnemyStateManager.HasEnemyState(ENEMY_STATE.DELFALCO))
		{
			if (DelfalcoBehaviour.Ins.alleywayChase)
			{
				DelfalcoBehaviour.Ins.PlayerRanAway();
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(2f, DelfalcoBehaviour.Ins.StageBehindJump);
			}
		}
	}
}
