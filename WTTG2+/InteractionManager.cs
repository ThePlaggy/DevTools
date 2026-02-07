using UnityEngine;

public class InteractionManager : MonoBehaviour
{
	public CAMERA_ID CameraToUse;

	public LayerMask Layer;

	public float Distance;

	private Vector3 aimFrom = new Vector3(0.5f, 0.5f, 0.5f);

	private RaycastHit interactionHit;

	private bool lockInteraction;

	private bool masterLock;

	private Camera myCamera;

	public CustomEvent Rescind = new CustomEvent(5);

	private void Awake()
	{
		GameManager.InteractionManager = this;
		GameManager.PauseManager.GamePaused += PlayerHitPause;
		GameManager.PauseManager.GameUnPaused += PlayerHitUnPause;
	}

	private void Start()
	{
		CameraManager.Get(CameraToUse, out myCamera);
	}

	private void FixedUpdate()
	{
		if (!lockInteraction)
		{
			if (Physics.Raycast(myCamera.ViewportPointToRay(aimFrom), out interactionHit, Distance, Layer.value))
			{
				interactionHit.collider.SendMessageUpwards("Receive");
			}
			else
			{
				Rescind.Execute();
			}
		}
		else
		{
			Rescind.Execute();
		}
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= PlayerHitPause;
		GameManager.PauseManager.GameUnPaused -= PlayerHitUnPause;
	}

	public void LockInteraction()
	{
		masterLock = true;
		lockInteraction = true;
	}

	public void UnLockInteraction()
	{
		masterLock = false;
		lockInteraction = false;
	}

	private void PlayerHitPause()
	{
		lockInteraction = true;
	}

	private void PlayerHitUnPause()
	{
		if (!masterLock)
		{
			lockInteraction = false;
		}
	}
}
