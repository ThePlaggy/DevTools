using System;
using System.Collections.Generic;

[Serializable]
public class MasterKeyData : DataObject
{
	public List<string> Keys { get; set; }

	public MasterKeyData(int SetID)
		: base(SetID)
	{
	}
}
