using UnityEngine;

public static class EventRewardManager
{
	public static void ChristmasReward()
	{
		EventManager.Ins.AddEventRewards("Christmas", Money: true, WiFi: false, Discount: true, Powerups: false, RandomKey: false, ItemWhitehat: true);
	}

	public static void EasterReward()
	{
		EventManager.Ins.AddEventRewards("Easter", Money: true, WiFi: false, Discount: true, Powerups: false, RandomKey: false, ItemWhitehat: true, Disco: true, Color.yellow);
	}

	public static void HalloweenReward()
	{
		EventManager.Ins.AddEventRewards("Halloween", Money: false, WiFi: true, Discount: false, Powerups: true, RandomKey: true, ItemWhitehat: false);
	}
}
