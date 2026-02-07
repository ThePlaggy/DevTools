using System;

[Serializable]
public class VPNMenuData : DataObject
{
	public int CurrentActiveVPN { get; set; }

	public VPNMenuData(int SetID)
		: base(SetID)
	{
	}
}
