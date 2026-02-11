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
        string name = GameManager.ManagerSlinger
            .WifiManager
            .GetCurrentHotspot()
            .gameObject
            .name;

        switch (name)
        {
            case "WifiDongleHotspot1":
                return Location.WINDOW;
            case "WifiDongleHotspot2":
                return Location.DESK;
            case "WifiDongleHotspot3":
                return Location.BED;
            case "WifiDongleHotspot4":
                return Location.BALCONY;
            default:
                return Location.DESK;
        }
    }
}
