using UnityEngine;

[RequireComponent(typeof(AudioReverbZone))]
[RequireComponent(typeof(HotZoneTrigger))]
public class ReverbZoneTrigger : MonoBehaviour
{
	private HotZoneTrigger myHotZone;

	private AudioReverbZone myReverbZone;

	private void Awake()
	{
		myReverbZone = GetComponent<AudioReverbZone>();
		myHotZone = GetComponent<HotZoneTrigger>();
	}

	private void Update()
	{
		if (myHotZone.IsHot)
		{
			myReverbZone.enabled = true;
		}
		else
		{
			myReverbZone.enabled = false;
		}
	}
}
