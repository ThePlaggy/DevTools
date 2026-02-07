using UnityEngine;

public class WifiDongleBehaviour : MonoBehaviour
{
	public MeshRenderer WifiDongleLevel1;

	public MeshRenderer WifiDongleLevel2;

	public MeshRenderer WifiDongleLevel3;

	private MeshRenderer currentActiveWifiDongle;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		currentActiveWifiDongle = WifiDongleLevel1;
		myInteractionHook.LeftClickAction += PickupDongle;
		GameManager.StageManager.TheGameIsLive += gameLive;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= PickupDongle;
	}

	public void PickupDongle()
	{
		if (StateManager.PlayerLocation != PLAYER_LOCATION.OUTSIDE)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp1);
			currentActiveWifiDongle.enabled = false;
			GameManager.ManagerSlinger.WifiManager.EnterWifiDonglePlacementMode();
		}
	}

	public void PlaceDongle(Vector3 newPOS, Vector3 newROT, bool PlaySound = true)
	{
		if (PlaySound)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		}
		base.transform.localPosition = newPOS;
		base.transform.localRotation = Quaternion.Euler(newROT);
		currentActiveWifiDongle.enabled = true;
	}

	public void RefreshActiveWifiDongleLevel()
	{
		currentActiveWifiDongle.enabled = false;
		switch (InventoryManager.WifiDongleLevel)
		{
		case 1:
			currentActiveWifiDongle = WifiDongleLevel2;
			break;
		case 2:
			currentActiveWifiDongle = WifiDongleLevel3;
			break;
		default:
			currentActiveWifiDongle = WifiDongleLevel1;
			break;
		}
		currentActiveWifiDongle.enabled = true;
	}

	private void gameLive()
	{
		RefreshActiveWifiDongleLevel();
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}
}
