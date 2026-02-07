using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
[RequireComponent(typeof(BoxCollider))]
public class DollMakerMarkerTrigger : MonoBehaviour
{
	[SerializeField]
	private int unitNumber;

	[SerializeField]
	private Mesh placeMesh;

	[SerializeField]
	private Vector3 spawnPOS;

	[SerializeField]
	private Vector3 spawnROT;

	private bool lockedOut;

	private BoxCollider myBoxCollider;

	private InteractionHook myInteractionHook;

	public int UnitNumber => unitNumber;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myBoxCollider = GetComponent<BoxCollider>();
		myInteractionHook.LeftClickAction += placeMarker;
	}

	private void Start()
	{
		myInteractionHook.ForceLock = true;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if (placeMesh != null)
		{
			Gizmos.DrawMesh(placeMesh, spawnPOS, Quaternion.Euler(spawnROT));
		}
	}

	public void Activate()
	{
		if (!lockedOut)
		{
			myInteractionHook.ForceLock = false;
		}
	}

	public void DeActivate()
	{
		myInteractionHook.ForceLock = true;
	}

	public void LockOut()
	{
		lockedOut = true;
	}

	private void placeMarker()
	{
		DeActivate();
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		EnemyManager.DollMakerManager.MarkerWasPlaced(unitNumber, spawnPOS, spawnROT);
	}

	[ContextMenu("Reset Placement")]
	private void resetPlacement()
	{
		spawnPOS = base.transform.position;
	}
}
