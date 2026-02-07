using UnityEngine;

public class RouterBehaviour : MonoBehaviour
{
	public static RouterBehaviour Ins;

	[HideInInspector]
	public bool Owned;

	[HideInInspector]
	public bool RouterIsActive;

	[SerializeField]
	private Material matOn;

	[SerializeField]
	private Material matOff;

	[HideInInspector]
	public AudioFileDefinition onSFX;

	[HideInInspector]
	public AudioFileDefinition offSFX;

	[SerializeField]
	private MeshRenderer myMeshRenderer;

	[SerializeField]
	private InteractionHook myInteractionHook;

	[SerializeField]
	private AudioHubObject myAudioHub;

	[SerializeField]
	private Material matOnLY2;

	[SerializeField]
	private Material matOnLY3;

	[SerializeField]
	private Material matOnLY4;

	[SerializeField]
	private Material matOnAll;

	[SerializeField]
	private Material matReset;

	[HideInInspector]
	public int routerHubSwitch;

	[HideInInspector]
	public bool IsJammed;

	[SerializeField]
	private Material matOnLY1JJ;

	[SerializeField]
	private Material matOnLY2JJ;

	[SerializeField]
	private Material matOnLY3JJ;

	[SerializeField]
	private Material matOnLY4JJ;

	[SerializeField]
	private Material matOnAllJJ;

	private Material matOn1;

	private Material matOn1234;

	private Material matOn2;

	private Material matOn3;

	private Material matOn4;

	private bool RouterLocked;

	private bool triggerActive;

	private float triggerFireWindow;

	private float triggerTimeStamp;

	public static bool HaveRouterDOSDrainerCheck
	{
		get
		{
			if (Ins == null)
			{
				return false;
			}
			return Ins.Owned && Ins.RouterIsActive && !Ins.IsJammed;
		}
	}

	public string RouterDebug
	{
		get
		{
			if (triggerFireWindow - (Time.time - triggerTimeStamp) > 0f)
			{
				return ((int)(triggerFireWindow - (Time.time - triggerTimeStamp))).ToString();
			}
			return 0.ToString();
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= leftClickAction;
		myInteractionHook.RightClickAction -= rightClickAction;
		Object.Destroy(onSFX);
		Object.Destroy(offSFX);
	}

	public void SoftBuild()
	{
		Ins = this;
		onSFX = Object.Instantiate(PoliceScannerBehaviour.Ins.onSFX);
		offSFX = Object.Instantiate(PoliceScannerBehaviour.Ins.offSFX);
		onSFX.MyAudioHub = AUDIO_HUB.PLAYER_HUB;
		onSFX.MyAudioLayer = AUDIO_LAYER.PLAYER;
		offSFX.MyAudioHub = AUDIO_HUB.PLAYER_HUB;
		offSFX.MyAudioLayer = AUDIO_LAYER.PLAYER;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		myMeshRenderer.enabled = false;
		myInteractionHook.LeftClickAction += leftClickAction;
		myInteractionHook.RightClickAction += rightClickAction;
		myMeshRenderer.material = matReset;
		routerHubSwitch = 0;
		matOn1 = matOn;
		matOn2 = matOnLY2;
		matOn3 = matOnLY3;
		matOn4 = matOnLY4;
		matOn1234 = matOnAll;
	}

	private void leftClickAction()
	{
		toggleRouter();
	}

	private void toggleRouter()
	{
		if (RouterLocked)
		{
			return;
		}
		if (RouterIsActive && routerHubSwitch == 4)
		{
			routerHubSwitch = 0;
			RouterIsActive = false;
			if (GameManager.ManagerSlinger.WifiManager.IsOnline)
			{
				GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
			}
			else
			{
				WifiMenuBehaviour.Ins.refreshNetworks();
			}
			myMeshRenderer.material = matReset;
			myAudioHub.PlaySound(offSFX);
			return;
		}
		RouterIsActive = true;
		routerHubSwitch++;
		JamTheRouterSilent();
		if (GameManager.ManagerSlinger.WifiManager.IsOnline)
		{
			GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
		}
		else
		{
			WifiMenuBehaviour.Ins.refreshNetworks();
		}
		switch (routerHubSwitch)
		{
		case 1:
			myMeshRenderer.material = matOn;
			break;
		case 2:
			myMeshRenderer.material = matOnLY2;
			break;
		case 3:
			myMeshRenderer.material = matOnLY3;
			break;
		case 4:
			myMeshRenderer.material = matOnLY4;
			break;
		default:
			myMeshRenderer.material = matOnAll;
			break;
		}
		myAudioHub.PlaySound(onSFX);
	}

	public void MoveMe(Vector3 SetPOS, Vector3 SetROT, Vector3 SetSCL)
	{
		Owned = true;
		myMeshRenderer.enabled = true;
		RouterIsActive = false;
		myMeshRenderer.material = matReset;
		routerHubSwitch = 0;
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
		base.transform.localScale = SetSCL;
		generateFireWindow();
		bool flag = true;
		AppCreator.RouterDocIcon.SetActive(value: true);
	}

	private void generateFireWindow(bool small = false)
	{
		triggerActive = true;
		triggerFireWindow = (small ? Random.Range(120f, 300f) : Random.Range(900f, 1800f));
		triggerTimeStamp = Time.time;
	}

	private void Update()
	{
		if (triggerActive && Time.time - triggerTimeStamp >= triggerFireWindow)
		{
			triggerActive = false;
			if (GameManager.ManagerSlinger.WifiManager.IsOnline && RouterIsActive)
			{
				JamTheRouter();
			}
			else
			{
				generateFireWindow(small: true);
			}
		}
	}

	public void ShowInteractionIcon()
	{
		UIInteractionManager.Ins.ShowKnob();
		UIInteractionManager.Ins.ShowEnterBraceMode();
		UIInteractionManager.Ins.ShowLeftMouseButtonAction();
		UIInteractionManager.Ins.ShowRightMouseButtonAction();
	}

	public void HideInteractionIcon()
	{
		UIInteractionManager.Ins.HideKnob();
		UIInteractionManager.Ins.HideEnterBraceMode();
		UIInteractionManager.Ins.HideLeftMouseButtonAction();
		UIInteractionManager.Ins.HideRightMouseButtonAction();
	}

	private void rightClickAction()
	{
		if (RouterIsActive && !RouterLocked)
		{
			RestartModem();
		}
	}

	public void RestartModem()
	{
		int num = routerHubSwitch;
		Material material = myMeshRenderer.material;
		RouterIsActive = false;
		routerHubSwitch = 0;
		RouterLocked = true;
		if (GameManager.ManagerSlinger.WifiManager.IsOnline)
		{
			GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
		}
		else
		{
			WifiMenuBehaviour.Ins.refreshNetworks();
		}
		myMeshRenderer.material = matOff;
		myAudioHub.PlaySound(CustomSoundLookUp.routerreset);
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			myMeshRenderer.material = matOn;
		});
		GameManager.TimeSlinger.FireTimer(1f, delegate
		{
			myMeshRenderer.material = matOnLY2;
		});
		GameManager.TimeSlinger.FireTimer(1.5f, delegate
		{
			myMeshRenderer.material = matOnLY3;
		});
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			myMeshRenderer.material = matOnLY4;
		});
		GameManager.TimeSlinger.FireTimer(2.5f, delegate
		{
			myMeshRenderer.material = matOnAll;
		});
		GameManager.TimeSlinger.FireTimer(2.75f, delegate
		{
			RouterSetEmission(191f, 191f, 191f);
		});
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			myMeshRenderer.material = material;
			RouterIsActive = true;
			routerHubSwitch = num;
			RouterLocked = false;
			IsJammed = false;
			RouterSetEmission(191f, 191f, 191f);
			if (GameManager.ManagerSlinger.WifiManager.IsOnline)
			{
				GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
			}
			else
			{
				WifiMenuBehaviour.Ins.refreshNetworks();
			}
		});
	}

	public void trollReset()
	{
		myAudioHub.PlaySound(CustomSoundLookUp.routerjammed);
	}

	public void JamTheRouter()
	{
		if (!IsJammed)
		{
			myAudioHub.PlaySound(CustomSoundLookUp.routerjammed);
			RouterSetEmission(255f, 80f, 0f);
			IsJammed = true;
			if (GameManager.ManagerSlinger.WifiManager.getCurrentWiFi() != null && GameManager.ManagerSlinger.WifiManager.getCurrentWiFi().routerOnlyWiFi)
			{
				GameManager.ManagerSlinger.WifiManager.DisconnectFromWifi();
			}
			WifiMenuBehaviour.Ins.refreshNetworks();
		}
	}

	public void trollReset2()
	{
		myAudioHub.PlaySound(CustomSoundLookUp.routerreset);
	}

	private void RouterSetEmission(float r, float g, float b)
	{
		if (r == 191f)
		{
			matOn = matOn1;
			matOnLY2 = matOn2;
			matOnLY3 = matOn3;
			matOnLY4 = matOn4;
			matOnAll = matOn1234;
		}
		else
		{
			matOn = matOnLY1JJ;
			matOnLY2 = matOnLY2JJ;
			matOnLY3 = matOnLY3JJ;
			matOnLY4 = matOnLY4JJ;
			matOnAll = matOnAllJJ;
		}
		if (b == 0.59197f)
		{
			r *= 0.59197f;
			g *= 0.59197f;
			b *= 0.59197f;
			matOn.SetColor("_EmissionColor", new Color(r, g, b));
			matOnLY2.SetColor("_EmissionColor", new Color(r, g, b));
			matOnLY3.SetColor("_EmissionColor", new Color(r, g, b));
			matOnLY4.SetColor("_EmissionColor", new Color(r, g, b));
			matOnAll.SetColor("_EmissionColor", new Color(r, g, b));
		}
		switch (routerHubSwitch)
		{
		case 1:
			myMeshRenderer.material = matOn;
			break;
		case 2:
			myMeshRenderer.material = matOnLY2;
			break;
		case 3:
			myMeshRenderer.material = matOnLY3;
			break;
		case 4:
			myMeshRenderer.material = matOnLY4;
			break;
		default:
			myMeshRenderer.material = matOnAll;
			break;
		}
	}

	public void JamTheRouterSilent()
	{
		if (!IsJammed)
		{
			RouterSetEmission(255f, 80f, 0f);
			IsJammed = true;
		}
	}
}
