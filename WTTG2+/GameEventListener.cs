using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
	public GameEvent Event;

	public UnityEvent Response;

	public DefinitionUnityEvent DefinitionResponses;

	public void OnEnable()
	{
		Event.RegisterListener(this);
	}

	public void OnDisable()
	{
		Event.UnregisterListener(this);
	}

	public void OnEventRaised()
	{
		Response.Invoke();
	}

	public void OnDefinitionEventRaised<T>(T SetDef) where T : Definition
	{
		DefinitionResponses.Invoke(SetDef);
	}
}
