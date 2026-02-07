using System.Collections.Generic;
using UnityEngine;

public class KeyPoll : PollBase
{
	public static KEY_CUE_MODE keyManipulatorData = KEY_CUE_MODE.DEFAULT;

	public static float BlueBarKeyCue_control = 1f;

	private Dictionary<string, string> currentVotes;

	public DOSTwitch myDOSTwitch;

	private bool voteIsLive;

	public KeyPoll(DOSTwitch myDosTwitch)
	{
		myDOSTwitch = myDosTwitch;
	}

	public static void DevDisableManipulator()
	{
		keyManipulatorData = KEY_CUE_MODE.DEFAULT;
	}

	public static void DevEnableManipulator(KEY_CUE_MODE kEY_CUE_MODE)
	{
		if (keyManipulatorData == KEY_CUE_MODE.DEFAULT)
		{
			keyManipulatorData = kEY_CUE_MODE;
			GameManager.TheCloud.SpawnManipulatorIcon(600f, ManipulatorHook.THE_MANIPULATOR.KEY, (kEY_CUE_MODE == KEY_CUE_MODE.ENABLED) ? ManipulatorHook.MANIPULATOR_TYPE.POSITIVE : ManipulatorHook.MANIPULATOR_TYPE.NEGATIVE);
			GameManager.TimeSlinger.FireTimer(600f, DevDisableManipulator);
		}
	}

	public void BeginVote()
	{
		currentVotes = new Dictionary<string, string>();
		TwitchManager.Ins.Hook.SendMessage("Key Cue Poll - Should the key cue be enabled or disabled for 5 minutes?");
		TwitchManager.Ins.Hook.SendMessage("!ENABLED - !DISABLED");
		voteIsLive = true;
		StartPollTimer();
	}

	public void CastVote(string userName, string theVote)
	{
		if (voteIsLive && theVote.Contains("!"))
		{
			string text = theVote.Replace("!", string.Empty);
			if (!currentVotes.ContainsKey(userName) && (text == "ENABLED" || text == "DISABLED"))
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
		string text = "ENABLED";
		voteIsLive = false;
		foreach (KeyValuePair<string, string> currentVote in currentVotes)
		{
			if (currentVote.Value == text)
			{
				num++;
			}
			else if (currentVote.Value == "DISABLED")
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
		TwitchManager.Ins.Hook.SendMessage("The Key Cue Poll Has Ended!");
		TwitchManager.Ins.Hook.SendMessage(string.Format(text + ": {0} - DISABLED: {1}", num, num2));
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
			TwitchManager.Ins.Hook.SendMessage("The Key Cue will remain enabled for the next 5 minutes!");
			TwitchManager.Ins.TriggerPopUp("The Key Cue will remain enabled for the next 5 minutes!");
			FireManipulator(KEY_CUE_MODE.ENABLED);
		}
		else if (num2 > num)
		{
			TwitchManager.Ins.Hook.SendMessage("The Key Cue will remain disabled for the next 5 minutes!");
			TwitchManager.Ins.TriggerPopUp("The Key Cue will remain disabled for the next 5 minutes!");
			FireManipulator(KEY_CUE_MODE.DISABLED);
		}
		if (!flag)
		{
			myDOSTwitch.setPollInactive();
		}
	}

	private void FireManipulator(KEY_CUE_MODE kEY_CUE_MODE)
	{
		if (keyManipulatorData != KEY_CUE_MODE.DEFAULT)
		{
			GameManager.TimeSlinger.FireTimer(15f, delegate
			{
				FireManipulator(kEY_CUE_MODE);
			});
		}
		else
		{
			keyManipulatorData = kEY_CUE_MODE;
			GameManager.TheCloud.SpawnManipulatorIcon(300f, ManipulatorHook.THE_MANIPULATOR.KEY, (kEY_CUE_MODE == KEY_CUE_MODE.ENABLED) ? ManipulatorHook.MANIPULATOR_TYPE.POSITIVE : ManipulatorHook.MANIPULATOR_TYPE.NEGATIVE);
			GameManager.TimeSlinger.FireTimer(300f, DisableManipulator);
		}
	}

	private void DisableManipulator()
	{
		keyManipulatorData = KEY_CUE_MODE.DEFAULT;
	}

	public static void resetControl()
	{
		BlueBarKeyCue_control = 1f;
	}
}
