using UnityEngine;

[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(Rigidbody))]
public class FlashBangBehaviour : MonoBehaviour
{
	private bool bangFired;

	private bool lookAtMeActive;

	private AudioHubObject myAudioHub;

	private MeshRenderer myMesnRenderer;

	private Rigidbody myRigidBody;

	private void Awake()
	{
		myMesnRenderer = GetComponent<MeshRenderer>();
		myRigidBody = GetComponent<Rigidbody>();
		myAudioHub = GetComponent<AudioHubObject>();
	}

	private void Update()
	{
		if (lookAtMeActive)
		{
			PoliceRoamJumper.Ins.TriggerCameraConstantLookAt(base.transform.position);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		lookAtMeActive = false;
		if (!bangFired)
		{
			bangFired = true;
			GameManager.TimeSlinger.FireTimer(0.25f, boom);
		}
	}

	public void Thrown()
	{
		lookAtMeActive = true;
	}

	private void boom()
	{
		myMesnRenderer.enabled = false;
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.FlashBang);
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.EarRing);
		UIManager.FlashPlayer();
		MainCameraHook.Ins.TriggerFlashBlur();
	}
}
