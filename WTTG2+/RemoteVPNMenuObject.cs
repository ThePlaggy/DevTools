using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemoteVPNMenuObject : MonoBehaviour
{
	private const float OPT_SPACING = 4f;

	private const float MENU_OPT_X = 0f;

	[SerializeField]
	private TextMeshProUGUI vpnLabel;

	[SerializeField]
	private TextMeshProUGUI dosCoinLabel;

	[SerializeField]
	private TextMeshProUGUI timeLabel;

	private int myIndex;

	private RectTransform myRT;

	private Vector2 spawnPOS = new Vector2(0f, 22f);

	private int myID;

	public void ClearMe()
	{
		myRT.anchoredPosition = spawnPOS;
		vpnLabel.SetText(string.Empty);
		dosCoinLabel.SetText(string.Empty);
		timeLabel.SetText(string.Empty);
	}

	public void BuildMe(RemoteVPNObject TheRemoteVPN, int myID)
	{
		string text = "VPN ";
		for (int i = 0; i < TheRemoteVPN.myLevel; i++)
		{
			text += "I";
		}
		vpnLabel.SetText(text);
		dosCoinLabel.SetText(TheRemoteVPN.DOSCoinValue);
		timeLabel.SetText(TheRemoteVPN.TimeValue);
		this.myID = myID;
	}

	public void PutMe(int SetIndex)
	{
		float y = 0f - (4f + ((float)myID * 4f + (float)myID * 22f));
		myRT.anchoredPosition = new Vector2(0f, y);
		switch (myID)
		{
		case 0:
		case 1:
			vpnLabel.color = Color.white;
			dosCoinLabel.color = Color.white;
			timeLabel.color = Color.white;
			vpnLabel.transform.parent.Find("VPNIcon").GetComponent<Image>().color = Color.white;
			dosCoinLabel.transform.parent.Find("DOSCoinIcon").GetComponent<Image>().color = Color.white;
			timeLabel.transform.parent.Find("TimeIcon").GetComponent<Image>().color = Color.white;
			break;
		case 2:
		case 3:
			vpnLabel.color = new Color(1f, 0.8f, 1f, 1f);
			dosCoinLabel.color = new Color(1f, 0.8f, 1f, 1f);
			timeLabel.color = new Color(1f, 0.8f, 1f, 1f);
			vpnLabel.transform.parent.Find("VPNIcon").GetComponent<Image>().color = Color.magenta;
			dosCoinLabel.transform.parent.Find("DOSCoinIcon").GetComponent<Image>().color = new Color(1f, 0.8f, 1f, 1f);
			timeLabel.transform.parent.Find("TimeIcon").GetComponent<Image>().color = new Color(1f, 0.8f, 1f, 1f);
			break;
		case 4:
			vpnLabel.color = new Color(0.7f, 1f, 1f, 1f);
			dosCoinLabel.color = new Color(0.7f, 1f, 1f, 1f);
			timeLabel.color = new Color(0.7f, 1f, 1f, 1f);
			vpnLabel.transform.parent.Find("VPNIcon").GetComponent<Image>().color = Color.cyan;
			dosCoinLabel.transform.parent.Find("DOSCoinIcon").GetComponent<Image>().color = new Color(0.7f, 1f, 1f, 1f);
			timeLabel.transform.parent.Find("TimeIcon").GetComponent<Image>().color = new Color(0.7f, 1f, 1f, 1f);
			break;
		}
	}

	public void SoftBuild(int SetIndex)
	{
		myRT = GetComponent<RectTransform>();
		myRT.anchoredPosition = spawnPOS;
		myIndex = SetIndex;
	}
}
