public static class DongleLocation
{
	public enum Location
	{
		DESK,
		WINDOW,
		BED,
		BALCONY
	}

	public static Location GetCurrentDongleLocation()
	{
		return GameManager.ManagerSlinger.WifiManager.GetCurrentHotspot().gameObject.name switch
		{
			"WifiDongleHotspot1" => Location.WINDOW, 
			"WifiDongleHotspot2" => Location.DESK, 
			"WifiDongleHotspot3" => Location.BED, 
			"WifiDongleHotspot4" => Location.BALCONY, 
			_ => Location.DESK, 
		};
	}
}
