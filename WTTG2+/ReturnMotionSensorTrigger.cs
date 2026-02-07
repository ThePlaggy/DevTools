using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class ReturnMotionSensorTrigger : MonoBehaviour
{
	[SerializeField]
	private GameObject motionSensorObject;

	private MeshRenderer motionSensorObjectMeshRenderer;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		motionSensorObjectMeshRenderer = motionSensorObject.GetComponent<MeshRenderer>();
		myInteractionHook.RecvAction += hoverAction;
		myInteractionHook.RecindAction += exitAction;
		myInteractionHook.LeftClickAction += leftClickAction;
	}

	private void Update()
	{
		if (StateManager.PlayerState == PLAYER_STATE.MOTION_SENSOR_PLACEMENT)
		{
			myInteractionHook.ForceLock = false;
		}
		else
		{
			myInteractionHook.ForceLock = true;
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.RecvAction -= hoverAction;
		myInteractionHook.RecindAction -= exitAction;
		myInteractionHook.LeftClickAction -= leftClickAction;
	}

	private void hoverAction()
	{
		motionSensorObject.transform.position = GameManager.ManagerSlinger.MotionSensorManager.CurrentMotionSensorSpawnLocation;
		motionSensorObject.transform.rotation = Quaternion.Euler(GameManager.ManagerSlinger.MotionSensorManager.CurrentMotionSensorSpawRotat);
		motionSensorObjectMeshRenderer.enabled = true;
	}

	private void exitAction()
	{
		motionSensorObjectMeshRenderer.enabled = false;
	}

	private void leftClickAction()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		UIInventoryManager.HideMotionSensor();
		motionSensorObjectMeshRenderer.enabled = false;
		GameManager.ManagerSlinger.MotionSensorManager.ReturnMotionSensor();
	}
}
