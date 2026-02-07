using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class TwitchIRC
{
	private static class Console
	{
		public static void WriteLine<T>(T message)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public string Username;

	public string Channel;

	public bool IAmConnected;

	public bool Killed;

	public int ReconnectTimeout = 30;

	public int MaxRecconectTries = 10;

	public int RecconectTries;

	public Action<ChatMessage> OnChatMessageRecv;

	public Action OnConnectionSuccess;

	public Action<Exception> OnError;

	private List<ChatMessage> recievedMessages;

	private Queue<string> commandQueue;

	private StreamWriter outputStream;

	private StreamReader inputStream;

	private Thread pingProc;

	private Thread outProc;

	private Thread inProc;

	private string oauth;

	private int errorCount;

	private bool inputDisposed;

	private long latestPongTimestamp;

	private long pingRequestTimestamp;

	public TwitchIRC()
	{
		recievedMessages = new List<ChatMessage>();
		commandQueue = new Queue<string>();
		pingProc = new Thread(IRCPing)
		{
			IsBackground = true
		};
		outProc = new Thread(IRCOutputProcedure);
		inProc = new Thread(IRCInputProcedure);
	}

	public void StartIRC(string username, string oauth, string channelName)
	{
		if (!Killed && !IAmConnected)
		{
			if (!oauth.StartsWith("oauth:"))
			{
				oauth = "oauth:" + oauth;
			}
			OnError = (Action<Exception>)Delegate.Combine(OnError, new Action<Exception>(TryToReconnect));
			Username = username.ToLower();
			Channel = channelName.ToLower();
			this.oauth = oauth;
			outProc.Start();
			inProc.Start();
			WarmUp();
		}
	}

	public void KillIRC()
	{
		Killed = true;
		outputStream.Dispose();
		inputStream.Dispose();
		pingProc.Abort();
		outProc.Abort();
		inProc.Abort();
	}

	public void SendChatMessage(string message)
	{
		Console.WriteLine("Send Message: " + message);
		try
		{
			SendIrcBuffer("PRIVMSG #" + Channel + " :" + message);
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex);
			OnError?.Invoke(ex);
		}
	}

	public void Update()
	{
		if (Killed || !IAmConnected || latestPongTimestamp == 0)
		{
			return;
		}
		if (120 - (TimeSlinger.CurrentTimestamp - latestPongTimestamp) < 0)
		{
			OnError?.Invoke(new Exception("Server didn't respond to Ping in time"));
		}
		lock (recievedMessages)
		{
			if (recievedMessages.Count <= 0)
			{
				return;
			}
			foreach (ChatMessage recievedMessage in recievedMessages)
			{
				OnChatMessageRecv?.Invoke(recievedMessage);
			}
			recievedMessages.Clear();
		}
	}

	private void WarmUp()
	{
		if (Killed)
		{
			Console.WriteLine("[TwitchIRC] WarmUp on Kill ?");
			return;
		}
		if (RecconectTries >= MaxRecconectTries)
		{
			Console.WriteLine("[TwitchIRC] WarmUp Failed - Max Reconnect Limit Reached");
			return;
		}
		RecconectTries++;
		Console.WriteLine($"[TwitchIRC] WarmUp, {RecconectTries}");
		try
		{
			TcpClient tcpClient = new TcpClient("irc.twitch.tv", 6667);
			inputStream = new StreamReader(tcpClient.GetStream());
			outputStream = new StreamWriter(tcpClient.GetStream());
			inputDisposed = false;
			outputStream.WriteLine("PASS " + oauth);
			outputStream.WriteLine("NICK " + Username);
			outputStream.WriteLine("JOIN #" + Channel);
			outputStream.Flush();
		}
		catch (Exception obj)
		{
			OnError?.Invoke(obj);
		}
	}

	private void TryToReconnect(Exception ex)
	{
		Console.WriteLine($"[TwitchIRC] Attempting to reconnect in {ReconnectTimeout}s");
		ClearOut();
		TimeSlinger.FireTimer(WarmUp, ReconnectTimeout);
	}

	private void SendIrcBuffer(string command)
	{
		Queue<string> queue = commandQueue;
		if (queue == null)
		{
			return;
		}
		lock (queue)
		{
			queue.Enqueue(command);
		}
	}

	private string ReadIrcBuffer()
	{
		if (inputDisposed)
		{
			return null;
		}
		if (inputStream == null)
		{
			return null;
		}
		try
		{
			return inputStream.ReadLine();
		}
		catch (Exception ex)
		{
			Console.WriteLine("[TwitchIRC] Read Buffer failed: " + ex.Message);
			errorCount++;
			if (errorCount > 10)
			{
				Console.WriteLine($"[TwitchIRC] Read Buffer error: {ex}");
				OnError?.Invoke(ex);
			}
			return null;
		}
	}

	private void IRCInputProcedure()
	{
		while (!Killed)
		{
			string text = ReadIrcBuffer();
			if (text == null)
			{
				continue;
			}
			Console.WriteLine("[TwitchIRC] buffer - " + text);
			if (text.Contains("PONG"))
			{
				latestPongTimestamp = TimeSlinger.CurrentTimestamp;
				Console.WriteLine($"[TwitchIrc] [{DateTime.UtcNow}]<--- IRCPong, {TimeSlinger.CurrentTimestamp - pingRequestTimestamp}ms");
			}
			if (text.Contains("Login authentication failed") || text.Contains("Login unsuccessful"))
			{
				OnError?.Invoke(new Exception("[TwitchIRC] Login authentication failed"));
				KillIRC();
				break;
			}
			if (text.Contains("PRIVMSG"))
			{
				lock (recievedMessages)
				{
					recievedMessages.Add(new ChatMessage
					{
						Username = text.Substring(1, text.IndexOf('!') - 1),
						Content = text.Substring(text.IndexOf(" :", StringComparison.Ordinal) + 2),
						Buffer = text
					});
				}
			}
			if (text.Split(' ')[1] == "353" && text.Contains("= #" + Channel) && !IAmConnected)
			{
				OnConnectionSuccess?.Invoke();
				IAmConnected = true;
				pingProc.Start();
			}
		}
	}

	private void IRCOutputProcedure()
	{
		try
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			while (!Killed)
			{
				Queue<string> queue = commandQueue;
				if (queue == null)
				{
					continue;
				}
				lock (queue)
				{
					if (queue.Count > 0 && stopwatch.ElapsedMilliseconds > 1750)
					{
						outputStream.WriteLine(queue.Peek());
						outputStream.Flush();
						queue.Dequeue();
						stopwatch.Reset();
						stopwatch.Start();
					}
				}
			}
		}
		catch (Exception obj)
		{
			Console.WriteLine("[TwitchIRC] IRCOutputProcedure error");
			OnError?.Invoke(obj);
		}
	}

	private void IRCPing()
	{
		while (IAmConnected)
		{
			SendIrcBuffer("PING irc.twitch.tv");
			Console.WriteLine($"[TwitchIrc] [{DateTime.UtcNow}]---> IRCPing");
			pingRequestTimestamp = TimeSlinger.CurrentTimestamp;
			Thread.Sleep(60000);
		}
	}

	private void ClearOut()
	{
		IAmConnected = false;
		inputDisposed = true;
		recievedMessages.Clear();
		commandQueue.Clear();
		inputStream?.Dispose();
		outputStream?.Dispose();
		errorCount = 0;
		latestPongTimestamp = 0L;
		pingRequestTimestamp = 0L;
	}
}
