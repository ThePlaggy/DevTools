using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class GetFlashLightTrigger : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer flashLightMesh;

	private FlashLightData myData;

	private int myID;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += getFlashLight;
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= getFlashLight;
	}

	private void getFlashLight()
	{
		if (EnvironmentManager.PowerState == POWER_STATE.OFF)
		{
			SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.NIGHTVISION);
		}
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		myInteractionHook.ForceLock = true;
		flashLightMesh.enabled = false;
		InventoryManager.OwnsFlashlight = true;
		myData.OwnsFlashLight = true;
		DataManager.Save(myData);
	}

	private void stageMe()
	{
		myData = DataManager.Load<FlashLightData>(myID);
		if (myData == null)
		{
			myData = new FlashLightData(myID);
			myData.OwnsFlashLight = false;
		}
		if (myData.OwnsFlashLight)
		{
			myInteractionHook.ForceLock = true;
			flashLightMesh.enabled = false;
			InventoryManager.OwnsFlashlight = true;
		}
		else
		{
			InventoryManager.OwnsFlashlight = false;
		}
		GameManager.StageManager.TheGameIsLive -= stageMe;
	}
}
