using System;

public static class EnvironmentManager
{
	private static PowerBehaviour _powerBehaviour;

	private static POWER_STATE _powerState;

	public static PowerBehaviour PowerBehaviour
	{
		get
		{
			return _powerBehaviour;
		}
		set
		{
			_powerBehaviour = value;
		}
	}

	public static POWER_STATE PowerState
	{
		get
		{
			return _powerState;
		}
		set
		{
			_powerState = value;
		}
	}

	public static void Clear()
	{
		_powerState = POWER_STATE.ON;
		_powerBehaviour = null;
		PowerStateManager.Reset();
		GC.Collect();
	}
}
