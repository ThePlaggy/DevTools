using UnityEngine;

public class EXEDoorTrigger : MonoBehaviour
{
	public CustomEvent playerEnterEvent = new CustomEvent();

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name == "roamController")
		{
			playerEnterEvent.Execute();
		}
	}
}
