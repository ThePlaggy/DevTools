using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class BlueWhisperBehaviour : MonoBehaviour
{
	public static BlueWhisperBehaviour Ins;

	private InteractionHook myInteractionHook;

	private MeshRenderer myMeshRenderer;

	private void Awake()
	{
		Ins = this;
		myMeshRenderer = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		GameManager.StageManager.Stage += stageMe;
		myInteractionHook.LeftClickAction += pickUpAction;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= pickUpAction;
	}

	public void SpawnMe()
	{
		myMeshRenderer.enabled = true;
		myInteractionHook.ForceLock = false;
	}

	private void pickUpAction()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		myInteractionHook.ForceLock = true;
		myMeshRenderer.enabled = false;
		BlueWhisperManager.Ins.PickedUpBlueWhisper();
	}

	private void stageMe()
	{
		myMeshRenderer.enabled = false;
		myInteractionHook.ForceLock = true;
		GameManager.StageManager.Stage -= stageMe;
	}
}
