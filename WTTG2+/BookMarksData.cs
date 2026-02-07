using System;
using System.Collections.Generic;

[Serializable]
public class BookMarksData : DataObject
{
	public Dictionary<int, BookmarkData> BookMarks { get; set; }

	public BookMarksData(int SetID)
		: base(SetID)
	{
	}
}
