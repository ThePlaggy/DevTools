using System;

[Serializable]
public class FlashLightBehData : DataObject
{
	public float BatteryLifeUsage { get; set; }

	public FlashLightBehData(int SetID)
		: base(SetID)
	{
	}
}
