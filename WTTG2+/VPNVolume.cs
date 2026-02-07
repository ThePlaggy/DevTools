using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VPNVolume : MonoBehaviour
{
	public VPNCurrencyData MyCurrency;

	[SerializeField]
	private float updateDelay = 0.1f;

	[SerializeField]
	private bool hideGizmo;

	private Bounds bounds;

	private BoxCollider boxCollider;

	private List<RemoteVPNObject> currentPlacedRemoteVPNS = new List<RemoteVPNObject>(5);

	private void Awake()
	{
		updateBounds();
		boxCollider.enabled = false;
		GameManager.WorldManager.AddVPNVolume(this);
	}

	private void OnDrawGizmos()
	{
		if (!hideGizmo)
		{
			updateBounds();
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}

	public bool VPNInRange(RemoteVPNObject TheRemoteVPN)
	{
		if (!bounds.Contains(TheRemoteVPN.CurrentLocation))
		{
			return false;
		}
		if (currentPlacedRemoteVPNS.Count > 0)
		{
			TheRemoteVPN.EnteredPlacementMode += clearRemoteVPNFromList;
			currentPlacedRemoteVPNS.Add(TheRemoteVPN);
			return false;
		}
		TheRemoteVPN.EnteredPlacementMode += clearRemoteVPNFromList;
		currentPlacedRemoteVPNS.Add(TheRemoteVPN);
		TheRemoteVPN.SetCurrency(MyCurrency);
		return true;
	}

	public bool VPNRangeCheck(Transform ThePosition, out float TimeValue)
	{
		if (!bounds.Contains(ThePosition.position))
		{
			TimeValue = 2500f;
			return false;
		}
		if (currentPlacedRemoteVPNS.Count > 0)
		{
			TimeValue = 2500f;
			return false;
		}
		TimeValue = MyCurrency.GenerateTime;
		return true;
	}

	private void updateBounds()
	{
		if (boxCollider == null)
		{
			boxCollider = GetComponent<BoxCollider>();
		}
		bounds = new Bounds(base.transform.TransformPoint(boxCollider.center), boxCollider.size);
	}

	private void clearRemoteVPNFromList(RemoteVPNObject TheRemoteVPN)
	{
		TheRemoteVPN.EnteredPlacementMode -= clearRemoteVPNFromList;
		currentPlacedRemoteVPNS.Remove(TheRemoteVPN);
		if (currentPlacedRemoteVPNS.Count == 1)
		{
			currentPlacedRemoteVPNS[0].SetCurrency(MyCurrency);
		}
	}
}
