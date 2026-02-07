using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NoirTunnelBehaviour : WindowBehaviour
{
	[SerializeField]
	private Color defaultCostColor;

	[SerializeField]
	private Color notEnoughCostColor;

	[SerializeField]
	private CanvasGroup hackerOverlayCG;

	[SerializeField]
	private AudioFileDefinition showLocationSFX;

	private Text cost1;

	private Text cost2;

	private InputField masterKeyText;

	private bool notEnoughFired;

	private TerminalLineObject termLine1;

	private TerminalLineObject termLine2;

	private TerminalLineObject termLine3;

	private Button unlockButton;

	protected new void Awake()
	{
		masterKeyText = LookUp.DesktopUI.NOIR_TUNNEL_MASTER_KEY_INPUT;
		unlockButton = LookUp.DesktopUI.NOIR_TUNNEL_UNLOCK_BUTTON;
		cost1 = LookUp.DesktopUI.NOIR_TUNNEL_COST1;
		cost2 = LookUp.DesktopUI.NOIR_TUNNEL_COST2;
		unlockButton.onClick.AddListener(veirfyKey);
		if (Themes.selected <= THEME.TWR)
		{
			ColorBlock colors = unlockButton.colors;
			colors.normalColor = new Color(0.326f, 0.706f, 1f);
			colors.highlightedColor = new Color(0.306f, 0.581f, 0.9f);
			colors.pressedColor = new Color(0.126f, 0.411f, 0.7f);
			colors.disabledColor = new Color(0f, 0.3f, 0.5f);
			unlockButton.colors = colors;
		}
		cost1.text = 250f.ToString();
		cost2.text = cost1.text;
		masterKeyText.characterLimit = 138;
		cost1.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 40f);
		cost2.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 40f);
		base.Awake();
	}

	protected new void OnDestroy()
	{
		unlockButton.onClick.RemoveListener(veirfyKey);
		base.OnDestroy();
	}

	protected override void OnLaunch()
	{
	}

	protected override void OnClose()
	{
		masterKeyText.text = string.Empty;
	}

	protected override void OnMax()
	{
	}

	protected override void OnMin()
	{
	}

	protected override void OnResized()
	{
	}

	protected override void OnUnMax()
	{
	}

	protected override void OnUnMin()
	{
	}

	private void veirfyKey()
	{
		if (masterKeyText.text.Trim().Equals(GameManager.TheCloud.MasterKey))
		{
			if (DOSCoinsCurrencyManager.CurrentCurrency >= 250f)
			{
				DOSCoinsCurrencyManager.RemoveCurrency(250f);
				if (!DifficultyManager.Nightmare)
				{
					EnemyManager.CultManager.StageEndJump();
				}
				else
				{
					LookUp.Doors.Door1.NightmareLock();
					LookUp.Doors.Door3.NightmareLock();
					LookUp.Doors.Door5.NightmareLock();
					LookUp.Doors.Door6.NightmareLock();
					LookUp.Doors.Door8.NightmareLock();
					LookUp.Doors.Door10.NightmareLock();
				}
				EnemyStateManager.LockEnemyState(STATE_LOCK_OCCASION.GAME_END);
				DataManager.LockSave = true;
				PowerStateManager.AddPowerStateLock(STATE_LOCK_OCCASION.GAME_END);
				HackerManager.GameEnded = true;
				presentLocation();
			}
			else if (!notEnoughFired)
			{
				notEnoughFired = true;
				GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.CantBuyItem);
				cost1.color = notEnoughCostColor;
				GameManager.TimeSlinger.FireTimer(2f, delegate
				{
					cost1.color = defaultCostColor;
					notEnoughFired = false;
				});
			}
		}
		else
		{
			masterKeyText.text = string.Empty;
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.CantBuyItem);
		}
	}

	private void presentLocation()
	{
		if (DifficultyManager.CasualMode)
		{
			SceneManager.LoadScene("casual");
		}
		else if (!DifficultyManager.Nightmare)
		{
			GameManager.AudioSlinger.MuteAudioLayer(AUDIO_LAYER.WEBSITE);
			LookUp.DesktopUI.DesktopGraphicRaycaster.enabled = false;
			GameManager.AudioSlinger.PlaySound(showLocationSFX);
			hackerOverlayCG.blocksRaycasts = true;
			hackerOverlayCG.ignoreParentGroups = true;
			computerController.Ins.SetMasterLock(setLock: true);
			ComputerCameraManager.Ins.TriggerShowEndLocation();
			GameManager.TimeSlinger.FireTimer(3.98f, delegate
			{
				ComputerCameraManager.Ins.ClearPostFXs();
				hackerOverlayCG.alpha = 1f;
				GameManager.AudioSlinger.PlaySound(GameManager.HackerManager.HackingTypeSFX);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 1.5f);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 2.3f);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 2.9f);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 5.5f);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 6.1f);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 6.7f);
				GameManager.AudioSlinger.PlaySoundWithCustomDelay(GameManager.HackerManager.HackingTypeSFX, 7.3f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine1, TERMINAL_LINE_TYPE.TYPE, "> ./NOIRTUNNEL", 0.2f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine2, TERMINAL_LINE_TYPE.TYPE, "  Loading NOIRTUNNEL v2.15", 0.2f, 0.2f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(out termLine3, TERMINAL_LINE_TYPE.TYPE, "  Locating...", 0.2f, 0.4f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty, 0.5f, 0.5f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "  Location Found!", 0.6f, 1.5f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty, 2.2f, 0.1f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "  Lat: 41.064282", 0.6f, 2.3f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "  Lon: -71.877133", 0.6f, 2.9f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty, 3.5f, 0.1f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.HARD, string.Empty, 3.5f, 0.1f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, "  Incoming Message...", 0.6f, 5.5f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, ">  Hey it's Adam.", 0.6f, 6.1f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, ">  You found the location! Excellent work!", 0.6f, 6.7f);
				GameManager.HackerManager.HackingTerminal.TerminalHelper.AddLine(TERMINAL_LINE_TYPE.TYPE, ">  Head there now by exiting the lobby.", 0.6f, 7.3f);
			});
			GameManager.TimeSlinger.FireTimer(12f, delegate
			{
				computerController.Ins.SetMasterLock(setLock: false);
			});
		}
		else
		{
			masterKeyText.text = "GG :-)";
			GameManager.TheCloud.ForceInsanityEnding();
		}
	}
}
