using System.Collections.Generic;
using UnityEngine;

public class DiscountPoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public DiscountPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Market Discount Poll - Shall the player get 25% discount for every item in zeroDay market? or Shadow market? or no discounts?");
		TwitchManager.Ins.Hook.SendMessage("!ZERODAY - !SHADOW - !NO");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "ZERODAY" || text == "SHADOW" || text == "NO"))
			{
				currentVotes.Add(userName, text);
			}
		}
	}

	protected override void PollEnd()
	{
		if (DifficultyManager.Nightmare)
		{
			PollEndNightmare();
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		voteIsLive = false;
		foreach (KeyValuePair<string, string> currentVote in currentVotes)
		{
			if (currentVote.Value == "ZERODAY")
			{
				num++;
			}
			else if (currentVote.Value == "SHADOW")
			{
				num2++;
			}
			else if (currentVote.Value == "NO")
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
		TwitchManager.Ins.Hook.SendMessage("The Discount Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage("ZERODAY: " + num + " - SHADOW: " + num2 + " - NO: " + num3);
		if (num == 0 && num2 == 0 && num3 == 0)
		{
			TwitchManager.Ins.Hook.SendMessage("No one voted, Story of my life!");
			myDOSTwitch.setPollInactive();
			return;
		}
		if ((num <= num2 || num <= num3) && (num2 <= num || num2 <= num3) && (num3 <= num || num3 <= num2))
		{
			TwitchManager.Ins.Hook.SendMessage("There's a tie! CHOOSING RANDOM!!!");
			num = ((num > 0) ? Random.Range(0, 1000) : 0);
			num2 = ((num2 > 0) ? Random.Range(0, 1000) : 0);
			num3 = ((num3 > 0) ? Random.Range(0, 1000) : 0);
		}
		if (num > num2 && num > num3)
		{
			TwitchManager.Ins.Hook.SendMessage("All items on zeroDay market are now 25% off.");
			TwitchManager.Ins.TriggerPopUp("All items on zeroDay market are now 25% off.");
			WindowManager.Get(SOFTWARE_PRODUCTS.ZERODAY).Launch();
			if (!ZeroDayProductObject.isDiscountOn)
			{
				for (int i = 0; i < GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts.Count; i++)
				{
					GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[i].myProductObject.DiscountMe();
				}
			}
		}
		else if (num2 > num && num2 > num3)
		{
			TwitchManager.Ins.Hook.SendMessage("All items on Shadow market are now 25% off.");
			TwitchManager.Ins.TriggerPopUp("All items on Shadow market are now 25% off.");
			WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
			if (!ShadowProductObject.isDiscountOn)
			{
				for (int j = 0; j < GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts.Count; j++)
				{
					GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[j].myProductObject.DiscountMe();
				}
			}
		}
		else if (num3 > num && num3 > num2)
		{
			TwitchManager.Ins.Hook.SendMessage("No discounts were made!");
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}

	public void BeginVoteNightmare()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Market Discount Poll - Shall the player get 25% discount for every item in both markets?");
		TwitchManager.Ins.Hook.SendMessage("!YES - !NO");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVoteNightmare(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "YES" || text == "NO"))
			{
				currentVotes.Add(userName, text);
			}
		}
	}

	private void PollEndNightmare()
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		voteIsLive = false;
		foreach (KeyValuePair<string, string> currentVote in currentVotes)
		{
			if (currentVote.Value == "YES")
			{
				num++;
			}
			else if (currentVote.Value == "NO")
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
		TwitchManager.Ins.Hook.SendMessage("The Discount Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage("YES: " + num + " - NO: " + num2);
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
			TwitchManager.Ins.Hook.SendMessage("All items are now 25% off.");
			TwitchManager.Ins.TriggerPopUp("All items are now 25% off.");
			WindowManager.Get(SOFTWARE_PRODUCTS.ZERODAY).Launch();
			if (!ZeroDayProductObject.isDiscountOn)
			{
				for (int i = 0; i < GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts.Count; i++)
				{
					GameManager.ManagerSlinger.ProductsManager.ZeroDayProducts[i].myProductObject.DiscountMe();
				}
			}
			WindowManager.Get(SOFTWARE_PRODUCTS.SHADOW_MARKET).Launch();
			if (!ShadowProductObject.isDiscountOn)
			{
				for (int j = 0; j < GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts.Count; j++)
				{
					GameManager.ManagerSlinger.ProductsManager.ShadowMarketProducts[j].myProductObject.DiscountMe();
				}
			}
		}
		else if (num2 > num)
		{
			TwitchManager.Ins.Hook.SendMessage("No discounts were made!");
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}
}
