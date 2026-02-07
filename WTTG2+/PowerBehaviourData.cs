using System;

[Serializable]
public class PowerBehaviourData : DataObject
{
	public bool LightsAreOff { get; set; }

	public PowerBehaviourData(int SetID)
		: base(SetID)
	{
	}
}
