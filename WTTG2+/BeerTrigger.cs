using System;
using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class BeerTrigger : MonoBehaviour
{
	private MeshRenderer mesh;

	private InteractionHook myInteractionHook;

	private InteractionHook deliverHook;

	private static readonly Vector3 deliverLocation = new Vector3(-1.8738f, 22.7549f, -7.296f);

	private static string wifiPassword;

	private bool ListeningForBeer;

	public static void SetWiFiPassword(string password)
	{
		wifiPassword = password;
	}

	private void Awake()
	{
		mesh = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += PickUpBeer;
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "DeliverBeer";
		gameObject.layer = 9;
		deliverHook = gameObject.AddComponent<InteractionHook>();
		deliverHook.StateActive = PLAYER_STATE.ROAMING;
		deliverHook.RequireLocationCheck = true;
		deliverHook.LocationToCheck = PLAYER_LOCATION.HALL_WAY5;
		deliverHook.ForceLock = true;
		deliverHook.LeftClickAction += DeliverBeer;
		gameObject.transform.position = new Vector3(-2.2038f, 23.2549f, -7.296f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 0.1f);
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	private void Start()
	{
		WifiInteractionManager.SetBeer504Password();
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= PickUpBeer;
	}

	private void Update()
	{
		if (ListeningForBeer && (ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position.x >= 8f || ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position.x <= -8f))
		{
			ListeningForBeer = false;
			omnomEnjoyyourbeer();
		}
	}

	private void PickUpBeer()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
		deliverHook.ForceLock = false;
	}

	private void DeliverBeer()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPutDown1);
		deliverHook.ForceLock = true;
		mesh.enabled = true;
		base.transform.position = deliverLocation;
		ListeningForBeer = true;
	}

	private void omnomEnjoyyourbeer()
	{
		GameManager.TimeSlinger.FireTimer(0.25f, delegate
		{
			mesh.enabled = false;
			deliverHook.gameObject.AddComponent<AudioHubObject>().PlaySound(CustomSoundLookUp.nyam);
			doorlogBehaviour.MayAddDoorlog("Apartment 504", mode: true);
			GameManager.TimeSlinger.FireTimer(0.1f, delegate
			{
				doorlogBehaviour.MayAddDoorlog("Apartment 504", mode: false);
			});
		});
		GameManager.TimeSlinger.FireTimer(35f, delegate
		{
			GameManager.ManagerSlinger.TextDocManager.CreateTextDoc((UnityEngine.Random.Range(0, 100) <= 3) ? "piwo.txt" : "beer.txt", "Thank you for the beer, You are free to use my WiFi" + Environment.NewLine + "The password is " + wifiPassword + Environment.NewLine + Environment.NewLine + " - The Beer Opening Tips Guy");
		});
	}
}
