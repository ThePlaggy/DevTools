using DG.Tweening;
using UnityEngine;

public class UIInteractionManager : MonoBehaviour
{
	public static UIInteractionManager Ins;

	private Tweener hideComputer;

	private Tweener hideComputerOn;

	private Tweener hideDollMakerMarker;

	private Tweener hideEBar;

	private Tweener hideEnterBraceMode;

	private Tweener hideHand;

	private Tweener hideHide;

	public Tweener hideHoldMode;

	private Tweener hideKnob;

	private Tweener hideLeap;

	private Tweener hideLeftMouseClick;

	private Tweener hideLightOff;

	private Tweener hideLightOn;

	private Tweener hideLock;

	private Tweener hideOpenDoor;

	private Tweener hidePeep;

	private Tweener hidePower;

	private Tweener hideRightMouseClick;

	private Tweener hideSit;

	private Tweener hideUnLock;

	private Tweener showComputer;

	private Tweener showComputerOn;

	private Tweener showDollMakerMarker;

	private Tweener showEBar;

	private Tweener showEnterBraceMode;

	private Tweener showHand;

	private Tweener showHide;

	public Tweener showHoldMode;

	private Tweener showKnob;

	private Tweener showLeap;

	private Tweener showLeftMouseClick;

	private Tweener showLightOff;

	private Tweener showLightOn;

	private Tweener showLock;

	private Tweener showOpenDoor;

	private Tweener showPeep;

	private Tweener showPower;

	private Tweener showRightMouseClick;

	private Tweener showSit;

	private Tweener showUnLock;

	private void Awake()
	{
		Ins = this;
		showLeftMouseClick = DOTween.To(() => new Vector2(-40f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LeftMouseClickTransform.anchoredPosition = x;
		}, new Vector2(-40f, -30f), 0.2f).SetEase(Ease.Linear);
		showLeftMouseClick.SetAutoKill(autoKillOnCompletion: false);
		showLeftMouseClick.Pause();
		hideLeftMouseClick = DOTween.To(() => new Vector2(-40f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LeftMouseClickTransform.anchoredPosition = x;
		}, new Vector2(-40f, 66f), 0.2f).SetEase(Ease.Linear);
		hideLeftMouseClick.SetAutoKill(autoKillOnCompletion: false);
		hideLeftMouseClick.Pause();
		showRightMouseClick = DOTween.To(() => new Vector2(40f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.RightMouseClickTransform.anchoredPosition = x;
		}, new Vector2(40f, -30f), 0.2f).SetEase(Ease.Linear);
		showRightMouseClick.SetAutoKill(autoKillOnCompletion: false);
		showRightMouseClick.Pause();
		hideRightMouseClick = DOTween.To(() => new Vector2(40f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.RightMouseClickTransform.anchoredPosition = x;
		}, new Vector2(40f, 66f), 0.2f).SetEase(Ease.Linear);
		hideRightMouseClick.SetAutoKill(autoKillOnCompletion: false);
		hideRightMouseClick.Pause();
		showOpenDoor = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.OpenDoorTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showOpenDoor.SetAutoKill(autoKillOnCompletion: false);
		showOpenDoor.Pause();
		hideOpenDoor = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.OpenDoorTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideOpenDoor.SetAutoKill(autoKillOnCompletion: false);
		hideOpenDoor.Pause();
		showLock = DOTween.To(() => new Vector2(80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LockTransform.anchoredPosition = x;
		}, new Vector2(80f, -30f), 0.2f).SetEase(Ease.Linear);
		showLock.SetAutoKill(autoKillOnCompletion: false);
		showLock.Pause();
		hideLock = DOTween.To(() => new Vector2(80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LockTransform.anchoredPosition = x;
		}, new Vector2(80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideLock.SetAutoKill(autoKillOnCompletion: false);
		hideLock.Pause();
		showUnLock = DOTween.To(() => new Vector2(80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.UnLockTransform.anchoredPosition = x;
		}, new Vector2(80f, -30f), 0.2f).SetEase(Ease.Linear);
		showUnLock.SetAutoKill(autoKillOnCompletion: false);
		showUnLock.Pause();
		hideUnLock = DOTween.To(() => new Vector2(80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.UnLockTransform.anchoredPosition = x;
		}, new Vector2(80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideUnLock.SetAutoKill(autoKillOnCompletion: false);
		hideUnLock.Pause();
		showLightOn = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LightOnTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showLightOn.SetAutoKill(autoKillOnCompletion: false);
		showLightOn.Pause();
		hideLightOn = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LightOnTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideLightOn.SetAutoKill(autoKillOnCompletion: false);
		hideLightOn.Pause();
		showLightOff = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LightOffTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showLightOff.SetAutoKill(autoKillOnCompletion: false);
		showLightOff.Pause();
		hideLightOff = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LightOffTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideLightOff.SetAutoKill(autoKillOnCompletion: false);
		hideLightOff.Pause();
		showPeep = DOTween.To(() => new Vector2(-86f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.PeepEyeTransform.anchoredPosition = x;
		}, new Vector2(-86f, -30f), 0.2f).SetEase(Ease.Linear);
		showPeep.SetAutoKill(autoKillOnCompletion: false);
		showPeep.Pause();
		hidePeep = DOTween.To(() => new Vector2(-86f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.PeepEyeTransform.anchoredPosition = x;
		}, new Vector2(-86f, 66f), 0.2f).SetEase(Ease.Linear);
		hidePeep.SetAutoKill(autoKillOnCompletion: false);
		hidePeep.Pause();
		showLeap = DOTween.To(() => new Vector2(-86f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LeapTransform.anchoredPosition = x;
		}, new Vector2(-86f, -30f), 0.2f).SetEase(Ease.Linear);
		showLeap.SetAutoKill(autoKillOnCompletion: false);
		showLeap.Pause();
		hideLeap = DOTween.To(() => new Vector2(-86f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.LeapTransform.anchoredPosition = x;
		}, new Vector2(-86f, 66f), 0.2f).SetEase(Ease.Linear);
		hideLeap.SetAutoKill(autoKillOnCompletion: false);
		hideLeap.Pause();
		showHand = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.HandTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showHand.SetAutoKill(autoKillOnCompletion: false);
		showHand.Pause();
		hideHand = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.HandTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideHand.SetAutoKill(autoKillOnCompletion: false);
		hideHand.Pause();
		showHide = DOTween.To(() => new Vector2(-84f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.HideTransform.anchoredPosition = x;
		}, new Vector2(-84f, -30f), 0.2f).SetEase(Ease.Linear);
		showHide.SetAutoKill(autoKillOnCompletion: false);
		showHide.Pause();
		hideHide = DOTween.To(() => new Vector2(-84f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.HideTransform.anchoredPosition = x;
		}, new Vector2(-84f, 66f), 0.2f).SetEase(Ease.Linear);
		hideHide.SetAutoKill(autoKillOnCompletion: false);
		hideHide.Pause();
		showSit = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.SitTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showSit.SetAutoKill(autoKillOnCompletion: false);
		showSit.Pause();
		hideSit = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.SitTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideSit.SetAutoKill(autoKillOnCompletion: false);
		hideSit.Pause();
		showComputer = DOTween.To(() => new Vector2(-92f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.ComputerTransform.anchoredPosition = x;
		}, new Vector2(-92f, -30f), 0.2f).SetEase(Ease.Linear);
		showComputer.SetAutoKill(autoKillOnCompletion: false);
		showComputer.Pause();
		hideComputer = DOTween.To(() => new Vector2(-92f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.ComputerTransform.anchoredPosition = x;
		}, new Vector2(-92f, 66f), 0.2f).SetEase(Ease.Linear);
		hideComputer.SetAutoKill(autoKillOnCompletion: false);
		hideComputer.Pause();
		showPower = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.PowerTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showPower.SetAutoKill(autoKillOnCompletion: false);
		showPower.Pause();
		hidePower = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.PowerTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hidePower.SetAutoKill(autoKillOnCompletion: false);
		hidePower.Pause();
		showComputerOn = DOTween.To(() => new Vector2(-84f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.ComputerOnTransform.anchoredPosition = x;
		}, new Vector2(-84f, -30f), 0.2f).SetEase(Ease.Linear);
		showComputerOn.SetAutoKill(autoKillOnCompletion: false);
		showComputerOn.Pause();
		hideComputerOn = DOTween.To(() => new Vector2(-84f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.ComputerOnTransform.anchoredPosition = x;
		}, new Vector2(-84f, 66f), 0.2f).SetEase(Ease.Linear);
		hideComputerOn.SetAutoKill(autoKillOnCompletion: false);
		hideComputerOn.Pause();
		showKnob = DOTween.To(() => new Vector2(-92f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.KnobTransform.anchoredPosition = x;
		}, new Vector2(-92f, -30f), 0.2f).SetEase(Ease.Linear);
		showKnob.SetAutoKill(autoKillOnCompletion: false);
		showKnob.Pause();
		hideKnob = DOTween.To(() => new Vector2(-92f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.KnobTransform.anchoredPosition = x;
		}, new Vector2(-92f, 66f), 0.2f).SetEase(Ease.Linear);
		hideKnob.SetAutoKill(autoKillOnCompletion: false);
		hideKnob.Pause();
		showDollMakerMarker = DOTween.To(() => new Vector2(-80f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.DollMakerMarkerTransform.anchoredPosition = x;
		}, new Vector2(-80f, -30f), 0.2f).SetEase(Ease.Linear);
		showDollMakerMarker.SetAutoKill(autoKillOnCompletion: false);
		showDollMakerMarker.Pause();
		hideDollMakerMarker = DOTween.To(() => new Vector2(-80f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.DollMakerMarkerTransform.anchoredPosition = x;
		}, new Vector2(-80f, 66f), 0.2f).SetEase(Ease.Linear);
		hideDollMakerMarker.SetAutoKill(autoKillOnCompletion: false);
		hideDollMakerMarker.Pause();
		showEnterBraceMode = DOTween.To(() => new Vector2(82f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.EnterBraceTransform.anchoredPosition = x;
		}, new Vector2(82f, -30f), 0.2f).SetEase(Ease.Linear);
		showEnterBraceMode.SetAutoKill(autoKillOnCompletion: false);
		showEnterBraceMode.Pause();
		hideEnterBraceMode = DOTween.To(() => new Vector2(82f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.EnterBraceTransform.anchoredPosition = x;
		}, new Vector2(82f, 66f), 0.2f).SetEase(Ease.Linear);
		hideEnterBraceMode.SetAutoKill(autoKillOnCompletion: false);
		hideEnterBraceMode.Pause();
		showHoldMode = DOTween.To(() => new Vector2(-86f, 66f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.HoldTransform.anchoredPosition = x;
		}, new Vector2(-86f, -30f), 0.2f).SetEase(Ease.Linear);
		showHoldMode.SetAutoKill(autoKillOnCompletion: false);
		showHoldMode.Pause();
		hideHoldMode = DOTween.To(() => new Vector2(-86f, -30f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.HoldTransform.anchoredPosition = x;
		}, new Vector2(-86f, 66f), 0.2f).SetEase(Ease.Linear);
		hideHoldMode.SetAutoKill(autoKillOnCompletion: false);
		hideHoldMode.Pause();
		showEBar = DOTween.To(() => Vector2.zero, delegate(Vector2 x)
		{
			LookUp.PlayerUI.EBarTransform.anchoredPosition = x;
		}, new Vector2(-55f, 0f), 0.2f).SetEase(Ease.Linear);
		showEBar.SetAutoKill(autoKillOnCompletion: false);
		showEBar.Pause();
		hideEBar = DOTween.To(() => new Vector2(-55f, 0f), delegate(Vector2 x)
		{
			LookUp.PlayerUI.EBarTransform.anchoredPosition = x;
		}, Vector2.zero, 0.2f).SetEase(Ease.Linear);
		hideEBar.SetAutoKill(autoKillOnCompletion: false);
		hideEBar.Pause();
	}

	public void ShowLeftMouseButtonAction()
	{
		showLeftMouseClick.Restart();
	}

	public void HideLeftMouseButtonAction()
	{
		hideLeftMouseClick.Restart();
	}

	public void ShowRightMouseButtonAction()
	{
		showRightMouseClick.Restart();
	}

	public void HideRightMouseButtonAction()
	{
		hideRightMouseClick.Restart();
	}

	public void ShowOpenDoor()
	{
		showOpenDoor.Restart();
	}

	public void HideOpenDoor()
	{
		hideOpenDoor.Restart();
	}

	public void ShowLock()
	{
		showLock.Restart();
	}

	public void HideLock()
	{
		hideLock.Restart();
	}

	public void ShowUnLock()
	{
		showUnLock.Restart();
	}

	public void HideUnLock()
	{
		hideUnLock.Restart();
	}

	public void ShowLightOn()
	{
		showLightOn.Restart();
	}

	public void HideLightOn()
	{
		hideLightOn.Restart();
	}

	public void ShowLightOff()
	{
		showLightOff.Restart();
	}

	public void HideLightOff()
	{
		hideLightOff.Restart();
	}

	public void ShowPeep()
	{
		showPeep.Restart();
	}

	public void HidePeep()
	{
		hidePeep.Restart();
	}

	public void ShowLeap()
	{
		showLeap.Restart();
	}

	public void HideLeap()
	{
		hideLeap.Restart();
	}

	public void ShowHand()
	{
		showHand.Restart();
	}

	public void HideHand()
	{
		hideHand.Restart();
	}

	public void ShowHide()
	{
		showHide.Restart();
	}

	public void HideHide()
	{
		hideHide.Restart();
	}

	public void ShowSit()
	{
		showSit.Restart();
	}

	public void HideSit()
	{
		hideSit.Restart();
	}

	public void ShowComputer()
	{
		showComputer.Restart();
	}

	public void HideComputer()
	{
		hideComputer.Restart();
	}

	public void ShowPower()
	{
		showPower.Restart();
	}

	public void HidePower()
	{
		hidePower.Restart();
	}

	public void ShowComputerOn()
	{
		showComputerOn.Restart();
	}

	public void HideComputerOn()
	{
		hideComputerOn.Restart();
	}

	public void ShowKnob()
	{
		showKnob.Restart();
	}

	public void HideKnob()
	{
		hideKnob.Restart();
	}

	public void ShowDollMakerMarker()
	{
		showDollMakerMarker.Restart();
	}

	public void HideDollMakerMarker()
	{
		hideDollMakerMarker.Restart();
	}

	public void ShowEnterBraceMode()
	{
		showEnterBraceMode.Restart();
	}

	public void HideEnterBraceMode()
	{
		hideEnterBraceMode.Restart();
	}

	public void ShowHoldMode()
	{
		showHoldMode.Restart();
	}

	public void HideHoldMode()
	{
		hideHoldMode.Restart();
	}

	public void ShowEBar()
	{
		showEBar.Restart();
	}

	public void HideEBar()
	{
		hideEBar.Restart();
	}
}
