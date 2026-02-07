using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class backdoorTextHook : MonoBehaviour
{
	private Text myText;

	public static backdoorTextHook Ins;

	private void Awake()
	{
		if (!base.gameObject.name.Contains("(Clone)"))
		{
			Ins = this;
			myText = GetComponent<Text>();
			GameManager.StageManager.TheGameIsLive += gameLive;
			InventoryManager.AddedSoftwareProduct.Event += softwareProductWasAdded;
		}
	}

	private void OnDestroy()
	{
		if (!base.gameObject.name.Contains("(Clone)"))
		{
			Ins = null;
			InventoryManager.AddedSoftwareProduct.Event -= softwareProductWasAdded;
		}
	}

	private void softwareProductWasAdded(SOFTWARE_PRODUCTS ProductID)
	{
		switch (ProductID)
		{
		case SOFTWARE_PRODUCTS.SPEED_POWERUP:
			SpeedPoll.MarketEnableManipulator(TWITCH_NET_SPEED.FAST);
			return;
		case SOFTWARE_PRODUCTS.KEY_POWERUP:
			KeyPoll.DevEnableManipulator(KEY_CUE_MODE.ENABLED);
			break;
		case SOFTWARE_PRODUCTS.BACKDOOR:
			InventoryManager.AddBackdoor();
			break;
		}
		myText.text = InventoryManager.GetBackdoorCount().ToString();
		if (BotnetBehaviour.Ins != null)
		{
			BotnetBehaviour.Ins.backdoorCount.text = InventoryManager.GetBackdoorCount().ToString();
		}
	}

	public void BackdoorsWereRemoved()
	{
		myText.text = InventoryManager.GetBackdoorCount().ToString();
		if (BotnetBehaviour.Ins != null)
		{
			BotnetBehaviour.Ins.backdoorCount.text = InventoryManager.GetBackdoorCount().ToString();
		}
	}

	private void gameLive()
	{
		myText.text = InventoryManager.GetBackdoorCount().ToString();
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}
}
