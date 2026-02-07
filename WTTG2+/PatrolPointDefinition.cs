using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PatrolPointDefinition : Definition
{
	public Vector3 Position;

	public List<GameEvent> Events;

	public void InvokeEvents()
	{
		if (Events != null)
		{
			for (int i = 0; i < Events.Count; i++)
			{
				Events[i].Raise();
			}
		}
	}
}
