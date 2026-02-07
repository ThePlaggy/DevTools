using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioHubObject))]
public class ShowerCurtianTrigger : MonoBehaviour
{
	public static ShowerCurtianTrigger Ins;

	[SerializeField]
	private Animator myAC;

	[SerializeField]
	private AudioFileDefinition OpenSFX;

	[SerializeField]
	private AudioFileDefinition CloseSFX;

	[SerializeField]
	private UnityEvent OpenEvents;

	[SerializeField]
	private UnityEvent CloseEvents;

	private AudioHubObject myAudioHub;

	private void Awake()
	{
		Ins = this;
		myAudioHub = GetComponent<AudioHubObject>();
	}

	public void TriggerOpen()
	{
		myAudioHub.PlaySound(OpenSFX);
		myAC.SetTrigger("triggerOpen");
		UpdatePeakValue(0f);
	}

	public void TriggerClose()
	{
		myAudioHub.PlaySound(CloseSFX);
		myAC.SetTrigger("triggerClose");
	}

	public void UpdatePeakValue(float SetValue)
	{
		myAC.SetFloat("Peak", SetValue);
	}

	public void AnimationCompleted(SHOWER_CURTIAN_STATES TheState)
	{
		switch (TheState)
		{
		case SHOWER_CURTIAN_STATES.OPENED:
			if (OpenEvents != null)
			{
				OpenEvents.Invoke();
			}
			break;
		case SHOWER_CURTIAN_STATES.CLOSED:
			if (CloseEvents != null)
			{
				CloseEvents.Invoke();
			}
			break;
		}
	}
}
