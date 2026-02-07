public class skyBreakWPA2Behavior : skyBreakWPABehavior
{
	private void Awake()
	{
		myCracker = WIFI_SECURITY.WPA2;
		crackerName = "WPA2";
	}
}
