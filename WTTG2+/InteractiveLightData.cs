using System;

[Serializable]
public class InteractiveLightData : DataObject
{
	public bool LightIsOff { get; set; }

	public InteractiveLightData(int SetID)
		: base(SetID)
	{
	}
}
