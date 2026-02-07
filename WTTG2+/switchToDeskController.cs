using UnityEngine;

public class switchToDeskController : MonoBehaviour
{
	private BoxCollider myBoxCollider;

	private void Awake()
	{
		myBoxCollider = GetComponent<BoxCollider>();
		GetComponent<InteractionHook>().LeftClickAction += switchToDeskCon;
	}

	private void OnDestroy()
	{
		GetComponent<InteractionHook>().LeftClickAction -= switchToDeskCon;
	}

	private void switchToDeskCon()
	{
		ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).LoseControl();
		ControllerManager.Get<deskController>(GAME_CONTROLLER.DESK).TakeOverFromRoam();
		ComputerChairObject.Ins.SetToInUsePosition();
	}
}
