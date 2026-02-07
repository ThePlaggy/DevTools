using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EasterEggManager : MonoBehaviour
{
	public static EasterEggManager Ins;

	[HideInInspector]
	public List<EasterEggTrigger> eggs = new List<EasterEggTrigger>();

	public const int AMOUNT_EGGS = 50;

	public int TOTAL_EGGS = 250;

	private bool Locked;

	public static int EasterEggsLeft;

	public static bool EventCompleted;

	public Text eggCounter;

	private void Awake()
	{
		Ins = this;
		Locked = false;
	}

	private void Start()
	{
		Object.Instantiate(CustomObjectLookUp.EggHolder);
		TOTAL_EGGS = CustomObjectLookUp.EggHolder.transform.childCount;
		BuildCounter();
	}

	private void BuildCounter()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("UI/UIComputer/DesktopUI/TopBar/TopLeftIconHolder/BackDoorIMG"), GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder").transform);
		gameObject.transform.position = new Vector3(273f, -8f, 19f);
		gameObject.GetComponent<Image>().sprite = CustomSpriteLookUp.eggIcon;
		GameObject gameObject2 = Object.Instantiate(GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder/CurrentBackDoors"), GameObject.Find("DesktopUI/TopBar/TopLeftIconHolder").transform);
		gameObject2.transform.position = new Vector3(327f, -20.5f, 19f);
		gameObject2.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 25f);
		Object.Destroy(gameObject2.GetComponent<backdoorTextHook>());
		eggCounter = gameObject2.GetComponent<Text>();
		eggCounter.text = "0/50";
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public void AddEgg(EasterEggTrigger EET)
	{
		if (Locked)
		{
			Debug.LogError("Easter Eggs array out of bounds");
			return;
		}
		eggs.Add(EET);
		if (eggs.Count == TOTAL_EGGS)
		{
			Locked = true;
			ShuffleList(eggs);
			InstantiateEggs();
		}
	}

	private void InstantiateEggs()
	{
		for (int i = 0; i < 50; i++)
		{
			EasterEggTrigger easterEggTrigger = eggs[i];
			easterEggTrigger.myInteractionHook.ForceLock = false;
			easterEggTrigger.mesh.enabled = true;
			bool flag = false;
		}
		for (int j = 50; j < TOTAL_EGGS; j++)
		{
			Object.Destroy(eggs[j].transform.parent.gameObject);
		}
		eggs.Clear();
	}

	private void ShuffleList<T>(IList<T> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = Random.Range(0, num + 1);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
