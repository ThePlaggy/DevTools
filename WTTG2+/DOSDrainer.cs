using DG.Tweening;
using UnityEngine;

public class DOSDrainer
{
	private bool consuming;

	public static bool canDosDrain()
	{
		return DOSCoinsCurrencyManager.CurrentCurrency >= 10f && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() != null && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().networkName != "TheProgrammingChair" && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().affectedByDosDrainer && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().networkStrength > 0;
	}

	public static bool dosDrainerInfectedWiFi()
	{
		return GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() != null && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().networkName != "TheProgrammingChair" && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().affectedByDosDrainer && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().networkStrength > 0;
	}

	public void tryConsume()
	{
		if (!consuming)
		{
			GameManager.TimeSlinger.FireTimer(RouterBehaviour.HaveRouterDOSDrainerCheck ? 5f : 1.5f, Consume);
			consuming = true;
		}
	}

	private void Consume()
	{
		if (canDosDrain())
		{
			DOSCoinsCurrencyManager.RemoveCurrency(DifficultyManager.Nightmare ? Random.Range(1f, 5f) : Random.Range(0.75f, 1.25f));
			float timeInterval = CustomSoundLookUp.newDOSDrainer.AudioClip.length / 2f;
			currencyTextHook.TextIns.DOColor(new Color(1f, 0.35f, 0.4f, 1f), timeInterval).OnComplete(delegate
			{
				currencyTextHook.TextIns.DOColor(Color.white, timeInterval);
			});
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.newDOSDrainer);
		}
		consuming = false;
	}
}
