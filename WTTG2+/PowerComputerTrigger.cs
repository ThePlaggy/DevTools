using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class PowerComputerTrigger : MonoBehaviour
{
	public static PowerComputerTrigger Ins;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		Ins = this;
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += powerOnComputer;
	}

	private void Start()
	{
		EnvironmentManager.PowerBehaviour.PowerOffEvent.Event += powerWentOut;
		EnvironmentManager.PowerBehaviour.PowerOnEvent.Event += powerWentOn;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= powerOnComputer;
	}

	public void Lock()
	{
		myInteractionHook.ForceLock = true;
	}

	public void UnLock()
	{
		myInteractionHook.ForceLock = false;
	}

	private void powerOnComputer()
	{
		ComputerPowerHook.Ins.PowerComputer();
	}

	private void powerWentOut()
	{
		myInteractionHook.ForceLock = true;
	}

	private void powerWentOn()
	{
		if (!ComputerPowerHook.Ins.PowerOn)
		{
			myInteractionHook.ForceLock = false;
		}
	}
}
