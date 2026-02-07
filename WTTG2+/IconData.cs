using System;

[Serializable]
public class IconData : DataObject
{
	public Vect2 MyPOS { get; set; }

	public IconData(int SetID)
		: base(SetID)
	{
	}
}
