using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ComputerPowerHook : MonoBehaviour
{
	public static ComputerPowerHook Ins;

	[SerializeField]
	private CanvasGroup powerLabelCG;

	[SerializeField]
	private TextMeshProUGUI powerLabelText;

	[SerializeField]
	private Material computerMaterial;

	private CanvasGroup myCG;

	private ComputerPowerData myData;

	private int myID;

	private GraphicRaycaster myRayCaster;

	public bool PowerOn { get; private set; }

	public bool FullyPoweredOn => !myRayCaster.enabled;

	private void Awake()
	{
		Ins = this;
		myID = base.transform.position.GetHashCode();
		myCG = GetComponent<CanvasGroup>();
		myRayCaster = GetComponent<GraphicRaycaster>();
		myRayCaster.enabled = false;
		GameManager.StageManager.Stage += stageMe;
	}

	public void ShutDownComputer()
	{
		if (GameManager.HackerManager.theSwan.SwanError || TarotManager.InDenial || TutorialAnnHook.YAAIRunning || DifficultyManager.HackerMode)
		{
			return;
		}
		GameManager.AudioSlinger.MuteAudioHub(AUDIO_HUB.COMPUTER_HUB);
		myRayCaster.enabled = true;
		powerLabelText.SetText("Powering Off");
		GameManager.ManagerSlinger.CursorManager.SetOverwrite(setValue: false);
		GameManager.ManagerSlinger.CursorManager.SwitchToDefaultCursor();
		GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
		switchToComputerController.Ins.Lock();
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
		{
			DOTween.To(() => powerLabelCG.alpha, delegate(float x)
			{
				powerLabelCG.alpha = x;
			}, 1f, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
			{
				GameManager.TimeSlinger.FireTimer(3f, shutComputerDown);
			});
		});
	}

	public void PowerComputer()
	{
		PowerOn = true;
		PowerComputerTrigger.Ins.Lock();
		powerLabelCG.alpha = 0f;
		powerLabelText.SetText("Powering On");
		switchToComputerController.Ins.UnLock();
		computerMaterial.EnableKeyword("_EMISSION");
		ComputerScreenHook.Ins.MeshRenderer.enabled = true;
		DOTween.To(() => powerLabelCG.alpha, delegate(float x)
		{
			powerLabelCG.alpha = x;
		}, 1f, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
		{
			GameManager.TimeSlinger.FireTimer(3f, powerComputerOn);
		});
	}

	private void shutComputerDown()
	{
		PowerOn = false;
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			computerController.Ins.LeaveMe();
		}
		computerMaterial.DisableKeyword("_EMISSION");
		ComputerScreenHook.Ins.MeshRenderer.enabled = false;
		PowerComputerTrigger.Ins.UnLock();
		myData.ComputerIsOff = true;
		DataManager.Save(myData);
	}

	private void powerComputerOn()
	{
		powerLabelCG.alpha = 0f;
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 0f, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
		{
			GameManager.AudioSlinger.UnMuteAudioHub(AUDIO_HUB.COMPUTER_HUB);
			myRayCaster.enabled = false;
		});
		myData.ComputerIsOff = false;
		DataManager.Save(myData);
	}

	private void stageMe()
	{
		myData = DataManager.Load<ComputerPowerData>(myID);
		if (myData == null)
		{
			myData = new ComputerPowerData(myID);
			myData.ComputerIsOff = false;
		}
		if (myData.ComputerIsOff)
		{
			PowerOn = false;
			PowerComputerTrigger.Ins.UnLock();
			computerMaterial.DisableKeyword("_EMISSION");
			ComputerScreenHook.Ins.MeshRenderer.enabled = false;
			switchToComputerController.Ins.Lock();
			myCG.alpha = 1f;
			powerLabelCG.alpha = 0f;
			powerLabelText.SetText("Powering On");
		}
		else
		{
			PowerOn = true;
			if (PowerComputerTrigger.Ins != null)
			{
				PowerComputerTrigger.Ins.Lock();
			}
			ComputerScreenHook.Ins.MeshRenderer.enabled = true;
			computerMaterial.EnableKeyword("_EMISSION");
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	public void ShutDownComputerInsantly()
	{
		GameManager.AudioSlinger.MuteAudioHub(AUDIO_HUB.COMPUTER_HUB);
		myRayCaster.enabled = true;
		powerLabelText.SetText("SYSTEM FAILURE");
		GameManager.ManagerSlinger.CursorManager.SetOverwrite(setValue: false);
		GameManager.ManagerSlinger.CursorManager.SwitchToDefaultCursor();
		GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
		switchToComputerController.Ins.Lock();
		DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear).OnComplete(delegate
		{
			DOTween.To(() => powerLabelCG.alpha, delegate(float x)
			{
				powerLabelCG.alpha = x;
			}, 1f, 0.5f).SetEase(Ease.Linear).OnComplete(delegate
			{
				GameManager.TimeSlinger.FireTimer(0.01f, shutComputerDown);
			});
		});
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ShowBack()
	{
		base.gameObject.SetActive(value: true);
	}
}
