using System;

[Serializable]
public class TenantData
{
	public int tenantUnit;

	public string tenantName;

	public int tenantAge;

	public string tenantNotes;

	public int tenantSex;

	public bool canBeTagged;

	public int tenantIndex;

	public TenantData()
	{
	}

	public TenantData(TenantDefinition Source)
	{
		tenantUnit = Source.tenantUnit;
		tenantName = Source.tenantName;
		tenantAge = Source.tenantAge;
		tenantNotes = Source.tenantNotes;
		tenantSex = (int)Source.tenantSex;
		canBeTagged = Source.canBeTagged;
	}
}
