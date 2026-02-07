using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class PowerSwitchTrigger : MonoBehaviour
{
	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += switchPowerOn;
	}

	private void Start()
	{
		myInteractionHook.ForceLock = true;
		EnvironmentManager.PowerBehaviour.PowerOnEvent.Event += powerWentOn;
		EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += powerWentOff;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= switchPowerOn;
	}

	private void switchPowerOn()
	{
		EnvironmentManager.PowerBehaviour.SwitchPowerOn();
	}

	private void powerWentOn()
	{
		myInteractionHook.ForceLock = true;
	}

	private void powerWentOff()
	{
		myInteractionHook.ForceLock = false;
	}
}
