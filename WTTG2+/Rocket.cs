using System.Collections;
using UnityEngine;

public class Rocket : MonoBehaviour
{
	public Rigidbody rig;

	public ConstantForce cf;

	public Transform IsKinematic;

	public float time = 3f;

	public GameObject renderer;

	private IEnumerator Start()
	{
		GetComponent<ConstantForce>().force = new Vector3(5f, 50f, 20f);
		GetComponent<Transform>().rotation = Quaternion.Euler(30f, 0f, 0f);
		yield return new WaitForSeconds(time);
		renderer.SetActive(value: false);
		rig.isKinematic = true;
		cf.enabled = false;
	}
}
