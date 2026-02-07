using System.Collections.Generic;
using UnityEngine;

public class HackerPoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public HackerPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("HACKERMANS Poll - Who will win, black hats or white hats?");
		TwitchManager.Ins.Hook.SendMessage("!WHITEHAT - !BLACKHAT");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "WHITEHAT" || text == "BLACKHAT"))
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
			if (currentVote.Value == "WHITEHAT")
			{
				num++;
			}
			else if (currentVote.Value == "BLACKHAT")
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
		TwitchManager.Ins.Hook.SendMessage("The HACKERMANS Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage($"WHITEHAT: {num} - BLACKHAT: {num2}");
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
			if (InventoryManager.GetBackdoorCount() <= 0)
			{
				TwitchManager.Ins.Hook.SendMessage("Whitehats won! The player does not have a backdoor hack, no DOS Coins earned.");
			}
			else if (Random.Range(0, 100) > ((DifficultyManager.LeetMode || DifficultyManager.Nightmare) ? 90 : 75))
			{
				InventoryManager.RemoveBackdoorHack();
				ItemWhitehats.GiveItemWhitehat(FromTwitch: true);
			}
			else
			{
				InventoryManager.RemoveBackdoorHack();
				float setAMT = ((Random.Range(0, 10) > 7) ? Random.Range(3.5f, 133.7f) : ((Random.Range(0, 100) <= 90) ? Random.Range(3.5f, 33.7f) : 3.5f));
				TwitchManager.Ins.Hook.SendMessage("Whitehats won! The player got " + setAMT + " DOS Coins from that hack.");
				TwitchManager.Ins.TriggerPopUp("Whitehats won! The player got " + setAMT + " DOS Coins from that hack.");
				GameManager.HackerManager.WhiteHatSound();
				DOSCoinsCurrencyManager.AddCurrency(setAMT);
			}
		}
		else if (num2 > num)
		{
			TwitchManager.Ins.Hook.SendMessage("The BLACKHATS Have Won! Launching the highest difficulty hack! HA HA HA HA!!!!");
			TwitchManager.Ins.TriggerPopUp("The BLACKHATS Have Won! Launching the highest difficulty hack! HA HA HA HA!!!!");
			GameManager.HackerManager.ForceTwitchHack();
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}
}
