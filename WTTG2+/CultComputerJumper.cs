using UnityEngine;

public class CultComputerJumper : MonoBehaviour
{
	public static CultComputerJumper Ins;

	private computerController myComputerController;

	private void Awake()
	{
		Ins = this;
		myComputerController = GetComponent<computerController>();
	}

	private void OnDestroy()
	{
	}

	public void AddLightsOffJump()
	{
		myComputerController.LeaveEvents.Event += playerLeftComputerModeLightsOffJump;
	}

	public void playerLeftComputerModeLightsOffJump()
	{
		myComputerController.LeaveEvents.Event -= playerLeftComputerModeLightsOffJump;
		EnemyManager.CultManager.StageDeskJump();
		CultDeskJumper.Ins.TriggerLightsOffJump();
	}

	public void AddLightsOffJumpAndGo()
	{
		myComputerController.LeaveEvents.Event += playerLeftComputerModeLightsOffJump;
		myComputerController.LeaveMe();
	}

	public void JustGo()
	{
		myComputerController.LeaveMe();
	}
}
