using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WifiDisconnect : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public Color hoverColor;

	private Color defaultColor;

	private void Start()
	{
		defaultColor = GetComponent<Image>().color;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		GameManager.ManagerSlinger.WifiManager.TriggerWifiMenu();
		GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GetComponent<Image>().color = hoverColor;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GetComponent<Image>().color = defaultColor;
	}
}
