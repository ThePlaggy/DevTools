using System;

[Serializable]
public struct VPNCurrencyData
{
	public float GenerateTime;

	public float GenerateDOSCoinValue;

	public VPNCurrencyData(float SetTime, float SetValue)
	{
		GenerateTime = SetTime;
		GenerateDOSCoinValue = SetValue;
	}
}
