using System;

[Serializable]
public class MotionSensorPlacementData
{
	public int LocationName;

	public SerTrans LocationPoisition;

	public MotionSensorPlacementData(PLAYER_LOCATION SetLocation, SerTrans SetPosition)
	{
		LocationName = (int)SetLocation;
		LocationPoisition = SetPosition;
	}
}
