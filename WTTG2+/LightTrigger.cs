using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(BoxCollider))]
public class LightTrigger : MonoBehaviour
{
	public InteractiveLight[] Lights;

	public AudioFileDefinition SwitchOnSFX;

	public AudioFileDefinition SwitchOffSFX;

	public Transform LightSwitch;

	public Vector3 SwitchOnPOS = Vector3.zero;

	public Vector3 SwitchOffPOS = Vector3.zero;

	private bool amBusy;

	public CustomEvent LightsWentOffEvent = new CustomEvent(2);

	public CustomEvent LightsWentOnEvent = new CustomEvent(2);

	private AudioHubObject myAudioHub;

	private LightTriggerData myData;

	private int myID;

	private InteractionHook myInteractionHook;

	private bool setLights = true;

	public bool LightsAreOn { get; private set; } = true;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		myInteractionHook = GetComponent<InteractionHook>();
		myAudioHub = GetComponent<AudioHubObject>();
		myInteractionHook.LeftClickAction += triggerLights;
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= triggerLights;
	}

	public void HoverCheck()
	{
		if (setLights)
		{
			UIInteractionManager.Ins.ShowLightOff();
		}
		else
		{
			UIInteractionManager.Ins.ShowLightOn();
		}
	}

	public void ExitHoverCheck()
	{
		if (setLights)
		{
			UIInteractionManager.Ins.HideLightOff();
		}
		else
		{
			UIInteractionManager.Ins.HideLightOn();
		}
	}

	public void triggerLights()
	{
		if (HalloweenEvent.LightsLocked || amBusy)
		{
			return;
		}
		myInteractionHook.ForceLock = true;
		amBusy = true;
		LightsAreOn = !LightsAreOn;
		myData.LightsAreOff = !LightsAreOn;
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].TriggerLight();
		}
		if (LightSwitch != null)
		{
			if (LightsAreOn)
			{
				LightsWentOnEvent.Execute();
				LightSwitch.localRotation = Quaternion.Euler(SwitchOnPOS);
				myAudioHub.PlaySound(SwitchOnSFX);
			}
			else
			{
				LightsWentOffEvent.Execute();
				LightSwitch.localRotation = Quaternion.Euler(SwitchOffPOS);
				myAudioHub.PlaySound(SwitchOffSFX);
			}
		}
		DataManager.Save(myData);
		GameManager.TimeSlinger.FireTimer(0.75f, delegate
		{
			setLights = LightsAreOn;
			amBusy = false;
			myInteractionHook.ForceLock = false;
		});
	}

	private void stageMe()
	{
		myData = DataManager.Load<LightTriggerData>(myID);
		if (myData == null)
		{
			myData = new LightTriggerData(myID);
			myData.LightsAreOff = false;
		}
		if (myData.LightsAreOff)
		{
			LightsAreOn = false;
			LightSwitch.localRotation = Quaternion.Euler(SwitchOffPOS);
			LightsWentOffEvent.Execute();
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	public void triggerLightsNoSound()
	{
		if (amBusy)
		{
			return;
		}
		myInteractionHook.ForceLock = true;
		amBusy = true;
		LightsAreOn = !LightsAreOn;
		myData.LightsAreOff = !LightsAreOn;
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].TriggerLight();
		}
		if (LightSwitch != null)
		{
			if (LightsAreOn)
			{
				LightsWentOnEvent.Execute();
				LightSwitch.localRotation = Quaternion.Euler(SwitchOnPOS);
			}
			else
			{
				LightsWentOffEvent.Execute();
				LightSwitch.localRotation = Quaternion.Euler(SwitchOffPOS);
			}
		}
		DataManager.Save(myData);
		GameManager.TimeSlinger.FireTimer(0.75f, delegate
		{
			setLights = LightsAreOn;
			amBusy = false;
			myInteractionHook.ForceLock = false;
		});
	}

	public void KILLLIGHT(int id)
	{
		myInteractionHook.ForceLock = true;
		amBusy = true;
		LightsAreOn = false;
		myData.LightsAreOff = true;
		for (int i = 0; i < Lights.Length; i++)
		{
			Lights[i].TriggerLight();
		}
		myInteractionHook.AllStates = false;
		myInteractionHook.StateActive = PLAYER_STATE.BUSY;
		myInteractionHook.LocationToCheck = PLAYER_LOCATION.UNKNOWN;
		myInteractionHook.RequireLocationCheck = true;
		ElectricLightManager.Ins.CrashLight((ElectricLightManager.LIGHT_LOC)id);
	}
}
