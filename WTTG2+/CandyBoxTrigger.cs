using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class CandyBoxTrigger : MonoBehaviour
{
	private MeshRenderer mesh;

	[HideInInspector]
	public InteractionHook myInteractionHook;

	[HideInInspector]
	public HalloweenEvent HE;

	private void Awake()
	{
		mesh = GetComponent<MeshRenderer>();
		myInteractionHook = base.gameObject.AddComponent<InteractionHook>();
		myInteractionHook.StateActive = PLAYER_STATE.ROAMING;
		myInteractionHook.LeftClickAction += catjamCandy;
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= catjamCandy;
	}

	public void appearCandybox()
	{
		myInteractionHook.ForceLock = false;
		mesh.enabled = true;
	}

	private void catjamCandy()
	{
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
		HE.PickupCandyBox();
	}
}
