using System;

[Serializable]
public class DollMakerMarkerData : DataObject
{
	public bool IsPlaced { get; set; }

	public Vect3 PlacedLocation { get; set; }

	public Vect3 PlaceRotation { get; set; }

	public DollMakerMarkerData(int SetID)
		: base(SetID)
	{
	}
}
