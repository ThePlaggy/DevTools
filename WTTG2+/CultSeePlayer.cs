using UnityEngine;

public class CultSeePlayer : MonoBehaviour
{
	[SerializeField]
	private Transform firePosition;

	[SerializeField]
	private float sphereRadius = 0.5f;

	[SerializeField]
	private float maxDistance = 5f;

	[SerializeField]
	private LayerMask visibleLayers;

	[SerializeField]
	private BoolUnityEvent playerVisibleSet;

	private bool lookForPlayer;

	private RaycastHit ray;

	private CultSpawnDefinition spawnData;

	private void Awake()
	{
	}

	private void FixedUpdate()
	{
		if (!lookForPlayer)
		{
			return;
		}
		if (Physics.CheckSphere(firePosition.position, sphereRadius, visibleLayers))
		{
			if (playerVisibleSet != null)
			{
				playerVisibleSet.Invoke(arg0: true);
			}
		}
		else if (Physics.SphereCast(firePosition.position, sphereRadius, base.transform.forward, out ray, maxDistance, visibleLayers))
		{
			if (playerVisibleSet != null)
			{
				playerVisibleSet.Invoke(arg0: true);
			}
		}
		else if (playerVisibleSet != null)
		{
			playerVisibleSet.Invoke(arg0: false);
		}
	}

	private void OnDrawGizmos()
	{
		if (firePosition != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(firePosition.position, sphereRadius);
			Gizmos.DrawLine(firePosition.position, firePosition.position + base.transform.forward * maxDistance);
			Gizmos.DrawWireSphere(firePosition.position + base.transform.forward * maxDistance, sphereRadius);
		}
	}

	public void StageSpawnData(Definition SetSpawnData)
	{
		spawnData = (CultSpawnDefinition)SetSpawnData;
	}

	public void SpawnAction()
	{
		if (spawnData != null && spawnData.SeePlayer)
		{
			lookForPlayer = true;
		}
	}

	public void DeSpawnAction()
	{
		lookForPlayer = false;
	}
}
