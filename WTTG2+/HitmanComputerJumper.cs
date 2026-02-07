using UnityEngine;

[RequireComponent(typeof(computerController))]
public class HitmanComputerJumper : MonoBehaviour
{
	public static HitmanComputerJumper Ins;

	[SerializeField]
	private float delayTriggerJumpTime = 10f;

	public computerController myComputerController;

	private float delayTriggerTimeStamp;

	private void Awake()
	{
		Ins = this;
		myComputerController = GetComponent<computerController>();
	}

	private void OnDestroy()
	{
		myComputerController.EnterEvents.Event -= playerEntersComputerModeDelayTrigger;
		myComputerController.LeaveEvents.Event -= playerLeftComputerModeDelayTrigger;
		myComputerController.LeaveEvents.Event -= playerLeftComputerModeTrigger;
	}

	public void AddDelayComputerJump()
	{
		myComputerController.EnterEvents.Event += playerEntersComputerModeDelayTrigger;
		myComputerController.LeaveEvents.Event += playerLeftComputerModeDelayTrigger;
		GameManager.TimeSlinger.FireTimer(delayTriggerJumpTime, delegate
		{
			doorlogBehaviour.MayAddDoorlog("Apartment 803", mode: true);
		});
	}

	public void AddComputerJump()
	{
		myComputerController.LeaveEvents.Event += playerLeftComputerModeTrigger;
		doorlogBehaviour.MayAddDoorlog("Apartment 803", mode: true);
	}

	private void playerEntersComputerModeDelayTrigger()
	{
		delayTriggerTimeStamp = Time.time;
	}

	private void playerLeftComputerModeDelayTrigger()
	{
		if (Time.time - delayTriggerTimeStamp >= delayTriggerJumpTime)
		{
			myComputerController.EnterEvents.Event -= playerEntersComputerModeDelayTrigger;
			myComputerController.LeaveEvents.Event -= playerLeftComputerModeDelayTrigger;
			HitmanDeskJumper.Ins.TriggerJump();
		}
	}

	private void playerLeftComputerModeTrigger()
	{
		myComputerController.LeaveEvents.Event -= playerLeftComputerModeTrigger;
		HitmanDeskJumper.Ins.TriggerJump();
	}
}
