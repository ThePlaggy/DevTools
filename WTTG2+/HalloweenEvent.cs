using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HalloweenEvent : MonoBehaviour
{
	private CandyBoxTrigger CandyBox;

	private int CandyAmount;

	private int PumpkinsLitUp;

	private int TotalCandyPickedup;

	public const float PUMPKIN_LIGHT_IDLE = 0.3f;

	public const float PUMPKIN_LIGHT_ACTIVE = 0.55f;

	public const float MAX_IGNITE = 5f;

	public const int PUMPKINS_COUNT = 6;

	public static bool LightsLocked;

	[HideInInspector]
	public List<Light> pumpkinLights = new List<Light>();

	[HideInInspector]
	public List<AudioSource> pumpkinAss = new List<AudioSource>();

	[HideInInspector]
	public List<ParticleSystem> pumpkinPS = new List<ParticleSystem>();

	private Text halloweenCounter;

	private void Awake()
	{
		GameManager.StageManager.TheGameIsLive += FireEvent;
		CandyAmount = 0;
		CandyBox = Object.Instantiate(CustomObjectLookUp.CandyBox).GetComponent<CandyBoxTrigger>();
		CandyBox.HE = this;
	}

	private void BuildHalloweenCounter()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("UI/UIComputer/DesktopUI/TopBar/TopLeftIconHolder/BackDoorIMG"), GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder").transform);
		gameObject.transform.position = new Vector3(273f, -8f, 19f);
		gameObject.GetComponent<Image>().sprite = CustomSpriteLookUp.pumpkinIcon;
		gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.2f, 0.9f);
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(72f, 72f);
		GameObject gameObject2 = Object.Instantiate(GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder/CurrentBackDoors"), GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder").transform);
		gameObject2.transform.position = new Vector3(327f, -20.5f, 19f);
		gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 25f);
		Object.Destroy(gameObject2.GetComponent<backdoorTextHook>());
		halloweenCounter = gameObject2.GetComponent<Text>();
		halloweenCounter.text = $"0/{6}";
	}

	private void FireEvent()
	{
		GameManager.StageManager.TheGameIsLive -= FireEvent;
		if (EventSlinger.HalloweenEvent)
		{
			BuildHalloweenCounter();
			GameObject gameObject = Object.Instantiate(CustomObjectLookUp.PumpkinJack);
			GameObject gameObject2 = gameObject.transform.Find("PumpkinLight").gameObject;
			gameObject.transform.position = new Vector3(3.9f, 40.515f, -4.62f);
			gameObject.transform.rotation = new Quaternion(0f, 147f, 0f, 25f);
			gameObject.transform.localScale = new Vector3(0.095f, 0.095f, 0.095f);
			gameObject2.transform.localPosition = new Vector3(0f, 0f, 1f);
			Light component = gameObject2.GetComponent<Light>();
			component.intensity = 0.3f;
			component.shadowBias = 0f;
			component.shadowStrength = 0f;
			component.shadows = LightShadows.None;
			pumpkinLights.Add(component);
			int num = 1;
			Object.Instantiate(gameObject, new Vector3(-5.6655f, 40.6187f, -4.62f), Quaternion.Euler(0f, 238.7139f, 0f)).AddComponent<PumpkinTrigger>().PutHE(this, num);
			num++;
			Object.Instantiate(gameObject, new Vector3(-2.2473f, 39.5951f, -1.0764f), Quaternion.Euler(0f, 331.3602f, 0f)).AddComponent<PumpkinTrigger>().PutHE(this, num);
			num++;
			Object.Instantiate(gameObject, new Vector3(-6.06f, 40.0951f, 2.3454f), Quaternion.Euler(0f, 342.942f, 0f)).AddComponent<PumpkinTrigger>().PutHE(this, num);
			num++;
			Object.Instantiate(gameObject, new Vector3(1.3545f, 40.5551f, 2.9454f), Quaternion.Euler(0f, 358.6692f, 0f)).AddComponent<PumpkinTrigger>().PutHE(this, num);
			num++;
			Object.Instantiate(gameObject, new Vector3(5.7f, 40.1f, 1.67f), Quaternion.Euler(0f, 111f, 0f)).AddComponent<PumpkinTrigger>().PutHE(this, num);
			gameObject.AddComponent<PumpkinTrigger>().PutHE(this, 0, PCPumpkin: true);
			GameManager.TimeSlinger.FireTimer(5.5f, delegate
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.WitchLaugh);
				EnemyManager.CultManager.turnOffAllLights();
				LightsLocked = true;
			});
			GameManager.TimeSlinger.FireTimer(Random.Range(888f, 1666f), SpawnCandyBox);
			GameManager.TimeSlinger.FireTimer(20f, MayForceKeyDiscover);
		}
	}

	public void OnPumpkinClicked(PumpkinTrigger PT)
	{
		if (PumpkinsLitUp >= 6 || CandyAmount <= 0)
		{
			return;
		}
		CandyAmount--;
		MayForceKeyDiscover();
		PT.TriggeredMe = true;
		PT.myInteractionHook.ForceLock = true;
		pumpkinLights[PT.myPumpkinID].DOIntensity(0.55f, 1f);
		AudioSource audioSource = PT.gameObject.AddComponent<AudioSource>();
		audioSource.clip = CustomSoundLookUp.fireplace;
		audioSource.volume = 0.25f;
		audioSource.minDistance = 0.1f;
		audioSource.maxDistance = 0.5f;
		audioSource.dopplerLevel = 0f;
		audioSource.spatialBlend = 1f;
		audioSource.loop = true;
		audioSource.Play();
		pumpkinAss.Add(audioSource);
		GameObject gameObject = Object.Instantiate(CustomObjectLookUp.MagicFireOrangeXProMax);
		pumpkinPS.Add(gameObject.GetComponent<ParticleSystem>());
		gameObject.transform.position = PT.transform.position;
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.WoodClick);
		PumpkinsLitUp++;
		halloweenCounter.text = $"{PumpkinsLitUp}/6";
		if (PumpkinsLitUp < 6)
		{
			return;
		}
		AudioSource RCAS = roamController.Ins.gameObject.AddComponent<AudioSource>();
		RCAS.clip = CustomSoundLookUp.fireplace;
		RCAS.volume = 0f;
		RCAS.minDistance = 0.1f;
		RCAS.maxDistance = 0.5f;
		RCAS.dopplerLevel = 0f;
		RCAS.spatialBlend = 1f;
		RCAS.loop = true;
		RCAS.Play();
		GameManager.TimeSlinger.FireTimer(10f, delegate
		{
			LightsLocked = false;
		});
		GameManager.TimeSlinger.FireTimer(14f, EventRewardManager.HalloweenReward);
		for (int num = 0; num < 6; num++)
		{
			RCAS.DOFade(1f, 7.5f);
			pumpkinLights[num].DOIntensity(5f, 10f);
			pumpkinAss[num].DOFade(10f, 10f).OnComplete(delegate
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.ignited);
				for (int i = 0; i < 6; i++)
				{
					RCAS.DOFade(0f, 1.75f);
					pumpkinAss[i].DOFade(0f, 3f);
					pumpkinLights[i].DOIntensity(0f, 2f);
					pumpkinPS[i].Stop();
				}
			});
		}
	}

	private void SpawnCandyBox()
	{
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.owl);
		CandyBox.appearCandybox();
	}

	public void PickupCandyBox()
	{
		CandyAmount++;
		TotalCandyPickedup++;
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.candyPickup);
		if (TotalCandyPickedup < 6)
		{
			GameManager.TimeSlinger.FireTimer(666f, SpawnCandyBox);
		}
	}

	private void MayForceKeyDiscover()
	{
		if (!DifficultyManager.LeetMode && !DifficultyManager.Nightmare)
		{
			GameManager.TheCloud.ForceKeyDiscover();
		}
	}
}
