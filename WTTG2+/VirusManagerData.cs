using System;

[Serializable]
public class VirusManagerData : DataObject
{
	public bool HasVirus { get; set; }

	public int VirusCount { get; set; }

	public VirusManagerData(int SetID)
		: base(SetID)
	{
	}
}
