using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotnetFoundDeviceBTN : MonoBehaviour
{
	public TMP_Text deviceName;

	public Image deviceIcon;

	public TMP_Text hackPrice;

	public Device device;

	public BotnetAppBTN hackBTN;

	private void Start()
	{
		hackBTN.setMyAction(Hack);
	}

	public void Hack()
	{
		if (!StateManager.BeingHacked)
		{
			if (BotnetBehaviour.Ins.connectedDevicesCount >= 10)
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.Denied);
			}
			else if (InventoryManager.GetBackdoorCount() >= device.hackPrice)
			{
				InventoryManager.RemoveBackdoorHack(device.hackPrice);
				BotnetBehaviour.Ins.LaunchKernelCompiler(device);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.Denied);
			}
		}
	}
}
