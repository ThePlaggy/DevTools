using System.Collections.Generic;
using UnityEngine;

public class SpeedPoll : PollBase
{
	public static bool speedManipulatorActive;

	public static TWITCH_NET_SPEED speedManipulatorData;

	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public SpeedPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public static void DevDisableManipulator()
	{
		speedManipulatorActive = false;
	}

	public static void DevEnableManipulator(TWITCH_NET_SPEED nET_SPEED)
	{
		if (!speedManipulatorActive)
		{
			speedManipulatorData = nET_SPEED;
			speedManipulatorActive = true;
			GameManager.TheCloud.SpawnManipulatorIcon(600f, ManipulatorHook.THE_MANIPULATOR.SPEED, (nET_SPEED == TWITCH_NET_SPEED.FAST) ? ManipulatorHook.MANIPULATOR_TYPE.POSITIVE : ManipulatorHook.MANIPULATOR_TYPE.NEGATIVE);
			GameManager.TimeSlinger.FireTimer(600f, DevDisableManipulator);
		}
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Page Loading Speed Poll - Should the websites load faster, or slower for 5 minutes?");
		TwitchManager.Ins.Hook.SendMessage("!FASTER - !SLOWER");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "FASTER" || text == "SLOWER"))
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
			if (currentVote.Value == "FASTER")
			{
				num++;
			}
			else if (currentVote.Value == "SLOWER")
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
		TwitchManager.Ins.Hook.SendMessage("The Page Loading Speed Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage($"FASTER: {num} - SLOWER: {num2}");
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
			TwitchManager.Ins.Hook.SendMessage("Sites will load faster for the next 5 minutes!");
			TwitchManager.Ins.TriggerPopUp("Sites will load faster for the next 5 minutes!");
			FireManipulator(TWITCH_NET_SPEED.FAST);
		}
		else if (num2 > num)
		{
			TwitchManager.Ins.Hook.SendMessage("Sites will load slower for the next 5 minutes!");
			TwitchManager.Ins.TriggerPopUp("Sites will load slower for the next 5 minutes!");
			FireManipulator(TWITCH_NET_SPEED.SLOW);
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}

	private void FireManipulator(TWITCH_NET_SPEED nET_SPEED)
	{
		if (speedManipulatorActive)
		{
			GameManager.TimeSlinger.FireTimer(15f, delegate
			{
				FireManipulator(nET_SPEED);
			});
			return;
		}
		speedManipulatorData = nET_SPEED;
		speedManipulatorActive = true;
		GameManager.TheCloud.SpawnManipulatorIcon(300f, ManipulatorHook.THE_MANIPULATOR.SPEED, (nET_SPEED == TWITCH_NET_SPEED.FAST) ? ManipulatorHook.MANIPULATOR_TYPE.POSITIVE : ManipulatorHook.MANIPULATOR_TYPE.NEGATIVE);
		GameManager.TimeSlinger.FireTimer(300f, DisableManipulator);
	}

	private void DisableManipulator()
	{
		speedManipulatorActive = false;
	}

	public static void MarketEnableManipulator(TWITCH_NET_SPEED nET_SPEED)
	{
		if (speedManipulatorActive)
		{
			GameManager.TimeSlinger.FireTimer(15f, MarketEnableManipulator, nET_SPEED);
			return;
		}
		speedManipulatorData = nET_SPEED;
		speedManipulatorActive = true;
		GameManager.TheCloud.SpawnManipulatorIcon(600f, ManipulatorHook.THE_MANIPULATOR.SPEED, (nET_SPEED == TWITCH_NET_SPEED.FAST) ? ManipulatorHook.MANIPULATOR_TYPE.POSITIVE : ManipulatorHook.MANIPULATOR_TYPE.NEGATIVE);
		GameManager.TimeSlinger.FireTimer(600f, delegate
		{
			speedManipulatorActive = false;
		});
	}
}
