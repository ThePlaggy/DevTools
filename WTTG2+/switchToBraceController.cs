using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class switchToBraceController : MonoBehaviour
{
	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.RightClickAction += enterBraceMode;
	}

	private void OnDestroy()
	{
		myInteractionHook.RightClickAction -= enterBraceMode;
	}

	private void enterBraceMode()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).LoseControl();
		ControllerManager.Get<braceController>(GAME_CONTROLLER.BRACE).TakeOverFromRoam();
	}
}
