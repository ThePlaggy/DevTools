using UnityEngine;

public class CamHookBehaviour : WindowBehaviour
{
	public static bool Interruptions;

	public static bool appLaunched;

	public static bool BoughtCameras;

	public static GameObject cam;

	protected override void OnLaunch()
	{
		if (!appLaunched)
		{
			appLaunched = true;
			SwitchCameraStatus(!Interruptions);
			DelfalcoBehaviour.Ins.CamsOpened();
		}
	}

	protected override void OnClose()
	{
		appLaunched = false;
		SwitchCameraStatus(enabled: false, silently: true);
		DelfalcoBehaviour.Ins.CamsClosed();
	}

	protected override void OnMin()
	{
		Debug.Log("deez nuts");
	}

	protected override void OnUnMin()
	{
		Debug.Log("deez balls");
	}

	protected override void OnMax()
	{
		Debug.Log("deez nuts");
	}

	protected override void OnUnMax()
	{
		Debug.Log("deez nuts");
	}

	protected override void OnResized()
	{
		Debug.Log("deez nuts");
	}

	public static void SwitchCameraStatus(bool enabled, bool silently = false)
	{
		Debug.Log("[CamHook] SecCams enabled = :" + enabled);
		if (AppCreator.SecCamera != null)
		{
			AppCreator.SecCamera.SetActive(enabled);
		}
		if (AppCreator.SignalInterruptionOverlay != null)
		{
			AppCreator.SignalInterruptionOverlay.SetActive(!enabled);
		}
		if (!silently && appLaunched && ComputerPowerHook.Ins.FullyPoweredOn && !CamBatteryController.CamsDed)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.cameraSwitch);
		}
	}

	public static void InstallCamera()
	{
		BoughtCameras = true;
		DelfalcoBehaviour.Ins.BoughtCams();
		AppCreator.SecCamsIconObject.SetActive(value: true);
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.SecCam);
		gameObject.transform.position = new Vector3(-3.85f, 41.3691f, -7.1809f);
		gameObject.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
		gameObject.transform.localScale = new Vector3(5f, 5f, 5f);
		cam = gameObject;
	}
}
