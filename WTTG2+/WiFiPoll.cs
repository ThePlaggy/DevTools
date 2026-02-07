using System.Collections.Generic;
using UnityEngine;

public class WiFiPoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public static WifiNetworkDefinition lastConnectedWifi;

	public static WifiNetworkDefinition targetWifi;

	public WiFiPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public static void ResetWiFiPoll()
	{
		targetWifi = null;
		lastConnectedWifi = null;
		WifiInteractionManager.DOSTwitchIns = "null";
	}

	public void BeginVote()
	{
		targetWifi = lastConnectedWifi;
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("WiFi Poll - Shall the player get a random WiFi unlocked? Or shall the current WiFi get locked?");
		TwitchManager.Ins.Hook.SendMessage("!LOCK - !UNLOCK");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "LOCK" || text == "UNLOCK"))
			{
				currentVotes.Add(userName, text);
			}
		}
	}

	protected override void PollEnd()
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		voteIsLive = false;
		foreach (KeyValuePair<string, string> currentVote in currentVotes)
		{
			if (currentVote.Value == "LOCK")
			{
				num++;
			}
			else if (currentVote.Value == "UNLOCK")
			{
				num2++;
			}
		}
		if (DOSTwitch.rockTheVote >= 0)
		{
			num = 0;
			num2 = 0;
			DOSTwitch.rockTheVote %= 2;
			switch (DOSTwitch.rockTheVote)
			{
			case 0:
				num = 1337;
				break;
			case 1:
				num2 = 1337;
				break;
			}
		}
		TwitchManager.Ins.Hook.SendMessage("The WiFi Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage($"LOCK: {num} - UNLOCK: {num2}");
		if (num == 0 && num2 == 0)
		{
			TwitchManager.Ins.Hook.SendMessage("No one voted, Story of my life!");
			myDOSTwitch.setPollInactive();
			return;
		}
		if (num == num2)
		{
			TwitchManager.Ins.Hook.SendMessage("There's a tie! CHOOSING RANDOM!!!");
			num = Random.Range(0, 1000);
			num2 = Random.Range(0, 1000);
		}
		if (num > num2)
		{
			if (Random.Range(0, 100) < 10 && targetWifi.interaction == WiFiInteractionType.LOCKED)
			{
				TwitchManager.Ins.Hook.SendMessage("Error!!! - D05_DR41N3R Infected this poll!!! Infecting a WiFi instead of locking it.");
				infectTheWiFi();
			}
			else
			{
				lockTheWiFi();
			}
		}
		else if (num2 > num)
		{
			unlockTheWiFi();
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}

	private void unlockTheWiFi()
	{
		WifiInteractionManager.UnlockTheWiFi(-2);
		TwitchManager.Ins.Hook.SendMessage("Unlocking " + WifiInteractionManager.DOSTwitchIns + "...");
		TwitchManager.Ins.TriggerPopUp("Unlocking " + WifiInteractionManager.DOSTwitchIns + "...");
		WifiMenuBehaviour.Ins.refreshNetworks();
	}

	private void lockTheWiFi()
	{
		WifiInteractionManager.LockTheWiFi(-4);
		TwitchManager.Ins.Hook.SendMessage("Locking " + WifiInteractionManager.DOSTwitchIns + "...");
		TwitchManager.Ins.TriggerPopUp("Locking " + WifiInteractionManager.DOSTwitchIns + "...");
		WifiMenuBehaviour.Ins.refreshNetworks();
	}

	private void infectTheWiFi()
	{
		targetWifi.affectedByDosDrainer = true;
		TwitchManager.Ins.Hook.SendMessage("Infecting " + targetWifi.networkName + "...");
		TwitchManager.Ins.TriggerPopUp("Infecting " + targetWifi.networkName + "...");
	}
}
