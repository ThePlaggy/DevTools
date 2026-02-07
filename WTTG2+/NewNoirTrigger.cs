using UnityEngine;

public class NewNoirTrigger : MonoBehaviour
{
	public CustomEvent playerEnterEvent = new CustomEvent();

	public CustomEvent playerLeftEvent = new CustomEvent();

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "roamController")
		{
			playerEnterEvent.Execute();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.name == "roamController")
		{
			playerLeftEvent.Execute();
		}
	}
}
