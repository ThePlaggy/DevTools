using System;

[Serializable]
public class StageManagerData : DataObject
{
	public bool ThreatsActivated { get; set; }

	public float TimeLeft { get; set; }

	public StageManagerData(int SetID)
		: base(SetID)
	{
	}
}
