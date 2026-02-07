using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialAnnHook : MonoBehaviour
{
	public static TutorialAnnHook Ins;

	[SerializeField]
	private GameObject annWindow;

	[SerializeField]
	private GameObject amHolder;

	[SerializeField]
	private GameObject browserHolder;

	private RectTransform annWindowRT;

	public static bool YAAIRunning;

	public static int NextVector;

	public static float TickToSpawnThings;

	private void Awake()
	{
		Ins = this;
		annWindowRT = annWindow.GetComponent<RectTransform>();
	}

	private void Start()
	{
		TickToSpawnThings = 10f;
	}

	public void ClearAM()
	{
		LookUp.DesktopUI.TUT_BROWSER.SetActive(value: false);
		annWindow.SetActive(value: false);
		amHolder.SetActive(value: false);
		browserHolder.SetActive(value: true);
		GameManager.BehaviourManager.AnnBehaviour.ClearFakeURL();
	}

	public void WTTG1Launch()
	{
		annWindowRT.sizeDelta = new Vector2((float)Screen.width - 410f, (float)Screen.height - 60f - 40f);
	}

	public void StartIdiotANNInit()
	{
		ComputerMuteBehaviour.Ins.trollUnMute();
		YAAIRunning = true;
		NextVector = 0;
		annWindowRT.sizeDelta = new Vector2(512f, 512f);
		ANNIdiot();
		GameManager.TimeSlinger.FireTimer(TickToSpawnThings, SpawnThing);
		if (GameManager.HackerManager.theSwan.isActivatedBefore)
		{
			return;
		}
		GameManager.TimeSlinger.FireTimer(36f, delegate
		{
			if (YAAIRunning)
			{
				GameManager.HackerManager.theSwan.ActivateTheSwan();
			}
		});
	}

	public void KillIdiotANNInit()
	{
		if (YAAIRunning)
		{
			YAAIRunning = false;
			NextVector = 0;
		}
	}

	public static void WTTG1Notes()
	{
		GameObject.Find("WindowHolder/Notes").GetComponent<RectTransform>().sizeDelta = new Vector2(333f, 555f);
	}

	public static void SourceCodeFixer()
	{
		WindowManager.Get(SOFTWARE_PRODUCTS.SOURCE_VIEWER).Window.GetComponent<RectTransform>().sizeDelta = new Vector2(950f, 650f);
	}

	public void ANNIdiot()
	{
		if (YAAIRunning)
		{
			annWindowRT.DOAnchorPos(GetNextVector(), Random.Range(0.5f, 0.75f)).SetEase(Ease.Linear).OnComplete(ANNIdiot);
		}
	}

	public Vector2 GetNextVector()
	{
		float x = 64f;
		float x2 = (float)Screen.width * 0.8f - 150f;
		float y = -64f;
		float y2 = 0f - ((float)Screen.height * 0.5f - 50f);
		Vector2 vector = new Vector2(x, y);
		Vector2 vector2 = new Vector2(x2, y);
		Vector2 vector3 = new Vector2(x2, y2);
		Vector2 vector4 = new Vector2(x, y2);
		Vector2[] array = new Vector2[4] { vector, vector2, vector3, vector4 };
		NextVector += Random.Range(0, 2) + 1;
		if (NextVector > 4)
		{
			NextVector = 1;
		}
		else if (NextVector > 3)
		{
			NextVector = 0;
		}
		return array[NextVector];
	}

	private void SpawnThing()
	{
		if (YAAIRunning)
		{
			GameObject gameObject = new GameObject();
			Image image = gameObject.AddComponent<Image>();
			float num = Random.Range(32f, 48f);
			image.GetComponent<RectTransform>().sizeDelta = new Vector2(num, num);
			Sprite[] array = new Sprite[6]
			{
				CustomSpriteLookUp.bug1,
				CustomSpriteLookUp.bug2,
				CustomSpriteLookUp.bug3,
				CustomSpriteLookUp.bug4,
				CustomSpriteLookUp.bug5,
				CustomSpriteLookUp.bug6
			};
			image.sprite = array[Random.Range(0, array.Length)];
			image.transform.SetParent(GameObject.Find("DesktopUI").transform);
			Vector3 mousePosition = Input.mousePosition;
			image.GetComponent<RectTransform>().localPosition = new Vector2(mousePosition.x, mousePosition.y - (float)Screen.height);
			if (TickToSpawnThings < 1f)
			{
				TickToSpawnThings = 1f;
			}
			TickToSpawnThings -= 0.75f;
			GameManager.TimeSlinger.FireTimer(TickToSpawnThings, SpawnThing);
			GameManager.TimeSlinger.FireTimer(Random.Range(30f, 60f), tempDespawnThing, gameObject);
		}
	}

	private void tempDespawnThing(GameObject thing)
	{
		Object.Destroy(thing);
	}
}
