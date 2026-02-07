using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class ReturnRemoteVPNTrigger : MonoBehaviour
{
	[SerializeField]
	private GameObject remoteVPNObject;

	private InteractionHook myInteractionHook;

	private MeshRenderer remoteVPNMeshRenderer;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		remoteVPNMeshRenderer = remoteVPNObject.GetComponent<MeshRenderer>();
		myInteractionHook.RecvAction += hoverAction;
		myInteractionHook.RecindAction += exitAction;
		myInteractionHook.LeftClickAction += leftClickAction;
	}

	private void Update()
	{
		if (StateManager.PlayerState == PLAYER_STATE.REMOTE_VPN_PLACEMENT)
		{
			myInteractionHook.ForceLock = false;
		}
		else
		{
			myInteractionHook.ForceLock = true;
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.RecvAction -= hoverAction;
		myInteractionHook.RecindAction -= exitAction;
		myInteractionHook.LeftClickAction -= leftClickAction;
	}

	private void hoverAction()
	{
		remoteVPNObject.transform.position = GameManager.ManagerSlinger.RemoteVPNManager.CurrentRemoteVPNSpawnLocation;
		remoteVPNObject.transform.rotation = Quaternion.Euler(GameManager.ManagerSlinger.RemoteVPNManager.CurrentRemoteVPNSpawnRotation);
		remoteVPNMeshRenderer.enabled = true;
	}

	private void exitAction()
	{
		remoteVPNMeshRenderer.enabled = false;
	}

	private void leftClickAction()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		UIInventoryManager.HideRemoteVPN();
		remoteVPNMeshRenderer.enabled = false;
		GameManager.ManagerSlinger.RemoteVPNManager.ReturnRemoteVPN();
	}
}
