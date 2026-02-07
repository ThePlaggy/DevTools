using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TwitchManager : MonoBehaviour
{
	public static TwitchManager Ins;

	public CustomEvent<TwitchAccount> LoginSuccess = new CustomEvent<TwitchAccount>();

	public CustomEvent<string> LoginFailed = new CustomEvent<string>();

	public TwitchAccount Account;

	public TwitchHook Hook;

	public bool LoggedIn;

	private TextMeshProUGUI popUpText;

	private GameObject popUpObject;

	private int loginTries;

	private bool popUpBusy;

	private bool isBusy;

	public void InitDOSTwitch()
	{
		if (ModSettings.DOSTwitchActive && LoggedIn)
		{
			if (!DOSTwitch.InitializedIRC)
			{
				DOSTwitch.InitializedIRC = true;
				new GameObject("TwitchController").AddComponent<TwitchHook>();
			}
			else
			{
				Debug.Log("[WARNING] Tried to initialize TwitchHook and TwitchIRC again, Memory overflow or bad connection?");
				Debug.Log("# Did the bootstrapper disconnected the previous scene properly?");
			}
		}
	}

	public void InitTitleSettings()
	{
		LoadData();
	}

	public void InitTTVPopUp()
	{
		popUpObject = UnityEngine.Object.Instantiate(CustomObjectLookUp.TwitchPopUp, GameObject.Find("UI/UICanvas").transform);
		popUpText = GameObject.Find("NotifyText").GetComponent<TextMeshProUGUI>();
		popUpText.font = LookUps.TitleFont;
	}

	public void TriggerPopUp(string Message)
	{
		if (popUpObject == null || popUpText == null)
		{
			return;
		}
		if (!popUpBusy)
		{
			popUpBusy = true;
			popUpObject.transform.DOMoveX(150f, 0.75f).OnComplete(delegate
			{
				popUpObject.transform.DOMoveX(-135f, 0.5f).SetDelay(5f);
				popUpBusy = false;
			});
		}
		popUpText.text = Message;
	}

	public void LoadData()
	{
		Account = new TwitchAccount().Load();
		if (Account != null && Account.DataExists)
		{
			LoggedIn = true;
			Debug.Log("[TTVManager] Welcome Back " + Account.Username + " (" + Account.Id + ")");
		}
	}

	public void Logout()
	{
		Account.Clear();
		LoggedIn = false;
		NewMenuManager.Ins.twitchTabLookup.UpdatePanelInfo();
	}

	public void CreateLoginTicket()
	{
		if (LoggedIn)
		{
			throw new Exception("[TTVManager] CreateTicket() - Already Logged In");
		}
		if (isBusy)
		{
			return;
		}
		isBusy = true;
		new HTTPHelper<ResponseModel>("/TTVGameAuth/ticket").Send(delegate(ResponseModel Response)
		{
			if (!Response.Success)
			{
				LoginFailed.Execute(Response.Message);
				throw new Exception("[TTVManager] TTVAuth Login Was Not Success: " + Response.Message);
			}
			Debug.Log("[TTVManager] TTVAuth Ticket Created: " + Response.Message);
			Application.OpenURL(Response.Data);
			TimeSlinger.FireInterval(delegate
			{
				if (loginTries == 10 || LoggedIn)
				{
					loginTries = 0;
					isBusy = false;
					TimeSlinger.KillInterval("TTV_TICKET_CHECK");
					Debug.Log("[TTVManager] TTVAuth Ticket Failed: " + Response.Message);
					LoginFailed.Execute("Exceeded Check Limit");
					NewMenuManager.Ins.twitchTabLookup.AutoLoginFailed();
				}
				else
				{
					loginTries++;
					Debug.Log($"[TTVManager] Performing TTVAccount_TicketRequest, Tries: {loginTries}");
					UnityMainThreadDispatcher.Instance().Enqueue(delegate
					{
						TicketRequest(Response.Message);
					});
				}
			}, 5f, "TTV_TICKET_CHECK");
		}, delegate(Exception Exception)
		{
			TimeSlinger.KillInterval("TTV_TICKET_CHECK");
			Debug.Log("[TTVManager] TTVAuth Ticket Exception Raised: " + Exception.Message);
			LoginFailed.Execute("Ticket Exception Raised");
			NewMenuManager.Ins.twitchTabLookup.AutoLoginFailed();
			isBusy = false;
		});
	}

	private void TicketRequest(string ticketId)
	{
		new HTTPHelper<TTVAccountModel>("/TTVGameAuth/ticketLogin?ticket=" + ticketId).Send(delegate(TTVAccountModel TTVAccountModel)
		{
			if (TTVAccountModel.Success)
			{
				if (TTVAccountModel.Data == null)
				{
					LoginFailed.Execute("OAuth Token is missing");
					throw new Exception("OAuth Token is missing");
				}
				TimeSlinger.KillInterval("TTV_TICKET_CHECK");
				Account = new TwitchAccount().Build(TTVAccountModel);
				LoggedIn = true;
				LoginSuccess.Execute(Account);
				NewMenuManager.Ins.twitchTabLookup.UpdatePanelInfo();
				loginTries = 0;
				isBusy = false;
				Debug.Log("[TTVManager] Ticket Login Response: " + TTVAccountModel.Message);
				Debug.Log("[TTVManager] Welcome Back " + Account.Username + " (" + Account.Id + ")");
			}
		}, delegate(Exception Exception)
		{
			TimeSlinger.KillInterval("TTV_TICKET_CHECK");
			Debug.Log("[TTVManager] TTVAuth TicketLogin Exception Raised: " + Exception.Message);
			LoginFailed.Execute("TicketLogin Exception Raised");
			NewMenuManager.Ins.twitchTabLookup.AutoLoginFailed();
			loginTries = 0;
			isBusy = false;
		});
	}

	public void TTVPollInProgress(string theText)
	{
		Ins.TriggerPopUp(theText + " in progress...");
	}

	private void Awake()
	{
		Ins = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnDestroy()
	{
		Ins = null;
	}
}
