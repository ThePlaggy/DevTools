using DG.Tweening;
using UnityEngine;

public class VWipeDialog : MonoBehaviour
{
	private bool activeself;

	public CustomEvent VWipeDialogWasPresented = new CustomEvent();

	public CustomEvent VWipeScanRemoveDone = new CustomEvent();

	private void Awake()
	{
		UIDialogManager.VWipeDialog = this;
	}

	private void Start()
	{
		activeself = false;
	}

	private void Update()
	{
	}

	public void Present()
	{
		if (!activeself)
		{
			activeself = true;
			LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.SetActive(value: true);
			VWipeApp.Ins.VirusCleaningContent(0f);
			VWipeApp.Ins.SetToLarge();
		}
	}

	public void PerformScanRemove(int NumOfVir)
	{
		AppCreator.VWipeCloseBTN.SetActive(value: false);
		VWipeApp.Ins.SetButtonLock(value: true);
		VWipeApp.Ins.AlphaButtons(0f);
		VWipeApp.Ins.AlphaVirusCleaningContent(1f);
		VWipeApp.Ins.ResizeToSmall();
		float num = Random.Range(5f, 12f);
		if (VWipeApp.Premium)
		{
			num *= 0.5f;
		}
		DOTween.To(() => LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount, delegate(float x)
		{
			LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount = x;
		}, 1f, num).SetEase(Ease.Linear).OnComplete(delegate
		{
			LookUp.DesktopUI.VWIPE_INFO_TEXT.text = "Oh no! Viruses!";
			LookUp.DesktopUI.VWIPE_VIRUS_FOUND_TEXT.text = "Viruses Found: " + NumOfVir;
			if (NumOfVir > 0)
			{
				float removeTime = Random.Range(6f, 10f) + (float)NumOfVir * Random.Range(3f, 5f);
				if (VWipeApp.Premium)
				{
					removeTime *= 0.75f;
				}
				GameManager.TimeSlinger.FireTimer(1.5f, delegate
				{
					LookUp.DesktopUI.VWIPE_INFO_TEXT.text = "Removing Viruses...";
					LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount = 0f;
					DOTween.To(() => LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount, delegate(float x)
					{
						LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount = x;
					}, 1f, removeTime).SetEase(Ease.Linear).SetDelay(0.5f)
						.OnComplete(delegate
						{
							LookUp.DesktopUI.VWIPE_INFO_TEXT.text = (TheSwanBehaviour.SwanActivatedBefore ? "Not all viruses removed... Can't remove TH3SW4N" : "Removed All Viruses!");
							GameManager.HackerManager.virusManager.ClearVirus();
							if (DOSDrainer.dosDrainerInfectedWiFi())
							{
								GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
							}
							GameManager.TimeSlinger.FireTimer(1.5f, Dismiss);
						});
				});
			}
			else if (DOSDrainer.dosDrainerInfectedWiFi())
			{
				LookUp.DesktopUI.VWIPE_INFO_TEXT.text = "Disconnecting from DOSDrainer infected WiFi...";
				GameManager.TimeSlinger.FireTimer(1f, delegate
				{
					GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
				});
				GameManager.TimeSlinger.FireTimer(3.5f, Dismiss);
			}
			else
			{
				LookUp.DesktopUI.VWIPE_INFO_TEXT.text = (TheSwanBehaviour.SwanActivatedBefore ? "Found TH3SW4N, Can't remove..." : "No Viruses Found!");
				GameManager.TimeSlinger.FireTimer(2.5f, Dismiss);
			}
		});
	}

	public void Dismiss()
	{
		VWipeApp.Ins.AlphaVirusCleaningContent(0f);
		VWipeApp.Ins.ResizeToLarge();
		GameManager.TimeSlinger.FireTimer(0.5f, VWipeApp.Ins.AlphaButtons, 1f);
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			VWipeApp.Ins.SetButtonLock(value: false);
			LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount = 0f;
			LookUp.DesktopUI.VWIPE_VIRUS_FOUND_TEXT.text = "Viruses Found: 0";
			LookUp.DesktopUI.VWIPE_INFO_TEXT.text = "Scanning For Viruses...";
			AppCreator.VWipeCloseBTN.SetActive(value: true);
		});
	}

	public void Close()
	{
		if (activeself)
		{
			activeself = false;
			LookUp.DesktopUI.VWIPE_DIALOG_HOLDER.SetActive(value: false);
		}
	}

	public void PerformScan(int NumOfVir)
	{
		AppCreator.VWipeCloseBTN.SetActive(value: false);
		VWipeApp.Ins.SetButtonLock(value: true);
		VWipeApp.Ins.AlphaButtons(0f);
		VWipeApp.Ins.AlphaVirusCleaningContent(1f);
		VWipeApp.Ins.ResizeToSmall();
		float num = Random.Range(5f, 12f);
		if (VWipeApp.Premium)
		{
			num *= 0.5f;
		}
		DOTween.To(() => LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount, delegate(float x)
		{
			LookUp.DesktopUI.VWIPE_PROGRESS_BAR.fillAmount = x;
		}, 1f, num).SetEase(Ease.Linear).OnComplete(delegate
		{
			if (NumOfVir > 0)
			{
				LookUp.DesktopUI.VWIPE_INFO_TEXT.text = "Oh no! Viruses!";
				LookUp.DesktopUI.VWIPE_VIRUS_FOUND_TEXT.text = "Viruses Found: " + NumOfVir;
				GameManager.TimeSlinger.FireTimer(2.5f, Dismiss);
			}
			else if (DOSDrainer.dosDrainerInfectedWiFi())
			{
				LookUp.DesktopUI.VWIPE_INFO_TEXT.text = "Disconnecting from DOSDrainer infected WiFi...";
				GameManager.TimeSlinger.FireTimer(1f, delegate
				{
					GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
				});
				GameManager.TimeSlinger.FireTimer(3.5f, Dismiss);
			}
			else
			{
				LookUp.DesktopUI.VWIPE_INFO_TEXT.text = (TheSwanBehaviour.SwanActivatedBefore ? "Found TH3SW4N, Can't remove..." : "No Viruses Found!");
				GameManager.TimeSlinger.FireTimer(2.5f, Dismiss);
			}
		});
	}
}
