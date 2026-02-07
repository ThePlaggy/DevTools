using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Backroomer : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Vector3 position = GameObject.Find("Backroomer").transform.position;
		position.y = 1333.5f;
		GameObject.Find("SecretController").transform.position = position;
		Object.Destroy(GameObject.Find("inthewaiting"));
	}
}
