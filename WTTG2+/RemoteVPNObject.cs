using System;
using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class RemoteVPNObject : MonoBehaviour
{
	public delegate void RemoteVPNPlacement(RemoteVPNObject TheRemoteVPN);

	private bool generateCurrencyActive;

	private VPNCurrencyData myCurrency;

	private InteractionHook myInteractionHook;

	private MeshRenderer myMeshRenderer;

	[NonSerialized]
	public int myLevel;

	[NonSerialized]
	public int myID;

	public Vector3 SpawnLocation { get; private set; } = Vector3.zero;

	public Vector3 SpawnRotation { get; private set; } = Vector3.zero;

	public Vector3 CurrentLocation => base.transform.position;

	public Transform Transform => base.transform;

	public bool Placed { get; private set; }

	public string DOSCoinValue => myCurrency.GenerateDOSCoinValue.ToString();

	public string TimeValue => myCurrency.GenerateTime.ToString();

	public event RemoteVPNPlacement EnteredPlacementMode;

	public event RemoteVPNPlacement IWasPlaced;

	private void Update()
	{
		if (Placed && generateCurrencyActive)
		{
			DOSCoinsCurrencyManager.AddPendingCurrency(Mathf.Lerp(0f, myCurrency.GenerateDOSCoinValue * (float)myLevel, Time.deltaTime / myCurrency.GenerateTime));
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= enterPlacementMode;
	}

	public void SoftBuild()
	{
		myMeshRenderer = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += enterPlacementMode;
		myMeshRenderer.enabled = false;
		myInteractionHook.ForceLock = true;
		GameManager.PauseManager.GamePaused += playerHitPause;
		GameManager.PauseManager.GameUnPaused += playerHitUnPause;
	}

	public void SpawnMe(Vector3 SetPosition, Vector3 SetRotation)
	{
		Placed = false;
		base.transform.position = SetPosition;
		base.transform.rotation = Quaternion.Euler(SetRotation);
		SpawnLocation = SetPosition;
		SpawnRotation = SetRotation;
		myMeshRenderer.enabled = true;
		myInteractionHook.ForceLock = false;
	}

	public void PlaceMe(Vector3 SetPosition, Vector3 SetRotation)
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		UIInventoryManager.HideRemoteVPN();
		Placed = true;
		base.transform.position = SetPosition;
		base.transform.rotation = Quaternion.Euler(SetRotation);
		myMeshRenderer.enabled = true;
		myInteractionHook.ForceLock = false;
		GameManager.WorldManager.SetVPNValues(this);
		this.IWasPlaced?.Invoke(this);
		generateCurrencyActive = true;
	}

	public void SetPlaceMe(SerTrans WhereTo)
	{
		Placed = true;
		base.transform.position = WhereTo.PositionToVector3;
		base.transform.rotation = Quaternion.Euler(WhereTo.RotationToVector3);
		myMeshRenderer.enabled = true;
		myInteractionHook.ForceLock = false;
		GameManager.WorldManager.SetVPNValues(this);
		generateCurrencyActive = true;
	}

	public void SetCurrency(VPNCurrencyData SetValue)
	{
		myCurrency = SetValue;
	}

	public void SetCurrency(VPNCurrencyDefinition SetValue)
	{
		myCurrency = new VPNCurrencyData(SetValue.GenerateTime, SetValue.GenerateDOSCoinValue);
	}

	private void enterPlacementMode()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp1);
		UIInventoryManager.ShowRemoteVPN();
		Placed = false;
		myMeshRenderer.enabled = false;
		myInteractionHook.ForceLock = true;
		this.EnteredPlacementMode?.Invoke(this);
	}

	private void playerHitPause()
	{
		generateCurrencyActive = false;
	}

	private void playerHitUnPause()
	{
		generateCurrencyActive = true;
	}

	public void UpdateMaterial(int id)
	{
		myID = id;
		switch (id)
		{
		case 0:
		case 1:
			myLevel = 1;
			break;
		case 2:
		case 3:
			myLevel = 2;
			SetLevel2Material();
			break;
		case 4:
			myLevel = 3;
			SetLevel3Material();
			break;
		}
	}

	public void SetLevel2Material()
	{
		myMeshRenderer.material.color = new Color(2f, 0f, 2f);
	}

	public void SetLevel3Material()
	{
		myMeshRenderer.material.color = new Color(0f, 2f, 2f);
	}
}
