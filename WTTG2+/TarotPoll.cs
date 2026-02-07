using System.Collections.Generic;
using UnityEngine;

public class TarotPoll : PollBase
{
	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public TarotPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Tarot Cards Poll - Should the Tarot Cards deck get a refill?");
		TwitchManager.Ins.Hook.SendMessage("!YES - !NO");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
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

	protected override void PollEnd()
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
		TwitchManager.Ins.Hook.SendMessage("The Tarot Cards Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage($"YES: {num} - NO: {num2}");
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
			TwitchManager.Ins.Hook.SendMessage("The Tarot Cards have been refilled!");
			TwitchManager.Ins.TriggerPopUp("The Tarot Cards have been refilled!");
			TarotRefiller.RefillCards();
		}
		else if (num2 > num)
		{
			TwitchManager.Ins.Hook.SendMessage("The Tarot Cards won't be refilled");
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}
}
