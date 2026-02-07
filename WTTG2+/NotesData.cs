using System;
using System.Collections.Generic;

[Serializable]
public class NotesData : DataObject
{
	public List<string> Notes { get; set; }

	public NotesData(int SetID)
		: base(SetID)
	{
	}
}
