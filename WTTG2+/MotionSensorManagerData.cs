using System;
using System.Collections.Generic;

[Serializable]
public class MotionSensorManagerData : DataObject
{
	public Dictionary<int, MotionSensorPlacementData> CurrentlyPlacedMotionSensors = new Dictionary<int, MotionSensorPlacementData>(4);

	public MotionSensorManagerData(int SetID)
		: base(SetID)
	{
	}
}
