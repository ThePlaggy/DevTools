using UnityEngine;

public class DelfalcoTrigger : MonoBehaviour
{
	public CustomEvent playerEnterEvent = new CustomEvent();

	public CustomEvent playerExitEvent = new CustomEvent();

	public CustomEvent delfalcoEnterEvent = new CustomEvent();

	public CustomEvent delfalcoExitEvent = new CustomEvent();

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "roamController")
		{
			playerEnterEvent.Execute();
		}
		if (other.gameObject.name.Contains("Delfalco"))
		{
			delfalcoEnterEvent.Execute();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name == "roamController")
		{
			playerExitEvent.Execute();
		}
		if (other.gameObject.name.Contains("Delfalco"))
		{
			delfalcoExitEvent.Execute();
		}
	}

	public void MoveMe(Vector3 newPos)
	{
		base.transform.position = newPos;
	}

	public void DeSpawnMe()
	{
		base.transform.position = Vector3.zero;
	}
}
