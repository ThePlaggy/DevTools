using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class switchToComputerController : MonoBehaviour
{
	public static switchToComputerController Ins;

	[HideInInspector]
	public InteractionDisplayHook computerDisplayHook;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		Ins = this;
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += switchToComputerCon;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= switchToComputerCon;
	}

	public void Lock()
	{
		myInteractionHook.ForceLock = true;
	}

	public void UnLock()
	{
		myInteractionHook.ForceLock = false;
	}

	private void switchToComputerCon()
	{
		ControllerManager.Get<deskController>(GAME_CONTROLLER.DESK).SwitchToComputerController();
	}
}
