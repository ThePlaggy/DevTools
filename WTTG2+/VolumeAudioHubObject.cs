using UnityEngine;

[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(BoxCollider))]
public class VolumeAudioHubObject : MonoBehaviour
{
	[SerializeField]
	private float updateDelay = 0.1f;

	[SerializeField]
	private float fadeDistance = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float minVolume;

	[SerializeField]
	[Range(0f, 1f)]
	private float maxVolume = 1f;

	private Bounds bounds;

	private BoxCollider boxCollider;

	private float distance;

	private Bounds fadeBounds;

	private Camera mainCamera;

	private AudioHubObject myAudioHubObject;

	private float setVolumeAMT;

	private void Awake()
	{
		mainCamera = Camera.main;
		myAudioHubObject = GetComponent<AudioHubObject>();
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
		UpdateBounds();
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(bounds.center, bounds.size);
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(fadeBounds.center, fadeBounds.size);
	}

	private void UpdateBounds()
	{
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		bounds = new Bounds(base.transform.TransformPoint(boxCollider.center), boxCollider.size);
		if (fadeDistance <= 0f)
		{
			fadeDistance = 0f;
		}
		fadeBounds = bounds;
		fadeBounds.Expand(fadeDistance);
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
				distance = 0f;
				setVolumeAMT = maxVolume;
			}
			else
			{
				Vector3 vector = bounds.ClosestPoint(position);
				distance = (vector - position).magnitude;
				if (distance <= fadeDistance)
				{
					setVolumeAMT = Mathf.Min((fadeDistance - distance) / fadeDistance, maxVolume);
				}
				else
				{
					setVolumeAMT = minVolume;
				}
			}
			myAudioHubObject.MuffleHub(setVolumeAMT);
		}
		else
		{
			setVolumeAMT = minVolume;
		}
	}
}
