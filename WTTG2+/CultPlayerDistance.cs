using UnityEngine;
using UnityEngine.Events;

public class CultPlayerDistance : MonoBehaviour
{
	[SerializeField]
	private UnityEvent ThresholdCrossedActions;

	private CultSpawnDefinition currentSpawnData;

	private float distance;

	private Camera mainCamera;

	private bool spawnActive;

	private void Awake()
	{
		mainCamera = Camera.main;
	}

	private void Update()
	{
		if (EnemyStateManager.IsEnemyStateLocked() || !spawnActive || !(currentSpawnData != null) || !currentSpawnData.CheckDistance)
		{
			return;
		}
		updateDistance();
		if (distance <= currentSpawnData.DistanceThreshold)
		{
			spawnActive = false;
			if (ThresholdCrossedActions != null)
			{
				ThresholdCrossedActions.Invoke();
			}
		}
	}

	public void StageSpawnData(Definition SetSpawnData)
	{
		currentSpawnData = (CultSpawnDefinition)SetSpawnData;
	}

	public void SpawnAction()
	{
		spawnActive = true;
	}

	public void DeSpawnAction()
	{
		spawnActive = false;
	}

	private void updateDistance()
	{
		Vector3 position = mainCamera.transform.position;
		distance = (base.transform.position - position).magnitude;
	}
}
