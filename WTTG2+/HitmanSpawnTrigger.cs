using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HitmanSpawnTrigger : MonoBehaviour
{
	[SerializeField]
	private HitmanSpawnDefUnityEvent TriggerEvents;

	[SerializeField]
	private HitmanSpawnDefinition spawnData;

	private bool isActive;

	private BoxCollider myBoxCollider;

	private void Awake()
	{
		myBoxCollider = GetComponent<BoxCollider>();
		myBoxCollider.enabled = false;
		isActive = false;
	}

	private void OnTriggerStay(Collider other)
	{
		if (isActive)
		{
			isActive = false;
			if (TriggerEvents != null)
			{
				TriggerEvents.Invoke(spawnData);
			}
		}
	}

	public void SetActive()
	{
		myBoxCollider.enabled = true;
		isActive = true;
	}

	public void Deactivate()
	{
		myBoxCollider.enabled = false;
		isActive = false;
	}

	public void FailSafe()
	{
		if (TriggerEvents != null)
		{
			TriggerEvents.Invoke(spawnData);
		}
	}
}
