using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class LOLPYDiscTrigger : MonoBehaviour
{
	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= leftClickAction;
	}

	private void leftClickAction()
	{
		myInteractionHook.ForceLock = true;
		GameManager.ManagerSlinger.LOLPYDiscManager.LOLPYDiscBeh.InsertMe();
	}

	private void theDiscWasPickedUp()
	{
		myInteractionHook.ForceLock = false;
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myInteractionHook.LeftClickAction += leftClickAction;
		myInteractionHook.ForceLock = true;
		GameManager.ManagerSlinger.LOLPYDiscManager.DiscWasPickedUp += theDiscWasPickedUp;
	}
}
