using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(DoorTrigger))]
public class HXVolDoorTrigger : MonoBehaviour
{
	[SerializeField]
	private float doorCloseTime;

	[SerializeField]
	private float doorOpenTime;

	[SerializeField]
	private HxVolumetricLight hxLight;

	[SerializeField]
	private PLAYER_LOCATION activeLocation;

	private float defaultHXLightInt;

	private DoorTrigger myDoorTrigger;

	private void Awake()
	{
		myDoorTrigger = GetComponent<DoorTrigger>();
		myDoorTrigger.DoorCloseEvent.AddListener(doorIsClosing);
		myDoorTrigger.DoorOpenEvent.AddListener(doorIsOpening);
		defaultHXLightInt = hxLight.Intensity;
	}

	private void OnDestroy()
	{
		myDoorTrigger.DoorCloseEvent.RemoveListener(doorIsClosing);
		myDoorTrigger.DoorOpenEvent.RemoveListener(doorIsOpening);
	}

	private void doorIsClosing()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP_ROOM)
		{
			DOTween.To(() => hxLight.Intensity, delegate(float x)
			{
				hxLight.Intensity = x;
			}, 0f, doorCloseTime);
		}
	}

	private void doorIsOpening()
	{
		if (StateManager.PlayerLocation == PLAYER_LOCATION.DEAD_DROP_ROOM)
		{
			DOTween.To(() => hxLight.Intensity, delegate(float x)
			{
				hxLight.Intensity = x;
			}, defaultHXLightInt, doorOpenTime);
		}
	}
}
