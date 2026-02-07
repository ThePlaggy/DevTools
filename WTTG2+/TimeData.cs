using System;

[Serializable]
public class TimeData : DataObject
{
	public int GameHour { get; set; }

	public int GameMin { get; set; }

	public TimeData(int SetID)
		: base(SetID)
	{
	}
}
