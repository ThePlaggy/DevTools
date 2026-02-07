using System;
using System.Collections.Generic;

[Serializable]
public class RemoteVPNManagerData : DataObject
{
	public List<VPNCurrencyData> CurrentVPNVolumesCurrencyData { get; set; }

	public Dictionary<int, SerTrans> CurrentlyPlacedRemoteVPNs { get; set; }

	public RemoteVPNManagerData(int SetID)
		: base(SetID)
	{
	}
}
