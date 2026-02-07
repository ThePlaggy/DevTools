using System.Collections.Generic;

public static class ControllerManager
{
	private static Dictionary<GAME_CONTROLLER, baseController> _controllers = new Dictionary<GAME_CONTROLLER, baseController>(EnumComparer.GameControllerCompare);

	public static void Add(baseController ControllerToAdd)
	{
		if (!_controllers.ContainsKey(ControllerToAdd.Controller))
		{
			_controllers.Add(ControllerToAdd.Controller, ControllerToAdd);
		}
	}

	public static T Get<T>(GAME_CONTROLLER ControllerType) where T : baseController
	{
		if (_controllers.ContainsKey(ControllerType))
		{
			return (T)_controllers[ControllerType];
		}
		return null;
	}

	public static void Remove(GAME_CONTROLLER ControllerToRemove)
	{
		_controllers.Remove(ControllerToRemove);
	}
}
