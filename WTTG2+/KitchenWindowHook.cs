using UnityEngine;

public class KitchenWindowHook : MonoBehaviour
{
	public static KitchenWindowHook Ins;

	[SerializeField]
	private AudioLayerMuffleTrigger patioDoorALMT;

	[SerializeField]
	private Vector3 openPOS;

	[SerializeField]
	private int keysFoundCount = 3;

	public bool isOpen;

	private Vector3 closedPOS;

	private short curKeyCount;

	private void Awake()
	{
		closedPOS = base.transform.localPosition;
		Ins = this;
		GameManager.TheCloud.KeyDiscoveredEvent.Event += keyWasDiscovered;
	}

	public void OpenWindow()
	{
		StateManager.PlayerStateChangeEvents.Event -= OpenWindow;
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			base.transform.localPosition = openPOS;
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.OUTSIDE, 0.25f);
			patioDoorALMT.MuffleAmount = 0.25f;
			isOpen = true;
		}
		else
		{
			StateManager.PlayerStateChangeEvents.Event += OpenWindow;
		}
	}

	private void keyWasDiscovered()
	{
		if (DifficultyManager.CasualMode)
		{
			OpenWindow();
			return;
		}
		curKeyCount++;
		if (curKeyCount == keysFoundCount)
		{
			OpenWindow();
		}
	}

	public void CloseWindow()
	{
		StateManager.PlayerStateChangeEvents.Event -= CloseWindow;
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER)
		{
			base.transform.localPosition = closedPOS;
			GameManager.AudioSlinger.MuffleAudioLayer(AUDIO_LAYER.OUTSIDE, 0f);
			patioDoorALMT.MuffleAmount = 0f;
			isOpen = false;
		}
		else
		{
			StateManager.PlayerStateChangeEvents.Event += CloseWindow;
		}
	}
}
