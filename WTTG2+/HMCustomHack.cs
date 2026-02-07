using UnityEngine;
using UnityEngine.UI;

public class HMCustomHack : MonoBehaviour
{
	public static bool IsCustomHack;

	public HMCustomNodeHexer nodeHexer;

	public HMCustomStackPusher stackPusher;

	public HMCustomVapeAttack vapeAttack;

	public HMCustomDOSBlocker dosBlocker;

	public HMBTNObject startHackBTN;

	public HMBTNObject closeBTN;

	private CUSTOM_HACK currentHack;

	private void Start()
	{
		startHackBTN.setMyAction(LaunchHack);
	}

	public void LaunchHack()
	{
		switch (currentHack)
		{
		case CUSTOM_HACK.NODE_HEXER:
			HackerModeManager.Ins.LaunchHackFromButton(HACK_TYPE.NODEHEXER, isCustom: true);
			break;
		case CUSTOM_HACK.STACK_PUSHER:
			HackerModeManager.Ins.LaunchHackFromButton(HACK_TYPE.STACKPUSHER, isCustom: true);
			break;
		case CUSTOM_HACK.VAPE_ATTACK:
			HackerModeManager.Ins.LaunchHackFromButton(HACK_TYPE.CLOUDGRID, isCustom: true);
			break;
		case CUSTOM_HACK.DOS_BLOCKER:
			HackerModeManager.Ins.LaunchHackFromButton(HACK_TYPE.DOSBLOCK, isCustom: true);
			break;
		}
	}

	public void Close()
	{
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			switch (currentHack)
			{
			case CUSTOM_HACK.NODE_HEXER:
				nodeHexer.gameObject.SetActive(value: false);
				break;
			case CUSTOM_HACK.STACK_PUSHER:
				stackPusher.gameObject.SetActive(value: false);
				break;
			case CUSTOM_HACK.VAPE_ATTACK:
				vapeAttack.gameObject.SetActive(value: false);
				break;
			case CUSTOM_HACK.DOS_BLOCKER:
				dosBlocker.gameObject.SetActive(value: false);
				break;
			}
		});
	}

	public void PresentHackOptions(CUSTOM_HACK customHack)
	{
		currentHack = customHack;
		switch (currentHack)
		{
		case CUSTOM_HACK.NODE_HEXER:
			nodeHexer.gameObject.SetActive(value: true);
			break;
		case CUSTOM_HACK.STACK_PUSHER:
			stackPusher.gameObject.SetActive(value: true);
			break;
		case CUSTOM_HACK.VAPE_ATTACK:
			vapeAttack.gameObject.SetActive(value: true);
			break;
		case CUSTOM_HACK.DOS_BLOCKER:
			dosBlocker.gameObject.SetActive(value: true);
			break;
		}
	}

	public static Slider AdjustSlider(Slider slider, float min = 0f, float max = 10f, float setValue = -1f)
	{
		if (min >= 0f)
		{
			slider.minValue = min;
		}
		if (max >= 0f)
		{
			slider.maxValue = max;
		}
		if (setValue >= 0f)
		{
			slider.value = setValue;
		}
		return slider;
	}
}
