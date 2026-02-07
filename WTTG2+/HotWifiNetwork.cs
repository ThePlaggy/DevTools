using System;

[Serializable]
public struct HotWifiNetwork
{
	public int Hash;

	public float HotTime;

	public float TimeStamp;

	public float TimeLeft;

	public HotWifiNetwork(int SetHash, float SetHotTime, float SetTimeStamp, float SetTimeLeft)
	{
		Hash = SetHash;
		HotTime = SetHotTime;
		TimeStamp = SetTimeStamp;
		TimeLeft = SetTimeLeft;
	}
}
