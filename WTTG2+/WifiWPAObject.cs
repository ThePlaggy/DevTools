public class WifiWPAObject
{
	public int CurrentInjectionAmount;

	private bool injectionReady;

	public WifiNetworkDefinition myWifiNetwork;

	public int TotalInjectionAmountAdded;

	public WifiWPAObject(WifiNetworkDefinition setWifiNetwork, int setTotalInjectionAmoutAdded, int currentInjectionAmout)
	{
		myWifiNetwork = setWifiNetwork;
		TotalInjectionAmountAdded = setTotalInjectionAmoutAdded;
		CurrentInjectionAmount = 0;
		injectionReady = false;
	}

	public void SetInjectionReady(bool setValue)
	{
		injectionReady = setValue;
	}

	public bool GetInjectionReady()
	{
		return injectionReady;
	}
}
