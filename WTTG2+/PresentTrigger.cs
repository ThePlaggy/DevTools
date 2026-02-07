using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class PresentTrigger : MonoBehaviour
{
	public bool reward;

	private MeshRenderer mesh;

	private int myID;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		mesh = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += pickUpPresent;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= pickUpPresent;
	}

	private void pickUpPresent()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
		if (reward)
		{
			EventRewardManager.ChristmasReward();
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.jingleBELLS);
		}
		else
		{
			EventManager.Ins.PresentWasPickedUp();
		}
	}
}
