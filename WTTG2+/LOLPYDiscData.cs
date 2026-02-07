using System;

[Serializable]
public class LOLPYDiscData : DataObject
{
	public bool WasInserted { get; set; }

	public LOLPYDiscData(int SetID)
		: base(SetID)
	{
	}
}
