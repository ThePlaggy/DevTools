using UnityEngine;
using UnityEngine.UI;

public class KeypadManager : MonoBehaviour
{
	public static KeypadManager Ins;

	public int Code;

	private string currentCodeCombo;

	protected bool doorLocked;

	protected GameObject gaypadCanvasRef;

	private float gracePeriodFireWindow;

	private float gracePeriodTimeStamp;

	private AudioHubObject myAho;

	private KeypadLookup myKeypad;

	private Text numpad;

	protected bool own;

	private bool stuck;

	private bool keypadTriggerActive;

	private float keypadFireWindow;

	private float keypadTimeStamp;

	public static bool Locked => Ins != null && Ins.own && (Ins.doorLocked || Ins.isGrace());

	public static bool Owned => Ins != null && Ins.own;

	public string GraceDebug => (gracePeriodFireWindow - (Time.time - gracePeriodTimeStamp) > 0f) ? ((int)(gracePeriodFireWindow - (Time.time - gracePeriodTimeStamp))).ToString() : 0.ToString();

	public string KeypadDebug => (keypadFireWindow - (Time.time - keypadTimeStamp) > 0f) ? ((int)(keypadFireWindow - (Time.time - keypadTimeStamp))).ToString() : 0.ToString();

	private void Awake()
	{
		Ins = this;
		doorLocked = false;
		currentCodeCombo = "";
		stuck = false;
	}

	private void Start()
	{
		LookUp.Doors.MainDoor.DoorOpenEvent.AddListener(DoorOpen);
	}

	private void Update()
	{
		bool flag = false;
		if (keypadTriggerActive && Time.time - keypadTimeStamp >= keypadFireWindow)
		{
			keypadTriggerActive = false;
			DrawNumber();
		}
	}

	private void OnDestroy()
	{
		Ins = null;
		LookUp.Doors.MainDoor.DoorOpenEvent.RemoveListener(DoorOpen);
	}

	protected void DrawGUI()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("MotionSensorButton"));
		gameObject.name = "KeypadID";
		Object.Destroy(gameObject.GetComponent<TopMenuIconBehaviour>());
		Object.Destroy(gameObject.GetComponent<GraphicsRayCasterCatcher>());
		Object.Destroy(gameObject.GetComponent<MouseClickSoundScrub>());
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)Screen.width + 8f - 320f + 45f, -20.5f);
		gameObject.transform.GetChild(0).GetComponent<Image>().sprite = CustomSpriteLookUp.I_Keypad;
		gameObject.transform.GetChild(1).GetComponent<Image>().sprite = CustomSpriteLookUp.I_Keypad;
		if (Themes.selected == THEME.WTTG2BETA)
		{
			gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
		}
		GameObject gameObject2 = Object.Instantiate(GameObject.Find("topClock1"));
		gameObject2.name = "KeypadPass";
		gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2((float)Screen.width + 8f - 320f + 135f, -20f);
		gameObject.transform.SetParent(GameObject.Find("topClock1").transform.parent);
		gameObject2.transform.SetParent(GameObject.Find("topClock1").transform.parent);
		Code = Random.Range(0, 100000);
		numpad = gameObject2.GetComponent<Text>();
		numpad.alignment = TextAnchor.MiddleLeft;
		numpad.text = Code.ToString("D5");
		generateFireWindow();
		myKeypad.Unlock();
		BuildTriggers();
		gaypadCanvasRef = Object.Instantiate(CustomObjectLookUp.gaypadCanvas);
		gaypadCanvasRef.SetActive(value: false);
	}

	private void generateFireWindow()
	{
		keypadTriggerActive = true;
		keypadFireWindow = Random.Range(DifficultyManager.Nightmare ? 150f : 240f, DifficultyManager.Nightmare ? 400f : 600f);
		keypadTimeStamp = Time.time;
	}

	private void DrawNumber()
	{
		Code = Random.Range(0, 100000);
		numpad.text = Code.ToString("D5");
		UnlockIt();
		generateFireWindow();
	}

	public void DoorOpen()
	{
		UnlockIt(0f);
	}

	private void LockIt()
	{
		if (!doorLocked)
		{
			doorLocked = true;
			myAho.PlaySound(CustomSoundLookUp.keypad_Correct);
			myKeypad.Lock();
		}
	}

	public void DevTriggerAction(bool locked)
	{
		if (!(myKeypad == null) && own)
		{
			if (locked)
			{
				LockIt();
			}
			else
			{
				UnlockIt(0f);
			}
		}
	}

	private void BuildTriggers()
	{
		GameObject gameObject = Object.Instantiate(GameObject.Find("BillBoard4"));
		gameObject.transform.position = new Vector3(-1.34f, 40.86f, -4.76f);
		gameObject.transform.localScale = new Vector3(2f, 2f, 2f);
		gameObject.GetComponent<CloseUpTrigger>().SetKeypad(new Vector3(-1.35f, 40.95f, -4.4f));
	}

	public static void ChangekeypadRCState(bool state)
	{
		GameManager.TimeSlinger.FireTimer(0.5f, delegate
		{
			if (Ins != null)
			{
				Ins.gaypadCanvasRef.SetActive(state);
			}
		});
	}

	public void ButtonAction(int id)
	{
		if (!doorLocked && !stuck)
		{
			if (id == -1)
			{
				CheckCode();
			}
			else if (currentCodeCombo.Length < 5)
			{
				myAho.PlaySound(CustomSoundLookUp.keypad_Click);
				currentCodeCombo += id;
				myKeypad.displayText.text = currentCodeCombo;
			}
		}
	}

	public void CheckCode()
	{
		if (currentCodeCombo.Length <= 0)
		{
			return;
		}
		if (LookUp.Doors.MainDoor.IsOpen)
		{
			myAho.PlaySound(CustomSoundLookUp.keypad_Wrong);
			currentCodeCombo = "";
			myKeypad.displayText.text = "DOOR OPEN";
			stuck = true;
			GameManager.TimeSlinger.FireTimer(1.5f, delegate
			{
				stuck = false;
				myKeypad.displayText.text = "UNLOCKED";
			});
		}
		else if (Code.ToString("D5") == currentCodeCombo)
		{
			LockIt();
			currentCodeCombo = "";
		}
		else
		{
			myAho.PlaySound(CustomSoundLookUp.keypad_Wrong);
			currentCodeCombo = "";
			myKeypad.displayText.text = "ERROR";
			stuck = true;
			GameManager.TimeSlinger.FireTimer(1.5f, delegate
			{
				stuck = false;
				myKeypad.displayText.text = "UNLOCKED";
			});
		}
	}

	public void DevChangeCode()
	{
		Code = Random.Range(0, 100000);
		numpad.text = Code.ToString("D5");
		generateFireWindow();
		UnlockIt();
	}

	public void DevChangeCode(int selectedCode)
	{
		if (own)
		{
			if (selectedCode < 0 || selectedCode > 99999)
			{
				DevChangeCode();
				return;
			}
			Code = selectedCode;
			numpad.text = Code.ToString("D5");
			generateFireWindow();
			UnlockIt();
		}
	}

	public static void BuyKeypad()
	{
		if (Ins != null)
		{
			Ins.PlayerBoughtKeypad();
		}
	}

	protected void PlayerBoughtKeypad()
	{
		if (!own)
		{
			own = true;
			GameObject gameObject = Object.Instantiate(CustomObjectLookUp.SecksDevKeypad);
			gameObject.transform.position = new Vector3(-1.34f, 40.86f, -4.86f);
			gameObject.transform.rotation = Quaternion.Euler(0f, 270f, 0f);
			gameObject.transform.localScale = new Vector3(0.125f, 0.125f, 0.125f);
			myKeypad = KeypadLookup.Ins;
			myAho = new GameObject("KeypadAHO").AddComponent<AudioHubObject>();
			myAho.transform.SetParent(myKeypad.transform);
			myAho.transform.localPosition = Vector3.zero;
			DrawGUI();
		}
	}

	public static void EnemyChangedKeypad()
	{
		if (Owned)
		{
			Ins.EnemyInterf();
		}
	}

	protected void EnemyInterf()
	{
		UnlockIt(0f);
	}

	protected bool isGrace()
	{
		return (int)(gracePeriodFireWindow - (Time.time - gracePeriodTimeStamp)) > 0;
	}

	private void UnlockIt(float grace = 10f)
	{
		if (doorLocked)
		{
			gracePeriodFireWindow = grace;
			gracePeriodTimeStamp = Time.time;
			doorLocked = false;
			myAho.PlaySound(CustomSoundLookUp.keypad_Unlock);
			myKeypad.Unlock();
		}
	}
}
