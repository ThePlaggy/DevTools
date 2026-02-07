using System;

[Serializable]
public class SteamSlingerData : DataObject
{
	public int ProductPickUpCount { get; set; }

	public SteamSlingerData(int SetID)
		: base(SetID)
	{
	}
}
