using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HotZoneTrigger : MonoBehaviour
{
	public bool IsHot;

	public CustomEvent LeftHotZoneEvent = new CustomEvent(2);

	public void OnTriggerExit(Collider other)
	{
		if (IsHot)
		{
			LeftHotZoneEvent.Execute();
		}
		IsHot = false;
	}

	public void OnTriggerStay(Collider other)
	{
		IsHot = true;
	}
}
