using System;
using UnityEngine;

[Serializable]
public class CultSpawnDefinition : Definition
{
	public PLAYER_LOCATION Location;

	public Vector3 Position;

	public Vector3 Rotation;

	public bool RotateSpawnTowardsPlayer;

	public bool LookAtPlayer;

	public bool SeePlayer;

	public bool CheckDistance;

	public float DistanceThreshold;

	public bool HasLookAwayTime;

	public float LookAwayTime;

	public GameEvent SpawnEvent;

	public void InvokeSpawnEvent()
	{
		if (SpawnEvent != null)
		{
			SpawnEvent.DefinitionRaise(this);
		}
	}
}
