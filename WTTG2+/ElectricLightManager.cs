using UnityEngine;

public class ElectricLightManager : MonoBehaviour
{
	public enum LIGHT_LOC
	{
		BED,
		CLOSET,
		COMPUTER,
		KITCHEN,
		BATHROOM
	}

	public static ElectricLightManager Ins;

	public AudioSource ass;

	public AudioSource[] lightAss;

	public Vector3[] computerLights;

	public Vector3[] closetLights;

	public Vector3[] bedLights;

	public Vector3[] kitchenLights;

	public Vector3[] bathroomLights;

	private bool Busy;

	private void Awake()
	{
		Ins = this;
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void CrashLight(LIGHT_LOC Light)
	{
		base.transform.position = GetLightLoc(Light);
		lightAss[Random.Range(0, lightAss.Length)].Play();
		GameManager.TimeSlinger.FireTimer(0.5f, StageMe, Light);
	}

	private void StageMe(LIGHT_LOC Light)
	{
		base.transform.position = GetLightLoc(Light);
		ass.pitch = Random.Range(0.7f, 1.5f);
		TriggerMe(Light);
	}

	private void TriggerMe(LIGHT_LOC Light)
	{
		ass.gameObject.SetActive(value: true);
		GameManager.TimeSlinger.FireTimer(5f, delegate
		{
			ass.gameObject.SetActive(value: false);
		});
		GameManager.TimeSlinger.FireTimer(Random.Range(80f, 180f), StageMe, Light);
	}

	private Vector3 GetLightLoc(LIGHT_LOC Light)
	{
		switch (Light)
		{
		case LIGHT_LOC.BED:
			GameObject.Find("lightSwitch01").transform.position = new Vector3(0f, -200f, 0f);
			return bedLights[Random.Range(0, bedLights.Length)];
		case LIGHT_LOC.CLOSET:
			GameObject.Find("lightSwitch02").transform.position = new Vector3(0f, -200f, 0f);
			return closetLights[Random.Range(0, closetLights.Length)];
		case LIGHT_LOC.KITCHEN:
			GameObject.Find("lightSwitch03").transform.position = new Vector3(0f, -200f, 0f);
			return kitchenLights[Random.Range(0, kitchenLights.Length)];
		case LIGHT_LOC.BATHROOM:
			GameObject.Find("lightSwitch04").transform.position = new Vector3(0f, -200f, 0f);
			return bathroomLights[Random.Range(0, bathroomLights.Length)];
		case LIGHT_LOC.COMPUTER:
			GameObject.Find("lightSwitch05").transform.position = new Vector3(0f, -200f, 0f);
			return computerLights[Random.Range(0, computerLights.Length)];
		default:
			return Vector3.zero;
		}
	}
}
