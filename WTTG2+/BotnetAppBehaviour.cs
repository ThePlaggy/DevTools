using UnityEngine;

public class BotnetAppBehaviour : WindowBehaviour
{
	public static bool OwnsBotnet;

	public static bool AppIsLaunched;

	public static BotnetAppBehaviour Ins;

	private void OnDisable()
	{
		Ins = null;
	}

	protected override void OnLaunch()
	{
		AppIsLaunched = true;
	}

	protected override void OnClose()
	{
		AppIsLaunched = false;
	}

	protected override void OnMin()
	{
	}

	protected override void OnUnMin()
	{
	}

	protected override void OnMax()
	{
	}

	protected override void OnUnMax()
	{
	}

	protected override void OnResized()
	{
	}

	public void InstalledApp()
	{
		OwnsBotnet = true;
		AppIsLaunched = false;
		AppCreator.BotNetAppIcon.SetActive(value: true);
		GameManager.TimeSlinger.FireTimer(Random.Range(800f, 1200f), BotnetBehaviour.Ins.AddNewDevice);
	}
}
