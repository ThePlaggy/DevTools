using System;

[Serializable]
public class WifiManagerData : DataObject
{
	public int ActiveWifiHotSpotIndex { get; set; }

	public int CurrentWifiNetworkIndex { get; set; }

	public bool IsConnected { get; set; }

	public int OwnedWifiDongleLevel { get; set; }

	public WifiManagerData(int SetID)
		: base(SetID)
	{
	}
}
