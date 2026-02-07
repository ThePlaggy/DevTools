using System;
using System.Collections.Generic;

[Serializable]
public class TextDocManagerData : DataObject
{
	public List<TextDocIconData> CurrentDocs { get; set; }

	public TextDocManagerData(int SetID)
		: base(SetID)
	{
	}
}
