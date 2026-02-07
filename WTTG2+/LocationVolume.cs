using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LocationVolume : MonoBehaviour
{
	public bool PlayerInside;

	[SerializeField]
	private PLAYER_LOCATION myLocation;

	[SerializeField]
	private float updateDelay = 0.1f;

	[SerializeField]
	private bool hideGizmo;

	private Bounds bounds;

	private BoxCollider boxCollider;

	private float distance;

	private Camera mainCamera;

	public PLAYER_LOCATION Location => myLocation;

	private void Awake()
	{
		mainCamera = Camera.main;
		UpdateBounds();
		boxCollider.enabled = false;
	}

	private void OnEnable()
	{
		InvokeRepeating("OnUpdate", 0f, updateDelay);
	}

	private void OnDisable()
	{
		CancelInvoke("OnUpdate");
	}

	private void OnDrawGizmos()
	{
		if (!hideGizmo)
		{
			UpdateBounds();
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}

	private void UpdateBounds()
	{
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		bounds = new Bounds(base.transform.TransformPoint(boxCollider.center), boxCollider.size);
	}

	private void OnUpdate()
	{
		InBounds();
	}

	private void InBounds()
	{
		if (mainCamera != null)
		{
			Vector3 position = mainCamera.transform.position;
			if (bounds.Contains(position))
			{
				if (!PlayerInside)
				{
					PlayerInside = true;
					GameManager.WorldManager.AddActiveLocationVolume(this);
				}
			}
			else if (PlayerInside)
			{
				PlayerInside = false;
				GameManager.WorldManager.RemoveActiveLocationVolume(this);
			}
		}
		else if (PlayerInside)
		{
			PlayerInside = false;
			GameManager.WorldManager.RemoveActiveLocationVolume(this);
		}
	}
}
