using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VPNMenuObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate void VPNMenuActions(VPN_LEVELS MyLevel);

	[SerializeField]
	private Text vpnName1;

	[SerializeField]
	private Text vpnName2;

	[SerializeField]
	private Image connectedIMG;

	[SerializeField]
	private Color hoverColor;

	public event VPNMenuActions IWasPressed;

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
