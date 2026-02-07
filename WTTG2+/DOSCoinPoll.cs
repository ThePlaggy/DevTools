using System;
using System.Collections.Generic;
using UnityEngine;

public class DOSCoinPoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private int randomDOS;

	private bool voteIsLive;

	public DOSCoinPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		randomDOS = 0;
		TwitchManager.Ins.Hook.SendMessage("DOS Coin Poll - Give or take DOS coins from the player");
		TwitchManager.Ins.Hook.SendMessage("!GIVE - !TAKE");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "GIVE" || text == "TAKE"))
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
			if (currentVote.Value == "GIVE")
			{
				num++;
				randomDOS += 10;
			}
			else if (currentVote.Value == "TAKE")
			{
				num2++;
				randomDOS -= 10;
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
				randomDOS = 100;
				break;
			case 1:
				num2 = 1337;
				randomDOS = -100;
				break;
			}
		}
		if (randomDOS > 100)
		{
			randomDOS = 100;
		}
		if (randomDOS < -100)
		{
			randomDOS = -100;
		}
		TwitchManager.Ins.Hook.SendMessage("The DOSCoin Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage("GIVE: " + num + " - TAKE: " + num2);
		if (num == 0 && num2 == 0)
		{
			TwitchManager.Ins.Hook.SendMessage("No one voted, Story of my life!");
			myDOSTwitch.setPollInactive();
			return;
		}
		if (num == num2)
		{
			TwitchManager.Ins.Hook.SendMessage("There's a tie! CHOOSING RANDOM!!!");
			num = UnityEngine.Random.Range(0, 1000);
			num2 = UnityEngine.Random.Range(0, 1000);
			randomDOS = (UnityEngine.Random.Range(0, 10) + 1) * 10;
		}
		if (num2 > num)
		{
			randomDOS = Math.Abs(randomDOS);
			TwitchManager.Ins.Hook.SendMessage("Taking " + randomDOS + " DOSCoins from the player!");
			TwitchManager.Ins.TriggerPopUp("Taking " + randomDOS + " DOSCoins from the player!");
			DOSCoinsCurrencyManager.RemoveCurrencyBypassNegative(randomDOS);
			GameManager.HackerManager.BlackHatSound();
		}
		else if (num > num2)
		{
			TwitchManager.Ins.Hook.SendMessage("Giving " + randomDOS + " DOSCoins to the player!");
			TwitchManager.Ins.TriggerPopUp("Giving " + randomDOS + " DOSCoins to the player!");
			DOSCoinsCurrencyManager.AddCurrency(randomDOS);
			GameManager.HackerManager.WhiteHatSound();
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}

	public static void ResetDOSStuff()
	{
	}
}
