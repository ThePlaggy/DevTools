using UnityEngine;
using UnityEngine.EventSystems;

public class bringMeToFrontForce : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public GameObject bringMeToFrontHolder;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (bringMeToFrontHolder != null && bringMeToFrontHolder.GetComponent<BringWindowToFrontBehaviour>() != null)
		{
			bringMeToFrontHolder.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		}
	}
}
