using System;
using System.Collections.Generic;

[Serializable]
public class DataContainer
{
	public Dictionary<int, IDataObject> MyData = new Dictionary<int, IDataObject>(20);

	public void Add<T>(int SetID, T DataToSave) where T : class, IDataObject
	{
		if (MyData.TryGetValue(SetID, out var value))
		{
			value = DataToSave;
			MyData[SetID] = value;
		}
		else
		{
			MyData.Add(SetID, DataToSave);
		}
	}
}
