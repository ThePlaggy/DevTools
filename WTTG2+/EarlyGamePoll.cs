using System.Collections.Generic;
using UnityEngine;

public class EarlyGamePoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public EarlyGamePoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Escalation Poll - Shall the player get free 100 DOS Coins, or release an Encounter enemy (Lucas/Tanner) or release a Task enemy (Bomb Maker/Doll Maker)");
		TwitchManager.Ins.Hook.SendMessage("!DOSCOINS - !ENCOUNTER - !TASK");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "DOSCOINS" || text == "ENCOUNTER" || text == "TASK"))
			{
				currentVotes.Add(userName, text);
			}
		}
	}

	protected override void PollEnd()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		bool flag2 = false;
		voteIsLive = false;
		foreach (KeyValuePair<string, string> currentVote in currentVotes)
		{
			if (currentVote.Value == "DOSCOINS")
			{
				num++;
			}
			else if (currentVote.Value == "ENCOUNTER")
			{
				num2++;
			}
			else if (currentVote.Value == "TASK")
			{
				num3++;
			}
		}
		if (DOSTwitch.rockTheVote >= 0)
		{
			num = 0;
			num2 = 0;
			num3 = 0;
			DOSTwitch.rockTheVote %= 3;
			switch (DOSTwitch.rockTheVote)
			{
			case 0:
				num = 1337;
				break;
			case 1:
				num2 = 1337;
				break;
			case 2:
				num3 = 1337;
				break;
			}
		}
		TwitchManager.Ins.Hook.SendMessage("The Escalation Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage($"DOSCOINS: {num} - ENCOUNTER: {num2} - TASK: {num3}");
		if (num == 0 && num2 == 0 && num3 == 0)
		{
			TwitchManager.Ins.Hook.SendMessage("No one voted, Story of my life!");
			myDOSTwitch.setPollInactive();
			GameManager.TheCloud.ForceKeyDiscover();
			return;
		}
		if ((num <= num2 || num <= num3) && (num2 <= num || num2 <= num3) && (num3 <= num || num3 <= num2))
		{
			TwitchManager.Ins.Hook.SendMessage("There's a tie! CHOOSING RANDOM!!!");
			num = ((num > 0) ? Random.Range(0, 1000) : 0);
			num2 = ((num2 > 0) ? Random.Range(0, 1000) : 0);
			num3 = ((num3 > 0) ? Random.Range(0, 1000) : 0);
			flag2 = true;
		}
		if (num > num2 && num > num3)
		{
			TwitchManager.Ins.Hook.SendMessage("The player got 100 DOS Coins.");
			TwitchManager.Ins.TriggerPopUp("The player got 100 DOS Coins.");
			DOSCoinsCurrencyManager.AddCurrency(100f);
			GameManager.HackerManager.WhiteHatSound();
			GameManager.TheCloud.ForceKeyDiscover();
		}
		else if (num2 > num && num2 > num3)
		{
			if (!flag2)
			{
				TwitchManager.Ins.Hook.SendMessage("Rolling out random encounter enemy...");
			}
			if (Random.Range(0, 2) == 0)
			{
				TwitchManager.Ins.Hook.SendMessage("Lucas has been set free!");
				TwitchManager.Ins.TriggerPopUp("Lucas has been set free!");
				EnemyManager.HitManManager.ReleaseTheHitman();
				KitchenWindowHook.Ins.OpenWindow();
				GameManager.TheCloud.ForceKeyDiscover();
			}
			else
			{
				TwitchManager.Ins.Hook.SendMessage("Tanner has been set free!");
				TwitchManager.Ins.TriggerPopUp("Tanner has been set free!");
				TannerManager.ForcefullyReleased = true;
				TannerDOSPopper.PopDOS(TannerDOSPopper.TannerMAXDOSPop);
				GameManager.TheCloud.ForceKeyDiscover();
			}
		}
		else if (num3 > num && num3 > num2)
		{
			if (!flag2)
			{
				TwitchManager.Ins.Hook.SendMessage("Rolling out random task enemy...");
			}
			if (Random.Range(0, 2) == 0)
			{
				TwitchManager.Ins.Hook.SendMessage("The Doll Maker is on the way! Shipping the LOLPY disc...");
				TwitchManager.Ins.TriggerPopUp("The Doll Maker is on the way! Shipping the LOLPY disc...");
				EnemyManager.DollMakerManager.ReleaseTheDollMaker();
				WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
				ItemSlinger.LOLPYDisc.myProductObject.shipItem();
			}
			else
			{
				TwitchManager.Ins.Hook.SendMessage("The Bomb Maker has been released! Shipping some sulphur...");
				TwitchManager.Ins.TriggerPopUp("The Bomb Maker has been released! Shipping some sulphur...");
				EnemyManager.BombMakerManager.ReleaseTheBombMaker();
				WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
				ItemSlinger.Sulfur.myProductObject.shipItem();
			}
			GameManager.TheCloud.ForceKeyDiscover();
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}
}
