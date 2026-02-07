using System.Collections.Generic;
using DG.Tweening;
using SWS;
using UnityEngine;

[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(splineMove))]
public class DroneBehaviour : MonoBehaviour
{
	[SerializeField]
	private PathManager[] pathAnimations = new PathManager[0];

	[SerializeField]
	private DOTweenAnimation[] bladeAnimations = new DOTweenAnimation[0];

	[SerializeField]
	private Vector3 deSpawnLocation = Vector3.zero;

	[SerializeField]
	private Vector3 packageLocalPosition = Vector3.zero;

	[SerializeField]
	private Vector3 packageLocalRotation = Vector3.zero;

	[SerializeField]
	private Vector3 preDeliveryLocation = Vector3.zero;

	[SerializeField]
	private Vector3 preDeliveryRotation = Vector3.zero;

	[SerializeField]
	private AudioFileDefinition droneNoise;

	private bool currentlyBusy;

	private ShippedProductObject currentShippingProduct;

	private AudioHubObject myAudioHub;

	private splineMove mySplineMovement;

	private List<ShippedProductObject> pendingProducts = new List<ShippedProductObject>();

	private void Awake()
	{
		GameManager.BehaviourManager.DroneBehaviour = this;
		myAudioHub = GetComponent<AudioHubObject>();
		mySplineMovement = GetComponent<splineMove>();
	}

	private void Start()
	{
		shutDownDrone();
		base.transform.position = deSpawnLocation;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
	}

	public void DeliverPackage(ShippedProductObject ThePackage)
	{
		if (!currentlyBusy)
		{
			currentlyBusy = true;
			currentShippingProduct = ThePackage;
			currentShippingProduct.gameObject.transform.SetParent(base.transform);
			currentShippingProduct.gameObject.transform.localPosition = packageLocalPosition;
			currentShippingProduct.gameObject.transform.localRotation = Quaternion.Euler(packageLocalRotation);
			beginDeliverPackage();
		}
		else
		{
			pendingProducts.Add(ThePackage);
		}
	}

	private void fireUpDrone()
	{
		myAudioHub.PlaySound(droneNoise);
		for (int i = 0; i < bladeAnimations.Length; i++)
		{
			bladeAnimations[i].DORestart();
		}
	}

	private void shutDownDrone()
	{
		myAudioHub.KillSound(droneNoise.AudioClip);
		for (int i = 0; i < bladeAnimations.Length; i++)
		{
			bladeAnimations[i].DOPause();
		}
	}

	private void beginDeliverPackage()
	{
		fireUpDrone();
		base.transform.position = preDeliveryLocation;
		base.transform.rotation = Quaternion.Euler(preDeliveryRotation);
		mySplineMovement.PathIsCompleted += deliverPackage;
		mySplineMovement.pathContainer = pathAnimations[0];
		mySplineMovement.ResetToStart();
		mySplineMovement.StartMove();
	}

	private void deliverPackage()
	{
		mySplineMovement.PathIsCompleted -= deliverPackage;
		currentShippingProduct.DroneDrop();
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			mySplineMovement.PathIsCompleted += doneWithDelivery;
			mySplineMovement.pathContainer = pathAnimations[1];
			mySplineMovement.ResetToStart();
			mySplineMovement.StartMove();
		});
	}

	private void doneWithDelivery()
	{
		mySplineMovement.PathIsCompleted -= doneWithDelivery;
		currentlyBusy = false;
		if (pendingProducts.Count > 0)
		{
			ShippedProductObject thePackage = pendingProducts[0];
			pendingProducts.RemoveAt(0);
			DeliverPackage(thePackage);
		}
		else
		{
			deSpawn();
		}
	}

	private void deSpawn()
	{
		mySplineMovement.PathIsCompleted -= deSpawn;
		shutDownDrone();
		base.transform.position = deSpawnLocation;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
	}
}
