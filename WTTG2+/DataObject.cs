using System;

[Serializable]
public class DataObject : IDataObject
{
	public int ID { get; set; }

	public DataObject(int SetID)
	{
		ID = SetID;
	}
}
