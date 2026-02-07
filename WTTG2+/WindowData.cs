using System;

[Serializable]
public class WindowData : DataObject
{
	public bool Opened { get; set; }

	public bool Maxed { get; set; }

	public bool Minned { get; set; }

	public Vect2 WindowSize { get; set; }

	public Vect2 WindowPosition { get; set; }

	public WindowData(int SetID)
		: base(SetID)
	{
	}
}
