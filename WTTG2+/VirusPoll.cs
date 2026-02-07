using System.Collections.Generic;
using UnityEngine;

public class VirusPoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public VirusPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Virus Poll - Shall the player get some viruses? Or Shall the player get rid of the viruses?");
		TwitchManager.Ins.Hook.SendMessage("!INSTALL - !CLEAN");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "INSTALL" || text == "CLEAN"))
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
		bool flag2 = false;
		bool flag3 = Random.Range(0, 100) < 5;
		voteIsLive = false;
		foreach (KeyValuePair<string, string> currentVote in currentVotes)
		{
			if (currentVote.Value == "INSTALL")
			{
				num++;
			}
			else if (currentVote.Value == "CLEAN")
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
		TwitchManager.Ins.Hook.SendMessage("The Virus Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage($"INSTALL: {num} - CLEAN: {num2}");
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
			flag2 = true;
		}
		if (num > num2)
		{
			if (num > 10)
			{
				num = 10;
			}
			if (num == 1 || flag2)
			{
				if (flag3 && !GameManager.HackerManager.theSwan.isActivatedBefore)
				{
					GameManager.HackerManager.theSwan.ActivateTheSwan();
					TwitchManager.Ins.Hook.SendMessage("Player's computer got infected by TH3SW4N!");
				}
				else
				{
					GameManager.HackerManager.virusManager.ForceVirus();
					TwitchManager.Ins.Hook.SendMessage("Player's computer got 1 VTrojan installed!");
				}
			}
			else if (flag3 && !GameManager.HackerManager.theSwan.isActivatedBefore)
			{
				GameManager.HackerManager.theSwan.ActivateTheSwan();
				TwitchManager.Ins.Hook.SendMessage("Player's computer got infected by TH3SW4N!");
			}
			else
			{
				for (int i = 0; i < num; i++)
				{
					GameManager.HackerManager.virusManager.ForceVirus();
				}
				TwitchManager.Ins.Hook.SendMessage("Player's computer got " + num + " VTrojans installed!");
			}
		}
		else if (num2 > num)
		{
			string message = "Cleaning the viruses...";
			TwitchManager.Ins.Hook.SendMessage("Cleaning the viruses...");
			if (TheSwanBehaviour.SwanActivatedBefore)
			{
				message = "Unable to clean TH3SW4N!";
				TwitchManager.Ins.Hook.SendMessage("Unable to clean TH3SW4N!");
			}
			if (DOSDrainer.canDosDrain())
			{
				message = "Disconnecting from DOSDrainer infected WiFi...";
				TwitchManager.Ins.Hook.SendMessage("Disconnecting from DOSDrainer infected WiFi...");
				GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
			}
			TwitchManager.Ins.TriggerPopUp(message);
			GameManager.HackerManager.virusManager.ClearVirus();
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}
}
