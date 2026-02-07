using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
	private readonly List<GameEventListener> eventListeners = new List<GameEventListener>();

	public void Raise()
	{
		for (int num = eventListeners.Count - 1; num >= 0; num--)
		{
			eventListeners[num].OnEventRaised();
		}
	}

	public void DefinitionRaise<T>(T SetDef) where T : Definition
	{
		for (int num = eventListeners.Count - 1; num >= 0; num--)
		{
			eventListeners[num].OnDefinitionEventRaised(SetDef);
		}
	}

	public void RegisterListener(GameEventListener listener)
	{
		eventListeners.Add(listener);
	}

	public void UnregisterListener(GameEventListener listener)
	{
		eventListeners.Remove(listener);
	}
}
