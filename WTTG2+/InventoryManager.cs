public static class InventoryManager
{
	public static CustomEvent<SOFTWARE_PRODUCTS> AddedSoftwareProduct = new CustomEvent<SOFTWARE_PRODUCTS>(10);

	public static bool OwnsFlashlight { get; set; }

	public static bool OwnsMotionSensorAudioCue { get; set; }

	public static bool OwnsKeyCue { get; set; }

	public static int WifiDongleLevel { get; set; }

	private static int BackdoorAmount { get; set; }

	public static void Clear()
	{
		OwnsFlashlight = false;
		OwnsMotionSensorAudioCue = false;
		OwnsKeyCue = false;
		WifiDongleLevel = 0;
		BackdoorAmount = 0;
		AddedSoftwareProduct.Clear();
		AddedSoftwareProduct = new CustomEvent<SOFTWARE_PRODUCTS>(10);
	}

	public static void AddProduct(ZeroDayProductDefinition TheProduct)
	{
		AddedSoftwareProduct.Execute(TheProduct.productID);
	}

	public static int AddBackdoor(int amount = 1)
	{
		return BackdoorAmount += amount;
	}

	public static int GetBackdoorCount()
	{
		return BackdoorAmount;
	}

	public static void RemoveBackdoorHack(int amount = 1)
	{
		BackdoorAmount -= amount;
		if (BackdoorAmount < 0)
		{
			BackdoorAmount = 0;
		}
		backdoorTextHook.Ins.BackdoorsWereRemoved();
	}

	public static void DrainBackdoors()
	{
		RemoveBackdoorHack(BackdoorAmount);
	}
}
