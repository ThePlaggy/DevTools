using System;

[Serializable]
public class ComputerPowerData : DataObject
{
	public bool ComputerIsOff { get; set; }

	public ComputerPowerData(int SetID)
		: base(SetID)
	{
	}
}
