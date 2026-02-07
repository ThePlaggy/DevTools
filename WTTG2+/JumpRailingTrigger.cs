using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class JumpRailingTrigger : MonoBehaviour
{
	public HotZoneTrigger BalcoyHotZone;

	public HotZoneTrigger StairWellHotZone;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += leftClickAction;
	}

	private void Update()
	{
		if (!BalcoyHotZone.IsHot && !StairWellHotZone.IsHot)
		{
			myInteractionHook.ForceLock = true;
		}
		else
		{
			myInteractionHook.ForceLock = false;
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.RightClickAction -= leftClickAction;
	}

	private void leftClickAction()
	{
		if (BalcoyHotZone.IsHot)
		{
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).JumpRailingFromBalcoy();
		}
		else if (StairWellHotZone.IsHot)
		{
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).JumpRailingFromStairWell();
		}
	}
}
