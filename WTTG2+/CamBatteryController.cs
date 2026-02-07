using UnityEngine;

public class CamBatteryController : MonoBehaviour
{
	public static bool CamsDed;

	public static CamBatteryController a;

	private const int BATTERY_DRAIN_RATIO = 4;

	private const int BATTERY_RECHARGE_RATIO = 3;

	public int CamSecks { get; set; }

	private void Awake()
	{
		a = this;
	}

	private void OnDestroy()
	{
		a = null;
	}

	private void Update()
	{
		if (GameManager.StageManager.GameIsLive)
		{
			if (CamSecks <= 30000)
			{
				AppCreator.CamBattery.sprite = CustomSpriteLookUp.BatteryMeter4;
			}
			else if (CamSecks > 30000 && CamSecks <= 60000)
			{
				AppCreator.CamBattery.sprite = CustomSpriteLookUp.BatteryMeter3;
			}
			else if (CamSecks > 60000 && CamSecks <= 90000)
			{
				AppCreator.CamBattery.sprite = CustomSpriteLookUp.BatteryMeter2;
			}
			else if (CamSecks > 90000 && CamSecks <= 120000)
			{
				AppCreator.CamBattery.sprite = CustomSpriteLookUp.BatteryMeter1;
			}
			else if (CamSecks > 120000)
			{
				AppCreator.CamBattery.sprite = CustomSpriteLookUp.BatteryMeter0;
			}
			CamsDed = CamSecks > 135000;
			AppCreator.CamHider2.SetActive(CamsDed);
		}
	}

	private void FixedUpdate()
	{
		if (CamHookBehaviour.appLaunched && ComputerPowerHook.Ins.FullyPoweredOn)
		{
			if (CamSecks <= 135000)
			{
				CamSecks += 4;
			}
		}
		else if (CamSecks > 0)
		{
			CamSecks -= 3;
		}
	}
}
