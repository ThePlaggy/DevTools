using UnityEngine;

public class TutorialProductBehaviour : TutorialStepper
{
	public static TutorialProductBehaviour Ins;

	[SerializeField]
	private ShadowMarketProductDefinition remoteVPNDef;

	private void Awake()
	{
		Ins = this;
	}

	public void AddRemoteVPNSpawn()
	{
		if (!DifficultyManager.Nightmare)
		{
			SpawnToDeadDropTrigger.Ins.PlayerSpawningToDeadDropEvent.Event += playerSpawnedToDeadDrop;
		}
	}

	private void playerSpawnedToDeadDrop()
	{
		SpawnToDeadDropTrigger.Ins.PlayerSpawningToDeadDropEvent.Event -= playerSpawnedToDeadDrop;
		GameManager.ManagerSlinger.ProductsManager.ShipProduct(remoteVPNDef);
	}
}
