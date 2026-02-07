using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyMenuBehaviour : MonoBehaviour
{
	private const float OPT_SPACING = 4f;

	private const float BOT_SPACING = 4f;

	[SerializeField]
	private int REMOTE_VPN_MENU_POOL_COUNT = 6;

	[SerializeField]
	private CanvasGroup noActiveRemoteVPNSCG;

	[SerializeField]
	private GameObject remoteVPNMenuObject;

	private Dictionary<RemoteVPNObject, RemoteVPNMenuObject> currentActiveRemoteVPNS = new Dictionary<RemoteVPNObject, RemoteVPNMenuObject>(6);

	private RectTransform myRT;

	private PooledStack<RemoteVPNMenuObject> remoteVPNMenuObjectPool;

	private CanvasGroup[] vpnMenuInactive;

	private void Awake()
	{
		myRT = GetComponent<RectTransform>();
		int index = REMOTE_VPN_MENU_POOL_COUNT + 1;
		remoteVPNMenuObjectPool = new PooledStack<RemoteVPNMenuObject>(delegate
		{
			index--;
			RemoteVPNMenuObject component = Object.Instantiate(remoteVPNMenuObject, myRT).GetComponent<RemoteVPNMenuObject>();
			component.SoftBuild(index);
			return component;
		}, REMOTE_VPN_MENU_POOL_COUNT);
		GameManager.StageManager.Stage += stageMe;
		List<CanvasGroup> list = new List<CanvasGroup>();
		for (int num = 0; num < 5; num++)
		{
			CanvasGroup canvasGroup = Object.Instantiate(noActiveRemoteVPNSCG, noActiveRemoteVPNSCG.transform.parent, worldPositionStays: true);
			canvasGroup.alpha = 1f;
			canvasGroup.transform.Find("noneActive").GetComponent<Text>().text = $"[VPN {num + 1} NOT ACTIVE]";
			canvasGroup.transform.Find("noneActive").GetComponent<Text>().fontSize = 18;
			canvasGroup.transform.Find("noneActive").GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.75f);
			canvasGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(canvasGroup.GetComponent<RectTransform>().anchoredPosition.x, canvasGroup.GetComponent<RectTransform>().anchoredPosition.y - 26f * (float)num);
			list.Add(canvasGroup);
		}
		vpnMenuInactive = list.ToArray();
	}

	private void OnDestroy()
	{
	}

	private void rebuildMenu()
	{
		float num = 0f;
		for (int i = 0; i < 5; i++)
		{
			vpnMenuInactive[i].alpha = 1f;
		}
		int num2 = 0;
		foreach (KeyValuePair<RemoteVPNObject, RemoteVPNMenuObject> currentActiveRemoteVPN in currentActiveRemoteVPNS)
		{
			currentActiveRemoteVPN.Value.PutMe(num2);
			num2++;
			vpnMenuInactive[currentActiveRemoteVPN.Key.myID].alpha = 0f;
		}
		Vector2 sizeDelta = new Vector2(myRT.sizeDelta.x, 138f);
		Vector2 anchoredPosition = new Vector2(myRT.anchoredPosition.x, 138f);
		myRT.sizeDelta = sizeDelta;
		myRT.anchoredPosition = anchoredPosition;
	}

	private void removeRemoteVPNFromMenu(RemoteVPNObject TheRemoteVPN)
	{
		if (currentActiveRemoteVPNS.TryGetValue(TheRemoteVPN, out var value))
		{
			value.ClearMe();
			currentActiveRemoteVPNS.Remove(TheRemoteVPN);
			remoteVPNMenuObjectPool.Push(value);
			rebuildMenu();
		}
	}

	private void addRemoteVPNToMenu(RemoteVPNObject TheRemoteVPN)
	{
		if (TheRemoteVPN.Placed)
		{
			if (!currentActiveRemoteVPNS.ContainsKey(TheRemoteVPN))
			{
				RemoteVPNMenuObject remoteVPNMenuObject = remoteVPNMenuObjectPool.Pop();
				remoteVPNMenuObject.BuildMe(TheRemoteVPN, TheRemoteVPN.myID);
				currentActiveRemoteVPNS.Add(TheRemoteVPN, remoteVPNMenuObject);
			}
			rebuildMenu();
		}
	}

	private void stageMe()
	{
		rebuildMenu();
		GameManager.StageManager.Stage -= stageMe;
		GameManager.ManagerSlinger.RemoteVPNManager.EnteredPlacementMode += removeRemoteVPNFromMenu;
		GameManager.ManagerSlinger.RemoteVPNManager.RemoteVPNWasReturned += removeRemoteVPNFromMenu;
		GameManager.ManagerSlinger.RemoteVPNManager.RemoteVPNWasPlaced += addRemoteVPNToMenu;
	}
}
