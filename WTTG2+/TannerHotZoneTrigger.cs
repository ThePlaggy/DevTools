using UnityEngine;

public class TannerHotZoneTrigger : MonoBehaviour
{
	public CustomEvent TriggerActions = new CustomEvent();

	private BoxCollider myBoxCollider;

	private bool isActive;

	public void SetActive()
	{
		myBoxCollider.enabled = true;
		isActive = true;
	}

	public void SetInActive()
	{
		myBoxCollider.enabled = false;
		isActive = false;
	}

	private void OnTriggerStay(Collider other)
	{
		if (isActive)
		{
			isActive = false;
			TriggerActions.Execute();
		}
	}

	private void Awake()
	{
		myBoxCollider = GetComponent<BoxCollider>();
		myBoxCollider.enabled = false;
		isActive = false;
	}

	private void OnDestroy()
	{
		TriggerActions.Clear();
	}
}
