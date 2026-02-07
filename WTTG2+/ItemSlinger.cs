public static class ItemSlinger
{
	public static ShadowMarketProductDefinition WiFiDongleLevel2 => _getProductById(0);

	public static ShadowMarketProductDefinition WiFiDongleLevel3 => _getProductById(1);

	public static ShadowMarketProductDefinition MotionSensor => _getProductById(2);

	public static ShadowMarketProductDefinition RemoteVPN => _getProductById(3);

	public static ShadowMarketProductDefinition PoliceScanner => _getProductById(4);

	public static ShadowMarketProductDefinition LOLPYDisc => _getProductById(5);

	public static ShadowMarketProductDefinition BlueWhisper => _getProductById(6);

	public static ShadowMarketProductDefinition RemoteVPNLevel2 => _getProductById(7);

	public static ShadowMarketProductDefinition RemoteVPNLevel3 => _getProductById(8);

	public static ShadowMarketProductDefinition Sulfur => _getProductById(9);

	public static ShadowMarketProductDefinition Router => _getProductById(10);

	public static ShadowMarketProductDefinition TarotCards => _getProductById(11);

	public static ShadowMarketProductDefinition SecCams => _getProductById(12);

	public static ShadowMarketProductDefinition Keypad => _getProductById(13);

	public static ShadowMarketProductDefinition StrongFlashlight => _getProductById(14);

	private static ShadowMarketProductDefinition _getProductById(int id)
	{
		return GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[id];
	}
}
