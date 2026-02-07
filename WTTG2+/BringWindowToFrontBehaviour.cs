using UnityEngine;
using UnityEngine.EventSystems;

public class BringWindowToFrontBehaviour : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public delegate void TapFuctions();

	public Transform parentTrans;

	public event TapFuctions OnTap;

	public void OnPointerDown(PointerEventData eventData)
	{
		Transform child = parentTrans.GetChild(parentTrans.childCount - 1);
		child.SetSiblingIndex(0);
		base.transform.SetSiblingIndex(parentTrans.childCount);
		if (this.OnTap != null)
		{
			this.OnTap();
		}
	}

	public void forceTap()
	{
		Transform child = parentTrans.GetChild(parentTrans.childCount - 1);
		child.SetSiblingIndex(0);
		base.transform.SetSiblingIndex(parentTrans.childCount);
		if (this.OnTap != null)
		{
			this.OnTap();
		}
	}
}
