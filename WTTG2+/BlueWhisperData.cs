using System;

[Serializable]
public class BlueWhisperData : DataObject
{
	public bool Pending { get; set; }

	public bool Owns { get; set; }

	public BlueWhisperData(int SetID)
		: base(SetID)
	{
	}
}
