using System.Collections.Generic;
using UnityEngine;

public class RemoteVPNManager : MonoBehaviour
{
	public delegate void RemoteVPNVoidActions(RemoteVPNObject TheRemoteVPN);

	[SerializeField]
	private int REMOTE_VPN_POOL_COUNT = 5;

	[SerializeField]
	private GameObject remoteVPNObject;

	[SerializeField]
	private Transform remoteVPNParent;

	[SerializeField]
	private Vector3[] remoteVPNSpawnLocations = new Vector3[0];

	[SerializeField]
	private Vector3[] remoteVPNSpawnRotations = new Vector3[0];

	[SerializeField]
	private VPNCurrencyDefinition[] vpnCurrencies = new VPNCurrencyDefinition[0];

	private Stack<Vector3> currentAvaibleRemoteVPNSpawnLocations = new Stack<Vector3>(5);

	private Stack<Vector3> currentAvaibleRemoteVPNSpawnRotations = new Stack<Vector3>(5);

	private List<VPNCurrencyDefinition> currentAvaibleVPNCurrencies = new List<VPNCurrencyDefinition>();

	private List<RemoteVPNObject> currentlyOwnedVPNS = new List<RemoteVPNObject>(5);

	private RemoteVPNObject currentRemoteVPN;

	private bool easyModeTweaked;

	private RemoteVPNManagerData myData;

	private int myID;

	private PooledStack<RemoteVPNObject> remoteVPNPool;

	public static bool GodSpot;

	public static int AmountOfRemoteVPNsLVL1;

	public static int AmountOfRemoteVPNsLVL2;

	public static int AmountOfRemoteVPNsLVL3;

	public Vector3 CurrentRemoteVPNSpawnLocation
	{
		get
		{
			if (currentRemoteVPN != null)
			{
				return currentRemoteVPN.SpawnLocation;
			}
			return remoteVPNSpawnLocations[0];
		}
	}

	public Vector3 CurrentRemoteVPNSpawnRotation
	{
		get
		{
			if (currentRemoteVPN != null)
			{
				return currentRemoteVPN.SpawnRotation;
			}
			return remoteVPNSpawnRotations[0];
		}
	}

	public event RemoteVPNVoidActions EnteredPlacementMode;

	public event RemoteVPNVoidActions RemoteVPNWasReturned;

	public event RemoteVPNVoidActions RemoteVPNWasPlaced;

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		GameManager.ManagerSlinger.RemoteVPNManager = this;
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.TheGameIsLive += gameLive;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
		remoteVPNPool = new PooledStack<RemoteVPNObject>(delegate
		{
			RemoteVPNObject component = Object.Instantiate(remoteVPNObject, remoteVPNParent).GetComponent<RemoteVPNObject>();
			component.SoftBuild();
			component.EnteredPlacementMode += remoteVPNWasPickedUp;
			component.IWasPlaced += remoteVPNWasPlaced;
			return component;
		}, REMOTE_VPN_POOL_COUNT);
		for (int num = remoteVPNSpawnLocations.Length - 1; num >= 0; num--)
		{
			currentAvaibleRemoteVPNSpawnLocations.Push(remoteVPNSpawnLocations[num]);
		}
		for (int num2 = remoteVPNSpawnRotations.Length - 1; num2 >= 0; num2--)
		{
			currentAvaibleRemoteVPNSpawnRotations.Push(remoteVPNSpawnRotations[num2]);
		}
		for (int num3 = 0; num3 < vpnCurrencies.Length; num3++)
		{
			if (vpnCurrencies[num3].GenerateTime.Equals(163f))
			{
				bool flag = false;
				vpnCurrencies[num3].GenerateDOSCoinValue = 5.328f;
				vpnCurrencies[num3].GenerateTime = 132f;
				Debug.Log("VPN Buff fix applied");
				bool flag2 = false;
			}
			currentAvaibleVPNCurrencies.Add(vpnCurrencies[num3]);
		}
		GodSpot = Random.Range(0, 100) <= (DifficultyManager.Nightmare ? 10 : 25);
		Debug.Log("Godspot = " + GodSpot);
		for (int num4 = 0; num4 < 5; num4++)
		{
			spawnRemoteVPN(num4);
		}
	}

	private void OnDestroy()
	{
		clearRemoteVPNS();
	}

	public void ReturnRemoteVPN()
	{
		if (currentRemoteVPN != null)
		{
			StateManager.PlayerState = PLAYER_STATE.ROAMING;
			if (this.RemoteVPNWasReturned != null)
			{
				this.RemoteVPNWasReturned(currentRemoteVPN);
			}
			currentRemoteVPN.SpawnMe(currentRemoteVPN.SpawnLocation, currentRemoteVPN.SpawnRotation);
			currentRemoteVPN = null;
		}
	}

	private static void AddDebugVPNS()
	{
		GameManager.ManagerSlinger.RemoteVPNManager.AddVPN(1);
		GameManager.ManagerSlinger.RemoteVPNManager.AddVPN(2);
		GameManager.ManagerSlinger.RemoteVPNManager.AddVPN(3);
	}

	public void AddVPN(int level)
	{
		switch (level)
		{
		case 1:
			if (AmountOfRemoteVPNsLVL1 < 2)
			{
				currentlyOwnedVPNS[AmountOfRemoteVPNsLVL1].gameObject.SetActive(value: true);
				AmountOfRemoteVPNsLVL1++;
			}
			break;
		case 2:
			if (AmountOfRemoteVPNsLVL2 < 2)
			{
				currentlyOwnedVPNS[AmountOfRemoteVPNsLVL2 + 2].gameObject.SetActive(value: true);
				AmountOfRemoteVPNsLVL2++;
			}
			break;
		case 3:
			if (AmountOfRemoteVPNsLVL3 < 1)
			{
				currentlyOwnedVPNS[AmountOfRemoteVPNsLVL3 + 4].gameObject.SetActive(value: true);
				AmountOfRemoteVPNsLVL3++;
			}
			break;
		}
	}

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		switch (TheProduct.productID)
		{
		case HARDWARE_PRODUCTS.REMOTE_VPN:
			AddVPN(1);
			break;
		case HARDWARE_PRODUCTS.REMOTE_VPN_LEVEL2:
			AddVPN(2);
			break;
		case HARDWARE_PRODUCTS.REMOTE_VPN_LEVEL3:
			AddVPN(3);
			break;
		}
	}

	private void spawnRemoteVPN(int id)
	{
		if (currentAvaibleRemoteVPNSpawnLocations.Count > 0 && currentAvaibleRemoteVPNSpawnRotations.Count > 0)
		{
			Vector3 setPosition = currentAvaibleRemoteVPNSpawnLocations.Pop();
			Vector3 setRotation = currentAvaibleRemoteVPNSpawnRotations.Pop();
			RemoteVPNObject remoteVPNObject = remoteVPNPool.Pop();
			remoteVPNObject.SpawnMe(setPosition, setRotation);
			remoteVPNObject.UpdateMaterial(id);
			currentlyOwnedVPNS.Add(remoteVPNObject);
			remoteVPNObject.gameObject.SetActive(value: false);
		}
	}

	private void clearRemoteVPNS()
	{
		for (int i = 0; i < currentlyOwnedVPNS.Count; i++)
		{
			remoteVPNPool.Push(currentlyOwnedVPNS[i]);
		}
		foreach (RemoteVPNObject item in remoteVPNPool)
		{
			item.EnteredPlacementMode -= remoteVPNWasPickedUp;
			item.IWasPlaced -= remoteVPNWasPlaced;
		}
		currentlyOwnedVPNS.Clear();
		remoteVPNPool.Clear();
	}

	private void remoteVPNWasPickedUp(RemoteVPNObject TheRemoteVPN)
	{
		currentRemoteVPN = TheRemoteVPN;
		StateManager.PlayerState = PLAYER_STATE.REMOTE_VPN_PLACEMENT;
		myData.CurrentlyPlacedRemoteVPNs.Remove(TheRemoteVPN.Transform.position.GetHashCode());
		DataManager.Save(myData);
		if (this.EnteredPlacementMode != null)
		{
			this.EnteredPlacementMode(TheRemoteVPN);
		}
	}

	private void remoteVPNWasPlaced(RemoteVPNObject TheRemoteVPN)
	{
		currentRemoteVPN = null;
		StateManager.PlayerState = PLAYER_STATE.ROAMING;
		SerTrans value = SerTrans.Convert(TheRemoteVPN.Transform.position, TheRemoteVPN.Transform.rotation.eulerAngles);
		myData.CurrentlyPlacedRemoteVPNs.Add(TheRemoteVPN.Transform.position.GetHashCode(), value);
		DataManager.Save(myData);
		if (this.RemoteVPNWasPlaced != null)
		{
			this.RemoteVPNWasPlaced(TheRemoteVPN);
		}
		int num = 0;
		for (int i = 0; i < currentlyOwnedVPNS.Count; i++)
		{
			if (currentlyOwnedVPNS[i].Placed)
			{
				num++;
			}
		}
		if (num >= 5)
		{
			SteamSlinger.Ins.UnlockSteamAchievement(STEAM_ACHIEVEMENT.PAIDTOSIT);
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<RemoteVPNManagerData>(myID);
		if (myData == null)
		{
			myData = new RemoteVPNManagerData(myID);
			myData.CurrentVPNVolumesCurrencyData = new List<VPNCurrencyData>(GameManager.WorldManager.CurrentVPNVolumes.Count);
			myData.CurrentlyPlacedRemoteVPNs = new Dictionary<int, SerTrans>(5);
			for (int i = 0; i < GameManager.WorldManager.CurrentVPNVolumes.Count; i++)
			{
				int index = Random.Range(0, currentAvaibleVPNCurrencies.Count);
				VPNCurrencyData item = new VPNCurrencyData(currentAvaibleVPNCurrencies[index].GenerateTime, currentAvaibleVPNCurrencies[index].GenerateDOSCoinValue);
				currentAvaibleVPNCurrencies.RemoveAt(index);
				myData.CurrentVPNVolumesCurrencyData.Add(item);
			}
		}
		for (int j = 0; j < GameManager.WorldManager.CurrentVPNVolumes.Count; j++)
		{
			GameManager.WorldManager.CurrentVPNVolumes[j].MyCurrency = myData.CurrentVPNVolumesCurrencyData[j];
		}
		DataManager.Save(myData);
		GameManager.StageManager.Stage -= stageMe;
	}

	private void gameLive()
	{
		int num = 0;
		foreach (KeyValuePair<int, SerTrans> currentlyPlacedRemoteVPN in myData.CurrentlyPlacedRemoteVPNs)
		{
			if (currentlyOwnedVPNS[num] != null)
			{
				currentlyOwnedVPNS[num].SetPlaceMe(currentlyPlacedRemoteVPN.Value);
				this.RemoteVPNWasPlaced?.Invoke(currentlyOwnedVPNS[num]);
			}
			num++;
		}
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}
}
