using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class TenantTrackManager : MonoBehaviour
{
	public static bool DidAyana;

	[SerializeField]
	private Image systemLockedImage;

	[SerializeField]
	private RectTransform borderBoxRT;

	[SerializeField]
	private RectTransform tenantWindowInfoRT;

	[SerializeField]
	private CanvasGroup tenantWindowInfoCG;

	[SerializeField]
	private Image tenantLeftButton;

	[SerializeField]
	private Image tenantBackButton;

	[SerializeField]
	private Image tenantRightButton;

	[SerializeField]
	private Sprite inactiveTenantLeftButtonSprite;

	[SerializeField]
	private Sprite inactiveTenantBackButtonSprite;

	[SerializeField]
	private Sprite inactiveTenantRightButtonSprite;

	[SerializeField]
	private Sprite activeTenantLeftButtonSprite;

	[SerializeField]
	private Sprite activeTenantBackButtonSprite;

	[SerializeField]
	private Sprite activeTenantRightButtonSprite;

	[SerializeField]
	private Text tenantFloorTitle;

	[SerializeField]
	private Text tenantUnitValue;

	[SerializeField]
	private Text tenantNameValue;

	[SerializeField]
	private Text tenantAgeValue;

	[SerializeField]
	private Text tenantNotesValue;

	[SerializeField]
	private TenantDefinition clintData;

	[SerializeField]
	private AudioFileDefinition UnLockSystemSFX;

	[SerializeField]
	private AudioFileDefinition[] KeyboardSFXS;

	[SerializeField]
	private TennantTrackFloorSelectObject[] floorSelectObjects = new TennantTrackFloorSelectObject[0];

	[SerializeField]
	private TenantDefinition[] tenants = new TenantDefinition[0];

	public List<TenantData> tenantList;

	private List<TenantDefinition> avaibleTenants = new List<TenantDefinition>();

	private int currentFloorIndex;

	private int currentTenantIndex;

	private int[] floors = new int[6] { 10, 8, 6, 5, 3, 1 };

	private Tweener hideTenantWindow;

	private bool lockInput;

	private TenantTrackData myData;

	private int myID;

	private LobbyComputerCameraManager myLobbyComputerCameraManager;

	private TENANT_TRACK_STATE myState;

	private Tweener showBorderBoxTween;

	private Tweener showTenantWindow;

	private bool systemIsLocked;

	private Dictionary<int, List<TenantData>> tenantData = new Dictionary<int, List<TenantData>>();

	private Dictionary<int, TenantData> tenantLookUp = new Dictionary<int, TenantData>();

	private int tenantOptionIndex;

	public TenantDefinition[] Tenants => tenants;

	public TenantDefinition[] TenantsAvailable => avaibleTenants.ToArray();

	public TenantData[] TenantDatas => tenantList.ToArray();

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		lockInput = true;
		GameManager.ManagerSlinger.TenantTrackManager = this;
		GameManager.StageManager.Stage += stageMe;
		myState = TENANT_TRACK_STATE.LOCKED;
		showBorderBoxTween = DOTween.To(() => new Vector3(0f, 0f, 1f), delegate(Vector3 x)
		{
			borderBoxRT.localScale = x;
		}, Vector3.one, 0.4f).SetEase(Ease.Linear);
		showBorderBoxTween.Pause();
		showBorderBoxTween.SetAutoKill(autoKillOnCompletion: false);
		showTenantWindow = DOTween.To(() => new Vector2(800f, 0f), delegate(Vector2 x)
		{
			tenantWindowInfoRT.sizeDelta = x;
		}, new Vector2(800f, 500f), 0.5f).SetEase(Ease.Linear).OnComplete(delegate
		{
			lockInput = false;
		});
		showTenantWindow.Pause();
		showTenantWindow.SetAutoKill(autoKillOnCompletion: false);
		hideTenantWindow = DOTween.To(() => new Vector2(800f, 500f), delegate(Vector2 x)
		{
			tenantWindowInfoRT.sizeDelta = x;
		}, new Vector2(800f, 0f), 0.5f).SetEase(Ease.Linear).OnComplete(delegate
		{
			lockInput = false;
			tenantWindowInfoCG.alpha = 0f;
		});
		hideTenantWindow.Pause();
		hideTenantWindow.SetAutoKill(autoKillOnCompletion: false);
	}

	private void Update()
	{
		takeInput();
	}

	public void UnLockSystem()
	{
		GameManager.AudioSlinger.PlaySound(UnLockSystemSFX);
		myLobbyComputerCameraManager.TriggerGlitch();
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			myState = TENANT_TRACK_STATE.FLOOR_SELECT;
			systemIsLocked = false;
			lockInput = false;
			systemLockedImage.enabled = false;
			showBorderBoxTween.Restart();
		});
	}

	public bool CheckIfFemaleTenant(int UnitNumber)
	{
		return UnitNumber != 0 && tenantLookUp[UnitNumber].tenantSex == 1 && tenantLookUp[UnitNumber].canBeTagged;
	}

	private void takeInput()
	{
		if (StateManager.GameState == GAME_STATE.LIVE && !systemIsLocked && !lockInput && ControllerManager.Get<lobbyComputerController>(GAME_CONTROLLER.LOBBY_COMPUTER).Active)
		{
			switch (myState)
			{
			case TENANT_TRACK_STATE.TENANT_SELECT:
				tenantSelectInput();
				break;
			case TENANT_TRACK_STATE.FLOOR_SELECT:
				floorSelectInput();
				break;
			}
		}
	}

	private void processKeyboardSFX()
	{
		int num = Random.Range(1, KeyboardSFXS.Length);
		AudioFileDefinition audioFileDefinition = KeyboardSFXS[num];
		GameManager.AudioSlinger.PlaySound(audioFileDefinition);
		KeyboardSFXS[num] = KeyboardSFXS[0];
		KeyboardSFXS[0] = audioFileDefinition;
	}

	private void floorSelectInput()
	{
		if (CrossPlatformInputManager.GetButtonDown("Up"))
		{
			processKeyboardSFX();
			currentFloorIndex--;
			if (currentFloorIndex < 0)
			{
				currentFloorIndex = 0;
			}
			floorSelectObjects[currentFloorIndex].ActivateMe();
			floorSelectObjects[currentFloorIndex + 1].DeActivateMe();
		}
		else if (CrossPlatformInputManager.GetButtonDown("Down"))
		{
			processKeyboardSFX();
			currentFloorIndex++;
			if (currentFloorIndex >= floorSelectObjects.Length - 1)
			{
				currentFloorIndex = floorSelectObjects.Length - 1;
			}
			floorSelectObjects[currentFloorIndex].ActivateMe();
			floorSelectObjects[currentFloorIndex - 1].DeActivateMe();
		}
		else if (CrossPlatformInputManager.GetButtonDown("Return"))
		{
			processKeyboardSFX();
			currentTenantIndex = 0;
			presentTenantInfoWindow();
		}
	}

	private void tenantSelectInput()
	{
		if (CrossPlatformInputManager.GetButtonDown("Left"))
		{
			processKeyboardSFX();
			tenantOptionIndex--;
			if (tenantOptionIndex < 0)
			{
				tenantOptionIndex = 0;
			}
		}
		else if (CrossPlatformInputManager.GetButtonDown("Right"))
		{
			processKeyboardSFX();
			tenantOptionIndex++;
			if (tenantOptionIndex > 2)
			{
				tenantOptionIndex = 2;
			}
		}
		else if (CrossPlatformInputManager.GetButtonDown("Return"))
		{
			processKeyboardSFX();
			switch (tenantOptionIndex)
			{
			case 2:
				currentTenantIndex++;
				if (currentTenantIndex > tenantData[floorSelectObjects[currentFloorIndex].FloorNumber].Count - 1)
				{
					currentTenantIndex = tenantData[floorSelectObjects[currentFloorIndex].FloorNumber].Count - 1;
				}
				updateTenantData();
				break;
			case 1:
				dismissTenantInfoWindow();
				break;
			case 0:
				currentTenantIndex--;
				if (currentTenantIndex < 0)
				{
					currentTenantIndex = 0;
				}
				updateTenantData();
				break;
			}
		}
		switch (tenantOptionIndex)
		{
		case 2:
			tenantLeftButton.sprite = inactiveTenantLeftButtonSprite;
			tenantBackButton.sprite = inactiveTenantBackButtonSprite;
			tenantRightButton.sprite = activeTenantRightButtonSprite;
			break;
		case 1:
			tenantLeftButton.sprite = inactiveTenantLeftButtonSprite;
			tenantBackButton.sprite = activeTenantBackButtonSprite;
			tenantRightButton.sprite = inactiveTenantRightButtonSprite;
			break;
		case 0:
			tenantLeftButton.sprite = activeTenantLeftButtonSprite;
			tenantBackButton.sprite = inactiveTenantBackButtonSprite;
			tenantRightButton.sprite = inactiveTenantRightButtonSprite;
			break;
		}
	}

	private void presentTenantInfoWindow()
	{
		updateTenantData();
		lockInput = true;
		myState = TENANT_TRACK_STATE.TENANT_SELECT;
		tenantWindowInfoCG.alpha = 1f;
		showTenantWindow.Restart();
	}

	private void dismissTenantInfoWindow()
	{
		lockInput = true;
		myState = TENANT_TRACK_STATE.FLOOR_SELECT;
		hideTenantWindow.Restart();
	}

	private void updateTenantData()
	{
		if (tenantData.ContainsKey(floorSelectObjects[currentFloorIndex].FloorNumber) && tenantData[floorSelectObjects[currentFloorIndex].FloorNumber][currentTenantIndex] != null)
		{
			tenantFloorTitle.text = "FLOOR " + floorSelectObjects[currentFloorIndex].FloorNumber;
			tenantUnitValue.text = tenantData[floorSelectObjects[currentFloorIndex].FloorNumber][currentTenantIndex].tenantUnit.ToString();
			tenantNameValue.text = tenantData[floorSelectObjects[currentFloorIndex].FloorNumber][currentTenantIndex].tenantName;
			tenantAgeValue.text = tenantData[floorSelectObjects[currentFloorIndex].FloorNumber][currentTenantIndex].tenantAge.ToString();
			tenantNotesValue.text = tenantData[floorSelectObjects[currentFloorIndex].FloorNumber][currentTenantIndex].tenantNotes;
		}
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		if (DifficultyManager.HackerMode)
		{
			return;
		}
		systemIsLocked = true;
		lockInput = true;
		currentFloorIndex = 0;
		floorSelectObjects[currentFloorIndex].ActivateMe();
		tenantOptionIndex = 1;
		tenantLeftButton.sprite = inactiveTenantLeftButtonSprite;
		tenantBackButton.sprite = activeTenantBackButtonSprite;
		tenantRightButton.sprite = inactiveTenantRightButtonSprite;
		myData = DataManager.Load<TenantTrackData>(myID);
		new TenantExtension().ExtendTenants(tenants);
		if (myData == null)
		{
			myData = new TenantTrackData(myID);
			myData.TenantData = new Dictionary<int, List<TenantData>>();
			for (int i = 0; i < 32; i++)
			{
				avaibleTenants.Add(tenants[i]);
			}
			for (int j = 0; j < floors.Length; j++)
			{
				int k = 0;
				List<TenantData> list = new List<TenantData>();
				for (; k < 6; k++)
				{
					if (floors[j] == 8 && k == 2)
					{
						TenantData item = new TenantData(clintData);
						list.Add(item);
						tenantList.Add(item);
					}
					else if (floors[j] == 5 && k == 3)
					{
						TenantDefinition tenantDefinition = ScriptableObject.CreateInstance<TenantDefinition>();
						tenantDefinition.tenantAge = 20;
						tenantDefinition.tenantName = "Dave Pilsner";
						tenantDefinition.tenantNotes = "This guy loves beer more than everything. Potentially insane. He has been caught opening a beer with abnormal items such as bricks or gardening equipment. He doesn't need anything else, just give him beer. HE WANTS BEER!";
						tenantDefinition.canBeTagged = false;
						tenantDefinition.tenantUnit = 504;
						tenantDefinition.tenantSex = SEX.MALE;
						TenantData item2 = new TenantData(tenantDefinition);
						list.Add(item2);
						tenantList.Add(item2);
					}
					else if (floors[j] == 6 && k == 1)
					{
						TenantDefinition tenantDefinition2 = ScriptableObject.CreateInstance<TenantDefinition>();
						tenantDefinition2.tenantAge = 19;
						tenantDefinition2.tenantName = "Riley Lager";
						tenantDefinition2.tenantNotes = "Almost never leaves apartment, Plays games with her friends until late nights, making loud rage noises, One of her friends usually goes out past midnight to buy chips and soda, Typical gamer nerds.";
						tenantDefinition2.canBeTagged = true;
						tenantDefinition2.tenantUnit = 602;
						tenantDefinition2.tenantSex = SEX.FEMALE;
						TenantData item3 = new TenantData(tenantDefinition2);
						list.Add(item3);
						tenantList.Add(item3);
					}
					else if (floors[j] == HitmanProxyBehaviour.ChosenRoomFloor && k == HitmanProxyBehaviour.ChosenRoomIndex)
					{
						TenantDefinition tenantDefinition3 = ScriptableObject.CreateInstance<TenantDefinition>();
						tenantDefinition3.tenantAge = 29;
						tenantDefinition3.tenantName = "Lucas Holmes";
						tenantDefinition3.tenantNotes = "I hear strange noises coming from the apartment at odd hours. I have received no complaints yet so it seems there is nothing to be concerned about at the moment but... the noises... I have never heard anything like it before.";
						tenantDefinition3.canBeTagged = false;
						tenantDefinition3.tenantUnit = HitmanProxyBehaviour.ChosenRoom;
						tenantDefinition3.tenantSex = SEX.MALE;
						TenantData item4 = new TenantData(tenantDefinition3);
						list.Add(item4);
						tenantList.Add(item4);
					}
					else if (floors[j] != 1 || k != 2)
					{
						int num = Random.Range(0, avaibleTenants.Count);
						TenantDefinition tenantDefinition4 = avaibleTenants[num];
						tenantDefinition4.tenantUnit = floors[j] * 100 + k + 1;
						list.Add(new TenantData(tenantDefinition4)
						{
							tenantIndex = num
						});
						tenantList.Add(new TenantData(tenantDefinition4)
						{
							tenantIndex = num
						});
						avaibleTenants.RemoveAt(num);
					}
				}
				myData.TenantData.Add(floors[j], list);
			}
			DataManager.Save(myData);
		}
		foreach (KeyValuePair<int, List<TenantData>> tenantDatum in myData.TenantData)
		{
			for (int l = 0; l < tenantDatum.Value.Count; l++)
			{
				tenantLookUp.Add(tenantDatum.Value[l].tenantUnit, tenantDatum.Value[l]);
				tenants[tenantDatum.Value[l].tenantIndex].tenantUnit = tenantDatum.Value[l].tenantUnit;
				if (tenantDatum.Value[l].tenantSex != 1)
				{
					bool canBeTagged = tenantDatum.Value[l].canBeTagged;
				}
			}
			tenantData.Add(tenantDatum.Key, tenantDatum.Value);
		}
		CameraManager.Get(CAMERA_ID.LOBBY_COMPUTER, out var ReturnCamera);
		if (ReturnCamera != null)
		{
			myLobbyComputerCameraManager = ReturnCamera.gameObject.GetComponent<LobbyComputerCameraManager>();
		}
	}

	public float CheckDollMakerPrice(int UnitNumber)
	{
		if (DifficultyManager.Nightmare)
		{
			return 15f;
		}
		if (tenantLookUp[UnitNumber].tenantAge < 30)
		{
			return 65f;
		}
		return 35f;
	}

	public bool CheckLucas(int UnitNumber)
	{
		return UnitNumber != 0 && tenantLookUp[UnitNumber].tenantName == "Lucas Holmes";
	}

	public bool CheckAyana(int UnitNumber)
	{
		return UnitNumber != 0 && tenantLookUp[UnitNumber].tenantName == "Ayana Armstrong";
	}

	public bool CheckGG(int UnitNumber)
	{
		return UnitNumber != 0 && tenantLookUp[UnitNumber].tenantName == "Riley Lager";
	}
}
