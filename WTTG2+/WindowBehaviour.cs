using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class WindowBehaviour : MonoBehaviour
{
	public static bool[] DocumentFix = new bool[8];

	public SOFTWARE_PRODUCTS Product;

	public SoftwareProductDefinition UniProductData;

	public GameObject Window;

	[SerializeField]
	private bool canBeMax;

	[SerializeField]
	private MaxWindowBehaviour maxBTN;

	[SerializeField]
	private bool canBeMin;

	protected bool IAmMaxxed;

	protected bool IAmMinned;

	private Vector2 maxedPOS = new Vector2(0f, -41f);

	private Vector2 maxedSize = new Vector2(Screen.width, (float)Screen.height - 41f - 40f);

	private Vector2 minAppWindowSize = new Vector2(335f, 75f);

	private WindowData myData;

	private int myID;

	private RectTransform myRT;

	private Vector2 preMaxPOS;

	private Vector2 preMaxSize;

	private Vector2 preMinWindowPOS;

	private Vector2 preMinWindowSize;

	[NonSerialized]
	private bool appliedTheme;

	protected virtual void Awake()
	{
		if (GetType() == typeof(TheSwanAppBehaviour))
		{
			Debug.Log("[TH3SW4N] Created");
			Product = SOFTWARE_PRODUCTS.THE_SWAN;
			Window = AppCreator.TheSwanAppObject;
			canBeMin = false;
		}
		if (GetType() == typeof(BotnetAppBehaviour))
		{
			Debug.Log("[BOTNET] Created");
			Product = SOFTWARE_PRODUCTS.BOTNET;
			Window = AppCreator.BotnetAppObject;
			canBeMin = false;
			BotnetAppBehaviour.Ins = GetComponent<BotnetAppBehaviour>();
		}
		if (GetType() == typeof(VoIPBehaviour))
		{
			Debug.Log("[VoIP] Created");
			Product = SOFTWARE_PRODUCTS.VOIP;
			Window = AppCreator.VoIPGameObject;
			canBeMin = false;
		}
		if (GetType() == typeof(RouterDocBehaviour))
		{
			Debug.Log("[RouterDoc] Created");
			Product = SOFTWARE_PRODUCTS.ROUTERDOC;
			Window = AppCreator.RouterDoc;
			canBeMin = false;
		}
		if (GetType() == typeof(CamHookBehaviour))
		{
			Debug.Log("[CamHooks] Created");
			Product = SOFTWARE_PRODUCTS.CAMHOOK;
			Window = AppCreator.CamHooks;
			canBeMin = false;
		}
		if (GetType() == typeof(EventPosterBehaviour))
		{
			Debug.Log("[EventPoster] Created");
			Product = SOFTWARE_PRODUCTS.EVENTPOSTER;
			Window = AppCreator.EventPosterObject;
			canBeMin = false;
		}
		if (GetType() == typeof(doorlogBehaviour))
		{
			Debug.Log("[doorLOG] Created");
			Product = SOFTWARE_PRODUCTS.DOORLOG;
			Window = AppCreator.doorlogApp;
			canBeMin = false;
			doorlogBehaviour.Ins = GetComponent<doorlogBehaviour>();
		}
		else if (Product == SOFTWARE_PRODUCTS.UNIVERSAL)
		{
			myID = UniProductData.GetHashCode();
		}
		else
		{
			myID = (int)Product;
		}
		myRT = Window.GetComponent<RectTransform>();
		WindowManager.Add(this);
		GameManager.StageManager.Stage += stageMe;
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected abstract void OnLaunch();

	protected abstract void OnClose();

	protected abstract void OnMin();

	protected abstract void OnUnMin();

	protected abstract void OnMax();

	protected abstract void OnUnMax();

	protected abstract void OnResized();

	public void Launch()
	{
		OnLaunch();
		if (!Window.activeSelf)
		{
			Window.SetActive(value: true);
			Window.GetComponent<BringWindowToFrontBehaviour>().forceTap();
			if (Themes.selected <= THEME.TWR && Product != SOFTWARE_PRODUCTS.ROUTERDOC && !appliedTheme)
			{
				switch (Product)
				{
				case SOFTWARE_PRODUCTS.ANN:
					Window.GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.annapp;
					break;
				case SOFTWARE_PRODUCTS.SKYBREAK:
				case SOFTWARE_PRODUCTS.DOORLOG:
					Window.GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.baseapptrans;
					break;
				case SOFTWARE_PRODUCTS.BOTNET:
					Window.GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.botnetapp;
					break;
				default:
					Window.GetComponent<Image>().sprite = ThemesLookUp.WTTG1TWR.baseapp;
					break;
				}
				appliedTheme = true;
			}
			if (Product == SOFTWARE_PRODUCTS.ANN && !TheCloud.InitANN)
			{
				TheCloud.InitANN = true;
				TutorialAnnHook.Ins.WTTG1Launch();
			}
			else if (Product == SOFTWARE_PRODUCTS.SKYBREAK && !TheCloud.InitSkybreak)
			{
				TheCloud.InitSkybreak = true;
				if (Themes.selected <= THEME.TWR)
				{
					Scrollbar component = GameObject.Find("skyBREAK/terminalWindowHolder/TerminalContentHolder").transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
					ColorBlock colors = component.colors;
					colors.normalColor = new Color(0.326f, 0.706f, 1f);
					colors.highlightedColor = new Color(0.306f, 0.581f, 0.9f);
					colors.pressedColor = new Color(0.126f, 0.411f, 0.7f);
					colors.disabledColor = new Color(0f, 0.3f, 0.5f);
					component.colors = colors;
				}
			}
			else if (Product == SOFTWARE_PRODUCTS.NOTES && !TheCloud.InitNotes)
			{
				TheCloud.InitNotes = true;
				TutorialAnnHook.WTTG1Notes();
			}
			else if (Product == SOFTWARE_PRODUCTS.SOURCE_VIEWER && !TheCloud.InitSource)
			{
				TheCloud.InitSource = true;
				TutorialAnnHook.SourceCodeFixer();
			}
			else if (Product == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				if (UniProductData != null)
				{
					if (UniProductData.ProductTitle == "ZONEWALL")
					{
						UniProductData.MinProductTitle = "Tanner";
						SetWindowTitle("Tanner");
					}
					if (UniProductData.ProductTitle == "N0D3H3X3R")
					{
						UniProductData.MinProductTitle = "Kidnapper";
						SetWindowTitle("Kidnapper");
					}
					if (UniProductData.ProductTitle == "memD3FR4G3R")
					{
						UniProductData.MinProductTitle = "Hacks";
						SetWindowTitle("Hacks");
					}
					if (UniProductData.ProductTitle == "stackPUSHER")
					{
						UniProductData.MinProductTitle = "Viruses";
						SetWindowTitle("Viruses");
					}
				}
				if (UniProductData.MinProductTitle == "Police" && !DocumentFix[0])
				{
					DocumentFix[0] = true;
					GameObject.Find("WindowHolder/PoliceDoc").GetComponent<RectTransform>().anchoredPosition = new Vector2(206f, -77f);
					GameObject.Find("WindowHolder/PoliceDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/PoliceDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Noir" && !DocumentFix[1])
				{
					DocumentFix[1] = true;
					GameObject.Find("WindowHolder/NoirDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/NoirDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Lucas" && !DocumentFix[2])
				{
					DocumentFix[2] = true;
					GameObject.Find("WindowHolder/LucasDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/LucasDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Breather" && !DocumentFix[3])
				{
					DocumentFix[3] = true;
					GameObject.Find("WindowHolder/BreatherDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/BreatherDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Tanner" && !DocumentFix[4])
				{
					DocumentFix[4] = true;
					GameObject.Find("WindowHolder/ZoneWallDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/ZoneWallDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Hacks" && !DocumentFix[5])
				{
					DocumentFix[5] = true;
					GameObject.Find("WindowHolder/MemDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/MemDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Viruses" && !DocumentFix[6])
				{
					DocumentFix[6] = true;
					GameObject.Find("WindowHolder/StackDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/StackDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
				if (UniProductData.MinProductTitle == "Kidnapper" && !DocumentFix[7])
				{
					DocumentFix[7] = true;
					GameObject.Find("WindowHolder/NodeDoc/TopBar/TopRightIcons/MaxBTN").SetActive(value: false);
					GameObject.Find("WindowHolder/NodeDoc/TopBar/TopRightIcons/MinBTN").SetActive(value: false);
				}
			}
		}
		else if (IAmMinned)
		{
			if (Product == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				GameManager.ManagerSlinger.AppManager.ForceUnMinApp(UniProductData);
			}
			else
			{
				GameManager.ManagerSlinger.AppManager.ForceUnMinApp(Product);
			}
		}
		else
		{
			Window.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		}
		if (myData != null)
		{
			myData.Opened = true;
			DataManager.Save(myData);
		}
	}

	public void Close()
	{
		OnClose();
		if (myData != null)
		{
			myData.Opened = false;
			DataManager.Save(myData);
		}
	}

	public void Max()
	{
		Window.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		IAmMaxxed = true;
		preMaxPOS = Window.GetComponent<RectTransform>().anchoredPosition;
		preMaxSize = Window.GetComponent<RectTransform>().sizeDelta;
		Sequence sequence = DOTween.Sequence().OnComplete(OnMax);
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().anchoredPosition = x;
		}, maxedPOS, 0.2f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().sizeDelta = x;
		}, maxedSize, 0.2f).SetEase(Ease.Linear));
		sequence.Play();
		if (myData != null)
		{
			myData.Maxed = true;
			DataManager.Save(myData);
		}
	}

	public void UnMax()
	{
		Window.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		IAmMaxxed = false;
		Sequence sequence = DOTween.Sequence().OnComplete(OnUnMax);
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().anchoredPosition = x;
		}, preMaxPOS, 0.2f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().sizeDelta = x;
		}, preMaxSize, 0.2f).SetEase(Ease.Linear));
		sequence.Play();
		if (myData != null)
		{
			myData.Maxed = false;
			DataManager.Save(myData);
		}
	}

	public void Min()
	{
		OnMin();
		IAmMinned = true;
		Window.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		preMinWindowPOS = Window.GetComponent<RectTransform>().anchoredPosition;
		preMinWindowSize = Window.GetComponent<RectTransform>().sizeDelta;
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			if (Product == SOFTWARE_PRODUCTS.UNIVERSAL)
			{
				GameManager.ManagerSlinger.AppManager.DoMinApp(UniProductData);
			}
			else
			{
				GameManager.ManagerSlinger.AppManager.DoMinApp(Product);
			}
		});
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().sizeDelta = x;
		}, minAppWindowSize, 0.15f).SetEase(Ease.Linear));
		sequence.Insert(0.15f, DOTween.To(() => Window.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().anchoredPosition = x;
		}, new Vector2(Window.GetComponent<RectTransform>().localPosition.x, 0f - (float)Screen.height), 0.25f).SetEase(Ease.Linear));
		sequence.Play();
		if (myData != null)
		{
			myData.Minned = true;
			DataManager.Save(myData);
		}
	}

	public void UnMin()
	{
		OnUnMin();
		IAmMinned = false;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().anchoredPosition, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().anchoredPosition = x;
		}, preMinWindowPOS, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => Window.GetComponent<RectTransform>().sizeDelta, delegate(Vector2 x)
		{
			Window.GetComponent<RectTransform>().sizeDelta = x;
		}, preMinWindowSize, 0.15f).SetEase(Ease.Linear));
		sequence.Play();
		if (myData != null)
		{
			myData.Minned = false;
			DataManager.Save(myData);
		}
	}

	public void Resized()
	{
		if (myData != null)
		{
			myData.WindowSize = myRT.sizeDelta.ToVect2();
			DataManager.Save(myData);
		}
		OnResized();
	}

	public void MoveMe(Vector2 NewPOS)
	{
		if (myData != null)
		{
			myData.WindowPosition = myRT.anchoredPosition.ToVect2();
			DataManager.Save(myData);
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<WindowData>(myID);
		if (myData == null)
		{
			myData = new WindowData(myID);
			myData.Opened = false;
			myData.Maxed = false;
			myData.Minned = false;
			myData.WindowSize = myRT.sizeDelta.ToVect2();
			myData.WindowPosition = myRT.anchoredPosition.ToVect2();
		}
		myRT.sizeDelta = myData.WindowSize.ToVector2;
		myRT.anchoredPosition = myData.WindowPosition.ToVector2;
		if (myData.Opened)
		{
			Launch();
		}
		if (canBeMax && myData.Maxed)
		{
			Max();
			maxBTN.HardMax();
		}
		if (canBeMin && myData.Minned)
		{
			Min();
		}
		GameManager.StageManager.Stage -= stageMe;
	}

	public void SetWindowTitle(string title)
	{
		foreach (object item in Window.transform.Find("TopBar").Find("Title").transform)
		{
			((Transform)item).GetComponent<Text>().text = title;
		}
	}

	public void SetModProps(SOFTWARE_PRODUCTS pRODUCTS, GameObject gObj)
	{
		Product = pRODUCTS;
		Window = gObj;
		Awake();
	}

	public static void ResetDF()
	{
		for (int i = 0; i < DocumentFix.Length; i++)
		{
			DocumentFix[i] = false;
		}
	}
}
