using System;

[Serializable]
public class FlashLightData : DataObject
{
	public bool OwnsFlashLight { get; set; }

	public FlashLightData(int SetID)
		: base(SetID)
	{
	}
}
