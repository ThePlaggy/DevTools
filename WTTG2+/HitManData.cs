using System;

[Serializable]
public class HitManData : DataObject
{
	public int KeysDiscoveredCount { get; set; }

	public bool IsActivated { get; set; }

	public float TimeLeftOnWindow { get; set; }

	public HitManData(int SetID)
		: base(SetID)
	{
	}
}
