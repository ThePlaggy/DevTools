using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TwitchTabLookup : MonoBehaviour
{
	public GameObject notSignedInBTNS;

	public GameObject signedInBTNS;

	public TMP_Text usernameText;

	public TMP_Text methodText;

	public TMP_Text expiresOnText;

	public GameObject holdOnMenu;

	public GameObject toggleIntegrationBTN;

	public GameObject manualLoginBTNS;

	public TMP_InputField usernameIF;

	public TMP_InputField oauthIF;

	private TwitchAccount account;

	private bool integrationOn;

	private string manualOauth;

	private string manualUsername;

	private void OnEnable()
	{
		GameObject.Find("NewMenuCanvas(Clone)/Sidebar UI/Sidebar/TwitchTab/NotSignedIn/Image").SetActive(value: false);
		Debug.Log("[Sidebar] Loading twitch data...");
		if (!TwitchManager.Ins.LoggedIn)
		{
			Debug.Log("[Sidebar] Twitch Data: Not Logged In");
			expiresOnText.text = "Unknown";
			methodText.text = "Unknown";
			usernameText.text = "Unknown";
			return;
		}
		account = TwitchManager.Ins.Account;
		Debug.Log("[Sidebar] Twitch Data: Logged In");
		Debug.Log("[Sidebar] Twitch Data: Account Name: " + account.Username);
		Debug.Log("[Sidebar] Twitch Data: Expires on: " + account.ExpiresIn);
		Debug.Log("[Sidebar] Twitch Data: Manual Login: " + account.Manual);
		usernameText.text = account.Username;
		if (account.ExpiresIn != 0)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(account.ExpiresIn);
			expiresOnText.text = dateTime.ToString(new CultureInfo(CultureInfo.InstalledUICulture.Name).DateTimeFormat.ShortDatePattern);
		}
		else
		{
			expiresOnText.text = "Unknown";
		}
		notSignedInBTNS.SetActive(value: false);
		signedInBTNS.SetActive(value: true);
		methodText.text = (account.Manual ? "manual" : "automatic");
		if (PlayerPrefs.GetInt("[MOD]TTVInt", 1) == 0)
		{
			integrationOn = false;
			toggleIntegrationBTN.GetComponent<Image>().color = Color.red;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().color = Color.red;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().text = "Twitch integration disabled";
			PlayerPrefs.SetInt("[MOD]TTVInt", 0);
		}
		else
		{
			integrationOn = true;
			toggleIntegrationBTN.GetComponent<Image>().color = Color.green;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().color = Color.green;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().text = "Twitch integration enabled";
			PlayerPrefs.SetInt("[MOD]TTVInt", 1);
		}
	}

	public void AutoLogin()
	{
		notSignedInBTNS.SetActive(value: false);
		holdOnMenu.SetActive(value: true);
		TwitchManager.Ins.CreateLoginTicket();
		NewMenuManager.Ins.newGameBTN.ActiveState(Active: false);
	}

	public void ManualLogin()
	{
		notSignedInBTNS.SetActive(value: false);
		manualLoginBTNS.SetActive(value: true);
		NewMenuManager.Ins.newGameBTN.ActiveState(Active: false);
	}

	public void ToggleIntegration()
	{
		if (integrationOn)
		{
			integrationOn = false;
			toggleIntegrationBTN.GetComponent<Image>().color = Color.red;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().color = Color.red;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().text = "Twitch integration disabled";
			PlayerPrefs.SetInt("[MOD]TTVInt", 0);
		}
		else
		{
			integrationOn = true;
			toggleIntegrationBTN.GetComponent<Image>().color = Color.green;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().color = Color.green;
			toggleIntegrationBTN.GetComponentInChildren<TMP_Text>().text = "Twitch integration enabled";
			PlayerPrefs.SetInt("[MOD]TTVInt", 1);
		}
	}

	public void LogOut()
	{
		TwitchManager.Ins.Logout();
		signedInBTNS.SetActive(value: false);
		notSignedInBTNS.SetActive(value: true);
	}

	public void UpdatePanelInfo(bool manual = false)
	{
		NewMenuManager.Ins.newGameBTN.ActiveState(Active: true);
		TwitchAccount twitchAccount = TwitchManager.Ins.Account;
		if (TwitchManager.Ins.LoggedIn)
		{
			manualUsername = twitchAccount.Username;
			manualOauth = twitchAccount.OAuth;
			signedInBTNS.SetActive(value: true);
			notSignedInBTNS.SetActive(value: false);
			holdOnMenu.SetActive(value: false);
			if (manual)
			{
				methodText.text = "manual";
			}
			else
			{
				methodText.text = "automatic";
			}
		}
		else
		{
			manualUsername = "Input Username";
			manualOauth = "...";
		}
		if (twitchAccount.ExpiresIn != 0)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(twitchAccount.ExpiresIn);
			expiresOnText.text = dateTime.ToString(new CultureInfo(CultureInfo.InstalledUICulture.Name).DateTimeFormat.ShortDatePattern);
		}
		else
		{
			expiresOnText.text = "Unknown";
		}
		usernameText.text = twitchAccount.Username;
		if (!TwitchManager.Ins.LoggedIn)
		{
			methodText.text = "unknown";
		}
	}

	public void ManualLoginConfirm()
	{
		manualUsername = usernameIF.text;
		manualOauth = (oauthIF.text.StartsWith("oauth:") ? oauthIF.text : ("oauth:" + oauthIF.text));
		TwitchManager.Ins.Account = new TwitchAccount
		{
			Username = manualUsername,
			OAuth = manualOauth,
			DataExists = true,
			ExpiresIn = 0L,
			Id = "Unknown",
			Login = manualUsername.ToLower(),
			Manual = true
		};
		TwitchManager.Ins.Account.Save();
		TwitchManager.Ins.LoggedIn = true;
		TwitchManager.Ins.LoginSuccess.Execute(TwitchManager.Ins.Account);
		UpdatePanelInfo(manual: true);
		manualLoginBTNS.SetActive(value: false);
	}

	public void ManualLoginReturn()
	{
		notSignedInBTNS.SetActive(value: true);
		manualLoginBTNS.SetActive(value: false);
		NewMenuManager.Ins.newGameBTN.ActiveState(Active: true);
	}

	public void AutoLoginFailed()
	{
	}

	private DateTime UnixTimeStampToDateTime(long unixTimestamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(unixTimestamp * 10000000);
	}
}
