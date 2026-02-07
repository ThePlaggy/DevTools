using UnityEngine;

public class TenantExtension
{
	public TenantDefinition[] ExtendTenants(TenantDefinition[] tenants)
	{
		TenantTrackManager.DidAyana = false;
		TenantDefinition ayanaArmstrong = GetAyanaArmstrong();
		NewTenantGenerator newTenantGenerator = new NewTenantGenerator();
		int num = Random.Range(10, 20);
		for (int i = 0; i < num; i++)
		{
			TenantDefinition tenantDefinition = newTenantGenerator.NewFemaleTenant(Random.Range(0, 100) > 10);
			tenants[i].canBeTagged = tenantDefinition.canBeTagged;
			tenants[i].tenantAge = tenantDefinition.tenantAge;
			tenants[i].tenantName = tenantDefinition.tenantName;
			tenants[i].tenantNotes = tenantDefinition.tenantNotes;
			tenants[i].tenantSex = tenantDefinition.tenantSex;
		}
		for (int j = num; j < 34; j++)
		{
			TenantDefinition tenantDefinition2 = newTenantGenerator.NewMaleTenant();
			tenants[j].canBeTagged = tenantDefinition2.canBeTagged;
			tenants[j].tenantAge = tenantDefinition2.tenantAge;
			tenants[j].tenantName = tenantDefinition2.tenantName;
			tenants[j].tenantNotes = tenantDefinition2.tenantNotes;
			tenants[j].tenantSex = tenantDefinition2.tenantSex;
		}
		tenants[Random.Range(0, num)] = ayanaArmstrong;
		return tenants;
	}

	private TenantDefinition GetAyanaArmstrong()
	{
		TenantDefinition tenantDefinition = ScriptableObject.CreateInstance<TenantDefinition>();
		tenantDefinition.tenantName = "Ayana Armstrong";
		tenantDefinition.tenantAge = 29;
		tenantDefinition.tenantNotes = "Always wears black, very mysterious. Guy named Adam visits her often.";
		tenantDefinition.tenantSex = SEX.FEMALE;
		tenantDefinition.canBeTagged = true;
		return tenantDefinition;
	}
}
