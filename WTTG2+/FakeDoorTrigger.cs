using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(BoxCollider))]
public class FakeDoorTrigger : MonoBehaviour
{
	[SerializeField]
	private AudioFileDefinition lockedDoorSFX;

	private AudioHubObject myAudioHubObject;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myAudioHubObject = GetComponent<AudioHubObject>();
		myInteractionHook.LeftClickAction += openDoorAction;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= openDoorAction;
	}

	private void openDoorAction()
	{
		myInteractionHook.ForceLock = true;
		myAudioHubObject.PlaySound(lockedDoorSFX);
		GameManager.TimeSlinger.FireTimer(1.2f, delegate
		{
			myInteractionHook.ForceLock = false;
		});
	}
}
