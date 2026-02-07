using UnityEngine;

public class CultLookAtPlayer : MonoBehaviour
{
	[SerializeField]
	private Transform neckBone;

	[SerializeField]
	private float lookRange;

	private float clampMax;

	private float clampMin;

	private Vector3 lookRotation = Vector3.zero;

	private Transform roamControllerTransform;

	private bool spawnActive;

	private CultSpawnDefinition spawnData;

	private void Awake()
	{
		GameManager.StageManager.Stage += stageMe;
	}

	private void LateUpdate()
	{
		if (spawnActive && spawnData != null && spawnData.LookAtPlayer)
		{
			Vector3 forward = roamControllerTransform.position - base.transform.position;
			lookRotation = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
			Vector3 zero = Vector3.zero;
			zero.x = neckBone.transform.rotation.x;
			zero.z = neckBone.transform.rotation.z;
			if (clampMin <= 0f)
			{
				float y = ((lookRotation.y >= 360f - Mathf.Abs(clampMin) || lookRotation.y <= clampMax) ? lookRotation.y : ((!(lookRotation.y > 180f)) ? clampMax : (360f - Mathf.Abs(clampMin))));
				zero.y = y;
			}
			else
			{
				zero.y = Mathf.Clamp(lookRotation.y, clampMin, clampMax);
			}
			neckBone.transform.rotation = Quaternion.Euler(zero);
		}
	}

	public void StageSpawnData(Definition SetSpawnData)
	{
		spawnData = (CultSpawnDefinition)SetSpawnData;
		if (spawnData.RotateSpawnTowardsPlayer)
		{
			Vector3 forward = roamController.Ins.transform.position - spawnData.Position;
			Vector3 eulerAngles = Quaternion.LookRotation(forward).eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			clampMin = eulerAngles.y - lookRange;
			clampMax = eulerAngles.y + lookRange;
		}
		else
		{
			clampMin = spawnData.Rotation.y - lookRange;
			clampMax = spawnData.Rotation.y + lookRange;
		}
	}

	public void SpawnAction()
	{
		if (spawnData != null && spawnData.RotateSpawnTowardsPlayer)
		{
			Vector3 forward = roamController.Ins.transform.position - spawnData.Position;
			Vector3 eulerAngles = Quaternion.LookRotation(forward).eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			clampMin = eulerAngles.y - lookRange;
			clampMax = eulerAngles.y + lookRange;
		}
		spawnActive = true;
	}

	public void DeSpawnAction()
	{
		spawnActive = false;
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		roamControllerTransform = roamController.Ins.transform;
	}
}
