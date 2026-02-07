using DG.Tweening;
using TMPro;
using UnityEngine;

public class PhoneBehaviour : MonoBehaviour
{
	public static PhoneBehaviour Ins;

	private static AudioFileDefinition chosenAudio;

	public static bool DevCall;

	public static AudioClip DevAudioClip;

	[SerializeField]
	private InteractionHook myInteractionHook;

	[SerializeField]
	private AudioHubObject myAHO;

	[SerializeField]
	private GameObject screen;

	[SerializeField]
	private Renderer screenRender;

	[SerializeField]
	private Renderer callRender;

	[SerializeField]
	private Renderer clockRender;

	[SerializeField]
	private Material idleScreenMat;

	[SerializeField]
	private Material callingScreenMat;

	[SerializeField]
	private Material inCallScreenMat;

	[SerializeField]
	private GameObject callHolder;

	[SerializeField]
	private GameObject clockHolder;

	[SerializeField]
	private TextMeshPro clockText;

	[HideInInspector]
	public AudioFileDefinition vibration;

	[HideInInspector]
	public AudioFileDefinition ringing;

	[HideInInspector]
	public AudioFileDefinition accept;

	[HideInInspector]
	public AudioFileDefinition hangup;

	[HideInInspector]
	public AudioFileDefinition[] breathCalls;

	[HideInInspector]
	public AudioFileDefinition[] talkCalls;

	private float bloomTimeStamp;

	[HideInInspector]
	private PHONE_MODE pHONE_mODE;

	private Timer phoneTimeoutTimer;

	private bool shouldRing;

	private bool shouldVibrate;

	private void Awake()
	{
		Ins = this;
		clockText.alignment = TextAlignmentOptions.Center;
		myInteractionHook.LeftClickAction += leftClickAction;
		myInteractionHook.MultiStatesActive = new PLAYER_STATE[2]
		{
			PLAYER_STATE.DESK,
			PLAYER_STATE.ROAMING
		};
		myInteractionHook.AllowMultiStates = true;
		pHONE_mODE = PHONE_MODE.OFF;
		BuildAudio();
	}

	private void Update()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER) && pHONE_mODE == PHONE_MODE.RINGING)
		{
			ringPhone();
		}
	}

	private void OnDestroy()
	{
		Ins = null;
		myInteractionHook.LeftClickAction -= leftClickAction;
	}

	private void BuildAudio()
	{
		breathCalls = new AudioFileDefinition[2];
		talkCalls = new AudioFileDefinition[5];
		vibration = CustomSoundLookUp.phoneVibrate;
		ringing = CustomSoundLookUp.cellPhoneRing;
		hangup = CustomSoundLookUp.hangUpPhone;
		accept = CustomSoundLookUp.answerPhone;
		breathCalls[0] = CustomSoundLookUp.breather_breathe1;
		breathCalls[1] = CustomSoundLookUp.breather_breathe2;
		talkCalls[0] = CustomSoundLookUp.breather_gettingcloser;
		talkCalls[1] = CustomSoundLookUp.breather_ifoundyou;
		talkCalls[2] = CustomSoundLookUp.breather_imcomingforyou;
		talkCalls[3] = CustomSoundLookUp.breather_knockknock;
		talkCalls[4] = CustomSoundLookUp.breather_rhyme;
		vibration.Volume = 0.3f;
		ringing.Volume = 0.3f;
		hangup.Volume = 0.3f;
		accept.Volume = 0.3f;
		breathCalls[0].Volume = 0.3f;
		breathCalls[1].Volume = 0.3f;
		talkCalls[0].Volume = 0.3f;
		talkCalls[1].Volume = 0.3f;
		talkCalls[2].Volume = 0.3f;
		talkCalls[3].Volume = 0.3f;
		talkCalls[4].Volume = 0.3f;
	}

	private void ringPhone()
	{
		float num = Mathf.PingPong(Time.time - bloomTimeStamp, 0.5f) + 0.5f;
		screenRender.material.SetColor("_EmissionColor", new Color(num, num, num, 1f));
	}

	private void MyPlaySound(AudioFileDefinition definition)
	{
		if (definition == vibration)
		{
			if (shouldVibrate)
			{
				myAHO.PlaySound(definition);
				GameManager.TimeSlinger.FireTimer(3f, MyPlaySound, definition);
			}
		}
		else if (!(definition == ringing))
		{
			myAHO.PlaySound(definition);
		}
		else if (shouldRing)
		{
			myAHO.PlaySound(definition);
			GameManager.TimeSlinger.FireTimer(6f, MyPlaySound, definition);
		}
	}

	private void MyKillSound(AudioFileDefinition definition)
	{
		myAHO.KillSound(definition.AudioClip);
	}

	public void ShowInteractionIcon()
	{
		UIInteractionManager.Ins.ShowPeep();
		UIInteractionManager.Ins.ShowLeftMouseButtonAction();
	}

	public void HideInteractionIcon()
	{
		UIInteractionManager.Ins.HidePeep();
		UIInteractionManager.Ins.HideLeftMouseButtonAction();
	}

	public void AttemptCall()
	{
		if (!DifficultyManager.CasualMode && !TarotVengeance.Killed(ENEMY_STATE.BREATHER))
		{
			if (pHONE_mODE == PHONE_MODE.OFF)
			{
				pHONE_mODE = PHONE_MODE.PREP_CALL;
				shouldVibrate = true;
				MyPlaySound(vibration);
				GameManager.TimeSlinger.FireTimer(1.5f, Call);
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(3f, AttemptCall);
			}
		}
	}

	public void leftClickAction()
	{
		switch (pHONE_mODE)
		{
		case PHONE_MODE.RINGING:
			Answer();
			break;
		case PHONE_MODE.OFF:
			CheckTheTime();
			break;
		case PHONE_MODE.IN_CALL:
			HangUp(hard: true);
			break;
		}
	}

	private void Call()
	{
		if (pHONE_mODE != PHONE_MODE.PREP_CALL)
		{
			return;
		}
		pHONE_mODE = PHONE_MODE.BUSY;
		shouldRing = true;
		MyPlaySound(ringing);
		screen.SetActive(value: true);
		screenRender.material = callingScreenMat;
		screenRender.material.DOColor(Color.white, "_EmissionColor", 0.5f).OnComplete(delegate
		{
			bloomTimeStamp = Time.time + 0.5f;
			pHONE_mODE = PHONE_MODE.RINGING;
			if (BlueWhisperManager.Ins.Owns)
			{
				GameManager.TimeSlinger.FireTimer(2.5f, Answer);
			}
			GameManager.TimeSlinger.FireHardTimer(out phoneTimeoutTimer, 30f, StopCalling);
		});
	}

	private void Answer()
	{
		if (pHONE_mODE == PHONE_MODE.RINGING)
		{
			pHONE_mODE = PHONE_MODE.BUSY;
			shouldVibrate = false;
			shouldRing = false;
			MyKillSound(vibration);
			MyKillSound(ringing);
			MyPlaySound(accept);
			GameManager.TimeSlinger.KillTimer(phoneTimeoutTimer);
			callHolder.SetActive(value: true);
			callRender.material = inCallScreenMat;
			callRender.material.DOColor(new Color(0f, 0f, 0f, 1f), 0.75f);
			callRender.material.DOColor(Color.white, "_EmissionColor", 0.75f).OnComplete(delegate
			{
				BreatherPlayTalk();
				screenRender.material = idleScreenMat;
				pHONE_mODE = PHONE_MODE.IN_CALL;
			});
		}
	}

	private void BreatherPlayTalk()
	{
		if (DevCall && DevAudioClip != null)
		{
			AudioFileDefinition audioFileDefinition = Object.Instantiate(breathCalls[0]);
			audioFileDefinition.AudioClip = DevAudioClip;
			chosenAudio = audioFileDefinition;
		}
		else if (TalkAudio())
		{
			chosenAudio = talkCalls[Random.Range(0, talkCalls.Length)];
		}
		else
		{
			chosenAudio = breathCalls[Random.Range(0, breathCalls.Length)];
		}
		MyPlaySound(chosenAudio);
		GameManager.TimeSlinger.FireTimer(chosenAudio.AudioClip.length + (DevCall ? 0.1f : 0.75f), delegate
		{
			HangUp(hard: false);
		});
	}

	private void HangUp(bool hard)
	{
		if (pHONE_mODE != PHONE_MODE.IN_CALL)
		{
			return;
		}
		pHONE_mODE = PHONE_MODE.BUSY;
		MyPlaySound(hangup);
		MyKillSound(chosenAudio);
		if (hard)
		{
			PlayerIgnoredCall();
		}
		clockHolder.SetActive(value: true);
		clockRender.material.DOColor(new Color(1f, 1f, 1f, 1f), "_FaceColor", 0.75f);
		callRender.material.DOColor(new Color(0f, 0f, 0f, 0f), 0.75f);
		callRender.material.DOColor(Color.black, "_EmissionColor", 0.75f).OnComplete(delegate
		{
			callHolder.SetActive(value: false);
			pHONE_mODE = PHONE_MODE.LOCKED_SCREEN;
			if (hard)
			{
				GameManager.TimeSlinger.FireTimer(3f, ForceClosePhone);
			}
			else
			{
				GameManager.TimeSlinger.FireTimer(3f, ClosePhone);
			}
		});
	}

	private void ClosePhone()
	{
		if (pHONE_mODE == PHONE_MODE.LOCKED_SCREEN)
		{
			pHONE_mODE = PHONE_MODE.BUSY;
			clockRender.material.DOColor(new Color(1f, 1f, 1f, 0f), "_FaceColor", 0.5f);
			screenRender.material.DOColor(Color.black, "_EmissionColor", 0.5f).OnComplete(delegate
			{
				screen.SetActive(value: false);
				clockHolder.SetActive(value: false);
				pHONE_mODE = PHONE_MODE.OFF;
				DespawnMe();
			});
		}
	}

	public static void PlacePhoneObject()
	{
		Object.Instantiate(CustomObjectLookUp.BreatherPhone, new Vector3(3.586f, 40.5581f, -4.258f), Quaternion.Euler(new Vector3(0f, 38f, 0f))).transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
	}

	private void StopCalling()
	{
		if (pHONE_mODE == PHONE_MODE.RINGING)
		{
			Debug.Log("[Phone] Force Stop Calling (30 sec timeout)");
			pHONE_mODE = PHONE_MODE.BUSY;
			shouldVibrate = false;
			shouldRing = false;
			MyKillSound(vibration);
			MyKillSound(ringing);
			MyPlaySound(hangup);
			PlayerIgnoredCall();
			screenRender.material = idleScreenMat;
			clockHolder.SetActive(value: true);
			clockRender.material.DOColor(new Color(1f, 1f, 1f, 1f), "_FaceColor", 0.75f).OnComplete(delegate
			{
				callHolder.SetActive(value: false);
				pHONE_mODE = PHONE_MODE.LOCKED_SCREEN;
				GameManager.TimeSlinger.FireTimer(3f, ForceClosePhone);
			});
		}
	}

	private void ForceClosePhone()
	{
		if (pHONE_mODE == PHONE_MODE.LOCKED_SCREEN)
		{
			pHONE_mODE = PHONE_MODE.BUSY;
			clockRender.material.DOColor(new Color(1f, 1f, 1f, 0f), "_FaceColor", 0.5f);
			screenRender.material.DOColor(Color.black, "_EmissionColor", 0.5f).OnComplete(delegate
			{
				screen.SetActive(value: false);
				clockHolder.SetActive(value: false);
				pHONE_mODE = PHONE_MODE.OFF;
				ForceDespawnMe();
			});
		}
	}

	public void UpdateClock(string time)
	{
		clockText.text = time;
	}

	private void CheckTheTime()
	{
		if (pHONE_mODE == PHONE_MODE.OFF)
		{
			pHONE_mODE = PHONE_MODE.BUSY;
			screen.SetActive(value: true);
			clockHolder.SetActive(value: true);
			screenRender.material = idleScreenMat;
			clockRender.material.DOColor(new Color(1f, 1f, 1f, 1f), "_FaceColor", 0.5f);
			screenRender.material.DOColor(Color.white, "_EmissionColor", 0.5f).OnComplete(delegate
			{
				pHONE_mODE = PHONE_MODE.LOCKED_SCREEN;
				GameManager.TimeSlinger.FireTimer(5f, SoftClosePhone);
			});
		}
	}

	private void SoftClosePhone()
	{
		if (pHONE_mODE == PHONE_MODE.LOCKED_SCREEN)
		{
			pHONE_mODE = PHONE_MODE.BUSY;
			clockRender.material.DOColor(new Color(1f, 1f, 1f, 0f), "_FaceColor", 0.5f);
			screenRender.material.DOColor(Color.black, "_EmissionColor", 0.5f).OnComplete(delegate
			{
				screen.SetActive(value: false);
				clockHolder.SetActive(value: false);
				pHONE_mODE = PHONE_MODE.OFF;
			});
		}
	}

	private bool TalkAudio()
	{
		return PhoneManager.Ins.TalkAudio();
	}

	private void PlayerIgnoredCall()
	{
		PhoneManager.Ins.PlayerIgnoredCall();
	}

	private void DespawnMe()
	{
		DevCall = false;
		DevAudioClip = null;
		PhoneManager.Ins.DespawnMe();
	}

	private void ForceDespawnMe()
	{
		DevCall = false;
		DevAudioClip = null;
		PhoneManager.Ins.ForceDespawnMe();
	}

	public void Lock()
	{
		myInteractionHook.ForceLock = true;
	}

	public void Unlock()
	{
		myInteractionHook.ForceLock = false;
	}
}
