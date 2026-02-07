public static class SulphurInventory
{
	public static int SulphurAmount;

	public static void AddSulphur(int amount)
	{
		SulphurAmount += amount;
		SulphurPackageObject.Ins.UpdateSulphurPackages();
	}

	public static void RemoveSulphur(int amount)
	{
		SulphurAmount -= amount;
		SulphurPackageObject.Ins.UpdateSulphurPackages();
	}
}
