using System;
using System.Collections.Generic;

[Serializable]
public class TenantTrackData : DataObject
{
	public Dictionary<int, List<TenantData>> TenantData { get; set; }

	public TenantTrackData(int SetID)
		: base(SetID)
	{
	}
}
