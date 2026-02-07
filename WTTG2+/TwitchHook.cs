using System;
using System.Linq;
using UnityEngine;

public class TwitchHook : MonoBehaviour
{
	public CustomEvent<ChatMessage> MessageReceived = new CustomEvent<ChatMessage>(2);

	public CustomEvent ClientConnected = new CustomEvent();

	public CustomEvent ConnectionFailed = new CustomEvent();

	public CustomEvent ConnectionLost = new CustomEvent();

	public string ChatDevUsername = string.Empty;

	private TwitchIRC TwitchIrc = new TwitchIRC();

	private string[] HardCodedDevs;

	public new void SendMessage(string Message)
	{
		TwitchIrc.SendChatMessage(Message);
	}

	private void OnError(Exception ex)
	{
		Debug.Log(ex.Message);
		Debug.Log("[TwitchHook] TwitchIRC Connection " + (TwitchIrc.IAmConnected ? "Lost" : "Failed"));
		TwitchManager.Ins.TriggerPopUp("Twitch Connection " + (TwitchIrc.IAmConnected ? "Lost" : "Failed") + "\nAttempting to reconnect");
		if (TwitchIrc.IAmConnected)
		{
			ConnectionLost.Execute();
		}
		else
		{
			ConnectionFailed.Execute();
		}
	}

	private void OnChatMessageRecv(ChatMessage message)
	{
		Debug.Log("[TwitchHook] Chat Message - " + message.Username + ": " + message.Content);
		if (message.Content.ToLower().StartsWith("!rtv ") && DOSTwitch.instance.pollActive)
		{
			if (!HardCodedDevs.Contains(message.Username.ToLower()) && (!(ChatDevUsername != string.Empty) || !(message.Username == ChatDevUsername)))
			{
				SendMessage("You are not a Chat Developer!");
				return;
			}
			DOSTwitch.rockTheVote = Math.Abs(int.Parse(message.Content.ToLower().Split(' ')[1]));
			SendMessage(message.Username + " rocked the vote Kappa");
			DOSTwitch.instance.currentPollRef?.RTVKillTimer();
		}
		MessageReceived.Execute(message);
	}

	private void OnConnectionSuccess()
	{
		Debug.Log("[TwitchHook] TwitchIRC Has Been Connected Successfully");
		TwitchManager.Ins.TriggerPopUp("Twitch Successfully Connected !");
		ClientConnected.Execute();
	}

	private void Awake()
	{
		Debug.Log("[TwitchHook] Initializing Hook...");
		if (TwitchManager.Ins != null)
		{
			TwitchManager.Ins.Hook = this;
			Debug.Log("[TwitchHook] Twitch Hook Initialized");
		}
		if (DOSTwitch.instance != null)
		{
			DOSTwitch.instance.Init();
		}
	}

	private void Start()
	{
		if (!(TwitchManager.Ins == null))
		{
			TwitchAccount account = TwitchManager.Ins.Account;
			TwitchIrc.StartIRC(account.Login, account.OAuth, account.Login);
			TwitchIRC twitchIrc = TwitchIrc;
			twitchIrc.OnError = (Action<Exception>)Delegate.Combine(twitchIrc.OnError, new Action<Exception>(OnError));
			TwitchIRC twitchIrc2 = TwitchIrc;
			twitchIrc2.OnChatMessageRecv = (Action<ChatMessage>)Delegate.Combine(twitchIrc2.OnChatMessageRecv, new Action<ChatMessage>(OnChatMessageRecv));
			TwitchIRC twitchIrc3 = TwitchIrc;
			twitchIrc3.OnConnectionSuccess = (Action)Delegate.Combine(twitchIrc3.OnConnectionSuccess, new Action(OnConnectionSuccess));
			HardCodedDevs = new string[5] { "otrexdev", "nasko222n", "kotzwurst", "ampercz1", "fiercethundr_" };
		}
	}

	private void LateUpdate()
	{
		if (TwitchIrc.IAmConnected)
		{
			TwitchIrc.Update();
		}
	}

	private void OnDisable()
	{
		TwitchIrc.KillIRC();
		ConnectionLost.Clear();
		ConnectionFailed.Clear();
		ClientConnected.Clear();
		MessageReceived.Clear();
		if (TwitchManager.Ins != null)
		{
			TwitchManager.Ins.Hook = null;
		}
	}
}
