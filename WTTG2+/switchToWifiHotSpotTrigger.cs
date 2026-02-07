using UnityEngine;

public class switchToWifiHotSpotTrigger : MonoBehaviour
{
	public GameObject myWifiHotspotObject;

	private void Awake()
	{
		GetComponent<InteractionHook>().LeftClickAction += placeDongle;
		GetComponent<InteractionHook>().RecvAction += triggerHover;
		GetComponent<InteractionHook>().RecindAction += triggerOffHover;
	}

	private void OnDestroy()
	{
		GetComponent<InteractionHook>().LeftClickAction -= placeDongle;
		GetComponent<InteractionHook>().RecvAction -= triggerHover;
		GetComponent<InteractionHook>().RecindAction -= triggerOffHover;
	}

	private void placeDongle()
	{
		myWifiHotspotObject.GetComponent<WifiHotspotObject>().PlaceDongleHere();
	}

	private void triggerHover()
	{
		myWifiHotspotObject.GetComponent<WifiHotspotObject>().ShowPreview();
	}

	private void triggerOffHover()
	{
		myWifiHotspotObject.GetComponent<WifiHotspotObject>().HidePreview();
	}
}
