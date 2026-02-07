using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class switchToLobbyComputerControllerTrigger : MonoBehaviour
{
	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += switchToLobbyComputerController;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= switchToLobbyComputerController;
	}

	private void switchToLobbyComputerController()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).LoseControl();
		ControllerManager.Get<lobbyComputerController>(GAME_CONTROLLER.LOBBY_COMPUTER).TakeControl();
	}
}
