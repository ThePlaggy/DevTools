using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class switchToPeepHoleController : MonoBehaviour
{
	public DoorTrigger MainDoorTrigger;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		MainDoorTrigger.DoorOpenEvent.AddListener(theDoorOpened);
		MainDoorTrigger.DoorCloseEvent.AddListener(theDoorClosed);
		myInteractionHook.LeftClickAction += switchToPeepHoleCon;
	}

	private void OnDestroy()
	{
		MainDoorTrigger.DoorOpenEvent.RemoveListener(theDoorOpened);
		MainDoorTrigger.DoorCloseEvent.RemoveListener(theDoorClosed);
		myInteractionHook.LeftClickAction -= switchToPeepHoleCon;
	}

	private void switchToPeepHoleCon()
	{
		ControllerManager.Get<peepHoleController>(GAME_CONTROLLER.PEEP_HOLE).DoTakeOver();
	}

	private void theDoorOpened()
	{
		myInteractionHook.ForceLock = true;
	}

	private void theDoorClosed()
	{
		myInteractionHook.ForceLock = false;
	}
}
