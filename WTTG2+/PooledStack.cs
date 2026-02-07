using System;
using System.Collections.Generic;

public class PooledStack<T> : Stack<T>
{
	private readonly Func<T> createCallback;

	public PooledStack(Func<T> createCallback, int preAllocate = 0)
	{
		this.createCallback = createCallback;
		if (createCallback != null)
		{
			for (int i = 0; i < preAllocate; i++)
			{
				Push(createCallback());
			}
		}
	}

	public new T Pop()
	{
		return (base.Count >= 1) ? base.Pop() : createCallback();
	}
}
