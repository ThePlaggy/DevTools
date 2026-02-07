using System;

[Serializable]
public class LightTriggerData : DataObject
{
	public bool LightsAreOff { get; set; }

	public LightTriggerData(int SetID)
		: base(SetID)
	{
	}
}
