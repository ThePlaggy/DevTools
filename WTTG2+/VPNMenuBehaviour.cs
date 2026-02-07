using UnityEngine;
using UnityEngine.EventSystems;

public class VPNMenuBehaviour : MonoBehaviour, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private int VPN_MENU_OBJ_POOL_COUNT = 5;

	[SerializeField]
	private GameObject VPNMenuObjectPrefab;

	[SerializeField]
	private CanvasGroup noVPNSOwnedCG;

	[SerializeField]
	private RectTransform sepRT;

	[SerializeField]
	private RectTransform disRT;

	[SerializeField]
	private VPNMenuDisObject disObject;

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
