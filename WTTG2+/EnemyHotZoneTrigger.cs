using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class EnemyHotZoneTrigger : MonoBehaviour
{
	[SerializeField]
	private UnityEvent triggerActions;

	private bool isActive;

	private BoxCollider myBoxCollider;

	private void Awake()
	{
		myBoxCollider = GetComponent<BoxCollider>();
		myBoxCollider.enabled = false;
		isActive = false;
	}

	private void OnDestroy()
	{
		triggerActions.RemoveAllListeners();
	}

	private void OnTriggerStay(Collider other)
	{
		if (isActive)
		{
			isActive = false;
			triggerActions.Invoke();
		}
	}

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
}
