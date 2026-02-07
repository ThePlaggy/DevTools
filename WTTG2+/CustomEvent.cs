using System;
using System.Collections.Generic;

public class CustomEvent
{
	private readonly List<Action> events;

	public int Count => events.Count;

	public event Action Event
	{
		add
		{
			events.Add(value);
		}
		remove
		{
			events.Remove(value);
		}
	}

	public CustomEvent(int capacity = 1)
	{
		events = new List<Action>(capacity);
	}

	public void Execute()
	{
		for (int i = 0; i < events.Count; i++)
		{
			if (events[i] != null)
			{
				events[i]();
			}
		}
	}

	public void ExecuteAndKill()
	{
		for (int i = 0; i < events.Count; i++)
		{
			if (events[i] != null)
			{
				events[i]();
				events.Remove(events[i]);
			}
		}
	}

	public void Clear()
	{
		events.Clear();
	}
}
public class CustomEvent<T>
{
	private readonly List<Action<T>> events;

	public event Action<T> Event
	{
		add
		{
			events.Add(value);
		}
		remove
		{
			events.Remove(value);
		}
	}

	public CustomEvent(int capacity = 1)
	{
		events = new List<Action<T>>(capacity);
	}

	public void Execute(T value)
	{
		for (int i = 0; i < events.Count; i++)
		{
			if (events[i] != null)
			{
				events[i](value);
			}
		}
	}

	public void Clear()
	{
		events.Clear();
	}
}
