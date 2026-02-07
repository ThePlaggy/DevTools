using System;

[Serializable]
public class BreatherData : DataObject
{
	public int KeysDiscoveredCount { get; set; }

	public bool ProductWasPickedUp { get; set; }

	public BreatherData(int SetID)
		: base(SetID)
	{
	}
}
