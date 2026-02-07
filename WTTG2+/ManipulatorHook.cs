using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ManipulatorHook : MonoBehaviour
{
	public enum MANIPULATOR_TYPE
	{
		IDLE,
		POSITIVE,
		NEGATIVE
	}

	public enum THE_MANIPULATOR
	{
		KEY,
		SPEED
	}

	public static ManipulatorHook Ins;

	public GameObject Holder;

	public Image KeyCueHolder;

	public Image WebBoostHolder;

	private Vector3 KeyCueLoc;

	private Vector3 WebBoostLoc;

	private void Awake()
	{
		Ins = this;
		GameManager.TimeSlinger.FireTimer(1f, stageMe);
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void RemoveBoostManipulator()
	{
		WebBoostHolder.sprite = CustomSpriteLookUp.WebBoostIdle;
		GameManager.TheCloud.speedActive = false;
	}

	public void RemoveKeyManipulator()
	{
		KeyCueHolder.sprite = CustomSpriteLookUp.KeyCueIdle;
		GameManager.TheCloud.keyActive = false;
	}

	public void UpdateBoostManipulator(MANIPULATOR_TYPE Type)
	{
		switch (Type)
		{
		case MANIPULATOR_TYPE.POSITIVE:
			WebBoostHolder.sprite = CustomSpriteLookUp.WebBoostPositive;
			break;
		case MANIPULATOR_TYPE.NEGATIVE:
			WebBoostHolder.sprite = CustomSpriteLookUp.WebBoostNegative;
			break;
		}
		WebBoostHolder.transform.position = new Vector3(WebBoostLoc.x / 2f, WebBoostLoc.y / 2f, 19f);
		WebBoostHolder.transform.localScale = new Vector3(25f, 25f, 25f);
		WebBoostHolder.transform.DOMove(WebBoostLoc, 0.5f);
		WebBoostHolder.transform.DOScale(Vector3.one, 0.5f);
	}

	public void UpdateKeyManipulator(MANIPULATOR_TYPE Type)
	{
		switch (Type)
		{
		case MANIPULATOR_TYPE.POSITIVE:
			KeyCueHolder.sprite = CustomSpriteLookUp.KeyCuePositive;
			break;
		case MANIPULATOR_TYPE.NEGATIVE:
			KeyCueHolder.sprite = CustomSpriteLookUp.KeyCueNegative;
			break;
		}
		KeyCueHolder.transform.position = new Vector3(KeyCueLoc.x / 2f, KeyCueLoc.y / 2f, 19f);
		KeyCueHolder.transform.localScale = new Vector3(25f, 25f, 25f);
		KeyCueHolder.transform.DOMove(KeyCueLoc, 0.5f);
		KeyCueHolder.transform.DOScale(Vector3.one, 0.5f);
	}

	private void stageMe()
	{
		Holder = Object.Instantiate(CustomObjectLookUp.ManipulatorHolder, GameObject.Find("BotBar").transform);
		KeyCueHolder = Holder.transform.Find("KeyCueHolder").GetComponent<Image>();
		WebBoostHolder = Holder.transform.Find("WebBoostHolder").GetComponent<Image>();
		RemoveBoostManipulator();
		RemoveKeyManipulator();
		KeyCueLoc = KeyCueHolder.transform.position;
		WebBoostLoc = WebBoostHolder.transform.position;
		BuildBin();
		OverrideInformation overrideInformation = Holder.AddComponent<OverrideInformation>();
		Holder.AddComponent<GraphicsRayCasterCatcher>();
		overrideInformation.oiw = Object.Instantiate(DifficultyManager.Nightmare ? CustomObjectLookUp.OverrideInfoNM : CustomObjectLookUp.OverrideInfo, Holder.transform).GetComponent<OverrideInformationWindow>();
	}

	private void BuildBin()
	{
		Object.Instantiate(WebBoostHolder, WebBoostHolder.transform, worldPositionStays: true).gameObject.AddComponent<BinBehaviour>();
	}
}
