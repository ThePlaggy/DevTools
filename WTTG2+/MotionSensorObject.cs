using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class MotionSensorObject : MonoBehaviour
{
	public delegate void motionSensorPlacement(MotionSensorObject TheMotionSensor);

	[SerializeField]
	private LayerMask enemyMask;

	[SerializeField]
	private float MOTION_SENSOR_SPHERE_CAST_RADIUS = 0.43f;

	[SerializeField]
	private float MOTION_SENSOR_SPHERE_CAST_DISTANCE = 2f;

	private InteractionHook myInteractionHook;

	private MeshRenderer myMeshRenderer;

	private int pickupCounter;

	private float myTimeStamp;

	public Vector3 SpawnLocation { get; private set; } = Vector3.zero;

	public Vector3 SpawnRotation { get; private set; } = Vector3.zero;

	public bool Placed { get; private set; }

	public PLAYER_LOCATION Location { get; private set; }

	public Transform Transform => base.transform;

	public event motionSensorPlacement EnteredPlacementMode;

	public event motionSensorPlacement IWasPlaced;

	public event motionSensorPlacement IWasTripped;

	private void Start()
	{
		pickupCounter = 0;
	}

	private void Update()
	{
		if (Time.time - myTimeStamp >= 1f)
		{
			pickupCounter = 0;
			myTimeStamp = Time.time;
		}
	}

	private void FixedUpdate()
	{
		if (!Placed)
		{
			return;
		}
		RaycastHit hitInfo;
		if (Physics.CheckSphere(base.transform.position + base.transform.forward * MOTION_SENSOR_SPHERE_CAST_RADIUS, MOTION_SENSOR_SPHERE_CAST_RADIUS, enemyMask.value))
		{
			if (this.IWasTripped != null)
			{
				this.IWasTripped(this);
			}
		}
		else if (Physics.SphereCast(base.transform.position + base.transform.forward * MOTION_SENSOR_SPHERE_CAST_RADIUS, MOTION_SENSOR_SPHERE_CAST_RADIUS, base.transform.forward, out hitInfo, MOTION_SENSOR_SPHERE_CAST_DISTANCE, enemyMask.value) && this.IWasTripped != null)
		{
			this.IWasTripped(this);
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= enterPlacementMode;
	}

	private void OnDrawGizmos()
	{
		if (Placed)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(base.transform.position + base.transform.forward * MOTION_SENSOR_SPHERE_CAST_RADIUS, MOTION_SENSOR_SPHERE_CAST_RADIUS);
			Gizmos.DrawLine(base.transform.position + base.transform.forward * MOTION_SENSOR_SPHERE_CAST_RADIUS, base.transform.position + base.transform.forward * (MOTION_SENSOR_SPHERE_CAST_DISTANCE + MOTION_SENSOR_SPHERE_CAST_RADIUS));
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position + base.transform.forward * (MOTION_SENSOR_SPHERE_CAST_DISTANCE + MOTION_SENSOR_SPHERE_CAST_RADIUS), MOTION_SENSOR_SPHERE_CAST_RADIUS);
		}
	}

	public void SoftBuild()
	{
		myMeshRenderer = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += enterPlacementMode;
		myMeshRenderer.enabled = false;
		myInteractionHook.ForceLock = true;
	}

	public void PlaceMe(Vector3 SetPosition, Vector3 SetRotation)
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		UIInventoryManager.HideMotionSensor();
		Placed = true;
		Location = StateManager.PlayerLocation;
		base.transform.position = SetPosition;
		base.transform.rotation = Quaternion.Euler(SetRotation);
		myMeshRenderer.enabled = true;
		myInteractionHook.ForceLock = false;
		if (this.IWasPlaced != null)
		{
			this.IWasPlaced(this);
		}
	}

	public void SpawnMe(Vector3 SetPosition, Vector3 SetRotation)
	{
		Placed = false;
		base.transform.position = SetPosition;
		base.transform.rotation = Quaternion.Euler(SetRotation);
		SpawnLocation = SetPosition;
		SpawnRotation = SetRotation;
		myMeshRenderer.enabled = true;
		myInteractionHook.ForceLock = false;
	}

	public void SetPlaceMe(SerTrans WhereTo, PLAYER_LOCATION Location)
	{
		Placed = true;
		base.transform.position = WhereTo.PositionToVector3;
		base.transform.rotation = Quaternion.Euler(WhereTo.RotationToVector3);
		this.Location = Location;
	}

	private void enterPlacementMode()
	{
		pickupCounter++;
		if (pickupCounter >= 2 || StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp1);
			UIInventoryManager.ShowMotionSensor();
			Placed = false;
			myMeshRenderer.enabled = false;
			myInteractionHook.ForceLock = true;
			if (this.EnteredPlacementMode != null)
			{
				this.EnteredPlacementMode(this);
			}
			pickupCounter = 0;
		}
	}
}
