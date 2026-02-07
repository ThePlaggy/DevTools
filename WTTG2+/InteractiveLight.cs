using UnityEngine;

[RequireComponent(typeof(Light))]
public class InteractiveLight : MonoBehaviour
{
	public Light MyLight;

	public Material[] EmissiveMats = new Material[0];

	public bool SetToOff;

	private bool beforeForceOff;

	private InteractiveLightData myData;

	private int myID;

	private void Awake()
	{
		myID = Mathf.Abs(base.transform.position.GetHashCode());
		MyLight = GetComponent<Light>();
		for (int i = 0; i < EmissiveMats.Length; i++)
		{
			EmissiveMats[i].EnableKeyword("_EMISSION");
		}
		GameManager.StageManager.Stage += stageMe;
	}

	public void TriggerLight()
	{
		SetToOff = !SetToOff;
		MyLight.enabled = !SetToOff;
		myData.LightIsOff = SetToOff;
		for (int i = 0; i < EmissiveMats.Length; i++)
		{
			if (SetToOff)
			{
				EmissiveMats[i].DisableKeyword("_EMISSION");
			}
			else
			{
				EmissiveMats[i].EnableKeyword("_EMISSION");
			}
		}
		DataManager.Save(myData);
	}

	public void ForceOff()
	{
		beforeForceOff = SetToOff;
		myData.LightIsOff = SetToOff;
		SetToOff = true;
		MyLight.enabled = false;
		for (int i = 0; i < EmissiveMats.Length; i++)
		{
			EmissiveMats[i].DisableKeyword("_EMISSION");
		}
		DataManager.Save(myData);
	}

	public void ReturnFromForceOff()
	{
		SetToOff = beforeForceOff;
		myData.LightIsOff = SetToOff;
		if (!SetToOff)
		{
			MyLight.enabled = true;
			for (int i = 0; i < EmissiveMats.Length; i++)
			{
				EmissiveMats[i].EnableKeyword("_EMISSION");
			}
		}
		DataManager.Save(myData);
	}

	private void stageMe()
	{
		myData = DataManager.Load<InteractiveLightData>(myID) ?? new InteractiveLightData(myID)
		{
			LightIsOff = false
		};
		if (myData.LightIsOff)
		{
			SetToOff = false;
			TriggerLight();
		}
		GameManager.StageManager.Stage -= stageMe;
	}
}
