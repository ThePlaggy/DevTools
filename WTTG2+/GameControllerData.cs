using System;

[Serializable]
public class GameControllerData
{
	public GAME_CONTROLLER Controller;

	public GAME_CONTROLLER_STATE ControllerState;

	public bool Active;

	public Vect3 LastPosition;

	public Vect3 LastRotation;

	public GameControllerData(GAME_CONTROLLER SetController, GAME_CONTROLLER_STATE SetControllerState, bool SetActive, Vect3 SetLastPosition, Vect3 SetLastRotation)
	{
		Controller = SetController;
		ControllerState = SetControllerState;
		Active = SetActive;
		LastPosition = SetLastPosition;
		LastRotation = SetLastRotation;
	}
}
