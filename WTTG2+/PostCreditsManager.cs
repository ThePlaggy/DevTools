using UnityEngine;

public class PostCreditsManager : MonoBehaviour
{
	public Vector3 spawnSpot;

	public static bool megasex;

	private void Awake()
	{
		spawnSpot = new Vector3(-1.7244f, 0.9299f, -509.3485f);
		GameObject.Find("SecretController").transform.position = spawnSpot;
		GameObject.Find("SecretController").transform.rotation = Quaternion.Euler(Vector3.zero);
	}
}
