using UnityEngine;

public class Insym : MonoBehaviour
{
	private bool HiInsym;

	public AudioSource asource;

	public AudioReverbFilter reverb;

	public void OnTriggerEnter(Collider other)
	{
		if (!HiInsym)
		{
			HiInsym = true;
			reverb.enabled = true;
			asource.Play();
		}
	}
}
